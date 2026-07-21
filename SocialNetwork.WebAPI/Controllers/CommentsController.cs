using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.WebAPI.Extensions;
using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Models.Comment;

namespace SocialNetwork.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [Authorize]
    [HttpPut("{commentId:guid}")]
    public async Task<ActionResult<CommentResponse>> UpdateComment(
        [FromRoute] Guid commentId,
        [FromBody] CreateOrUpdateCommentRequest createOrUpdateCommentRequest)
    {
        var result = await commentService.UpdateCommentAsync(commentId, createOrUpdateCommentRequest);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId)
    {
        var result = await commentService.DeleteCommentAsync(commentId);

        return result.ToActionResult();
    }
}