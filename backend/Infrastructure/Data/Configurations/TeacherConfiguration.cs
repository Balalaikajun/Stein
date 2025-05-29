using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Surname)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(t => t.Patronymic)
            .IsRequired()
            .HasMaxLength(32);
    }
}