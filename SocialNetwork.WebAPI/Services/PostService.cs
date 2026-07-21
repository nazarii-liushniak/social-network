using FluentResults;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Errors;
using SocialNetwork.WebAPI.Helpers;
using SocialNetwork.WebAPI.Interfaces.Contexts;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Services;

public class PostService(
    TimeProvider timeProvider,
    IUserContext userContext,
    IPostRepository postRepository,
    ILikeRepository likeRepository
) : IPostService
{
    public async Task<Result<PostResponse>> CreatePostAsync(
        CreateOrUpdatePostRequest createOrUpdatePostRequest,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userContext.UserId!.Value;
        
        var post = new Post
        {
            Id = Guid.Empty,
            UserId = currentUserId,
            Content = createOrUpdatePostRequest.Content,
            ImageUrl = createOrUpdatePostRequest.ImageUrl,
            CreatedAt = timeProvider.GetUtcNow(),
        };
        
        postRepository.AddPost(post);
        await postRepository.SaveChangesAsync(cancellationToken);
        
        return (await postRepository.GetPostModelAsync(currentUserId, post.Id, cancellationToken))!;
    }

    public async Task<Result<PostWithAuthorResponse>> GetPostAsync(
        Guid postId,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = userContext.UserId;
        
        var postWithAuthor = await postRepository.GetPostWithAuthorAsync(currentUserId, postId, cancellationToken);
        
        if (postWithAuthor == null)
            return Result.Fail(new NotFoundError($"Post with ID {postId} not found"));
        
        return postWithAuthor;
    }

    public async Task<Result<PagedResponse<PostWithAuthorResponse>>> GetFeedAsync(
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));

        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var postId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));
        
        var currentUserId = userContext.UserId!.Value;
        
        var postsWithAuthor = await postRepository
            .GetFeedAsync(currentUserId, timestamp, postId, limit + 1, cancellationToken);

        string? nextCursor = null;
        if (postsWithAuthor.Count > limit)
        {
            postsWithAuthor = postsWithAuthor.SkipLast(1).ToList();

            var lastPostWithAuthor = postsWithAuthor.Last();
            nextCursor = CursorHelper.GenerateCursor(lastPostWithAuthor.CreatedAt, lastPostWithAuthor.Id);
        }

        return new PagedResponse<PostWithAuthorResponse>(postsWithAuthor, nextCursor);
    }

    public async Task<Result<PagedResponse<PostResponse>>> GetPostsAsync(
        Guid userId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));

        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var postId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));
        
        var currentUserId = userContext.UserId;

        var posts = await postRepository.GetPostsAsync(currentUserId, userId, timestamp, postId, limit + 1, cancellationToken);

        string? nextCursor = null;
        if (posts.Count > limit)
        {
            posts = posts.SkipLast(1).ToList();

            var lastPost = posts.Last();
            nextCursor = CursorHelper.GenerateCursor(lastPost.CreatedAt, lastPost.Id);
        }

        return new PagedResponse<PostResponse>(posts, nextCursor);
    }

    public async Task<Result<PostResponse>> UpdatePostAsync(
        Guid postId,
        CreateOrUpdatePostRequest createOrUpdatePostRequest,
        CancellationToken cancellationToken = default)
    {
        var post = await postRepository.GetPostAsync(postId, cancellationToken);
        
        if (post == null)
            return Result.Fail(new NotFoundError($"Post with ID {postId} not found"));
        
        var currentUserId = userContext.UserId!.Value;
        
        if (post.UserId != currentUserId)
            return Result.Fail(new ForbiddenError($"Post with ID {postId} not owned by you"));

        post.Content = createOrUpdatePostRequest.Content;
        post.ImageUrl = createOrUpdatePostRequest.ImageUrl;
        
        await postRepository.SaveChangesAsync(cancellationToken);

        return (await postRepository.GetPostModelAsync(currentUserId, postId, cancellationToken))!;
    }

    public async Task<Result> DeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        var post = await postRepository.GetPostAsync(postId, cancellationToken);

        if (post == null)
            return Result.Fail(new NotFoundError($"Post with ID {postId} not found"));
        
        var currentUserId = userContext.UserId!.Value;
        
        if (post.UserId != currentUserId)
            return Result.Fail(new ForbiddenError($"Post with ID {postId} not owned by you"));

        postRepository.DeletePost(post);
        await postRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> LikePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        var existsPost = await postRepository.ExistsPostAsync(postId, cancellationToken);
        if (!existsPost)
            return Result.Fail(new NotFoundError($"Post with ID {postId} not found"));
        
        var currentUserId = userContext.UserId!.Value;
        
        var like = await likeRepository.GetLikeAsync(currentUserId, postId, cancellationToken);
        if (like != null)
            return Result.Fail(new AlreadyExistsError($"Post with ID {postId} already liked by user with ID {currentUserId}"));
        
        like = new Like
        {
            UserId = currentUserId,
            PostId = postId,
            CreatedAt = timeProvider.GetUtcNow(),
        };
            
        likeRepository.AddLike(like);
        await likeRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> UnlikePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        var existsPost = await postRepository.ExistsPostAsync(postId, cancellationToken);
        if (!existsPost)
            return Result.Fail(new NotFoundError($"Post with ID {postId} not found"));
        
        var currentUserId = userContext.UserId!.Value;

        var like = await likeRepository.GetLikeAsync(currentUserId, postId, cancellationToken);

        if (like == null)
            return Result.Fail(new NotFoundError($"User with ID {currentUserId} not liked post with ID {postId}"));

        likeRepository.DeleteLike(like);
        await likeRepository.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<PagedResponse<ShortProfileResponse>>> GetUsersLikedPostAsync(
        Guid postId,
        string? cursor,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            return Result.Fail(new ValidationError("Limit must be greater than 0 and less than 100"));

        var existsPost = await postRepository.ExistsPostAsync(postId, cancellationToken);
        if (!existsPost)
            return Result.Fail(new NotFoundError($"Post with ID {postId} not found"));

        if (!CursorHelper.TryParseCursor(cursor, out var timestamp, out var userId))
            return Result.Fail(new InvalidCursorError($"Cursor is invalid"));
        
        var shortProfiles = await likeRepository.GetUsersLikedPostAsync(postId, timestamp, userId, limit + 1, cancellationToken);

        string? nextCursor = null;
        if (shortProfiles.Count > limit)
        {
            shortProfiles = shortProfiles.SkipLast(1).ToList();

            var lastShortProfile = shortProfiles.Last();
            nextCursor = CursorHelper.GenerateCursor(lastShortProfile.Timestamp, lastShortProfile.Id);
        }

        return new PagedResponse<ShortProfileResponse>(shortProfiles, nextCursor);
    }
}