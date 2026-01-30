using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Models.Message;

public class Chat
{
    public required ShortProfile OtherUser { get; set; }
    public required string LastMessageContent { get; set; }
    public required DateTime LastMessageTimestamp { get; set; }
}