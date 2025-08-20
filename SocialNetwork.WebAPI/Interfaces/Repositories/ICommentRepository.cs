using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment> AddCommentAsync(Comment comment);
    Task<Comment?> GetCommentAsync(Guid id);
    Task<IEnumerable<Comment>> GetCommentsAsync(Guid postId);
    Task<Comment?> UpdateCommentAsync(Comment comment);
    Task<Comment?> DeleteCommentAsync(Guid id);
}