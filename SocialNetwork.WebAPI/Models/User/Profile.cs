using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SocialNetwork.WebAPI.Models.Post;

namespace SocialNetwork.WebAPI.Models.User;

public class Profile
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string? FullName { get; set; }
    public required string? Bio { get; set; }
    [Url]
    public required string? ProfileImageUrl { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required bool? IsFollowedByMe { get; set; }
    public required int FollowersCount { get; set; }
    public required int FollowingCount { get; set; }
    public required Posts PostsPreview { get; set; }
}