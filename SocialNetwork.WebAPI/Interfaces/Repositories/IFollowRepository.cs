using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IFollowRepository
{
    void AddFollow(Follow follow);
    Task<Follow?> GetFollowAsync(
        Guid followerId,
        Guid followeeId,
        CancellationToken cancellationToken = default);
    void DeleteFollow(Follow follow);
    Task DeleteUserFollowsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}