using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;

namespace SocialNetwork.WebAPI.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SocialNetworkDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SocialNetworkDb"))
        );

        return services;
    }
}