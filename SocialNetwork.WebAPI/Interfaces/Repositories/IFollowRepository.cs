using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IFollowRepository
{
    Task AddFollowAsync(Follow follow);
    Task<bool> DeleteFollowAsync(Guid followerId, Guid followeeId);
    Task<int> GetFollowersCountAsync(Guid userId);
    Task<int> GetFollowingCountAsync(Guid userId);
    Task<bool> IsFollowedByUserAsync(Guid followerId, Guid followeeId);
}