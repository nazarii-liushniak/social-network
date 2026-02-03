using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class MessageRepository(SocialNetworkDbContext context) : IMessageRepository
{
    private readonly SocialNetworkDbContext _context = context;

    public async Task<Message> AddMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        
        await _context.SaveChangesAsync();
        
        return message;
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(
        Guid userId,
        Guid otherUserId,
        DateTime timestamp,
        Guid messageId,
        int limit)
    {
        var messages = await _context.Messages
            .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) 
                    || (m.SenderId == otherUserId && m.ReceiverId == userId))
            .OrderByDescending(m => m.SentAt)
            .ThenBy(m => m.Id)
            .Where(m => m.SentAt < timestamp ||
                        (m.SentAt == timestamp && m.ReceiverId > otherUserId))
            .Take(limit)
            .ToListAsync();
        
        return messages;
    }

    public async Task<IEnumerable<(
            User OtherUser,
            string LastMessageContent,
            DateTime LastMessageTimestamp
    )>> GetChatsAsync(Guid userId)
    {
        var chats = await _context.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => new
            {
                Message = m,
                OtherUserId = m.SenderId == userId ? m.ReceiverId : m.SenderId
            })
            .GroupBy(x => x.OtherUserId)
            .Select(g => g
                .OrderByDescending(x => x.Message.SentAt)
                .Select(x => new
                {
                    OtherUser = x.Message.SenderId == userId
                        ? x.Message.Receiver
                        : x.Message.Sender,
                    LastMessageContent = x.Message.Content,
                    LastMessageSentAt = x.Message.SentAt,
                })
                .FirstOrDefault()
            )
            .OrderByDescending(c => c!.LastMessageSentAt)
            .ToListAsync();

        return chats.Select(c => (c.OtherUser, c.LastMessageContent, c.LastMessageSentAt));
    }

    public async Task<Message?> UpdateMessageAsync(Message message)
    {
        var dbMessage = await _context.Messages.FindAsync(message.Id);
        
        if (dbMessage == null)
            return null;
        
        message.Id = dbMessage.Id;
        message.SenderId = dbMessage.SenderId;
        message.ReceiverId = dbMessage.ReceiverId;
        message.SentAt = dbMessage.SentAt;
        
        _context.Messages.Update(message);
        
        await _context.SaveChangesAsync();
        
        return message;
    }

    public async Task<Message?> DeleteMessageAsync(Guid messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        
        if (message == null)
            return null;
        
        _context.Messages.Remove(message);
        
        await _context.SaveChangesAsync();
        
        return message;
    }
}