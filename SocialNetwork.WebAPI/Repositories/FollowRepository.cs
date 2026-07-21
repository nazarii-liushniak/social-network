using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class FollowRepository(SocialNetworkDbContext context) : IFollowRepository
{
    public void AddFollow(Follow follow)
    {
        context.Follows.Add(follow);
    }

    public async Task<Follow?> GetFollowAsync(
        Guid followerId,
        Guid followeeId,
        CancellationToken cancellationToken = default)
    {
        return await context.Follows.FindAsync([followerId, followeeId], cancellationToken);
    }

    public void DeleteFollow(Follow follow)
    {
        context.Follows.Remove(follow);
    }

    public async Task DeleteUserFollowsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await context.Follows
            .Where(f => f.FolloweeId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}