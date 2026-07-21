namespace SocialNetwork.WebAPI.Models;

public record TokensResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}