using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface ICommentRepository
{
    Task AddCommentAsync(Comment comment);
    Task<Comment?> GetCommentAsync(Guid id);
    Task<IEnumerable<Comment>> GetCommentsAsync(
        Guid postId,
        DateTime timestamp,
        Guid commentId,
        int limit);
    Task SaveChangesAsync();
    Task<bool> DeleteCommentAsync(Guid id);
    Task<Dictionary<Guid, int>> GetCommentsCountByPostIdsAsync(IEnumerable<Guid> postIds);
}