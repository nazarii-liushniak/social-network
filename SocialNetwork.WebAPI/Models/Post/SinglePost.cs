using SocialNetwork.WebAPI.Models.Comment;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Models.Post;

public class SinglePost
{
    public Guid Id { get; set; }
    public required ShortProfile Author { get; set; }
    public required string Content { get; set; }
    public string? ImageUrl { get; set; }
    public int CommentsCount { get; set; }
    public required Comments CommentsPreview { get; set; }
    public int LikesCount { get; set; }
    public bool IsLikedByMe { get; set; }
    public DateTime Timestamp { get; set; }
}