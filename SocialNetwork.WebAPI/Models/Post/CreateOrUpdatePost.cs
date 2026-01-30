using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Models.Post;

public class CreateOrUpdatePost
{
    public required string Content { get; set; }
    [Url]
    public required string? ImageUrl { get; set; }
}