using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IChatService
{
    public IEnumerable<Chat> GetChats(Guid userId);
    public Chat GetChat(Guid userId, Guid anotherUserId);
}