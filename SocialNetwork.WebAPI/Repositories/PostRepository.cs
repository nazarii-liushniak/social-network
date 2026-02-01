using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class PostRepository(SocialNetworkDbContext context) :  IPostRepository
{
    private readonly SocialNetworkDbContext _context = context;

    public async Task<Post> AddPostAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
        
        await _context.SaveChangesAsync();
        
        return post;
    }

    public async Task<Post?> GetPostAsync(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);

        return post;
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(
        Guid userId,
        DateTime? timestamp,
        Guid postId,
        int limit)
    {
        var posts = await _context.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .Where(p => p.CreatedAt > timestamp || 
                        (p.CreatedAt == timestamp && p.Id > postId))
            .Take(limit)
            .ToListAsync();
        
        return posts;
    }

    public async Task<IEnumerable<Post>> GetFeed(Guid userId)
    {
        var posts = await _context.Follows
            .Where(f => f.FollowerId == userId)
            .SelectMany(f => f.Followee.Posts)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        
        return posts;
    }

    public async Task<Post?> UpdatePostAsync(Post post)
    {
        var dbPost = await _context.Posts.FindAsync(post.Id);

        if (dbPost == null)
            return null;

        post.Id = dbPost.Id;
        post.UserId = dbPost.UserId;
        post.CreatedAt = dbPost.CreatedAt;

        _context.Posts.Update(post);

        await _context.SaveChangesAsync();

        return post;
    }

    public async Task<Post?> DeletePostAsync(Guid id)
    {
        var post = await _context.Posts.FindAsync(id);
        
        if (post == null)
            return null;
        
        _context.Posts.Remove(post);
        
        await _context.SaveChangesAsync();
        
        return post;
    }
}