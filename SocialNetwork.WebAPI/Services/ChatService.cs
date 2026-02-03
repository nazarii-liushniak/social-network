using SocialNetwork.WebAPI.Helpers;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models.Message;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Services;

public class ChatService(
    IUserRepository userRepository,
    IMessageRepository messageRepository
) : IChatService
{
    public async Task<IEnumerable<Chat>?> GetChatsAsync(Guid userId)
    {
        var userExists = await userRepository.ExistsUserAsync(userId);
        if (!userExists)
            return null;
        
        var chats = await messageRepository
            .GetChatsAsync(userId);

        var chatModels = chats.Select(c => new Chat
        {
            OtherUser = new ShortProfile
            {
                Id = c.OtherUser.Id,
                Username = c.OtherUser.Username,
                FullName = c.OtherUser.FullName,
                ProfileImageUrl = c.OtherUser.ProfileImageUrl,
            },
            LastMessageContent = c.LastMessageContent,
            LastMessageTimestamp = c.LastMessageTimestamp,
        });
        
        return chatModels;
    }

    public async Task<Messages?> GetChatAsync(
        Guid userId,
        Guid otherUserId,
        string? cursor,
        int limit)
    {
        var otherUserExists = await userRepository.ExistsUserAsync(otherUserId);
        if (!otherUserExists)
            return null;
        
        DateTime timestamp;
        Guid messageId;

        if (cursor == null)
        {
            timestamp = DateTime.UtcNow;
            messageId = Guid.Empty;
        }
        else
        {
            (timestamp, messageId) = CursorHelper.ParseCursor(cursor);
        }

        var messageEntities = (await messageRepository
            .GetMessagesAsync(userId, otherUserId, timestamp, messageId, limit))
            .ToList();

        var messagesModels = messageEntities.Select(m => new Message
        {
            Id = m.Id,
            Direction = m.ReceiverId == userId
                ? MessageDirection.Incoming
                : MessageDirection.Outgoing,
            Content = m.Content,
            Timestamp = m.SentAt,
        })
        .ToList();
        
        var lastMessage = messagesModels.LastOrDefault();
        var messages = new Messages
        {
            Items = messagesModels,
            NextCursor = CursorHelper.GenerateCursor(
                lastMessage?.Timestamp ?? DateTime.UtcNow,
                lastMessage?.Id ?? Guid.Empty),
        };

        return messages;
    }
}