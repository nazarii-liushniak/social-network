using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsUserWithUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsUserWithEmailAsync(string email, CancellationToken cancellationToken = default);
    void AddUser(User user);
    Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserResponse?> GetUserModelAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ProfileResponse?> GetUserProfileAsync(
        Guid? currentUserId,
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShortProfileResponse>> GetUsersAsync(
        DateTimeOffset? timestamp,
        Guid? userId,
        int limit,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShortProfileResponse>> GetFollowersAsync(
        Guid userId,
        DateTimeOffset? timestamp,
        Guid? followerId,
        int limit,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ShortProfileResponse>> GetFolloweesAsync(
        Guid userId,
        DateTimeOffset? timestamp,
        Guid? followeeId,
        int limit,
        CancellationToken cancellationToken = default);
    void DeleteUser(User user);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}