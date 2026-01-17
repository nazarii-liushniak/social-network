namespace SocialNetwork.WebAPI.Models.Post;

public class CreatePost
{
    public required string Content { get; set; }
    public string? ImageUrl { get; set; }
}