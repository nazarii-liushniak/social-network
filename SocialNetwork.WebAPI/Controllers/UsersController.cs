using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models.User;
using Microsoft.AspNetCore.JsonPatch;

namespace SocialNetwork.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    IUserService userService
) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<Profile>> GetUser(
        [FromRoute] Guid userId,
        [FromQuery] int postsLimit = 20)
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentUserId = currentUserIdString == null
            ? (Guid?)null
            : Guid.Parse(currentUserIdString);
        
        var user = await userService
            .GetUserProfileAsync(currentUserId, userId, postsLimit);

        if (user == null)
            return NotFound("User not found");
        
        return Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserInfo>> GetMe()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);
        
        var userInfo = await userService.GetUserInfoAsync(userId);
        
        return Ok(userInfo);
    }

    [Authorize]
    [HttpPatch("me")]
    public async Task<IActionResult> PatchMe(
        [FromBody] JsonPatchDocument<UpdateUserInfo> userInfoPatch)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);
        
        _ = await userService.UpdateUserAsync(userId, userInfoPatch);
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMe()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);
        
        _ = await userService.DeleteUserAsync(userId);
        
        return NoContent();
    }
    
    [HttpGet("{userId:guid}/posts")]
    public async Task<ActionResult> GetPosts(
        [FromRoute] Guid userId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 20)
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var currentUserId = Guid.Parse(currentUserIdString);
        
        var posts = await userService.GetPostsAsync(currentUserId, userId, cursor, limit);
        
        if (posts == null)
            return NotFound("User not found");
        
        return Ok(posts);
    }

    [HttpGet("{userId:guid}/followers")]
    public async Task<ActionResult<ShortProfiles>> GetFollowers(
        [FromRoute] Guid userId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 20)
    {
        var followers = await userService.GetFollowersAsync(userId, cursor, limit);
        
        if (followers == null)
            return NotFound("User not found");
        
        return Ok(followers);
    }

    [Authorize]
    [HttpPost("{userId:guid}/followers")]
    public async Task<IActionResult> Follow([FromRoute] Guid userId)
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var currentUserId = Guid.Parse(currentUserIdString);

        var followeeExists = await userService.FollowAsync(currentUserId, userId);

        if (!followeeExists)
            return NotFound("User not found");
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{userId:guid}/followers")]
    public async Task<IActionResult> Unfollow([FromRoute] Guid userId)
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var currentUserId = Guid.Parse(currentUserIdString);

        var followeeExists = await userService.UnfollowAsync(currentUserId, userId);
        
        if (!followeeExists)
            return NotFound("User not found");
        
        return NoContent();
    }

    [HttpGet("{userId:guid}/following")]
    public async Task<ActionResult<ShortProfiles>> GetFollowing(
        [FromRoute] Guid userId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 50)
    {
        var following = await userService.GetFollowingAsync(userId, cursor, limit);

        if (following == null)
            return NotFound("User not found");
        
        return Ok(following);
    }
}