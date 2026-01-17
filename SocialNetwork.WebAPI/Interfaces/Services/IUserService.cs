using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IUserService
{
    public Profile GetUserProfile(Guid userId);
    public UserInfo GetUserInfo(Guid userId);
    public UserInfo UpdateUser(JsonPatchDocument<UserInfo> userInfo);
    public bool DeleteUser(Guid userId);
    public IEnumerable<PostElement> GetPosts(Guid userId);
    public bool Follow(Guid followerId, Guid followeeId);
    public bool Unfollow(Guid followerId, Guid followeeId);
    public IEnumerable<Profile> GetFollowers(Guid userId);
    public IEnumerable<Profile> GetFollowing(Guid userId);
}