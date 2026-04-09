using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ReenbitChat.Shared.Services;
using System.Security.Claims;

namespace ReenbitChat.Api.Hubs;

/// <summary>
/// SignalR Hub responsible for real-time communication between clients.
/// Handles joining rooms, sending, updating, and deleting messages.
/// </summary>
[Authorize] 
public class ChatHub : Hub
{
    private readonly IMessageStorageService _messageService;

    public ChatHub(IMessageStorageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary> Helper method to extract the display name from the JWT token claims. </summary>
    private string GetUserDisplayName()
    {
        return Context.User?.FindFirst("DisplayName")?.Value 
               ?? Context.User?.FindFirst(ClaimTypes.Email)?.Value 
               ?? "Unknown";
    }

    /// <summary> Adds the current user to a specific chat room group. </summary>
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        
        var displayName = GetUserDisplayName();
        await Clients.Group(roomName).SendAsync("ReceiveSystemMessage", $"{displayName} connected to the chat");
    }

    /// <summary> Removes the current user from a specific chat room group. </summary>
    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        
        var displayName = GetUserDisplayName();
        await Clients.Group(roomName).SendAsync("ReceiveSystemMessage", $"{displayName} disconnected from the chat");
    }

    /// <summary> Processes and broadcasts a new message to all clients in the room. </summary>
    public async Task SendMessage(string roomName, string text)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            throw new HubException("User ID not found in token.");

        var displayName = GetUserDisplayName();

        // Save message to DB and perform Azure Sentiment Analysis
        var result = await _messageService.SaveMessageAsync(text, roomName, userId);

        await Clients.Group(roomName).SendAsync("ReceiveMessage", result.Id, displayName, text, result.Sentiment);
    }

    /// <summary> Broadcasts a deletion event to remove a message from clients' UI. </summary>
    public async Task DeleteMessage(string roomName, Guid messageId)
    {
        await Clients.Group(roomName).SendAsync("MessageDeleted", messageId);
    }

    /// <summary> Broadcasts an update event to modify an existing message on clients' UI. </summary>
    public async Task UpdateMessage(string roomName, Guid messageId, string newText)
    {
        await Clients.Group(roomName).SendAsync("MessageUpdated", messageId, newText);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}