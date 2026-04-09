namespace ReenbitChat.Shared.Dtos.RoomDto;

public record RoomRequest
{
    public required string Name { get; set; }
}