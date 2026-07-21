using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Extensions;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.Post;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    IUserService userService,
    IPostService postService
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ShortProfileResponse>>> GetUsers(
        [FromQuery] string? cursor,
        [FromQuery] int limit = 50)
    {
        var result = await userService.GetUsersAsync(cursor, limit);

        return result.ToActionResult();
    }
    
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<ProfileResponse>> GetUser([FromRoute] Guid userId)
    {
        var result = await userService.GetUserProfileAsync(userId);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe()
    {
        var result = await userService.GetUserModelAsync();
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UpdateUserModelRequest updateUserModelRequest)
    {
        var result = await userService.UpdateUserModelAsync(updateUserModelRequest);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        var result = await userService.DeleteUserAsync();
        
        return result.ToActionResult();
    }
    
    [HttpGet("{userId:guid}/posts")]
    public async Task<ActionResult<PagedResponse<PostResponse>>> GetPosts(
        [FromRoute] Guid userId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 20)
    {
        var result = await postService.GetPostsAsync(userId, cursor, limit);
        
        return result.ToActionResult();
    }

    [HttpGet("{userId:guid}/followers")]
    public async Task<ActionResult<PagedResponse<ShortProfileResponse>>> GetFollowers(
        [FromRoute] Guid userId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 100)
    {
        var result = await userService.GetFollowersAsync(userId, cursor, limit);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("{userId:guid}/followers")]
    public async Task<IActionResult> Follow([FromRoute] Guid userId)
    {
        var result = await userService.FollowAsync(userId);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpDelete("{userId:guid}/followers")]
    public async Task<IActionResult> Unfollow([FromRoute] Guid userId)
    {
        var result = await userService.UnfollowAsync(userId);
        
        return result.ToActionResult();
    }

    [HttpGet("{userId:guid}/following")]
    public async Task<ActionResult<PagedResponse<ShortProfileResponse>>> GetFollowing(
        [FromRoute] Guid userId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 50)
    {
        var result = await userService.GetFolloweesAsync(userId, cursor, limit);
        
        return result.ToActionResult();
    }
}