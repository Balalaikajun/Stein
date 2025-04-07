using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class StudentConfiguration:IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(s => s.Surname)
            .IsRequired()
            .HasMaxLength(32);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(32);
        
        builder.Property(s => s.Patronymic)
            .IsRequired()
            .HasMaxLength(32);

        builder.HasOne<Group>(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => new
            {
                s.GroupSpecializationId,
                s.GroupYear,
                s.GroupId
            })
            .OnDelete(DeleteBehavior.Cascade);
    }
}