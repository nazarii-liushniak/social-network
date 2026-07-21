using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Models.User;

public record UpdateUserModelRequest
{
    public required string Username { get; set; }
    public required string? FullName { get; set; }
    public required string? Description { get; set; }
    [Url]
    public required string? ProfileImageUrl { get; set; }
}