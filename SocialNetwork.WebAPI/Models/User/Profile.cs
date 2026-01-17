using SocialNetwork.WebAPI.Models.Post;

namespace SocialNetwork.WebAPI.Models.User;

public class Profile
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public required string Bio { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsFollowedByMe { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public required Posts PostsPreview { get; set; }
}