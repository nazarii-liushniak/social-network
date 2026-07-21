using FluentResults;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Errors;
using SocialNetwork.WebAPI.Helpers;
using SocialNetwork.WebAPI.Hubs;
using SocialNetwork.WebAPI.Interfaces.Contexts;
using SocialNetwork.WebAPI.Interfaces.Hubs;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Services;

public class ChatService(
    TimeProvider timeProvider,
    IUserContext userContext,
    IUserRepository userRepository,
    IMessageRepository messageRepository,
    IHubContext<ChatHub, IChatClient> hubContext) : IChatService
{
    public async Task<Result<MessageResponse>> SendMessageAsync(
        Guid otherUserId,
        CreateMessageRequest createMessageRequest,
        CancellationToken cancellationToken = default)
    {
        var existsUser = await userRepository.ExistsUserAsync(otherUserId, cancellationToken);
        if (!existsUser)
            return Result.Fail(new NotFoundError($"User with ID {otherUserId} not found"));
        
        var currentUserId = userContext.UserId!.Value;
        
        var message = new Message()
        {
            Id = Guid.Empty,
            SenderId = currentUserId,
            ReceiverId = otherUserId,
            Content = createMessageRequest.Content,
            SentAt = timeProvider.GetUtcNow(),
        };

        messageRepository.AddMessage(message);
        await messageRepository.SaveChangesAsync(cancellationToken);
        
        var messageResponse = new MessageResponse()
        {
            Id = message.Id,
            Direction = MessageDirection.Outgoing,
            Content = message.Content,
            SentAt = message.SentAt,
        };
        
        await hubContext.Clients
            .User(otherUserId.ToString())
            .ReceiveMessage(messageResponse with { Direction = MessageDirection.Incoming });

        return messageResponse;
    }

    public async Task<Result<PagedResponse<ChatResponse>>> GetChatsAsync(
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));
        
        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var userId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));
        
        var currentUserId = userContext.UserId!.Value;

        var chats = await messageRepository
            .GetChatsAsync(currentUserId, timestamp, userId, limit + 1, cancellationToken);

        string? nextCursor = null;
        if (chats.Count > limit)
        {
            chats = chats.SkipLast(1).ToList();

            var lastChat = chats.Last();
            nextCursor = CursorHelper.GenerateCursor(lastChat.LastMessageTimestamp, lastChat.OtherUser.Id);
        }
        
        return new PagedResponse<ChatResponse>(chats, nextCursor);
    }

    public async Task<Result<PagedResponse<MessageResponse>>> GetChatAsync(
        Guid otherUserId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));
        
        var existsOtherUser = await userRepository.ExistsUserAsync(otherUserId, cancellationToken);
        if (!existsOtherUser)
            return Result.Fail(new NotFoundError($"User with ID {otherUserId} not found"));
        
        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var messageId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));
        
        var currentUserId = userContext.UserId!.Value;

        var messages = await messageRepository.GetMessagesAsync(currentUserId, otherUserId, timestamp, messageId, limit + 1, cancellationToken);
        
        string? nextCursor = null;
        if (messages.Count > limit)
        {
            messages = messages.SkipLast(1).ToList();

            var lastMessage = messages.Last();
            nextCursor = CursorHelper.GenerateCursor(lastMessage.SentAt, lastMessage.Id);
        }

        return new PagedResponse<MessageResponse>(messages, nextCursor);
    }
}