using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SocialNetwork.WebAPI.Models.Post;

public class Post
{
    public required Guid Id { get; set; }
    public required string Content { get; set; }
    [Url]
    public required string? ImageUrl { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required int? CommentsCount { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required int? LikesCount { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required bool? IsLikedByMe { get; set; }
    public required DateTime Timestamp { get; set; }
}