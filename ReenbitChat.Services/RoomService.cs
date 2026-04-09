using Microsoft.EntityFrameworkCore;
using ReenbitChat.Data.Contexts;
using ReenbitChat.Data.Entities;
using ReenbitChat.Shared.Dtos.RoomDto;
using ReenbitChat.Shared.Services;

namespace ReenbitChat.Services;

/// <summary>
/// Server-side implementation for managing chat rooms in the database.
/// </summary>
public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _dbContext;

    public RoomService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<RoomResponse>> GetRoomsAsync()
    {
        return await _dbContext.ChatRooms
            .Select(r => new RoomResponse { Id = r.Id, Name = r.Name, CreatorId = r.CreatorId })
            .ToListAsync();
    }

    public async Task<bool> CreateRoomAsync(RoomRequest request, string userId)
    {
        if (await _dbContext.ChatRooms.AnyAsync(r => r.Name == request.Name)) return false;

        var room = new ChatRoom { Name = request.Name, CreatorId = userId };
        _dbContext.ChatRooms.Add(room);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRoomAsync(Guid id, RoomRequest request, string userId)
    {
        var room = await _dbContext.ChatRooms.FindAsync(id);
        
        if (room == null || room.CreatorId != userId) return false;
        
        if (await _dbContext.ChatRooms.AnyAsync(r => r.Name == request.Name && r.Id != id)) return false;

        room.Name = request.Name;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRoomAsync(Guid id, string userId)
    {
        var room = await _dbContext.ChatRooms.FindAsync(id);
        
        if (room == null || room.CreatorId != userId) return false;

        _dbContext.ChatRooms.Remove(room);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public event Action? OnRoomsUpdated;
    public void NotifyRoomsUpdated() => OnRoomsUpdated?.Invoke();
}