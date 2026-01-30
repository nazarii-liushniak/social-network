namespace SocialNetwork.WebAPI.Models.Message;

public class Chats
{
    public required IEnumerable<Chat> Items { get; set; }
    public required string NextCursor { get; set; }
}