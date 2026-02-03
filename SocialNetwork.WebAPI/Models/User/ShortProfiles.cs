namespace SocialNetwork.WebAPI.Models.User;

public class ShortProfiles
{
    public required IEnumerable<ShortProfile> Items { get; set; }
    public required string NextCursor { get; set; }
}