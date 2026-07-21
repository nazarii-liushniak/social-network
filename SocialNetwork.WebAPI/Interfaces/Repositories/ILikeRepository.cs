using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface ILikeRepository
{
    void AddLike(Like like);
    Task<Like?> GetLikeAsync(
        Guid userId,
        Guid postId,
        CancellationToken cancellationToken = default);
    void DeleteLike(Like like);
    Task DeleteUserLikes(Guid user, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShortProfileResponse>> GetUsersLikedPostAsync(
        Guid postId,
        DateTimeOffset? timestamp,
        Guid? userId,
        int limit,
        CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}