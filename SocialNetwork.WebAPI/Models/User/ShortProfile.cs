namespace SocialNetwork.WebAPI.Models.User;

public class ShortProfile
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required string ProfilePicture { get; set; }
}