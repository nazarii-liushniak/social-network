using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models.Comment;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(
    IPostService postService,
    ICommentService commentService
) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost([FromBody] CreateOrUpdatePost post)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);
        
        // Null suppression is used because user with userId ID always exists.
        var createdPost = (await postService.PostAsync(userId, post))!;
        
        return CreatedAtAction(
            nameof(GetPost), 
            new { postId = createdPost.Id },
            createdPost);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<Feed>> GetFeed(
        [FromQuery] string? cursor,
        [FromQuery] int limit = 50)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);
        
        // Null suppression is used because user with userId ID always exists.
        var feed = (await postService.GetFeedAsync(userId, cursor, limit))!;
        
        return Ok(feed);
    }

    [HttpGet("{postId:guid}")]
    public async Task<ActionResult<PostWithAuthorAndComments>> GetPost(
        [FromRoute] Guid postId,
        [FromQuery] int commentsLimit = 50)
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserId = currentUserIdString == null
            ? (Guid?)null
            : Guid.Parse(currentUserIdString);

        var post = await postService.GetPostAsync(currentUserId, postId, commentsLimit);

        if (post == null)
            return NotFound("Post not found");

        return Ok(post);
    }

    [Authorize]
    [HttpPatch("{postId:guid}")]
    public async Task<IActionResult> PatchPost(
        [FromRoute] Guid postId,
        [FromBody] JsonPatchDocument<CreateOrUpdatePost> postPatch)
    {
        var postExists = await postService.UpdatePostAsync(postId, postPatch);

        if (!postExists)
            return NotFound("Post not found");
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{postId:guid}")]
    public async Task<IActionResult> DeletePost([FromRoute] Guid postId)
    {
        var postExists = await postService.DeletePostAsync(postId);
        
        if (!postExists)
            return NotFound("Post not found");
        
        return NoContent();
    }

    [HttpGet("{postId:guid}/likes")]
    public async Task<ActionResult<ShortProfiles>> GetLikes(
        [FromRoute] Guid postId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 100)
    {
        var likes = await postService.GetUsersLikedPostAsync(postId, cursor, limit);
        
        if (likes == null)
            return NotFound("Post not found");
        
        return Ok(likes);
    }

    [Authorize]
    [HttpPost("{postId:guid}/likes")]
    public async Task<IActionResult> LikePost([FromRoute] Guid postId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);

        var postExists = await postService.LikePostAsync(userId, postId);

        if (!postExists)
            return NotFound("Post not found");
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{postId:guid}/likes")]
    public async Task<IActionResult> UnlikePost([FromRoute] Guid postId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);

        var postExists = await postService.UnlikePostAsync(userId, postId);
        
        if (!postExists)
            return NotFound("Post not found");
        
        return NoContent();
    }

    [HttpGet("{postId:guid}/comments")]
    public async Task<ActionResult<Comments>> GetComments(
        [FromRoute] Guid postId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 50)
    {
        var comments = await commentService.GetCommentsAsync(postId, cursor, limit);
        
        if (comments == null)
            return NotFound("Post not found");

        return Ok(comments);
    }

    [Authorize]
    [HttpPost("{postId:guid}/comments")]
    public async Task<ActionResult<Comment>> CreateComment(
        [FromRoute] Guid postId,
        [FromBody] CreateOrUpdateComment createComment)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);

        var comment = await commentService.CreateCommentAsync(userId, postId, createComment);
        
        if (comment == null)
            return NotFound("Post not found");
        
        return Ok(comment);
    }

    [Authorize]
    [HttpPatch("{postId:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> UpdateComment(
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId,
        [FromBody] JsonPatchDocument<CreateOrUpdateComment> updateCommentPatch)
    {
        var commentExists = await commentService
            .UpdateCommentAsync(postId, commentId, updateCommentPatch);

        if (!commentExists)
            return NotFound("Comment or post not found");

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{postId:guid}/comments/{commentId:guid}")]
    public async Task<IActionResult> DeleteComment(
        [FromRoute] Guid postId,
        [FromRoute] Guid commentId)
    {
        var commentExists = await commentService.DeleteCommentAsync(postId, commentId);

        if (!commentExists)
            return NotFound("");

        return NoContent();
    }
}