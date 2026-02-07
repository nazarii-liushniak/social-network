using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController(IChatService chatService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Chat>>> GetChatsAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);
        
        // Null suppression used because user with userId ID always exists.
        var chats = await chatService.GetChatsAsync(userId) ?? [];
        
        return Ok(chats);
    }

    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<Messages>> GetMessages(
        [FromRoute] Guid userId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 100)
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var currentUserId = Guid.Parse(currentUserIdString);

        var messages = await chatService.GetChatAsync(currentUserId, userId, cursor, limit);

        if (messages == null)
            return NotFound("User not found");
        
        return Ok(messages);
    }
}