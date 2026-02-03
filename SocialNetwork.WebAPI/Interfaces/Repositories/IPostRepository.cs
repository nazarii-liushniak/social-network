using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IPostRepository
{
    Task<bool> ExistsPostAsync(Guid postId);
    Task AddPostAsync(Post post);
    Task<Post?> GetPostAsync(Guid postId);
    Task<Post?> GetPostWithCommentsAsync(Guid postId, int commentsLimit);
    Task<IEnumerable<Post>> GetPostsAsync(
        Guid userId,
        DateTime? timestamp,
        Guid postId,
        int limit);
    Task<IEnumerable<Post>> GetFeedAsync(
        Guid userId,
        DateTime timestamp,
        Guid postId,
        int limit);
    Task SaveChangesAsync();
    Task<bool> DeletePostAsync(Guid postId);
}