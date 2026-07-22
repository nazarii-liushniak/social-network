using SocialNetwork.WebAPI.Contexts;
using SocialNetwork.WebAPI.Interfaces.Contexts;

namespace SocialNetwork.WebAPI.Extensions;

public static class ContextExtensions
{
    public static IServiceCollection AddContexts(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}