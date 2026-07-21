using FluentResults;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Errors;
using SocialNetwork.WebAPI.Helpers;
using SocialNetwork.WebAPI.Interfaces.Contexts;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.Comment;

namespace SocialNetwork.WebAPI.Services;

public class CommentService(
    TimeProvider timeProvider,
    IUserContext userContext,
    IPostRepository postRepository,
    ICommentRepository commentRepository
) : ICommentService
{
    public async Task<Result<CommentResponse>> CreateCommentAsync(
        Guid postId,
        CreateOrUpdateCommentRequest createOrUpdateCommentRequest,
        CancellationToken cancellationToken = default)
    {
        var existsPost = await postRepository.ExistsPostAsync(postId, cancellationToken);
        if (!existsPost)
            return Result.Fail(new NotFoundError($"Post with ID {postId} not found"));
        
        var currentUserId = userContext.UserId!.Value;

        var comment = new Comment
        {
            Id = Guid.Empty,
            PostId = postId,
            UserId = currentUserId,
            Content = createOrUpdateCommentRequest.Content,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        commentRepository.AddComment(comment);
        await commentRepository.SaveChangesAsync(cancellationToken);

        return (await commentRepository.GetCommentModelAsync(comment.Id, cancellationToken))!;
    }

    public async Task<Result<PagedResponse<CommentResponse>>> GetCommentsAsync(
        Guid postId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));

        var existsPost = await postRepository.ExistsPostAsync(postId, cancellationToken);
        if (!existsPost)
            return Result.Fail(new NotFoundError($"Post with ID {postId} not found"));
        
        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var commentId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));

        var comments = await commentRepository
            .GetCommentsAsync(postId, timestamp, commentId, limit, cancellationToken);

        string? nextCursor = null;
        if (comments.Count > limit)
        {
            comments = comments.SkipLast(1).ToList();

            var lastComment = comments.Last();
            nextCursor = CursorHelper.GenerateCursor(lastComment.CreatedAt, lastComment.Id);
        }

        return new PagedResponse<CommentResponse>(comments, nextCursor);
    }

    public async Task<Result<CommentResponse>> UpdateCommentAsync(
        Guid commentId,
        CreateOrUpdateCommentRequest createOrUpdateCommentRequest,
        CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetCommentModelAsync(commentId, cancellationToken);

        if (comment == null)
            return Result.Fail(new NotFoundError($"Comment with ID {commentId} not found"));
        
        var currentUserId = userContext.UserId!.Value;

        if (comment.Author.Id != currentUserId)
            return Result.Fail(new ForbiddenError($"Comment with {commentId} not owned by you"));

        comment.Content = createOrUpdateCommentRequest.Content;
        await commentRepository.SaveChangesAsync(cancellationToken);

        return (await commentRepository.GetCommentModelAsync(comment.Id, cancellationToken))!;
    }

    public async Task<Result> DeleteCommentAsync(Guid commentId, CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetCommentAsync(commentId, cancellationToken);

        if (comment == null)
            return Result.Fail(new NotFoundError($"Comment with ID {commentId} not found"));
        
        var currentUserId = userContext.UserId!.Value;

        if (comment.UserId != currentUserId)
            return Result.Fail(new ForbiddenError($"Comment with ID {commentId} not owned by you"));
        
        commentRepository.DeleteComment(comment);
        await commentRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}