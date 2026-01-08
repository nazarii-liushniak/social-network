namespace SocialNetwork.WebAPI.Models.User;

public class UserInfo
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string Bio { get; set; }
    public string? ProfilePicture { get; set; }
}