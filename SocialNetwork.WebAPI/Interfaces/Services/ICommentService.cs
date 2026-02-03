using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Models.Comment;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface ICommentService
{
    public Task<Comment?> CreateCommentAsync(
        Guid userId,
        Guid postId,
        CreateOrUpdateComment comment);
    public Task<Comments?> GetCommentsAsync(Guid postId, string? cursor, int limit);
    public Task<bool> UpdateCommentAsync(
        Guid userId,
        JsonPatchDocument<CreateOrUpdateComment> commentPatch);
    public Task<bool> DeleteCommentAsync(Guid commentId);
}