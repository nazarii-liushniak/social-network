namespace SocialNetwork.WebAPI.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Post Post { get; set; } = null!;
    public User Author { get; set; } = null!;
}