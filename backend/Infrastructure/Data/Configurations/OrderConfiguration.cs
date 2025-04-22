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
            .HasForeignKey(o => o.StudentID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasDiscriminator(o => o.Discriminator)
            .HasValue<EnrollmentOrder>(nameof(EnrollmentOrder))
            .HasValue<TransferOrder>(nameof(TransferOrder))
            .HasValue<ExpulsionOrder>(nameof(ExpulsionOrder));
    }
}