using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User> AddUserAsync(User user);
    Task<User?> GetUserAsync(Guid userId);
    Task<IEnumerable<User>> GetUsersAsync();
    Task<IEnumerable<User>> GetFollowersAsync(Guid userId);
    Task<IEnumerable<User>> GetFollowingsAsync(Guid userId);
    Task<User?> UpdateUserAsync(User user);
    Task<User?> DeleteUserAsync(Guid id);
}