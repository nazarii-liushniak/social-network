using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace SocialNetwork.WebAPI.Extensions;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetRequiredSection("JwtSettings");

        var secretKey = jwtSettings["Secret"]
            ?? throw new InvalidOperationException("Option JwtSettings:Secret not found in appsettings.json");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    NameClaimType = ClaimTypes.NameIdentifier,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var ticket = context.Request.Query["ticket"];
                        var path = context.Request.Path;

                        if (!string.IsNullOrWhiteSpace(ticket) && path.StartsWithSegments("/hubs/chat"))
                        {
                            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

                            if (cache.TryGetValue(ticket, out string? userId))
                            {
                                cache.Remove(ticket);
                                
                                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId!) };
                                var identity = new ClaimsIdentity(claims, context.Scheme.Name);
                                context.Principal = new ClaimsPrincipal(identity);
                                context.Success();
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}