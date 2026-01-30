namespace SocialNetwork.WebAPI.Models.Message;

public class Message
{
    public required Guid Id { get; set; }
    public required MessageDirection Direction { get; set; }
    public required string Content { get; set; }
    public required DateTime Timestamp { get; set; }
}