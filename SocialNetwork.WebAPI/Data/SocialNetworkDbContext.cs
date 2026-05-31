using Microsoft.EntityFrameworkCore;
using SocialNetwork.WebAPI.Configurations;
using SocialNetwork.WebAPI.Converters;
using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Data;

public class SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
    : DbContext(options)
{
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new CommentConfiguration().Configure(modelBuilder.Entity<Comment>());
        new FollowConfiguration().Configure(modelBuilder.Entity<Follow>());
        new LikeConfiguration().Configure(modelBuilder.Entity<Like>());
        new MessageConfiguration().Configure(modelBuilder.Entity<Message>());
        new PostConfiguration().Configure(modelBuilder.Entity<Post>());
        new UserConfiguration().Configure(modelBuilder.Entity<User>());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();
    }
}