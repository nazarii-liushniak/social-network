using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.WebAPI.Settings;

public class AuthSettings
{
    [Required, Range(1, int.MaxValue)]
    public int RefreshTokenExpirationDays { get; set; }
}