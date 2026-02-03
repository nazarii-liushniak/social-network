using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface ILikeRepository
{
    Task AddLikeAsync(Like like);
    Task<bool> DeleteLikeAsync(Guid userId, Guid postId);
    Task<int> GetLikesCountAsync(Guid postId);
    Task<bool> IsLikedAsync(Guid postId, Guid userId);
    Task<Dictionary<Guid, int>> GetLikesCountByPostIdsAsync(IEnumerable<Guid> postIds);
    Task<HashSet<Guid>> GetLikedPostsByUserAsync(Guid userId, IEnumerable<Guid> postIds);
    Task<IEnumerable<Like>> GetUsersLikedPostAsync(
        Guid postId,
        DateTime timestamp,
        Guid userId);
}