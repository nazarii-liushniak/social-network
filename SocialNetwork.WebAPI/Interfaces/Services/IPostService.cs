using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IPostService
{
    public Task<Post?> PostAsync(Guid userId, CreateOrUpdatePost post);
    public Task<Feed?> GetFeedAsync(Guid userId, string? cursor, int limit);
    public Task<PostWithAuthorAndComments?> GetPostAsync(
        Guid? currentUserId,
        Guid postId,
        int commentsLimit);
    public Task<bool> UpdatePostAsync(
        Guid postId,
        JsonPatchDocument<CreateOrUpdatePost> postPatch);
    public Task<bool> DeletePostAsync(Guid postId);
    public Task<bool> LikePostAsync(Guid userId, Guid postId);
    public Task<bool> UnlikePostAsync(Guid userId, Guid postId);
    public Task<ShortProfiles?> GetUsersLikedPostAsync(Guid postId, string? cursor, int limit);
}