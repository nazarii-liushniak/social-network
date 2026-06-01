using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Data;

namespace SocialNetwork.WebAPI.Extensions;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SocialNetworkDb") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");
        
        services.AddDbContext<SocialNetworkDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        return services;
    }
}