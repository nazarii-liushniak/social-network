using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Models.Message;
using MessageEntity = SocialNetwork.WebAPI.Entities.Message;

namespace SocialNetwork.WebAPI.Hubs;

[Authorize]
public class ChatHub(IMessageRepository messageRepository) : Hub
{
    public async Task SendMessage(CreateMessage createMessage)
    {
        var message = new MessageEntity
        {
            Id = Guid.NewGuid(),
            SenderId = Guid.Parse(Context.UserIdentifier!),
            ReceiverId = createMessage.ReceiverId,
            Content = createMessage.Content,
            SentAt = DateTime.UtcNow,
        };
        
        await messageRepository.AddMessageAsync(message);

        var messageForSender = new Message
        {
            Id = message.Id,
            Direction = MessageDirection.Outgoing,
            Content = message.Content,
            Timestamp = message.SentAt,
        };

        var messageForReceiver = new Message
        {
            Id = message.Id,
            Direction = MessageDirection.Incoming,
            Content = message.Content,
            Timestamp = message.SentAt,
        };
        
        await Clients.User(createMessage.ReceiverId.ToString())
            .SendAsync("ReceiveMessage", messageForReceiver);
        await Clients.Caller.SendAsync("ReceiveMessage", messageForSender);
    }
}