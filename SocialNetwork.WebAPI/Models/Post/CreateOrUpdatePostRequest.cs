using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Models.Post;

public record CreateOrUpdatePostRequest
{
    public required string Content { get; set; }
    [Url]
    public required string? ImageUrl { get; set; }
}