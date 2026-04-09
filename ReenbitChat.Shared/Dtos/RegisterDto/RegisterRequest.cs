namespace ReenbitChat.Shared.Dtos.RegisterDto;

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string DisplayName { get; set; }
}