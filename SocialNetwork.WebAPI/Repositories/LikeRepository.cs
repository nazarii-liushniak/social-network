using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class LikeRepository(SocialNetworkDbContext context) : ILikeRepository
{
    private readonly SocialNetworkDbContext _context = context;
    
    public async Task<Like> AddLikeAsync(Like like)
    {
        await _context.Likes.AddAsync(like);
        
        await _context.SaveChangesAsync();
        
        return like;
    }

    public async Task<Like?> DeleteLikeAsync(Guid likeId)
    {
        var like = await _context.Likes.FindAsync(likeId);
        
        if (like == null)
            return null;
        
        _context.Likes.Remove(like);
        
        await _context.SaveChangesAsync();
        
        return like;
    }

    public async Task<int> GetLikesCountAsync(Guid postId)
    {
        var count = await _context.Likes
            .Where(l => l.PostId == postId)
            .CountAsync();
        
        return count;
    }

    public async Task<bool> IsLikedAsync(Guid postId, Guid userId)
    {
        var isLiked = await _context.Likes
            .Where(l => l.PostId == postId && l.UserId == userId)
            .AnyAsync();
        
        return isLiked;
    }

    public async Task<Dictionary<Guid, int>> GetLikesCountByPostIdsAsync(IEnumerable<Guid> postIds)
    {
        var postIdList = postIds.ToList();
        
        var likeCounts = await _context.Likes
            .Where(l => postIdList.Contains(l.PostId))
            .GroupBy(l => l.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToListAsync();
        
        return likeCounts.ToDictionary(x => x.PostId, x => x.Count);
    }

    public async Task<HashSet<Guid>> GetLikedPostsByUserAsync(Guid userId, IEnumerable<Guid> postIds)
    {
        var postIdList = postIds.ToList();
        
        var likedPostIds = await _context.Likes
            .Where(l => l.UserId == userId && postIdList.Contains(l.PostId))
            .Select(l => l.PostId)
            .ToListAsync();
        
        return new HashSet<Guid>(likedPostIds);
    }
}