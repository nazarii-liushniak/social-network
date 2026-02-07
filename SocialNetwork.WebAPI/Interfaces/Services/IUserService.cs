using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IUserService
{
    public Task<Profile?> GetUserProfileAsync(Guid? currentUserId, Guid userId, int limit);
    public Task<UserInfo?> GetUserInfoAsync(Guid userId);
    public Task<bool> UpdateUserAsync(
        Guid userId,
        JsonPatchDocument<UpdateUserInfo> userInfoPatch);
    public Task<bool> DeleteUserAsync(Guid userId);
    public Task<Posts?> GetPostsAsync(
        Guid currentUserId,
        Guid userId,
        string? cursor,
        int limit);
    public Task<bool> FollowAsync(Guid followerId, Guid followeeId);
    public Task<bool> UnfollowAsync(Guid followerId, Guid followeeId);
    public Task<ShortProfiles?> GetFollowersAsync(Guid userId, string? cursor, int limit);
    public Task<ShortProfiles?> GetFollowingAsync(Guid userId, string? cursor, int limit);
}