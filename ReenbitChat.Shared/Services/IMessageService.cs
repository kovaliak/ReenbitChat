using ReenbitChat.Shared.Dtos.MessageDto;

namespace ReenbitChat.Shared.Services;

/// <summary>
/// Provides operations for retrieving and managing chat messages history.
/// </summary>
public interface IMessageService
{
    /// <summary> Retrieves the chat history for a specific room. </summary>
    Task<List<MessageResponse>> GetChatHistoryAsync(string roomName);
    
    /// <summary> Updates the text of an existing message. </summary>
    Task<bool> UpdateMessageAsync(Guid id, string newText, string userId);
    
    /// <summary> Deletes a message by its ID. </summary>
    Task<bool> DeleteMessageAsync(Guid id, string userId);
}