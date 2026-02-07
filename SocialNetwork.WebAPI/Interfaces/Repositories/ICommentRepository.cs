using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface ICommentRepository
{
    Task AddCommentAsync(Comment comment);
    Task<Comment?> GetCommentAsync(Guid postId, Guid commentId);
    Task<IEnumerable<Comment>> GetCommentsAsync(
        Guid postId,
        DateTime timestamp,
        Guid commentId,
        int limit);
    Task SaveChangesAsync();
    Task<bool> DeleteCommentAsync(Guid postId, Guid commentId);
    Task<Dictionary<Guid, int>> GetCommentsCountByPostIdsAsync(IEnumerable<Guid> postIds);
}