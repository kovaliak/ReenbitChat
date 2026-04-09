using ReenbitChat.Shared.Enums;

namespace ReenbitChat.Shared.Services;

/// <summary>
/// Handles real-time chat communication using SignalR.
/// </summary>
public interface IChatService
{
    /// <summary> Triggered when a new user message is received. </summary>
    event Action<Guid, string, string, Sentiment>? OnMessageReceived;
    
    /// <summary> Triggered when a system notification (e.g., user joined/left) is received. </summary>
    event Action<string>? OnSystemMessageReceived;
    
    /// <summary> Triggered when an existing message is edited. </summary>
    event Action<Guid, string>? OnMessageUpdated;
    
    /// <summary> Triggered when a message is deleted. </summary>
    event Action<Guid>? OnMessageDeleted;
    
    /// <summary> Indicates whether the SignalR connection is currently active. </summary>
    bool IsConnected { get; }
    
    /// <summary> Establishes a connection to the SignalR chat hub. </summary>
    Task ConnectAsync();
    
    /// <summary> Sends a message to a specific chat room. </summary>
    Task SendMessageAsync(string roomName, string message);
    
    /// <summary> Updates an existing message in real-time. </summary>
    Task UpdateMessageAsync(string roomName, Guid messageId, string newText);
    
    /// <summary> Deletes a message in real-time. </summary>
    Task DeleteMessageAsync(string roomName, Guid messageId);
    
    /// <summary> Disconnects from the SignalR chat hub. </summary>
    Task DisconnectAsync();
    
    /// <summary> Joins a specific chat room group. </summary>
    Task JoinRoom(string roomName);
    
    /// <summary> Leaves a specific chat room group. </summary>
    Task LeaveRoom(string roomName);
}