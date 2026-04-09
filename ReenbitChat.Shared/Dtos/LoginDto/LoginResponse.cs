namespace ReenbitChat.Shared.Dtos.LoginDto;

public record LoginResponse
{
    public required string AccessToken { get; set; }
}