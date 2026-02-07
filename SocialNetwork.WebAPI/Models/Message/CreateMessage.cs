using Newtonsoft.Json;

namespace SocialNetwork.WebAPI.Models.Message;

public class CreateMessage
{
    public required Guid ReceiverId { get; set; }
    public required string Content { get; set; }
}