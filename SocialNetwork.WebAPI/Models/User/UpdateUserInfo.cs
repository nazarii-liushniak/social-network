using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Models.User;

public class UpdateUserInfo
{
    public required string Username { get; set; }
    public required string? FullName { get; set; }
    public required string? Bio { get; set; }
    [Url]
    public required string? ProfileImageUrl { get; set; }
}