using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IChatService
{
    public Task<Chats?> GetChatsAsync(Guid userId, string? cursor, int limit);
    public Task<Messages?> GetChatAsync(
        Guid userId,
        Guid otherUserId,
        string? cursor,
        int limit);
}