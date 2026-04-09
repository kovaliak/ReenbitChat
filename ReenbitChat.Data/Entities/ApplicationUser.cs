using Microsoft.AspNetCore.Identity;

namespace ReenbitChat.Data.Entities;

/// <summary>
/// Custom application user extending the default ASP.NET Core Identity user.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary> The public name displayed to other users in the chat UI. </summary>
    public string? DisplayName { get; set; }
}