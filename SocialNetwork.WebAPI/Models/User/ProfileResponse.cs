using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SocialNetwork.WebAPI.Models.User;

public record ProfileResponse
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string? FullName { get; set; }
    public required string? Description { get; set; }
    [Url]
    public required string? ProfileImageUrl { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required bool? IsFollowedByMe { get; set; }
    public required int FollowersCount { get; set; }
    public required int FolloweesCount { get; set; }
}