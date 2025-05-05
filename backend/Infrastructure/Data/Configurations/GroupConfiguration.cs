using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class GroupConfiguration: IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => new
        {
            g.SpecializationId,
            g.Year,
            g.Index
        });

        builder.HasOne<Specialization>(g => g.Specialization)
            .WithMany()
            .HasForeignKey(g => g.SpecializationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(g => g.Year)
            .IsRequired();
            
        builder.Property(g => g.Index)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property(g => g.Acronym)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.HasOne<Teacher>(g => g.Teacher)
            .WithMany(t => t.Groups)
            .HasForeignKey(g => g.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}