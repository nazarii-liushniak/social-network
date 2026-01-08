namespace SocialNetwork.WebAPI.Models.Post;

public class PostElement
{
    public Guid PostId { get; set; }
    public required string Content { get; set; }
    public string? ImageUrl { get; set; }
    public int CommentsCount { get; set; }
    public int LikesCount { get; set; }
    public int IsLikedByMe { get; set; }
    public DateTime Timestamp { get; set; }
}