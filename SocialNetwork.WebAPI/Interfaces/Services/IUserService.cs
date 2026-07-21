using FluentResults;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IUserService
{
    Task<Result<PagedResponse<ShortProfileResponse>>> GetUsersAsync(
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
    Task<Result<ProfileResponse>> GetUserProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetUserModelAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> UpdateUserModelAsync(
        UpdateUserModelRequest updateUserModelRequest,
        CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(CancellationToken cancellationToken = default);
    Task<Result> FollowAsync(Guid followeeId, CancellationToken cancellationToken = default);
    Task<Result> UnfollowAsync(Guid followeeId, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<ShortProfileResponse>>> GetFollowersAsync(
        Guid userId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<ShortProfileResponse>>> GetFolloweesAsync(
        Guid userId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
}