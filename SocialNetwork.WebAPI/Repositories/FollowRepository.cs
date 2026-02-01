using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class FollowRepository(SocialNetworkDbContext context) : IFollowRepository
{

    public async Task AddFollowAsync(Follow follow)
    {
        await context.Follows.AddAsync(follow);
        
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Follow>> GetFollowingsByUser(Guid userId)
    {
        var users = await context.Follows
            .Where(f => f.FollowerId == userId)
            .Include(f => f.Followee)
            .ToListAsync();

        return users;
    }

    public async Task<bool> DeleteFollowAsync(Guid followerId, Guid followeeId)
    {
        var deletedRows = await context.Follows
            .Where(f => f.FollowerId == followerId && f.FolloweeId == followeeId)
            .ExecuteDeleteAsync();

        await context.SaveChangesAsync();

        return deletedRows > 0;
    }

    public async Task<int> GetFollowersCountAsync(Guid userId)
    {
        return await context.Follows
            .CountAsync(f => f.FolloweeId == userId);
    }
    
    public async Task<int> GetFollowingCountAsync(Guid userId)
    {
        return await context.Follows
            .CountAsync(f => f.FollowerId == userId);
    }

    public async Task<bool> IsFollowedByUserAsync(Guid followerId, Guid followeeId)
    {
        return await context.Follows
            .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
    }
}