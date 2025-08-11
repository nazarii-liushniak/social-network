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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Comments
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");

            entity.HasKey(c => c.Id);
            
            entity.Property(c => c.Id)
                .HasColumnName("id");
            
            entity.Property(c => c.PostId)
                .HasColumnName("post_id");
            
            entity.Property(c => c.UserId)
                .HasColumnName("user_id");

            entity.Property(c => c.Content)
                .HasColumnName("content")
                .HasMaxLength(int.MaxValue)
                .IsRequired();

            entity.Property(c => c.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Follows
        modelBuilder.Entity<Follow>(entity =>
        {
            entity.ToTable("follows");

            entity.HasKey(f => new { f.FollowerId, f.FolloweeId });
            
            entity.Property(f => f.FollowerId)
                .HasColumnName("follower_id");
            
            entity.Property(f => f.FolloweeId)
                .HasColumnName("followee_id");

            entity.Property(f => f.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.HasOne(f => f.Follower)
                .WithMany(u => u.Followings)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Likes
        modelBuilder.Entity<Like>(entity =>
            {
            entity.ToTable("likes");

            entity.HasKey(l => new { l.PostId, l.UserId });
            
            entity.Property(l => l.PostId)
                .HasColumnName("post_id");
            
            entity.Property(l => l.UserId)
                .HasColumnName("user_id");

            entity.Property(l => l.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Messages
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("messages");

            entity.HasKey(m => m.Id);
            
            entity.Property(m => m.Id)
                .HasColumnName("id");
            
            entity.Property(m => m.SenderId)
                .HasColumnName("sender_id");
            
            entity.Property(m => m.ReceiverId)
                .HasColumnName("receiver_id");

            entity.Property(m => m.Content)
                .HasColumnName("content")
                .HasMaxLength(int.MaxValue)
                .IsRequired();

            entity.Property(m => m.SentAt)
                .HasColumnName("sent_at")
                .IsRequired();

            entity.HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Posts
        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("posts");

            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Id)
                .HasColumnName("id");
            
            entity.Property(p => p.UserId)
                .HasColumnName("user_id");

            entity.Property(p => p.Content)
                .HasColumnName("content")
                .HasMaxLength(int.MaxValue)
                .IsRequired();

            entity.Property(p => p.ImageUrl)
                .HasColumnName("image_url")
                .HasMaxLength(255);

            entity.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasColumnName("id")
                .IsRequired();

            entity.Property(u => u.Username)
                .HasColumnName("username")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(u => u.Salt)
                .HasColumnName("salt")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(u => u.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(100);

            entity.Property(u => u.Bio)
                .HasColumnName("bio")
                .HasMaxLength(int.MaxValue);

            entity.Property(u => u.ProfilePicture)
                .HasColumnName("profile_picture")
                .HasMaxLength(255);

            entity.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}