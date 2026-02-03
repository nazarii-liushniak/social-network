namespace SocialNetwork.WebAPI.Models.Post;

public class Feed
{
    public required IEnumerable<PostWithAuthor> Items { get; set; }
    public required string NextCursor { get; set; }
}