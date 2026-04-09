namespace ReenbitChat.Shared.Dtos.RoomDto;

public record RoomResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? CreatorId { get; set; }
}