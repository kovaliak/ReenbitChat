using ReenbitChat.Shared.Dtos.LoginDto;
using ReenbitChat.Shared.Dtos.RegisterDto;
using ReenbitChat.Shared.Dtos.UserProfileDto;

namespace ReenbitChat.Shared.Services;

/// <summary>
/// Defines authentication and user management operations.
/// </summary>
public interface IAuthService
{
    /// <summary> Authenticates a user and retrieves a JWT access token. </summary>
    Task<string> LoginAsync(LoginRequest request);
    
    /// <summary> Registers a new user account. Returns an error message if failed, or null if successful. </summary>
    Task<string?> RegisterAsync(RegisterRequest request);
    
    /// <summary> Logs out the current user by removing their local credentials. </summary>
    Task LogoutAsync();
    
    /// <summary> Retrieves the profile information of the currently authenticated user. </summary>
    Task<UserProfileResponse?> GetUserProfileAsync();
    
    /// <summary> Changes the password for the current user. Returns an error message if failed. </summary>
    Task<string?> ChangePasswordAsync(PasswordRequest request);
    
    /// <summary> Deletes the user account permanently. Returns an error message if failed. </summary>
    Task<string?> DeleteAccountAsync(DeleteAccountRequest request);
}