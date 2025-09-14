using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class FollowRepository(SocialNetworkDbContext context) : IFollowRepository
{
    private readonly SocialNetworkDbContext _context = context;

    public async Task<Follow> AddFollowAsync(Follow follow)
    {
        await _context.Follows.AddAsync(follow);
        
        await _context.SaveChangesAsync();
        
        return follow;
    }

    public async Task<IEnumerable<User>> GetFollowingsByUser(Guid userId)
    {
        var users = await _context.Follows
            .Where(f => f.FollowerId == userId)
            .Select(f => f.Followee)
            .ToListAsync();

        return users;
    }

    public async Task<Follow?> DeleteFollowAsync(Guid followId)
    {
        var follow = await _context.Follows.FindAsync(followId);

        if (follow == null)
            return null;

        _context.Follows.Remove(follow);

        await _context.SaveChangesAsync();

        return follow;
    }
}