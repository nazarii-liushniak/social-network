using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsUserAsync(Guid userId);
    Task<User> AddUserAsync(User user);
    Task<User?> GetUserAsync(Guid userId);
    Task<User?> GetUserWithPostsAsync(Guid userId, int limit);
    Task<IEnumerable<User>> GetUsersAsync();
    Task<IEnumerable<Follow>> GetFollowersAsync(
        Guid userId,
        DateTime timestamp,
        Guid followerId,
        Guid followeeId,
        int limit);
    Task<IEnumerable<Follow>> GetFollowingsAsync(
        Guid userId,
        DateTime timestamp,
        Guid followerId,
        Guid followeeId,
        int limit);
    Task UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(Guid userId);
}