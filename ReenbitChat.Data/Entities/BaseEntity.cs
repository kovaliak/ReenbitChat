namespace ReenbitChat.Data.Entities;

/// <summary>
/// An abstract base class providing common properties like Id and audit timestamps for domain entities.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}