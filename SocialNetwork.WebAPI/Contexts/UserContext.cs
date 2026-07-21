using System.Security.Claims;
using SocialNetwork.WebAPI.Interfaces.Contexts;

namespace SocialNetwork.WebAPI.Contexts;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    public Guid? UserId
    {
        get
        {
            var userIdString = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }
    }
}