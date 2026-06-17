using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class MatchIncidenceConfiguration : IEntityTypeConfiguration<MatchIncidence>
{
    public void Configure(EntityTypeBuilder<MatchIncidence> builder)
    {
        builder.HasKey(i => i.IncidenceId);
        builder.Property(i => i.Minute).IsRequired();
        
        builder.HasDiscriminator<string>("IncidenceType")
            .HasValue<YellowCard>("Yellow")
            .HasValue<RedCard>("Red");
        
        builder.HasOne(i => i.Team)
            .WithMany()
            .HasForeignKey("TeamId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}