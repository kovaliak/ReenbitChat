using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReenbitChat.Api.Hubs;
using ReenbitChat.Data.Contexts;
using ReenbitChat.Data.Entities;
using ReenbitChat.Services;
using ReenbitChat.Shared.Dtos.RegisterDto;
using ReenbitChat.Shared.Dtos.RoomDto;
using ReenbitChat.Shared.Dtos.UserProfileDto;
using ReenbitChat.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatConnection")));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageStorageService, MessageService>();
builder.Services.AddScoped<IRoomService, RoomService>();

var frontendUrl = builder.Configuration["FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl is not configured in appsettings.json");

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR().AddAzureSignalR(builder.Configuration.GetConnectionString("AzureSignalR"));

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("BlazorPolicy");

app.Use(async (context, next) =>
{
    var accessToken = context.Request.Query["access_token"];
    var path = context.Request.Path;

    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
    {
        context.Request.Headers.Authorization = $"Bearer {accessToken}";
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<ApplicationUser>();

// Custom registration endpoint to support DisplayName
app.MapPost("/api/auth/register-custom", async (RegisterRequest request, UserManager<ApplicationUser> userManager) =>
{
    var user = new ApplicationUser 
    { 
        UserName = request.Email, 
        Email = request.Email, 
        DisplayName = request.DisplayName 
    };

    var result = await userManager.CreateAsync(user, request.Password);

    if (result.Succeeded)
    {
        await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("DisplayName", request.DisplayName));
        return Results.Ok();
    }

    return Results.BadRequest(result.Errors);
});

// Retrieves profile info for the currently authenticated user
app.MapGet("/api/users/me", async (ClaimsPrincipal user, UserManager<ApplicationUser> userManager) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
    {
        return Results.Unauthorized();
    }

    var appUser = await userManager.FindByIdAsync(userId);
    if (appUser == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new UserProfileResponse
    {
        Id = appUser.Id,
        Email = appUser.Email ?? "Unknown",
        DisplayName = appUser.DisplayName!
    });
})
.RequireAuthorization();

// Deletes the currently authenticated user's account (requires password confirmation)
app.MapPost("/api/users/delete-me", async (DeleteAccountRequest request, ClaimsPrincipal user, UserManager<ApplicationUser> userManager) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

    var appUser = await userManager.FindByIdAsync(userId);
    if (appUser == null) return Results.NotFound();

    var isPasswordValid = await userManager.CheckPasswordAsync(appUser, request.Password);
    if (!isPasswordValid)
    {
        return Results.BadRequest("Invalid password. Identity confirmation failed.");
    }

    var result = await userManager.DeleteAsync(appUser);
    if (result.Succeeded)
    {
        return Results.Ok();
    }

    return Results.BadRequest(result.Errors);
})
.RequireAuthorization();

// SIGNALR HUB
app.MapHub<ChatHub>("/chathub");

// Gets the chat history for a specific room
app.MapGet("/api/messages", async (string roomName, IMessageService messageService) =>
{
    var history = await messageService.GetChatHistoryAsync(roomName);
    return Results.Ok(history);
})
.RequireAuthorization();

// Edits an existing message
app.MapPut("/api/messages/{id:guid}", async (Guid id, [FromBody] string text, ClaimsPrincipal user, IMessageService messageService) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var success = await messageService.UpdateMessageAsync(id, text, userId!);
    return success ? Results.Ok() : Results.BadRequest();
}).RequireAuthorization();

// Deletes a message
app.MapDelete("/api/messages/{id:guid}", async (Guid id, ClaimsPrincipal user, IMessageService messageService) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var success = await messageService.DeleteMessageAsync(id, userId!);
    return success ? Results.Ok() : Results.BadRequest();
}).RequireAuthorization();

// Gets all available chat rooms
app.MapGet("/api/rooms", async (IRoomService roomService) =>
{
    var rooms = await roomService.GetRoomsAsync();
    return Results.Ok(rooms);
})
.RequireAuthorization();

// Creates a new chat room
app.MapPost("/api/rooms", async (RoomRequest request, ClaimsPrincipal user, IRoomService roomService) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

    var success = await roomService.CreateRoomAsync(request, userId); 
    if (!success) return Results.BadRequest("Room with this name already exists.");
    return Results.Ok();
}).RequireAuthorization();

// Renames an existing chat room
app.MapPut("/api/rooms/{id:guid}", async (Guid id, RoomRequest request, ClaimsPrincipal user, IRoomService roomService) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

    var success = await roomService.UpdateRoomAsync(id, request, userId);
    return success ? Results.Ok() : Results.BadRequest("Failed to update room.");
}).RequireAuthorization();

// Deletes a chat room
app.MapDelete("/api/rooms/{id:guid}", async (Guid id, ClaimsPrincipal user, IRoomService roomService) =>
{
    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

    var success = await roomService.DeleteRoomAsync(id, userId);
    return success ? Results.Ok() : Results.BadRequest("Failed to delete room.");
})
.RequireAuthorization();

app.Run();