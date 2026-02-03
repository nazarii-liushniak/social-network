using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class LikeRepository(SocialNetworkDbContext context) : ILikeRepository
{
    
    public async Task AddLikeAsync(Like like)
    {
        await context.Likes.AddAsync(like);
        
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteLikeAsync(Guid userId, Guid postId)
    {
        var deletedRows = await context.Likes
            .Where(l => l.UserId == userId && l.PostId == postId)
            .ExecuteDeleteAsync();
        
        return deletedRows > 0;
    }

    public async Task<int> GetLikesCountAsync(Guid postId)
    {
        var count = await context.Likes
            .Where(l => l.PostId == postId)
            .CountAsync();
        
        return count;
    }

    public async Task<bool> IsLikedAsync(Guid postId, Guid userId)
    {
        var isLiked = await context.Likes
            .Where(l => l.PostId == postId && l.UserId == userId)
            .AnyAsync();
        
        return isLiked;
    }

    public async Task<Dictionary<Guid, int>> GetLikesCountByPostIdsAsync(IEnumerable<Guid> postIds)
    {
        var postIdList = postIds.ToList();
        
        var likeCounts = await context.Likes
            .Where(l => postIdList.Contains(l.PostId))
            .GroupBy(l => l.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToListAsync();
        
        return likeCounts.ToDictionary(x => x.PostId, x => x.Count);
    }

    public async Task<HashSet<Guid>> GetLikedPostsByUserAsync(Guid userId, IEnumerable<Guid> postIds)
    {
        var postIdList = postIds.ToList();
        
        var likedPostIds = await context.Likes
            .Where(l => l.UserId == userId && postIdList.Contains(l.PostId))
            .Select(l => l.PostId)
            .ToListAsync();
        
        return new HashSet<Guid>(likedPostIds);
    }

    public async Task<IEnumerable<Like>> GetUsersLikedPostAsync(
        Guid postId,
        DateTime timestamp,
        Guid userId)
    {
        var users = await context.Likes
            .Where(l => l.PostId == postId)
            .OrderByDescending(l => l.CreatedAt)
            .ThenBy(l => l.UserId)
            .Where(l => l.CreatedAt < timestamp ||
                        (l.CreatedAt == timestamp && l.UserId > userId))
            .Include(l => l.User)
            .ToListAsync();

        return users;
    }
}