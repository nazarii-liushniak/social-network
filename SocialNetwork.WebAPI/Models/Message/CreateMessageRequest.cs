namespace SocialNetwork.WebAPI.Models.Message;

public record CreateMessageRequest
{
    public required string Content { get; set; }
}