using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne<Student>(o => o.Student)
            .WithMany()
            .HasForeignKey(o => o.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(o => o.FromGroup)
            .WithMany(g => g.ExplitOrders)
            .HasForeignKey(o => new
            {
                o.FromSpecializationId,
                o.FromYear,
                o.FromGroupId
            })
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(o => o.ToGroup)
            .WithMany(g => g.EnrollmentOrders)
            .HasForeignKey(o => new
            {
                o.ToSpecializationId,
                o.ToYear,
                o.ToGroupId
            })
            .OnDelete(DeleteBehavior.Cascade);
    }
}