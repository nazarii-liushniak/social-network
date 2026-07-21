using FluentResults;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.Comment;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface ICommentService
{
    Task<Result<CommentResponse>> CreateCommentAsync(
        Guid postId,
        CreateOrUpdateCommentRequest createOrUpdateCommentRequest,
        CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<CommentResponse>>> GetCommentsAsync(
        Guid postId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
    Task<Result<CommentResponse>> UpdateCommentAsync(
        Guid commentId,
        CreateOrUpdateCommentRequest createOrUpdateCommentRequest,
        CancellationToken cancellationToken = default);
    Task<Result> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default);
}