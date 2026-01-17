using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Models.Message;

public class Chat
{
    public required ShortProfile Author { get; set; }
    public required string LastMessageContent { get; set; }
    public DateTime LastMessageTimestamp { get; set; }
}