using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Models.User;

public class UserInfo
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string? FullName { get; set; }
    public required string? Description { get; set; }
    [Url]
    public required string? ProfileImageUrl { get; set; }
}