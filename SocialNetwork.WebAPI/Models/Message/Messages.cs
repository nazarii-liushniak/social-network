namespace SocialNetwork.WebAPI.Models.Message;

public class Messages
{
    public required IEnumerable<Message> Items { get; set; }
    public required string NextCursor { get; set; }
}