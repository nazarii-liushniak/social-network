using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");

        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .HasColumnName("id");
        
        builder.Property(c => c.PostId)
            .HasColumnName("post_id");
        
        builder.Property(c => c.UserId)
            .HasColumnName("user_id");

        builder.Property(c => c.Content)
            .HasColumnName("content")
            .HasMaxLength(int.MaxValue)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(c => new { c.CreatedAt, c.Id });
    }
}