using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class UserRepository(SocialNetworkDbContext context) : IUserRepository
{
    public async Task<bool> ExistsUserAsync(Guid userId)
    {
        return await context.Users
            .AnyAsync(u => u.Id == userId);
    }
    
    public async Task<User> AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);
        
        await context.SaveChangesAsync();
        
        return user;
    }

    public async Task<User?> GetUserAsync(Guid userId)
    {
        var user =  await context.Users.FindAsync(userId);
        
        return user;
    }

    public async Task<User?> GetUserWithPostsAsync(Guid userId, int limit)
    {
        var user = await context.Users
            .Include(u => u.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Take(limit))
            .FirstOrDefaultAsync(u => u.Id == userId);

        return user;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        var users = await context.Users.ToListAsync();
        
        return users;
    }

    public async Task<IEnumerable<Follow>> GetFollowersAsync(
        Guid userId,
        DateTime timestamp,
        Guid followerId,
        Guid followeeId,
        int limit)
    {
        var users = await context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Followers)
            .OrderByDescending(f => f.CreatedAt)
            .ThenBy(f => new { f.FollowerId, f.FolloweeId })
            .Where(f => f.CreatedAt > timestamp || 
                        (f.CreatedAt == timestamp && f.FollowerId < followerId) ||
                        (f.CreatedAt == timestamp && f.FollowerId == followerId && f.FolloweeId > followeeId))
            .Take(limit)
            .Include(f => f.Follower)
            .ToListAsync();
        
        return users;
    }

    public async Task<IEnumerable<Follow>> GetFollowingsAsync(
        Guid userId,
        DateTime timestamp,
        Guid followerId,
        Guid followeeId,
        int limit)
    {
        var users = await context.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Followings)
            .OrderByDescending(f => f.CreatedAt)
            .ThenBy(f => new { f.FollowerId, f.FolloweeId })
            .Where(f => f.CreatedAt > timestamp || 
                        (f.CreatedAt == timestamp && f.FollowerId < followerId) ||
                        (f.CreatedAt == timestamp && f.FollowerId == followerId && f.FolloweeId > followeeId))
            .Take(limit)
            .Include(f => f.Followee)
            .ToListAsync();
        
        return users;
    }

    public async Task UpdateUserAsync(User user)
    {
        context.Users.Update(user);
        
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var deletedRows = await context.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();
        
        return deletedRows > 0;
    }
}