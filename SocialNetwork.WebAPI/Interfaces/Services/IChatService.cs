using FluentResults;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IChatService
{
    Task<Result<MessageResponse>> SendMessageAsync(
        Guid otherUserId,
        CreateMessageRequest createMessageRequest,
        CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<ChatResponse>>> GetChatsAsync(
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<MessageResponse>>> GetChatAsync(
        Guid otherUserId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
}