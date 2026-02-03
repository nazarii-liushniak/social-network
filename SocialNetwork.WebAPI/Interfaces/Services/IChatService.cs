using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IChatService
{
    public Task<IEnumerable<Chat>?> GetChatsAsync(Guid userId);
    public Task<Messages?> GetChatAsync(
        Guid userId,
        Guid otherUserId,
        string? cursor,
        int limit);
}