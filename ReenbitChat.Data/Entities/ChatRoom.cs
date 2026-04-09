using System.ComponentModel.DataAnnotations.Schema;

namespace ReenbitChat.Data.Entities;

/// <summary>
/// Represents a chat room where users can join and exchange messages.
/// </summary>
[Table("chat_rooms")]
public class ChatRoom : BaseEntity
{
    public required string Name { get; set; }
    public string? CreatorId { get; set; }
    
    /// <summary> Navigation property to the user who created the room. </summary>
    public ApplicationUser? Creator { get; set; } = null!;
}