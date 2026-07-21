namespace SocialNetwork.WebAPI.Models.User;

public record CreateUserRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FullName { get; set; }
    public required string Description { get; set; }
    public required string ProfileImageUrl { get; set; }
}