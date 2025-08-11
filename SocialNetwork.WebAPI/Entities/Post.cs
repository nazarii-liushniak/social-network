namespace SocialNetwork.WebAPI.Entities;

public class Post
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}