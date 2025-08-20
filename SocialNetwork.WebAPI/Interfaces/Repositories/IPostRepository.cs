using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IPostRepository
{
    Task<Post> AddPostAsync(Post post);
    Task<Post?> GetPostAsync(Guid id);
    Task<IEnumerable<Post>> GetPostsAsync(Guid userId);
    Task<Post?> UpdatePostAsync(Post post);
    Task<Post?> DeletePostAsync(Guid id);
}