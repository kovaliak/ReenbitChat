using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace ReenbitChat.Client.Providers;

/// <summary>
/// Custom AuthenticationStateProvider for Blazor WebAssembly.
/// Manages user authentication state by reading the JWT token from local storage.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieves the current authentication state. Extracts token from local storage and validates it.
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");

        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var userInfo = await _httpClient.GetFromJsonAsync<UserInfoResponse>("/manage/info");
            
            if (userInfo != null)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, userInfo.Email) };
                var identity = new ClaimsIdentity(claims, "Bearer");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
        }
        catch
        {
            // If token is invalid or expired, remove it and log out
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "accessToken");
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    /// <summary>
    /// Updates the application state to authenticated immediately after a successful login.
    /// </summary>
    public void MarkUserAsAuthenticated(string token, string email)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var claims = new[] { new Claim(ClaimTypes.Name, email) };
        var identity = new ClaimsIdentity(claims, "Bearer");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}

public class UserInfoResponse
{
    public string Email { get; set; } = string.Empty;
}