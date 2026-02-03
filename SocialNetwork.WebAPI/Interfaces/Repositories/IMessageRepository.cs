using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message> AddMessageAsync(Message message);
    Task<IEnumerable<Message>> GetMessagesAsync(
        Guid userId,
        Guid otherUserId,
        DateTime timestamp,
        Guid messageId,
        int limit);

    Task<IEnumerable<(
        User OtherUser,
        string LastMessageContent,
        DateTime LastMessageTimestamp
    )>> GetChatsAsync(Guid userId);
    Task<Message?> UpdateMessageAsync(Message message);
    Task<Message?> DeleteMessageAsync(Guid messageId);
}