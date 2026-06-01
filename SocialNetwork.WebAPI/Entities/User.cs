namespace SocialNetwork.WebAPI.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? FullName { get; set; }
    public string? Description { get; set; }
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public ICollection<Follow> Followers { get; set; } = new HashSet<Follow>();
    public ICollection<Follow> Followees { get; set; } = new HashSet<Follow>();
    public ICollection<Like> Likes { get; set; } = new HashSet<Like>();
    public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new HashSet<Message>();
    public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
}