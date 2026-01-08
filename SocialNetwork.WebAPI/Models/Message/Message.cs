namespace SocialNetwork.WebAPI.Models.Message;

public class Message
{
    public Guid Id { get; set; }
    public MessageDirection Direction { get; set; }
    public required string Content { get; set; }
    public DateTime Timestamp { get; set; }
}