using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SocialNetwork.WebAPI.Models.Post;

namespace SocialNetwork.WebAPI.Models.User;

public class Profile
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    [Url]
    public string? ProfileImageUrl { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsFollowedByMe { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public required Posts PostsPreview { get; set; }
}