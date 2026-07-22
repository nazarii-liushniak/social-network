using SocialNetwork.WebAPI.Settings;

namespace SocialNetwork.WebAPI.Extensions;

public static class SettingsExtensions
{
    public static IServiceCollection AddSettingsOptions(this IServiceCollection services)
    {
        services.AddOptions<JwtSettings>()
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<AuthSettings>()
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}