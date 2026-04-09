using System.Net.Http.Json;
using Microsoft.JSInterop;
using ReenbitChat.Shared.Dtos.RoomDto;
using ReenbitChat.Shared.Services;
using System.Net.Http.Headers;

namespace ReenbitChat.Client.Services;

/// <summary>
/// Client-side service that communicates with the backend API to manage chat rooms.
/// </summary>
public class ApiRoomService : IRoomService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public ApiRoomService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    private async Task SetAuthHeader()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<RoomResponse>> GetRoomsAsync()
    {
        await SetAuthHeader();
        return await _httpClient.GetFromJsonAsync<List<RoomResponse>>("api/rooms") ?? new();
    }

    public async Task<bool> CreateRoomAsync(RoomRequest request, string userId = "")
    {
        await SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/rooms", request); 
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateRoomAsync(Guid id, RoomRequest request, string userId = "")
    {
        await SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/rooms/{id}", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteRoomAsync(Guid id, string userId = "")
    {
        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"api/rooms/{id}");
        return response.IsSuccessStatusCode;
    }

    public event Action? OnRoomsUpdated;
    public void NotifyRoomsUpdated() => OnRoomsUpdated?.Invoke();
}