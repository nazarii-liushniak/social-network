using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SocialNetwork.WebAPI.Models.Comment;
using SocialNetwork.WebAPI.Models.User;

namespace SocialNetwork.WebAPI.Models.Post;

public class PostWithAuthorAndComments
{
    public required Guid Id { get; set; }
    public required ShortProfile Author { get; set; }
    public required string Content { get; set; }
    [Url]
    public required string? ImageUrl { get; set; }
    public required int CommentsCount { get; set; }
    public required Comments CommentsPreview { get; set; }
    public required int LikesCount { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required bool? IsLikedByMe { get; set; }
    public required DateTime Timestamp { get; set; }
}