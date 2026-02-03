using Microsoft.AspNetCore.JsonPatch;
using SocialNetwork.WebAPI.Helpers;
using SocialNetwork.WebAPI.Interfaces.Repositories;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models.Comment;
using SocialNetwork.WebAPI.Models.User;
using CommentEntity = SocialNetwork.WebAPI.Entities.Comment;

namespace SocialNetwork.WebAPI.Services;

public class CommentService(
    IUserRepository userRepository,
    IPostRepository postRepository,
    ICommentRepository commentRepository
) : ICommentService
{
    public async Task<Comment?> CreateCommentAsync(
        Guid userId,
        Guid postId,
        CreateOrUpdateComment comment)
    {
        var postExists = await postRepository.ExistsPostAsync(userId);
        if (!postExists)
            return null;

        var commentEntity = new CommentEntity
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            UserId = userId,
            Content = comment.Content,
            CreatedAt = DateTime.UtcNow,
        };

        await commentRepository.AddCommentAsync(commentEntity);
        
        var user = await userRepository.GetUserAsync(userId);
        if (user == null)
            return null;

        var commentModel = new Comment
        {
            Id = commentEntity.Id,
            Author = new ShortProfile
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                ProfileImageUrl = user.ProfileImageUrl,
            },
            Content = commentEntity.Content,
            Timestamp = commentEntity.CreatedAt,
        };
        
        return commentModel;
    }

    public async Task<Comments?> GetCommentsAsync(Guid postId, string? cursor, int limit = 50)
    {
        var postExists = await postRepository.ExistsPostAsync(postId);
        if (!postExists)
            return null;
        
        DateTime timestamp;
        Guid commentId;

        if (cursor == null)
        {
            timestamp = DateTime.UtcNow;
            commentId = Guid.Empty;
        }
        else
        {
            (timestamp, commentId) = CursorHelper.ParseCursor(cursor);
        }

        var comments = await commentRepository
            .GetCommentsAsync(postId, timestamp, commentId, limit);

        var commentsList = comments.Select(c => new Comment
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
        
        var lastComment = commentsList.LastOrDefault();
        var commentsModel = new Comments
        {
            Items = commentsList,
            NextCursor = CursorHelper.GenerateCursor(
                lastComment?.Timestamp ?? DateTime.UtcNow,
                lastComment?.Id ?? Guid.Empty),
        };

        return commentsModel;
    }

    public async Task<bool> UpdateCommentAsync(
        Guid userId,
        JsonPatchDocument<CreateOrUpdateComment> commentPatch)
    {
        var comment = await commentRepository.GetCommentAsync(userId);
        
        if (comment == null)
            return false;

        var updateComment = new CreateOrUpdateComment
        {
            Content = comment.Content,
        };
        
        commentPatch.ApplyTo(updateComment);
        
        comment.Content = updateComment.Content;

        await commentRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId)
    {
        return await commentRepository.DeleteCommentAsync(commentId);
    }
}