using ReenbitChat.Shared.Enums;

namespace ReenbitChat.Shared.Services;

/// <summary>
/// Handles the persistence of chat messages and sentiment analysis.
/// </summary>
public interface IMessageStorageService
{
    /// <summary> Saves a new message to the database and evaluates its sentiment using Azure Cognitive Services. </summary>
    Task<(Guid Id, Sentiment Sentiment)> SaveMessageAsync(string text, string roomName, string userId);
}