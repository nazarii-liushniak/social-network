using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Entities;
using SocialNetwork.WebAPI.Helpers;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;
using Post = SocialNetwork.WebAPI.Models.Post.Post;

namespace SocialNetwork.WebAPI.Services;

public class UserService(
    IUserRepository userRepository,
    IPostRepository postRepository,
    IFollowRepository followRepository,
    ILikeRepository likeRepository,
    ICommentRepository commentRepository) : IUserService
{
    public async Task<Profile?> GetUserProfileAsync(
        Guid? currentUserId,
        Guid userId,
        int limit)
    {
        var user = await userRepository.GetUserAsync(userId);
        
        if (user == null)
            return null;
        
        var followersCount = await followRepository.GetFollowersCountAsync(userId);
        var followingCount = await followRepository.GetFollowingCountAsync(userId);
        
        var isFollowedByMe = currentUserId == null
            ? (bool?)null
            : await followRepository.IsFollowedByUserAsync(currentUserId.Value, userId);

        var postIds = user.Posts.Select(p => p.Id).ToList();
        
        var commentsCountMap = await commentRepository
            .GetCommentsCountByPostIdsAsync(postIds);
        var likesCountMap = await likeRepository
            .GetLikesCountByPostIdsAsync(postIds);
        var likedByMeSet = currentUserId == null
            ? null
            : await likeRepository.GetLikedPostsByUserAsync(currentUserId.Value, postIds);

        var posts = user.Posts
            .Select(p => new Post
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CommentsCount = commentsCountMap.GetValueOrDefault(p.Id),
                LikesCount = likesCountMap.GetValueOrDefault(p.Id),
                IsLikedByMe = likedByMeSet?.Contains(p.Id),
                Timestamp = p.CreatedAt,
            })
            .ToList();
        
        var lastPost = posts.LastOrDefault();
        var profile = new Profile
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Description = user.Description,
            ProfileImageUrl = user.ProfileImageUrl,
            IsFollowedByMe = isFollowedByMe,
            FollowersCount = followersCount,
            FollowingCount = followingCount,
            PostsPreview = new Posts
            {
                Items = posts,
                NextCursor = CursorHelper.GenerateCursor(
                    lastPost?.Timestamp ?? DateTime.Now,
                    lastPost?.Id ?? Guid.Empty),
            }
        };

        return profile;
    }

    public async Task<UserInfo?> GetUserInfoAsync(Guid userId)
    {
        var user = await userRepository.GetUserAsync(userId);
        
        if (user == null)
            return null;

        var userInfo = new UserInfo
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

    public async Task<bool> UpdateUserAsync(
        Guid userId,
        JsonPatchDocument<UpdateUserInfo> userInfoPatch)
    {
        var user = await userRepository.GetUserAsync(userId);

        if (user == null)
            return false;

        var updateUserInfo = new UpdateUserInfo
        {
            Username = user.Username,
            FullName = user.FullName,
            Description = user.Description,
            ProfileImageUrl = user.ProfileImageUrl,
        };
        
        userInfoPatch.ApplyTo(updateUserInfo);

        user.Username = updateUserInfo.Username;
        user.FullName = updateUserInfo.FullName;
        user.Description = updateUserInfo.Description;
        user.ProfileImageUrl = updateUserInfo.ProfileImageUrl;
        
        await userRepository.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        return await userRepository.DeleteUserAsync(userId);
    }

    public async Task<Posts?> GetPostsAsync(
        Guid currentUserId,
        Guid userId,
        string? cursor,
        int limit = 20)
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

        var postEntities = (await postRepository
            .GetPostsAsync(userId, timestamp, postId, limit))
            .ToList();
        
        var postIds = postEntities.Select(p => p.Id).ToList();
        
        var commentsCountMap = await commentRepository
            .GetCommentsCountByPostIdsAsync(postIds);
        var likesCountMap = await likeRepository
            .GetLikesCountByPostIdsAsync(postIds);
        var likedByMeSet = await likeRepository
            .GetLikedPostsByUserAsync(currentUserId, postIds);

        var postModels = postEntities
            .Select(p => new Post
            {
                Id = p.Id,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                CommentsCount = commentsCountMap.GetValueOrDefault(p.Id),
                LikesCount = likesCountMap.GetValueOrDefault(p.Id),
                IsLikedByMe = likedByMeSet.Contains(p.Id),
                Timestamp = p.CreatedAt,
            })
            .ToList();

        var lastPost = postModels.LastOrDefault();
        var posts = new Posts
        {
            Items = postModels,
            NextCursor = CursorHelper.GenerateCursor(
                lastPost?.Timestamp ?? DateTime.Now,
                lastPost?.Id ?? Guid.Empty),
        };

        return posts;
    }

    public async Task<bool> FollowAsync(Guid followerId, Guid followeeId)
    {
        var followeeExists = await userRepository.ExistsUserAsync(followeeId);
        if (!followeeExists)
            return false;

        var existsFollow = await followRepository
            .ExistsFollowAsync(followerId, followeeId);

        if (existsFollow)
            return true;
        
        var follow = new Follow
        {
            FollowerId = followerId,
            FolloweeId =  followeeId,
            CreatedAt = DateTime.UtcNow,
        };

        await followRepository.AddFollowAsync(follow);

        return true;
    }

    public async Task<bool> UnfollowAsync(Guid followerId, Guid followeeId)
    {
        var followeeExists = await userRepository.ExistsUserAsync(followeeId);
        if (!followeeExists)
            return false;

        return await followRepository.DeleteFollowAsync(followerId, followeeId);
    }
    
    public async Task<ShortProfiles?> GetFollowersAsync(
        Guid userId,
        string? cursor,
        int limit)
    {
        var userExists = await userRepository.ExistsUserAsync(userId);
        if (!userExists)
            return null;
        
        DateTime timestamp;
        Guid followerId;
        Guid followeeId;

        if (cursor == null)
        {
            timestamp = DateTime.UtcNow;
            followerId = Guid.Empty;
            followeeId = Guid.Empty;
        }
        else
        {
            (timestamp, followerId, followeeId) = CursorHelper.ParseCursorForFollows(cursor);
        }

        var follows = (await userRepository
                .GetFollowersAsync(userId, timestamp, followerId, followeeId, limit))
            .ToList();

        var shortProfileModels = follows
            .Select(f => new ShortProfile
            {
                Id = f.Follower.Id,
                Username = f.Follower.Username,
                FullName = f.Follower.FullName,
                ProfileImageUrl = f.Follower.ProfileImageUrl,
            })
            .ToList();

        var lastFollow = follows.LastOrDefault();
        var shortProfiles = new ShortProfiles
        {
            Items = shortProfileModels,
            NextCursor = CursorHelper.GenerateCursor(
                lastFollow?.CreatedAt ?? DateTime.UtcNow,
                lastFollow?.FollowerId ?? Guid.Empty,
                lastFollow?.FolloweeId ?? Guid.Empty),
        };

        return shortProfiles;
    }
    
    public async Task<ShortProfiles?> GetFollowingAsync(
        Guid userId,
        string? cursor,
        int limit)
    {
        var userExists = await userRepository.ExistsUserAsync(userId);
        if (!userExists)
            return null;
        
        DateTime timestamp;
        Guid followerId;
        Guid followeeId;

        if (cursor == null)
        {
            timestamp = DateTime.UtcNow;
            followerId = Guid.Empty;
            followeeId = Guid.Empty;
        }
        else
        {
            (timestamp, followerId, followeeId) = CursorHelper.ParseCursorForFollows(cursor);
        }

        var follows = (await userRepository
                .GetFollowingsAsync(userId, timestamp, followerId, followeeId, limit))
            .ToList();

        var shortProfileModels = follows
            .Select(f => new ShortProfile
            {
                Id = f.Followee.Id,
                Username = f.Followee.Username,
                FullName = f.Followee.FullName,
                ProfileImageUrl = f.Followee.ProfileImageUrl,
            })
            .ToList();

        var lastFollow = follows.LastOrDefault();
        var shortProfiles = new ShortProfiles
        {
            Items = shortProfileModels,
            NextCursor = CursorHelper.GenerateCursor(
                lastFollow?.CreatedAt ?? DateTime.UtcNow,
                lastFollow?.FollowerId ?? Guid.Empty,
                lastFollow?.FolloweeId ?? Guid.Empty),
        };

        return shortProfiles;
    }
}