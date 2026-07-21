namespace SocialNetwork.WebAPI.Entities;

public class RefreshToken
{
    public Guid Token { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public User User { get; set; } = null!;
}