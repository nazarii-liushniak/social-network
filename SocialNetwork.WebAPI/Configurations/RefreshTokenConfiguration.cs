using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(rt => rt.Token);

        builder.Property(rt => rt.Token)
            .HasColumnName("token");
        
        builder.Property(rt => rt.UserId)
            .HasColumnName("user_id");
        
        builder.Property(rt => rt.CreatedAt)
            .HasColumnName("created_at");
        
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
    }
}