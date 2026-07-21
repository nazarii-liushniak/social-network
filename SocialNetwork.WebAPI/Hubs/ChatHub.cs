using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.WebAPI.Interfaces.Hubs;

namespace SocialNetwork.WebAPI.Hubs;

[Authorize]
public class ChatHub : Hub<IChatClient>
{
}