using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.Property(u => u.Username)
            .HasColumnName("username")
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255);

        builder.Property(u => u.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(100);

        builder.Property(u => u.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(u => u.ProfileImageUrl)
            .HasColumnName("profile_image_url")
            .HasMaxLength(1000);

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(u => u.Username)
            .HasDatabaseName("uq_users_username")
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .HasDatabaseName("uq_users_email")
            .IsUnique();
    }
}