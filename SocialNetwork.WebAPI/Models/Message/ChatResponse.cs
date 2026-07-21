using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Models.Message;

public record ChatResponse
{
    public required Author OtherUser { get; set; }
    public required string LastMessageContent { get; set; }
    public required DateTimeOffset LastMessageTimestamp { get; set; }
}