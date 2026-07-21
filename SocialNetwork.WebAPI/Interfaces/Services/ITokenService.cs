namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(Guid userId);
}