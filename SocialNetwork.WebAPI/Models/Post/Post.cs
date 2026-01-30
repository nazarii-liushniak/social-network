using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace SocialNetwork.WebAPI.Models.Post;

public class Post
{
    public required Guid PostId { get; set; }
    public required string Content { get; set; }
    [Url]
    public required string? ImageUrl { get; set; }
    public required int CommentsCount { get; set; }
    public required int LikesCount { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required int? IsLikedByMe { get; set; }
    public required DateTime Timestamp { get; set; }
}