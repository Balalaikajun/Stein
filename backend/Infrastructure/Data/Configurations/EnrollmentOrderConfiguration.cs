using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class EnrollmentOrderConfiguration:IEntityTypeConfiguration<EnrollmentOrder>
{
    public void Configure(EntityTypeBuilder<EnrollmentOrder> builder)
    {
        builder.ConfigureGroupTo();
    }
}