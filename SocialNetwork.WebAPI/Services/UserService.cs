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

public class UserService(
    TimeProvider timeProvider,
    IUserContext userContext,
    IUserRepository userRepository,
    IFollowRepository followRepository,
    IMessageRepository messageRepository,
    ICommentRepository commentRepository,
    ILikeRepository likeRepository) : IUserService
{
    public async Task<Result<PagedResponse<ShortProfileResponse>>> GetUsersAsync(
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));

        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var userId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));

        var shortProfiles = await userRepository.GetUsersAsync(timestamp, userId, limit + 1, cancellationToken);
        
        string? nextCursor = null;
        if (shortProfiles.Count > limit)
        {
            shortProfiles = shortProfiles.SkipLast(1).ToList();

            var lastShortProfile = shortProfiles.Last();
            nextCursor = CursorHelper.GenerateCursor(lastShortProfile.Timestamp, lastShortProfile.Id);
        }

        return new PagedResponse<ShortProfileResponse>(shortProfiles, nextCursor);
    }

    public async Task<Result<ProfileResponse>> GetUserProfileAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userContext.UserId;
        var userProfile = await userRepository.GetUserProfileAsync(currentUserId, userId, cancellationToken);
        
        if (userProfile == null)
            return Result.Fail(new NotFoundError($"User with ID {userId} not found"));

        return userProfile;
    }

    public async Task<Result<UserResponse>> GetUserModelAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = userContext.UserId!.Value;
        
        var userModel = await userRepository.GetUserModelAsync(currentUserId, cancellationToken);
        
        if (userModel == null)
            return Result.Fail(new NotFoundError($"User with ID {currentUserId} not found"));
        
        return userModel;
    }

    public async Task<Result<UserResponse>> UpdateUserModelAsync(
        UpdateUserModelRequest updateUserModelRequest,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userContext.UserId!.Value;
        var user = await userRepository.GetUserAsync(currentUserId, cancellationToken);

        if (user == null)
            return Result.Fail(new NotFoundError($"User with ID {currentUserId} not found"));

        user.Username = updateUserModelRequest.Username;
        user.FullName = updateUserModelRequest.FullName;
        user.Description = updateUserModelRequest.Description;
        user.ProfileImageUrl = updateUserModelRequest.ProfileImageUrl;

        await userRepository.SaveChangesAsync(cancellationToken);
        
        var userInfo = new UserResponse()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Description = user.Description,
            ProfileImageUrl = user.ProfileImageUrl,
        };

        return userInfo;
    }

    public async Task<Result> DeleteUserAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = userContext.UserId!.Value;
        var user = await userRepository.GetUserAsync(currentUserId, cancellationToken);

        await likeRepository.DeleteUserLikes(currentUserId, cancellationToken);
        await commentRepository.DeleteUserCommentsAsync(currentUserId, cancellationToken);
        await followRepository.DeleteUserFollowsAsync(currentUserId, cancellationToken);
        await messageRepository.DeleteUserMessagesAsync(currentUserId, cancellationToken);
        
        userRepository.DeleteUser(user!);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> FollowAsync(Guid followeeId, CancellationToken cancellationToken = default)
    {
        var existsFollowee = await userRepository.ExistsUserAsync(followeeId, cancellationToken);

        if (!existsFollowee)
            return Result.Fail(new NotFoundError($"Followee with ID {followeeId} not found"));
        
        var currentUserId = userContext.UserId!.Value;

        var follow = await followRepository.GetFollowAsync(currentUserId, followeeId, cancellationToken);

        if (follow != null) return Result.Fail(
            new AlreadyExistsError($"User with ID {currentUserId} already follows user with ID {followeeId}"));
        
        follow = new Follow
        {
            FollowerId = currentUserId,
            FolloweeId =  followeeId,
            CreatedAt = timeProvider.GetUtcNow(),
        };

        followRepository.AddFollow(follow);
        await followRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> UnfollowAsync(Guid followeeId, CancellationToken cancellationToken = default)
    {
        var existsFollowee = await userRepository.ExistsUserAsync(followeeId, cancellationToken);

        if (!existsFollowee)
            return Result.Fail(new NotFoundError($"Followee with ID {followeeId} not found"));
        
        var currentUserId = userContext.UserId!.Value;
        
        var follow = await followRepository.GetFollowAsync(currentUserId, followeeId, cancellationToken);

        if (follow == null)
            return Result.Fail(new NotFoundError($"User with ID {currentUserId} already is not following user with ID {followeeId}"));

        followRepository.DeleteFollow(follow);
        await followRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
    
    public async Task<Result<PagedResponse<ShortProfileResponse>>> GetFollowersAsync(
        Guid userId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));

        var existsUser = await userRepository.ExistsUserAsync(userId, cancellationToken);

        if (!existsUser)
            return Result.Fail(new NotFoundError($"User with ID {userId} not found"));
        
        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var followerId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));

        var shortProfiles = await userRepository
            .GetFollowersAsync(userId, timestamp, followerId, limit + 1, cancellationToken);

        string? nextCursor = null;
        if (shortProfiles.Count > limit)
        {
            shortProfiles = shortProfiles.SkipLast(1).ToList();

            var lastShortProfile = shortProfiles.Last();
            nextCursor = CursorHelper.GenerateCursor(lastShortProfile.Timestamp, lastShortProfile.Id);
        }

        var pagedResult = new PagedResponse<ShortProfileResponse>(shortProfiles, nextCursor);

        return pagedResult;
    }
    
    public async Task<Result<PagedResponse<ShortProfileResponse>>> GetFolloweesAsync(
        Guid userId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));

        var existsUser = await userRepository.ExistsUserAsync(userId, cancellationToken);
        if (!existsUser)
            return Result.Fail(new NotFoundError($"User with ID {userId} not found"));
        
        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var followeeId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));

        var shortProfiles = await userRepository
                .GetFolloweesAsync(userId, timestamp, followeeId, limit + 1, cancellationToken);

        string? nextCursor = null;
        if (shortProfiles.Count > limit)
        {
            shortProfiles = shortProfiles.SkipLast(1).ToList();

            var lastShortProfile = shortProfiles.Last();
            nextCursor = CursorHelper.GenerateCursor(lastShortProfile.Timestamp, lastShortProfile.Id);
        }
        
        var pagedResult = new PagedResponse<ShortProfileResponse>(shortProfiles, nextCursor);

        return pagedResult;
    }
}