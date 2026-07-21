using Microsoft.EntityFrameworkCore;
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
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SocialNetworkDbContext).Assembly);
    }
}