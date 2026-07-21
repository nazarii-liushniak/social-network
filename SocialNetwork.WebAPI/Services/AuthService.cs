using FluentResults;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Errors;
using SocialNetwork.WebAPI.Helpers;
using SocialNetwork.WebAPI.Interfaces.Contexts;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Services;

public class AuthService(
    TimeProvider timeProvider,
    IUserContext userContext,
    IConfiguration configuration,
    ITokenService tokenService,
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository) : IAuthService
{
    public async Task<Result<UserResponse>> RegisterAsync(CreateUserRequest createUserRequest, CancellationToken cancellationToken = default)
    {
        var existsUserWithUsername =
            await userRepository.ExistsUserWithUsernameAsync(createUserRequest.Username, cancellationToken);
        var existsUserWithEmail =
            await userRepository.ExistsUserWithEmailAsync(createUserRequest.Email, cancellationToken);

        if (existsUserWithUsername)
            return Result.Fail(new AlreadyExistsError($"User with username {createUserRequest.Username} already exists"));
        
        if (existsUserWithEmail)
            return Result.Fail(new AlreadyExistsError($"User with email {createUserRequest.Email} already exists"));
        
        var user = new User()
        {
            Id = Guid.Empty,
            Username = createUserRequest.Username,
            Email = createUserRequest.Email,
            PasswordHash = PasswordHelper.HashPassword(createUserRequest.Password),
            FullName = createUserRequest.FullName,
            Description = createUserRequest.Description,
            ProfileImageUrl = createUserRequest.ProfileImageUrl,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        userRepository.AddUser(user);
        await userRepository.SaveChangesAsync(cancellationToken);         

        return new UserResponse()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Description = user.Description,
            ProfileImageUrl = user.ProfileImageUrl,
        };
    }

    public async Task<Result<TokensResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetUserByUsernameAsync(loginRequest.Username, cancellationToken);
        if (user == null)
            return Result.Fail(new AuthenticationError($"Error during login"));
        
        if (!PasswordHelper.VerifyPassword(loginRequest.Password, user.PasswordHash))
            return Result.Fail(new AuthenticationError($"Error during login"));
        
        var jwtToken = tokenService.GenerateToken(user.Id);

        var refreshToken = new RefreshToken()
        {
            Token = Guid.NewGuid(),
            UserId = user.Id,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        refreshTokenRepository.AddToken(refreshToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new TokensResponse()
        {
            AccessToken = jwtToken,
            RefreshToken = refreshToken.Token.ToString(),
        };
    }

    public async Task<Result> LogoutAsync(string refreshTokenString, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(refreshTokenString, out var refreshTokenGuid))
            return Result.Fail(new InvalidRefreshToken($"Refresh token {refreshTokenString} is invalid"));
        
        var refreshToken = await refreshTokenRepository.GetRefreshTokenAsync(refreshTokenGuid, cancellationToken);
        if (refreshToken == null)
            return Result.Fail(new NotFoundError($"Refresh token {refreshTokenString} not found"));

        var currentUserId = userContext.UserId!.Value;
        if (refreshToken.UserId != currentUserId)
            return Result.Fail(new ForbiddenError($"Refresh token {refreshTokenString} is not owned by you"));

        refreshTokenRepository.DeleteToken(refreshToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<TokensResponse>> RefreshTokensAsync(string refreshTokenString, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(refreshTokenString, out var refreshTokenGuid))
            return Result.Fail(new InvalidRefreshToken($"Refresh token {refreshTokenString} is invalid"));
        
        var refreshToken = await refreshTokenRepository.GetRefreshTokenAsync(refreshTokenGuid, cancellationToken);
        if (refreshToken == null)
            return Result.Fail(new NotFoundError($"Refresh token {refreshTokenString} not found"));
        
        if (IsRefreshTokenExpired(refreshToken))
            return Result.Fail(new ExpiredRefreshTokenError($"Refresh token {refreshTokenString} is expired"));
        
        var jwtToken = tokenService.GenerateToken(refreshToken.UserId);

        var newRefreshToken = new RefreshToken()
        {
            Token = Guid.NewGuid(),
            UserId = refreshToken.UserId,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        refreshTokenRepository.AddToken(newRefreshToken);
        refreshTokenRepository.DeleteToken(refreshToken);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new TokensResponse()
        {
            AccessToken = jwtToken,
            RefreshToken = newRefreshToken.Token.ToString(),
        };
    }

    public async Task<Result> ChangePasswordAsync(
        string oldPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userContext.UserId!.Value;
        
        var user = await userRepository.GetUserAsync(currentUserId, cancellationToken);

        if (user == null)
            return Result.Fail(new AuthenticationError($"User with ID {currentUserId} not found"));
        
        if (!PasswordHelper.VerifyPassword(oldPassword, user.PasswordHash))
            return Result.Fail(new AuthenticationError($"Password for user with ID {currentUserId} is incorrect"));
        
        user.PasswordHash = PasswordHelper.HashPassword(newPassword);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    private bool IsRefreshTokenExpired(RefreshToken refreshToken)
    {
        var refreshTokenExpirationDaysString = configuration["RefreshTokenExpirationDays"]
            ?? throw new InvalidOperationException("Refresh token expiration days are missing in appsettings.json.");
        var refreshTokenExpirationDays = int.Parse(refreshTokenExpirationDaysString);
        
        return timeProvider.GetUtcNow() - refreshToken.CreatedAt > TimeSpan.FromDays(refreshTokenExpirationDays);
    }
}