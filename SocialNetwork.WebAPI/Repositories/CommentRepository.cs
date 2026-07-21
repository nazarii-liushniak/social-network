using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Models.Comment;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Repositories;

public class CommentRepository(SocialNetworkDbContext context) : ICommentRepository
{

    public void AddComment(Comment comment)
    {
        context.Comments.Add(comment);
    }

    public async Task<Comment?> GetCommentAsync(Guid commentId, CancellationToken cancellationToken = default)
    {
        return await context.Comments.FindAsync([commentId], cancellationToken);
    }

    public async Task<CommentResponse?> GetCommentModelAsync(Guid commentId, CancellationToken cancellationToken = default)
    {
        return await context.Comments
            .Where(c => c.Id == commentId)
            .Select(c => new CommentResponse()
            {
                Id = c.Id,
                Author = new Author()
                {
                    Id = c.Author.Id,
                    Username = c.Author.Username,
                    FullName = c.Author.FullName,
                    ProfileImageUrl = c.Author.ProfileImageUrl,
                },
                Content = c.Content,
                CreatedAt = c.CreatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CommentResponse>> GetCommentsAsync(Guid postId,
        DateTimeOffset? timestamp,
        Guid? commentId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);
        
        var query = context.Comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .ThenBy(c => c.Id)
            .AsQueryable();

        if (timestamp.HasValue && commentId.HasValue)
        {
            query = query.Where(c => c.CreatedAt < timestamp.Value
                || (c.CreatedAt == timestamp.Value && c.Id > commentId.Value));
        }
        
        return await query
            .Take(limit)
            .Select(c => new CommentResponse()
            {
                Id = c.Id,
                Author = new Author()
                {
                    Id = c.Author.Id,
                    Username = c.Author.Username,
                    FullName = c.Author.FullName,
                    ProfileImageUrl = c.Author.ProfileImageUrl,
                },
                Content = c.Content,
                CreatedAt = c.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }

    public void DeleteComment(Comment comment)
    {
        context.Comments.Remove(comment);
    }

    public async Task DeleteUserCommentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await context.Comments
            .Where(c => c.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}