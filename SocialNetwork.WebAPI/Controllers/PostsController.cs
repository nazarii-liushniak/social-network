using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Extensions;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models;
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
    public async Task<ActionResult<PostResponse>> CreatePost([FromBody] CreateOrUpdatePostRequest createOrUpdatePostRequest)
    {
        var result = await postService.CreatePostAsync(createOrUpdatePostRequest);

        return result.ToActionResult(pm => CreatedAtAction(nameof(GetPost), new { postId = pm.Id }, pm));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<PostWithAuthorResponse>>> GetFeed(
        [FromQuery] string? cursor,
        [FromQuery] int limit = 50)
    {
        var result = await postService.GetFeedAsync(cursor, limit);
        
        return result.ToActionResult();
    }

    [HttpGet("{postId:guid}")]
    public async Task<ActionResult<PostWithAuthorResponse>> GetPost(
        [FromRoute] Guid postId)
    {
        var result = await postService.GetPostAsync(postId);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpPut("{postId:guid}")]
    public async Task<ActionResult<PostResponse>> UpdatePost(
        [FromRoute] Guid postId,
        [FromBody] CreateOrUpdatePostRequest createOrUpdatePostRequest)
    {
        var result = await postService.UpdatePostAsync(postId, createOrUpdatePostRequest);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpDelete("{postId:guid}")]
    public async Task<IActionResult> DeletePost([FromRoute] Guid postId)
    {
        var result = await postService.DeletePostAsync(postId);

        return result.ToActionResult();
    }

    [HttpGet("{postId:guid}/likes")]
    public async Task<ActionResult<PagedResponse<ShortProfileResponse>>> GetLikes(
        [FromRoute] Guid postId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 100)
    {
        var result = await postService.GetUsersLikedPostAsync(postId, cursor, limit);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("{postId:guid}/likes")]
    public async Task<IActionResult> LikePost([FromRoute] Guid postId)
    {
        var result = await postService.LikePostAsync(postId);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpDelete("{postId:guid}/likes")]
    public async Task<IActionResult> UnlikePost([FromRoute] Guid postId)
    {
        var result = await postService.UnlikePostAsync(postId);
        
        return result.ToActionResult();
    }

    [HttpGet("{postId:guid}/comments")]
    public async Task<ActionResult<PagedResponse<CommentResponse>>> GetComments(
        [FromRoute] Guid postId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 50)
    {
        var result = await commentService.GetCommentsAsync(postId, cursor, limit);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("{postId:guid}/comments")]
    public async Task<ActionResult<CommentResponse>> CreateComment(
        [FromRoute] Guid postId,
        [FromBody] CreateOrUpdateCommentRequest createOrUpdateCommentRequest)
    {
        var result = await commentService.CreateCommentAsync(postId, createOrUpdateCommentRequest);
        
        return result.ToActionResult();
    }
}