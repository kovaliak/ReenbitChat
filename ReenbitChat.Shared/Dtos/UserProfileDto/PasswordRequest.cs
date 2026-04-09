namespace ReenbitChat.Shared.Dtos.UserProfileDto;

public class PasswordRequest
{
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}