using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Models.User;

public record ShortProfileResponse
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string? FullName { get; set; }
    [Url]
    public required string? ProfileImageUrl { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
}