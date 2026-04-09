using System.Net.Http.Json;
using Microsoft.JSInterop;
using ReenbitChat.Shared.Dtos.LoginDto;
using ReenbitChat.Shared.Dtos.RegisterDto;
using ReenbitChat.Shared.Dtos.UserProfileDto;
using ReenbitChat.Shared.Services;

namespace ReenbitChat.Client.Services;

/// <summary>
/// Client-side service that communicates with the backend API for authentication and user management.
/// </summary>
public class ApiAuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public ApiAuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/login", request);

        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            var token = loginResponse?.AccessToken ?? string.Empty;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", token);
            return token;
        }
        else
        {   
            throw new Exception("Login failed");
        }
    }

    public async Task<string?> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register-custom", request);

        if (response.IsSuccessStatusCode)
            return null;

        var error = await response.Content.ReadAsStringAsync();
        return "Registration failed. " + error;
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserProfileResponse>("api/users/me");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user profile: {ex.Message}");
            return null;
        }
    }
    public async Task<string?> ChangePasswordAsync(PasswordRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/manage/info", request);

        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        var error = await response.Content.ReadAsStringAsync();
        return "Failed to change password. Check your inputs.";
    }

    public async Task<string?> DeleteAccountAsync(DeleteAccountRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/delete-me", request);

        if (response.IsSuccessStatusCode)
        {
            await LogoutAsync(); 
            return null;
        }

        var error = await response.Content.ReadAsStringAsync();
        return "Failed to delete account. " + error;
    }
}