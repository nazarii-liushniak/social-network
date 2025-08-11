namespace SocialNetwork.WebAPI.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Salt { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
    public ICollection<Follow> Followings { get; set; } = new List<Follow>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}