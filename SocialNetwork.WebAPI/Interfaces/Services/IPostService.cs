using FluentResults;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Interfaces.Services;

public interface IPostService
{
    Task<Result<PostResponse>> CreatePostAsync(
        CreateOrUpdatePostRequest createOrUpdatePostRequest,
        CancellationToken cancellationToken = default);
    Task<Result<PostWithAuthorResponse>> GetPostAsync(
        Guid postId,
        CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<PostWithAuthorResponse>>> GetFeedAsync(
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<PostResponse>>> GetPostsAsync(
        Guid userId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
    Task<Result<PostResponse>> UpdatePostAsync(
        Guid postId,
        CreateOrUpdatePostRequest createOrUpdatePostRequest,
        CancellationToken cancellationToken = default);
    Task<Result> DeletePostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<Result> LikePostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<Result> UnlikePostAsync(Guid postId, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<ShortProfileResponse>>> GetUsersLikedPostAsync(
        Guid postId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default);
}