using SocialNetwork.WebAPI.Interfaces.Services;
using SocialNetwork.WebAPI.Services;

namespace SocialNetwork.WebAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IChatService, ChatService>();
        
        return services;
    }
}