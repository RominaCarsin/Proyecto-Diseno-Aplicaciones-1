using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(n => n.UserId).IsRequired();
        builder.Property(n => n.Message).IsRequired();
        builder.Property(n => n.CreatedAt).IsRequired();
        builder.Property(n => n.IsRead).IsRequired();
        builder.Property(n => n.MatchId).IsRequired();

        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
