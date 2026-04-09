using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReenbitChat.Data.Contexts;
using ReenbitChat.Data.Entities;
using ReenbitChat.Shared.Dtos.MessageDto;
using ReenbitChat.Shared.Enums;
using ReenbitChat.Shared.Services;

namespace ReenbitChat.Services;

/// <summary>
/// Server-side service for managing messages and integrating Azure Text Analytics.
/// </summary>
public class MessageService : IMessageService, IMessageStorageService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly TextAnalyticsClient? _textAnalyticsClient;

    public MessageService(ApplicationDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;

        var endpoint = config["AzureTextAnalytics:Endpoint"];
        var apiKey = config["AzureTextAnalytics:ApiKey"];

        // Initialize Azure Text Analytics Client if credentials are provided
        if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(apiKey))
        {
            var credentials = new AzureKeyCredential(apiKey);
            var uri = new Uri(endpoint);
            _textAnalyticsClient = new TextAnalyticsClient(uri, credentials);
        }
    }

    /// <summary>
    /// Saves a message to the database and performs sentiment analysis if configured.
    /// </summary>
    public async Task<(Guid Id, Sentiment Sentiment)> SaveMessageAsync(string text, string roomName, string userId)
    {
        var room = await _dbContext.ChatRooms.FirstOrDefaultAsync(r => r.Name == roomName);
        if (room == null)
        {
            room = new ChatRoom { Name = roomName, CreatorId = userId };
            _dbContext.ChatRooms.Add(room);
        }

        var sentimentEnum = Sentiment.NotAnalyzed;
        if (_textAnalyticsClient != null)
        {
            try
            {
                DocumentSentiment docSentiment = await _textAnalyticsClient.AnalyzeSentimentAsync(text);
                sentimentEnum = docSentiment.Sentiment switch
                {
                    TextSentiment.Positive => Sentiment.Positive,
                    TextSentiment.Negative => Sentiment.Negative,
                    TextSentiment.Neutral => Sentiment.Neutral,
                    TextSentiment.Mixed => Sentiment.Neutral,
                    _ => Sentiment.NotAnalyzed
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Azure AI Error: {ex.Message}");
            }
        }

        var message = new Message
        {
            Text = text,
            UserId = userId,
            ChatRoomId = room.Id,
            Sentiment = sentimentEnum
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        return (message.Id, message.Sentiment);
    }

    public async Task<List<MessageResponse>> GetChatHistoryAsync(string roomName)
    {
        return await _dbContext.Messages
            .Where(m => m.ChatRoom.Name == roomName)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new MessageResponse
            {
                Id = m.Id,
                UserName = m.User!.DisplayName ?? "Unknown", 
                Text = m.Text,
                CreatedAt = m.CreatedAt,
                Sentiment = m.Sentiment
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateMessageAsync(Guid id, string newText, string userId)
    {
        var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (message == null) return false;
        
        message.Text = newText;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMessageAsync(Guid id, string userId)
    {
        var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (message == null) return false;
        _dbContext.Messages.Remove(message);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}