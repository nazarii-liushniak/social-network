using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Repositories;

public class LikeRepository(SocialNetworkDbContext context) : ILikeRepository
{
    public void AddLike(Like like)
    {
        context.Likes.Add(like);
    }

    public async Task<Like?> GetLikeAsync(
        Guid userId,
        Guid postId,
        CancellationToken cancellationToken = default)
    {
        return await context.Likes.FindAsync([postId, userId], cancellationToken);
    }

    public void DeleteLike(Like like)
    {
        context.Likes.Remove(like);
    }

    public async Task DeleteUserLikes(Guid userId, CancellationToken cancellationToken = default)
    {
        await context.Likes
            .Where(l => l.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ShortProfileResponse>> GetUsersLikedPostAsync(Guid postId,
        DateTimeOffset? timestamp,
        Guid? userId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        limit = Math.Clamp(limit, 0, 100);

        var query = context.Likes
            .Where(l => l.PostId == postId)
            .OrderByDescending(l => l.CreatedAt)
            .ThenBy(l => l.UserId)
            .AsQueryable();

        if (timestamp.HasValue && userId.HasValue)
        {
            query = query.Where(l => l.CreatedAt < timestamp.Value
                || (l.CreatedAt == timestamp.Value && l.UserId > userId.Value));
        }
        
        return await query
            .Take(limit)
            .Select(l => new ShortProfileResponse()
            {
                Id = l.User.Id,
                Username = l.User.Username,
                FullName = l.User.FullName,
                ProfileImageUrl = l.User.ProfileImageUrl,
                Timestamp = l.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}