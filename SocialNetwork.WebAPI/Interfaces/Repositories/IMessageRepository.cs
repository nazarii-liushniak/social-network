using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IMessageRepository
{
    void AddMessage(Message message);
    Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(
        Guid userId,
        Guid otherUserId,
        DateTimeOffset? timestamp,
        Guid? messageId,
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ChatResponse>> GetChatsAsync(
        Guid userId,
        DateTimeOffset? timestamp,
        Guid? otherUserId,
        int limit,
        CancellationToken cancellationToken = default);
    void DeleteMessage(Message message);
    Task DeleteUserMessagesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}