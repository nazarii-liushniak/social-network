using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Extensions;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models;
using SocialNetwork.WebAPI.Models.Message;

namespace SocialNetwork.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatsController(IChatService chatService) : ControllerBase
{
    [Authorize]
    [HttpPost("{otherUserId:guid}")]
    public async Task<ActionResult<MessageResponse>> SendMessage(
        [FromRoute] Guid otherUserId,
        [FromBody] CreateMessageRequest createMessageRequest)
    {
        var result = await chatService.SendMessageAsync(otherUserId, createMessageRequest);

        return result.ToActionResult();
    }
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ChatResponse>>> GetChatsAsync(
        [FromQuery] string? cursor,
        [FromQuery] int limit = 100)
    {
        var result = await chatService.GetChatsAsync(cursor, limit);
        
        return result.ToActionResult();
    }

    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<PagedResponse<MessageResponse>>> GetMessages(
        [FromRoute] Guid userId,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 100)
    {
        var result = await chatService.GetChatAsync(userId, cursor, limit);
        
        return result.ToActionResult();
    }
}