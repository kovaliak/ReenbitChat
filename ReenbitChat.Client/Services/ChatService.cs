using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using ReenbitChat.Shared.Enums;
using ReenbitChat.Shared.Services;

namespace ReenbitChat.Client.Services;

/// <summary>
/// Client-side SignalR service responsible for real-time bidirectional communication.
/// </summary>
public class ChatService : IChatService, IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _hubUrl;
    private HubConnection? _hubConnection;

    public event Action<Guid, string, string, Sentiment>? OnMessageReceived;
    public event Action<string>? OnSystemMessageReceived;
    public event Action<Guid, string>? OnMessageUpdated;
    public event Action<Guid>? OnMessageDeleted;

    public ChatService(IJSRuntime jsRuntime, IConfiguration config) 
    {
        _jsRuntime = jsRuntime;
        
        var apiUrl = config["ApiBaseUrl"] ?? throw new Exception("ApiBaseUrl is not configured");
        _hubUrl = $"{apiUrl.TrimEnd('/')}/chathub"; 
    }

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public async Task ConnectAsync()
    {
        if (IsConnected) return;
        
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
        if (string.IsNullOrEmpty(token)) throw new Exception("Unauthorized");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token)!;
            })
            .Build();

        _hubConnection.On<Guid, string, string, Sentiment>("ReceiveMessage", (id, user, message, sentiment) => 
            OnMessageReceived?.Invoke(id, user, message, sentiment));        
        _hubConnection.On<string>("ReceiveSystemMessage", (msg) => OnSystemMessageReceived?.Invoke(msg));
        _hubConnection.On<Guid, string>("MessageUpdated", (id, text) => OnMessageUpdated?.Invoke(id, text));
        _hubConnection.On<Guid>("MessageDeleted", (id) => OnMessageDeleted?.Invoke(id));

        await _hubConnection.StartAsync();
    }

    public async Task SendMessageAsync(string roomName, string message) => 
        await _hubConnection!.SendAsync("SendMessage", roomName, message);

    public async Task UpdateMessageAsync(string roomName, Guid messageId, string newText) => 
        await _hubConnection!.SendAsync("UpdateMessage", roomName, messageId, newText);

    public async Task DeleteMessageAsync(string roomName, Guid messageId) => 
        await _hubConnection!.SendAsync("DeleteMessage", roomName, messageId);

    public async Task JoinRoom(string roomName) => await _hubConnection!.InvokeAsync("JoinRoom", roomName);
    public async Task LeaveRoom(string roomName) => await _hubConnection!.InvokeAsync("LeaveRoom", roomName);
    public async Task DisconnectAsync() { if (_hubConnection != null) await _hubConnection.DisposeAsync(); }
    public async ValueTask DisposeAsync() => await DisconnectAsync();
}