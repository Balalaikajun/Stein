using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public static class EntityTypeBuilderExtensions
{
    public static void ConfigureGroupFrom<T>(this EntityTypeBuilder<T> builder) where T : class, IHasGroupFrom
    {
        builder.Property(o => o.FromSpecializationId)
            .IsRequired()
            .HasColumnName("FromSpecializationId");

        builder.Property(o => o.FromYear)
            .IsRequired()
            .HasColumnName("FromYear");

        builder.Property(o => o.FromGroupId)
            .IsRequired()
            .HasColumnName("FromGroupId");

        builder.HasOne(o => o.FromGroup)
            .WithMany()
            .HasForeignKey(o => new
            {
                o.FromSpecializationId,
                o.FromYear,
                o.FromGroupId
            })
            .OnDelete(DeleteBehavior.Cascade);
    }

    public static void ConfigureGroupTo<T>(this EntityTypeBuilder<T> builder) where T : class, IHasGroupTo
    {
        builder.Property(o => o.ToSpecializationId)
            .IsRequired()
            .HasColumnName("ToSpecializationId");

        builder.Property(o => o.ToYear)
            .IsRequired()
            .HasColumnName("ToYear");

        builder.Property(o => o.ToGroupId)
            .IsRequired()
            .HasColumnName("ToGroupId");

        builder.HasOne(o => o.ToGroup)
            .WithMany()
            .HasForeignKey(o => new
            {
                o.ToSpecializationId,
                o.ToYear,
                o.ToGroupId
            })
            .OnDelete(DeleteBehavior.Cascade);
    }
}