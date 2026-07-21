using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Interfaces.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageResponse messageResponse);
}