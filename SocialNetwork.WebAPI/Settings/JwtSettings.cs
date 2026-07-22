using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Settings;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    
    [Required, MinLength(32)]
    public required string Secret { get; set; }
    [Required, Url]
    public required string Issuer { get; set; }
    [Required, Url]
    public required string Audience { get; set; }
    [Required, Range(0, int.MaxValue)]
    public int ExpiryMinutes { get; set; }
}