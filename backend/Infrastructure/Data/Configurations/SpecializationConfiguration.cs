using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Data.Configurations;

public class SpecializationConfiguration:IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
     builder.HasKey(s => s.Id);
     
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(25);
        
        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(150);
        
        builder.Property(s => s.Acronym)
            .IsRequired()
            .HasMaxLength(15);
        
        builder.HasOne<Department>(s => s.Department)
            .WithMany(d => d.Specializations)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}