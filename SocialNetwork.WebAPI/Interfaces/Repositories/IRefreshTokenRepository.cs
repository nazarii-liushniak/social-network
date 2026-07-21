using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    void AddToken(RefreshToken refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(Guid token, CancellationToken cancellationToken = default);
    void DeleteToken(RefreshToken refreshToken);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}