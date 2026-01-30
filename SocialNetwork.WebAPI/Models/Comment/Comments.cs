namespace SocialNetwork.WebAPI.Models.Comment;

public class Comments
{
    public required IEnumerable<Comment> Items { get; set; }
    public required string NextCursor { get; set; }
}