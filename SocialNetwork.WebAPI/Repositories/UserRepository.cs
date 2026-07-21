using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Repositories;

public class UserRepository(SocialNetworkDbContext context) : IUserRepository
{
    public async Task<bool> ExistsUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(
                u => u.Id == userId,
                cancellationToken);
    }

    public async Task<bool> ExistsUserWithUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(
                u => u.Username == username,
                cancellationToken);
    }
    
    public async Task<bool> ExistsUserWithEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(
                u => u.Username == email,
                cancellationToken);
    }
    
    public void AddUser(User user)
    {
        context.Users.Add(user);
    }

    public async Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Users.FindAsync([userId], cancellationToken);
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserResponse?> GetUserModelAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserResponse()
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Description = u.Description,
                ProfileImageUrl = u.ProfileImageUrl,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProfileResponse?> GetUserProfileAsync(
        Guid? currentUserId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Where(u => u.Id == userId)
            .Select(u => new ProfileResponse()
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Description = u.Description,
                ProfileImageUrl = u.ProfileImageUrl,
                IsFollowedByMe = currentUserId.HasValue 
                    ? u.Followers.Any(f => f.FollowerId == currentUserId.Value) 
                    : null,
                FollowersCount = u.Followers.Count,
                FolloweesCount = u.Followees.Count,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShortProfileResponse>> GetUsersAsync(DateTimeOffset? timestamp,
        Guid? userId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);

        var query = context.Users.AsQueryable();

        if (timestamp.HasValue && userId.HasValue)
        {
            query = query.Where(u => u.CreatedAt < timestamp.Value
                || (u.CreatedAt == timestamp.Value && u.Id > userId.Value));
        }

        return await query
            .OrderByDescending(u => u.CreatedAt)
            .ThenBy(u => u.Id)
            .Take(limit)
            .Select(u => new ShortProfileResponse()
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                ProfileImageUrl = u.ProfileImageUrl,
                Timestamp = u.CreatedAt,
            }
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShortProfileResponse>> GetFollowersAsync(Guid userId,
        DateTimeOffset? timestamp,
        Guid? followerId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);

        var query = context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Followers);
        
        if (timestamp.HasValue && followerId.HasValue)
        {
            query = query.Where(f => f.CreatedAt < timestamp.Value
                || (f.CreatedAt == timestamp.Value && f.FollowerId > followerId.Value));
        }
        
        return await query
            .OrderByDescending(f => f.CreatedAt)
            .ThenBy(f => f.FollowerId)
            .Take(limit)
            .Select(f => new ShortProfileResponse()
            {
                Id = f.Follower.Id,
                Username = f.Follower.Username,
                FullName = f.Follower.FullName,
                ProfileImageUrl = f.Follower.ProfileImageUrl,
                Timestamp = f.CreatedAt,
            }
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShortProfileResponse>> GetFolloweesAsync(Guid userId,
        DateTimeOffset? timestamp,
        Guid? followeeId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);

        var query = context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Followees);
        
        if (timestamp.HasValue && followeeId.HasValue)
        {
            query = query.Where(f => f.CreatedAt < timestamp.Value
                || (f.CreatedAt == timestamp.Value && f.FolloweeId > followeeId.Value));
        }
        
        return await query
            .OrderByDescending(f => f.CreatedAt)
            .ThenBy(f => f.FolloweeId)
            .Take(limit)
            .Select(f => new ShortProfileResponse()
            {
                Id = f.Followee.Id,
                Username = f.Followee.Username,
                FullName = f.Followee.FullName,
                ProfileImageUrl = f.Followee.ProfileImageUrl,
                Timestamp = f.CreatedAt
            }
            )
            .ToListAsync(cancellationToken);
    }

    public void DeleteUser(User user)
    {
        context.Users.Remove(user);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}