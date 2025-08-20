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

    public async Task<IEnumerable<Message>> GetMessagesAsync(Guid senderId, Guid receiverId)
    {
        var messages = await _context.Messages
            .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId)
            .ToListAsync();
        
        return messages;
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