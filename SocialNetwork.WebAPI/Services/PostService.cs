using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Helpers;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models.Comment;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;
using Comment = SocialNetwork.WebAPI.Models.Comment.Comment;
using Post = SocialNetwork.WebAPI.Models.Post.Post;
using PostEntity = SocialNetwork.WebAPI.Entities.Post;

namespace SocialNetwork.WebAPI.Services;

public class PostService(
    IUserRepository userRepository,
    IPostRepository postRepository,
    ICommentRepository commentRepository,
    ILikeRepository likeRepository
) : IPostService
{
    public async Task<Post?> PostAsync(Guid userId, CreateOrUpdatePost post)
    {
        var userExists = await userRepository.ExistsUserAsync(userId);
        if (!userExists)
            return null;

        var postEntity = new PostEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CreatedAt = DateTime.UtcNow,
        };
        
        await postRepository.AddPostAsync(postEntity);

        return new Post
        {
            Id = postEntity.Id,
            Content = postEntity.Content,
            ImageUrl = postEntity.ImageUrl,
            CommentsCount = null,
            LikesCount = null,
            IsLikedByMe = null,
            Timestamp = postEntity.CreatedAt,
        };
    }

    public async Task<Feed?> GetFeedAsync(Guid userId, string? cursor, int limit = 50)
    {
        var userExists = await userRepository.ExistsUserAsync(userId);
        if (!userExists)
            return null;

        DateTime timestamp;
        Guid postId;

        if (cursor == null)
        {
            timestamp = DateTime.UtcNow;
            postId = Guid.Empty;
        }
        else
        {
            (timestamp, postId) = CursorHelper.ParseCursor(cursor);
        }
        
        var postEntitiesList = (await postRepository
            .GetFeedAsync(userId, timestamp, postId, limit)).ToList();
        
        var postIds = postEntitiesList.Select(p => p.Id).ToList();
        
        var commentsCountMap = await commentRepository
            .GetCommentsCountByPostIdsAsync(postIds);
        var likesCountMap = await likeRepository
            .GetLikesCountByPostIdsAsync(postIds);
        var likedByMeSet = await likeRepository
            .GetLikedPostsByUserAsync(userId, postIds);
        
        var posts = postEntitiesList
            .Select(p => new PostWithAuthor
            {
                Id = p.Id,
                Author = new ShortProfile
                {
                    Id = p.User.Id,
                    Username = p.User.Username,
                    FullName = p.User.FullName,
                    ProfileImageUrl = p.User.ProfileImageUrl,
                },
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CommentsCount = commentsCountMap.GetValueOrDefault(p.Id),
                LikesCount = likesCountMap.GetValueOrDefault(p.Id),
                IsLikedByMe = likedByMeSet.Contains(p.Id),
                Timestamp = p.CreatedAt,
            })
            .ToList();

        var lastPost = posts.LastOrDefault();
        var feed = new Feed
        {
            Items = posts,
            NextCursor = CursorHelper.GenerateCursor(
                lastPost?.Timestamp ?? DateTime.UtcNow,
                lastPost?.Id ?? Guid.Empty),
        };

        return feed;
    }

    public async Task<PostWithAuthorAndComments?> GetPostAsync(
        Guid currentUserId,
        Guid postId,
        int commentsLimit = 50)
    {
        var post = await postRepository.GetPostWithCommentsAsync(postId, commentsLimit);
        
        if (post == null)
            return null;

        var commentsCount = (await commentRepository
            .GetCommentsCountByPostIdsAsync([postId]))[postId];
        var likesCount = (await likeRepository
            .GetLikesCountAsync(postId));
        var isLikedByMe = (await likeRepository
            .IsLikedAsync(currentUserId, postId));

        var postComments = post.Comments
            .Select(c => new Comment
            {
                Id = c.Id,
                Author = new ShortProfile
                {
                    Id = c.User.Id,
                    Username = c.User.Username,
                    FullName = c.User.FullName,
                    ProfileImageUrl = c.User.ProfileImageUrl,
                },
                Content = c.Content,
                Timestamp = c.CreatedAt,
            })
            .ToList();

        var lastComment = postComments.LastOrDefault();
        var postWithAuthorAndComments = new PostWithAuthorAndComments
        {
            Id = post.Id,
            Author = new ShortProfile
            {
                Id = post.User.Id,
                Username = post.User.Username,
                FullName = post.User.FullName,
                ProfileImageUrl = post.User.ProfileImageUrl,
            },
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            CommentsCount = commentsCount,
            CommentsPreview = new Comments
            {
                Items = postComments,
                NextCursor = CursorHelper.GenerateCursor(
                    lastComment?.Timestamp ?? DateTime.UtcNow,
                    lastComment?.Id ?? Guid.Empty),
            },
            LikesCount = likesCount,
            IsLikedByMe = isLikedByMe,
            Timestamp = post.CreatedAt,
        };
        
        return postWithAuthorAndComments;
    }

    public async Task<bool> UpdatePostAsync(
        Guid postId,
        JsonPatchDocument<CreateOrUpdatePost> postPatch)
    {
        var post = await postRepository.GetPostAsync(postId);
        
        if (post == null)
            return false;

        var updatePost = new CreateOrUpdatePost
        {
            Content = post.Content,
            ImageUrl = post.ImageUrl,
        };
        
        postPatch.ApplyTo(updatePost);

        post.Content = updatePost.Content;
        post.ImageUrl = updatePost.ImageUrl;
        
        await postRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletePostAsync(Guid postId)
    {
        return await postRepository.DeletePostAsync(postId);
    }

    public async Task<bool> LikePostAsync(Guid userId, Guid postId)
    {
        var userExists = await userRepository.ExistsUserAsync(userId);
        if (!userExists)
            return false;

        var isAlreadyLiked = await likeRepository.IsLikedAsync(userId, postId);
        if (isAlreadyLiked)
            return false;
        
        var like = new Like
        {
            UserId = userId,
            PostId = postId,
            CreatedAt = DateTime.UtcNow,
        };
            
        await likeRepository.AddLikeAsync(like);

        return true;
    }

    public async Task<bool> UnlikePostAsync(Guid userId, Guid postId)
    {
        var userExists = await userRepository.ExistsUserAsync(userId);
        if (!userExists)
            return false;

        var isAlreadyLiked = await likeRepository.IsLikedAsync(userId, postId);
        if (!isAlreadyLiked)
            return false;
            
        await likeRepository.DeleteLikeAsync(userId, postId);

        return true;
    }

    public async Task<ShortProfiles?> GetUsersLikedPostAsync(
        Guid postId,
        string? cursor,
        int limit)
    {
        var postExists = await postRepository.ExistsPostAsync(postId);
        if (!postExists)
            return null;

        DateTime timestamp;
        Guid userId;

        if (cursor == null)
        {
            timestamp = DateTime.UtcNow;
            userId = Guid.Empty;
        }
        else
        {
            (timestamp, userId) = CursorHelper.ParseCursor(cursor);
        }
        
        var likes = (await likeRepository
            .GetUsersLikedPostAsync(postId, timestamp, userId))
            .ToList();

        var shortProfilesList = likes.Select(u => new ShortProfile
        {
            Id = u.User.Id,
            Username = u.User.Username,
            FullName = u.User.FullName,
            ProfileImageUrl = u.User.ProfileImageUrl,
        })
        .ToList();

        var lastLike = likes.LastOrDefault();
        var shortProfiles = new ShortProfiles
        {
            Items = shortProfilesList,
            NextCursor = CursorHelper.GenerateCursor(
                lastLike?.CreatedAt ?? DateTime.UtcNow,
                lastLike?.UserId ?? Guid.Empty),
        };

        return shortProfiles;
    }
}