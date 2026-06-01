using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialNetwork.WebAPI.Entities;

namespace SocialNetwork.WebAPI.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Id)
            .HasColumnName("id");
        
        builder.Property(m => m.SenderId)
            .HasColumnName("sender_id");
        
        builder.Property(m => m.ReceiverId)
            .HasColumnName("receiver_id");

        builder.Property(m => m.Content)
            .HasColumnName("content")
            .HasMaxLength(3000);

        builder.Property(m => m.SentAt)
            .HasColumnName("sent_at");

        builder.HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId);

        builder.HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}