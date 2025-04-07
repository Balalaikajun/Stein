using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AcademicPerformanceConfiguration:IEntityTypeConfiguration<AcademicPerformance>
{
    public void Configure(EntityTypeBuilder<AcademicPerformance> builder)
    {
        builder.HasKey(ap => new { ap.Year, ap.Month, ap.StudentId });

        builder.Property(ap => ap.Year)
            .IsRequired();
        
        builder.Property(ap => ap.Month)
            .IsRequired();
        
        builder.Property(ap => ap.StudentId)
            .IsRequired();
        
        builder.Property(ap => ap.Performance)
            .IsRequired();
    }
}