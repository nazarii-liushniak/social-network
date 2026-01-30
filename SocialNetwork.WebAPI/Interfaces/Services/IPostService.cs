using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Models.Comment;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IPostService
{
    public Task<Post> PostAsync(Guid userId, CreateOrUpdatePost post);
    public Task<Feed?> GetFeedAsync(Guid userId, string? cursor, int limit);
    public Task<PostWithAuthorAndComments?> GetPostAsync(Guid postId);
    public Task<bool> UpdatePostAsync(
        Guid userId,
        JsonPatchDocument<CreateOrUpdatePost> postPatch);
    public Task<bool> DeletePostAsync(Guid postId);
    public Task<bool> LikePostAsync(Guid postId);
    public Task<bool> UnlikePostAsync(Guid postId);
    public Task<ShortProfiles?> GetUsersLikedPostAsync(Guid userId, string? cursor, int limit);
    public Task<Comment?> CreateCommentAsync(Guid userId, CreateOrUpdateComment comment);
    public Task<Comments?> GetCommentsAsync(Guid postId, string? cursor, int limit);
    public Task<bool> UpdateCommentAsync(
        Guid userId,
        JsonPatchDocument<CreateOrUpdateComment> commentPatch);
    public Task<bool> DeleteCommentAsync(Guid commentId);
}