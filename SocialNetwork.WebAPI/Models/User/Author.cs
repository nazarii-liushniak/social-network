using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Models.User;

public record Author
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string? FullName { get; set; }
    [Url]
    public required string? ProfileImageUrl { get; set; }
}