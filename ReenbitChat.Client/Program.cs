using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ReenbitChat.Client;
using ReenbitChat.Client.Providers;
using ReenbitChat.Client.Services;
using ReenbitChat.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

var apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("API base URL is not configured.");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
builder.Services.AddScoped<IAuthService, ApiAuthService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IMessageService, ApiMessageService>();
builder.Services.AddScoped<IRoomService, ApiRoomService>();

await builder.Build().RunAsync();
