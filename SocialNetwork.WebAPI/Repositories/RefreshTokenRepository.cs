using SocialNetwork.WebAPI.Data;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Interfaces.Repositories;

namespace SocialNetwork.WebAPI.Repositories;

public class RefreshTokenRepository(
    SocialNetworkDbContext context) : IRefreshTokenRepository
{
    public void AddToken(RefreshToken refreshToken)
    {
        context.RefreshTokens.Add(refreshToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(Guid token, CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens
            .FindAsync([token], cancellationToken);
    }

    public void DeleteToken(RefreshToken refreshToken)
    {
        context.RefreshTokens.Remove(refreshToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}