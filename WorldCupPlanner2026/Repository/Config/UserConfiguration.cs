using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.UserId);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.Name).IsRequired();
        builder.Property(u => u.Surname).IsRequired();
        builder.Property(u => u.Email).IsRequired();
        builder.Property(u => u.DateOfBirth).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Roles).IsRequired().HasConversion<int>();
    }
}