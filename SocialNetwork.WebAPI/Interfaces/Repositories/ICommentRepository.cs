using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Models.Comment;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface ICommentRepository
{
    void AddComment(Comment comment);
    Task<Comment?> GetCommentAsync(Guid commentId, CancellationToken cancellationToken = default);
    Task<CommentResponse?> GetCommentModelAsync(Guid commentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommentResponse>> GetCommentsAsync(
        Guid postId,
        DateTimeOffset? timestamp,
        Guid? commentId,
        int limit,
        CancellationToken cancellationToken = default);
    void DeleteComment(Comment comment);
    Task DeleteUserCommentsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}