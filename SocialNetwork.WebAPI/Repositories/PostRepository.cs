using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class PostRepository(SocialNetworkDbContext context) :  IPostRepository
{
    public async Task<bool> ExistsPostAsync(Guid postId)
    {
        return await context.Posts
            .AnyAsync(p => p.Id == postId);
    }

    public async Task AddPostAsync(Post post)
    {
        await context.Posts.AddAsync(post);
        
        await context.SaveChangesAsync();
    }

    public async Task<Post?> GetPostAsync(Guid postId)
    {
        return await context.Posts.FindAsync(postId);
    }

    public async Task<Post?> GetPostWithCommentsAsync(Guid postId, int commentsLimit)
    {
        var post = await context.Posts
            .Where(p => p.Id == postId)
            .Include(p => p.User)
            .Include(p => p.Comments
                .OrderByDescending(c => c.CreatedAt)
                .Take(commentsLimit))
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync();

        return post;
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(
        Guid userId,
        DateTime? timestamp,
        Guid postId,
        int limit)
    {
        var posts = await context.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .Where(p => p.CreatedAt < timestamp || 
                        (p.CreatedAt == timestamp && p.Id > postId))
            .Take(limit)
            .ToListAsync();
        
        return posts;
    }

    public async Task<IEnumerable<Post>> GetFeedAsync(
        Guid userId,
        DateTime timestamp,
        Guid postId,
        int limit)
    {
        var posts = await context.Follows
            .Where(f => f.FollowerId == userId)
            .SelectMany(f => f.Followee.Posts)
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .Where(p => p.CreatedAt < timestamp || 
                        (p.CreatedAt == timestamp && p.Id > postId))
            .Take(limit)
            .Include(p => p.User)
            .ToListAsync();
        
        return posts;
    }

    public async Task<Post?> UpdatePostAsync(Post post)
    {
        var dbPost = await context.Posts.FindAsync(post.Id);

        if (dbPost == null)
            return null;

        post.Id = dbPost.Id;
        post.UserId = dbPost.UserId;
        post.CreatedAt = dbPost.CreatedAt;

        context.Posts.Update(post);

        await context.SaveChangesAsync();

        return post;
    }

    public async Task<bool> DeletePostAsync(Guid postId)
    {
        var deletedRows = await context.Posts
            .Where(p => p.Id == postId)
            .ExecuteDeleteAsync();

        return deletedRows > 0;
    }
}