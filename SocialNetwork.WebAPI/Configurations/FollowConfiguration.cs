using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Configurations;

public class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("follows");

        builder.HasKey(f => new { f.FollowerId, f.FolloweeId });
        
        builder.Property(f => f.FollowerId)
            .HasColumnName("follower_id");
        
        builder.Property(f => f.FolloweeId)
            .HasColumnName("followee_id");

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created_at");

        builder.HasOne(f => f.Follower)
            .WithMany(u => u.Followees)
            .HasForeignKey(f => f.FollowerId);

        builder.HasOne(f => f.Followee)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict); // For SQL Server
    }
}