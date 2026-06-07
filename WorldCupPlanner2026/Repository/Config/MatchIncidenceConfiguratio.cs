using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Config;

public class MatchIncidenceConfiguration : IEntityTypeConfiguration<MatchIncidence>
{
    public void Configure(EntityTypeBuilder<MatchIncidence> builder)
    {
        builder.HasKey(i => i.IncidenceId);
        builder.Property(i => i.Minute).IsRequired();

        // TPH: una sola tabla, columna discriminadora explícita.
        // Agregar VarIntervention a futuro = un valor más acá, sin cambiar esquema.
        builder.HasDiscriminator<string>("IncidenceType")
            .HasValue<YellowCard>("Yellow")
            .HasValue<RedCard>("Red");

        // Cada incidencia apunta al equipo al que pertenece.
        builder.HasOne(i => i.Team)
            .WithMany()
            .HasForeignKey("TeamId")
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}