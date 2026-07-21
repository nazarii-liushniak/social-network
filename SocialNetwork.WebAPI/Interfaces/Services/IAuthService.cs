using FluentResults;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IAuthService
{
    Task<Result<UserResponse>> RegisterAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken = default);
    Task<Result<TokensResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(string refreshTokenString, CancellationToken cancellationToken = default);
    Task<Result<TokensResponse>> RefreshTokensAsync(string refreshTokenString, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(
        string oldPassword,
        string newPassword,
        CancellationToken cancellationToken = default);
}