using ReenbitChat.Shared.Dtos.RoomDto;

namespace ReenbitChat.Shared.Services;

/// <summary>
/// Defines operations for managing chat rooms.
/// </summary>
public interface IRoomService
{
    /// <summary> Retrieves all available chat rooms. </summary>
    Task<List<RoomResponse>> GetRoomsAsync();
    
    /// <summary> Creates a new chat room. Returns true if successful. </summary>
    Task<bool> CreateRoomAsync(RoomRequest request, string userId = "");
    
    /// <summary> Updates the name of an existing chat room. </summary>
    Task<bool> UpdateRoomAsync(Guid id, RoomRequest request, string userId = "");
    
    /// <summary> Deletes a chat room by its ID. </summary>
    Task<bool> DeleteRoomAsync(Guid id, string userId = "");

    /// <summary> Event triggered when the room list is updated. </summary>
    event Action? OnRoomsUpdated;
    
    /// <summary> Notifies subscribers that the room list has been updated. </summary>
    void NotifyRoomsUpdated();
}