using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class KnockoutConfiguration : IEntityTypeConfiguration<Knockout>
{
    public void Configure(EntityTypeBuilder<Knockout> builder)
    {
        builder.HasKey(p => p.KnockoutId);
        builder.Property(k => k.SimulationSeed).IsRequired();
        builder.Property(k => k.SemillaCrucesFase).IsRequired();
        builder.HasMany(k => k.QualifiedTeams).WithOne().OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(k => k.Matches).WithOne().OnDelete(DeleteBehavior.NoAction);

        // TO DO: _shuffleService es una dependencia inyectada en el constructor público.
        // EF usa el constructor privado vacío para hidratar, así que _shuffleService
        // queda null al leer desde BD. Cualquier método que lo use (InitializeRoundOf32,
        // PairTeams, SimulateMatches) va a explotar en runtime si se llama sobre una
        // instancia hidratada por EF.
        // Cuando se separe la lógica a KnockoutService, este problema desaparece.
    }
}