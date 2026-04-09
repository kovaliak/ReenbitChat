using ReenbitChat.Shared.Enums;

namespace ReenbitChat.Shared.Dtos.MessageDto;

public record MessageResponse
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public required string UserName { get; set; }
    public required string Text { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Sentiment Sentiment { get; set; } = Sentiment.NotAnalyzed;
}