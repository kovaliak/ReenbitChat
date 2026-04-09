using System.ComponentModel.DataAnnotations.Schema;
using ReenbitChat.Shared.Enums;

namespace ReenbitChat.Data.Entities;

/// <summary>
/// Represents a single chat message sent by a user within a specific chat room.
/// </summary>
[Table("messages")]
public class Message : BaseEntity
{
    /// <summary> The actual text content of the message. </summary>
    public required string Text { get; set; }
    
    public string? UserId { get; set; }
    public required Guid ChatRoomId { get; set; }
    
    /// <summary> Navigation property to the user who sent the message. </summary>
    public ApplicationUser? User { get; set; } = null!;
    
    /// <summary> Navigation property to the chat room where the message belongs. </summary>
    public ChatRoom ChatRoom { get; set; } = null!;
    
    /// <summary> The result of Azure Cognitive Services sentiment analysis. </summary>
    public required Sentiment Sentiment { get; set; }
}