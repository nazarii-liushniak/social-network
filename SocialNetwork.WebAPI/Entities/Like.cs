namespace SocialNetwork.WebAPI.Entities;

public class Like
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
}