using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Models.Message;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Repositories;

public class MessageRepository(SocialNetworkDbContext context) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public async Task<IReadOnlyList<MessageResponse>> GetMessagesAsync(Guid userId,
        Guid otherUserId,
        DateTimeOffset? timestamp,
        Guid? messageId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);

        var query = context.Messages
            .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) 
                    || (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderByDescending(m => m.SentAt)
            .ThenBy(m => m.Id)
            .AsQueryable();

        if (timestamp.HasValue && messageId.HasValue)
        {
            query = query.Where(m => m.SentAt < timestamp.Value
                || (m.SentAt == timestamp.Value && m.Id > messageId.Value));
        }
            
        return await query
            .Take(limit)
            .Select(m => new MessageResponse()
            {
                Id = m.Id,
                Direction = m.SenderId == userId
                    ? MessageDirection.Outgoing
                    : MessageDirection.Incoming,
                Content = m.Content,
                SentAt = m.SentAt,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChatResponse>> GetChatsAsync(Guid userId,
        DateTimeOffset? timestamp,
        Guid? otherUserId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);

        var query = context.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => new
            {
                Message = m,
                OtherUserId = m.SenderId == userId ? m.ReceiverId : m.SenderId,
            })
            .GroupBy(a => a.OtherUserId)
            .Select(g => g
                .OrderByDescending(a => a.Message.SentAt)
                .Select(a => new ChatResponse()
                    {
                        OtherUser = new Author()
                        {
                            Id = a.Message.SenderId == userId
                                ? a.Message.Receiver.Id
                                : a.Message.Sender.Id,
                            Username = a.Message.SenderId == userId
                                ? a.Message.Receiver.Username
                                : a.Message.Sender.Username,
                            FullName = a.Message.SenderId == userId
                                ? a.Message.Receiver.FullName
                                : a.Message.Sender.FullName,
                            ProfileImageUrl = a.Message.SenderId == userId
                                ? a.Message.Receiver.ProfileImageUrl
                                : a.Message.Sender.ProfileImageUrl,
                        },
                        LastMessageContent = a.Message.Content,
                        LastMessageTimestamp = a.Message.SentAt,
                    })
                .First()
            );
        
        if (timestamp.HasValue && otherUserId.HasValue)
        {
            query = query.Where(c => c.LastMessageTimestamp < timestamp.Value 
                || (c.LastMessageTimestamp == timestamp.Value && c.OtherUser.Id > otherUserId.Value));
        }

        return await query
            .OrderByDescending(c => c.LastMessageTimestamp)
            .ThenBy(c => c.OtherUser.Id)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task DeleteUserMessagesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await context.Messages
            .Where(m => m.SenderId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}