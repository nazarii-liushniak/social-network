using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface ILikeRepository
{
    Task<Like> AddLikeAsync(Like like);
    Task<Like?> DeleteLikeAsync(Guid likeId);
    Task<int> GetLikesCountAsync(Guid postId);
    Task<bool> IsLikedAsync(Guid postId, Guid userId);
    Task<Dictionary<Guid, int>> GetLikesCountByPostIdsAsync(IEnumerable<Guid> postIds);
    Task<HashSet<Guid>> GetLikedPostsByUserAsync(Guid userId, IEnumerable<Guid> postIds);
}