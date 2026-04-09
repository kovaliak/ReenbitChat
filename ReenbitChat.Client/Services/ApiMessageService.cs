using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using ReenbitChat.Shared.Dtos.MessageDto;
using ReenbitChat.Shared.Services;

namespace ReenbitChat.Client.Services;

/// <summary>
/// Client-side service that communicates with the backend API for message history and management.
/// </summary>
public class ApiMessageService : IMessageService 
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public ApiMessageService(HttpClient httpClient, IJSRuntime jsRuntime)
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

    public async Task<List<MessageResponse>> GetChatHistoryAsync(string roomName)
    {
        await SetAuthHeader();
        var messages = await _httpClient.GetFromJsonAsync<List<MessageResponse>>($"api/messages?roomName={roomName}");
        return messages ?? new List<MessageResponse>();
    }

    public async Task<bool> UpdateMessageAsync(Guid id, string newText, string userId)
    {
        await SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/messages/{id}", newText);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMessageAsync(Guid id, string userId)
    {
        await SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"api/messages/{id}");
        return response.IsSuccessStatusCode;
    }
}