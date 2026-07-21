using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Repositories;

public class PostRepository(SocialNetworkDbContext context) :  IPostRepository
{
    public async Task<bool> ExistsPostAsync(
        Guid postId,
        CancellationToken cancellationToken = default)
    {
        return await context.Posts
            .AnyAsync(
                p => p.Id == postId,
                cancellationToken);
    }

    public void AddPost(Post post)
    {
        context.Posts.Add(post);
    }

    public async Task<Post?> GetPostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return await context.Posts.FindAsync([postId], cancellationToken);
    }

    public async Task<PostResponse?> GetPostModelAsync(
        Guid? currentUserId,
        Guid postId,
        CancellationToken cancellationToken = default)
    {
        return await context.Posts
            .Where(p => p.Id == postId)
            .Select(p => new PostResponse()
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CommentsCount = p.Comments.Count,
                LikesCount = p.Likes.Count,
                IsLikedByMe = currentUserId.HasValue
                    ? p.Likes.Any(l => l.UserId == currentUserId)
                    : null,
                CreatedAt = p.CreatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PostWithAuthorResponse?> GetPostWithAuthorAsync(
        Guid? currentUserId,
        Guid postId,
        CancellationToken cancellationToken = default)
    {
        return await context.Posts
            .Where(p => p.Id == postId)
            .Select(p => new PostWithAuthorResponse()
            {
                Id = p.Id,
                Author = new Author()
                {
                    Id = p.Author.Id,
                    Username = p.Author.Username,
                    FullName = p.Author.FullName,
                    ProfileImageUrl = p.Author.ProfileImageUrl,
                },
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CommentsCount = p.Comments.Count,
                LikesCount = p.Likes.Count,
                IsLikedByMe = currentUserId.HasValue
                    ? p.Likes.Any(l => l.UserId == currentUserId)
                    : null,
                CreatedAt = p.CreatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PostResponse>> GetPostsAsync(Guid? currentUserId,
        Guid userId,
        DateTimeOffset? timestamp,
        Guid? postId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);

        var query = context.Posts
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .AsQueryable();

        if (timestamp.HasValue && postId.HasValue)
        {
            query = query.Where(p => p.CreatedAt < timestamp.Value
                || (p.CreatedAt == timestamp.Value && p.Id > postId.Value));
        }
        
        return await query
            .Take(limit)
            .Select(p => new PostResponse()
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CommentsCount = p.Comments.Count,
                LikesCount = p.Likes.Count,
                IsLikedByMe = currentUserId.HasValue
                    ? p.Likes.Any(l => l.UserId == currentUserId)
                    : null,
                CreatedAt = p.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PostWithAuthorResponse>> GetFeedAsync(Guid userId,
        DateTimeOffset? timestamp,
        Guid? postId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);

        var query = context.Posts
            .Where(p => context.Follows.Any(f => f.FollowerId == userId && f.FolloweeId == p.UserId))
            .OrderByDescending(p => p.CreatedAt)
            .ThenBy(p => p.Id)
            .AsQueryable();

        if (timestamp.HasValue && postId.HasValue)
        {
            query = query.Where(p => p.CreatedAt < timestamp.Value
                || (p.CreatedAt == timestamp.Value && p.Id > postId.Value));
        }
        
        return await query
            .Take(limit)
            .Select(p => new PostWithAuthorResponse()
            {
                Id = p.Id,
                Author = new Author()
                {
                    Id = p.Author.Id,
                    Username = p.Author.Username,
                    FullName = p.Author.FullName,
                    ProfileImageUrl = p.Author.ProfileImageUrl,
                },
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CommentsCount = p.Comments.Count,
                LikesCount = p.Likes.Count,
                IsLikedByMe = p.Likes.Any(l => l.UserId == userId),
                CreatedAt = p.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }

    public void DeletePost(Post post)
    {
        context.Posts.Remove(post);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}