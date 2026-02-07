using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class CommentRepository(SocialNetworkDbContext context) : ICommentRepository
{

    public async Task AddCommentAsync(Comment comment)
    {
        await context.Comments.AddAsync(comment);
        
        await context.SaveChangesAsync();
    }

    public async Task<Comment?> GetCommentAsync(Guid postId, Guid commentId)
    {
        var comment = await context.Comments
            .Where(c => c.Id == commentId && c.PostId == postId)
            .SingleOrDefaultAsync();
        
        return comment;
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync(
        Guid postId,
        DateTime timestamp,
        Guid commentId,
        int limit)
    {
        var comments = await context.Comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .ThenBy(c => c.Id)
            .Where(c => c.CreatedAt < timestamp ||
                        (c.CreatedAt == timestamp && c.Id > commentId))
            .Take(limit)
            .Include(c => c.User)
            .ToListAsync();
        
        return comments;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task<bool> DeleteCommentAsync(Guid postId, Guid commentId)
    {
        var deletedRows = await context.Comments
            .Where(c => c.Id == commentId && c.PostId == postId)
            .ExecuteDeleteAsync();
        
        return deletedRows > 0;
    }

    public async Task<Dictionary<Guid, int>> GetCommentsCountByPostIdsAsync(IEnumerable<Guid> postIds)
    {
        var postIdList = postIds.ToList();
        
        var commentCounts = await context.Comments
            .Where(c => postIdList.Contains(c.PostId))
            .GroupBy(c => c.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToListAsync();
        
        return commentCounts.ToDictionary(x => x.PostId, x => x.Count);
    }
}