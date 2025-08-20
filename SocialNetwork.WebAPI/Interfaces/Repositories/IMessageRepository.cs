using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message> AddMessageAsync(Message message);
    Task<IEnumerable<Message>> GetMessagesAsync(Guid senderId, Guid receiverId);
    Task<Message?> UpdateMessageAsync(Message message);
    Task<Message?> DeleteMessageAsync(Guid messageId);
}