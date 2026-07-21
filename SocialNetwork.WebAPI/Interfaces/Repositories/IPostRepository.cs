using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Models.Post;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IPostRepository
{
    Task<bool> ExistsPostAsync(
        Guid postId,
        CancellationToken cancellationToken = default);
    void AddPost(Post post);
    Task<Post?> GetPostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<PostResponse?> GetPostModelAsync(
        Guid? currentUserId,
        Guid postId,
        CancellationToken cancellationToken = default);
    Task<PostWithAuthorResponse?> GetPostWithAuthorAsync(
        Guid? currentUserId,
        Guid postId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PostResponse>> GetPostsAsync(
        Guid? currentUserId,
        Guid userId,
        DateTimeOffset? timestamp,
        Guid? postId,
        int limit,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PostWithAuthorResponse>> GetFeedAsync(
        Guid userId,
        DateTimeOffset? timestamp,
        Guid? postId,
        int limit,
        CancellationToken cancellationToken = default);
    void DeletePost(Post post);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}