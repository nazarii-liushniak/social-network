using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SocialNetwork.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(IMemoryCache memoryCache) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public ActionResult<string> CreateTicket()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var ticket = Guid.NewGuid().ToString("N");

        memoryCache.Set(ticket, userId, TimeSpan.FromSeconds(20));

        return Ok(ticket);
    }
}