using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.WebAPI.Interfaces.Services;

namespace SocialNetwork.WebAPI.Services;

public class TokenService(
    TimeProvider timeProvider,
    IConfiguration configuration) : ITokenService
{
    public string GenerateToken(Guid userId)
    {
        var jwtSettings = configuration.GetRequiredSection("JwtSettings");
        var secretKey = jwtSettings["Secret"]
            ?? throw new InvalidOperationException("JWT secret not found in appsettings.json");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: timeProvider.GetUtcNow().AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]
                ?? throw new InvalidOperationException("JwtSettings:ExpiryMinutes not found in appsettings.json"))).UtcDateTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}