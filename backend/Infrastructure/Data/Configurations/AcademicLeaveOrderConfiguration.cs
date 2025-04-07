using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class AcademicLeaveOrderConfiguration:IEntityTypeConfiguration<AcademicLeaveOrder>
{
    public void Configure(EntityTypeBuilder<AcademicLeaveOrder> builder)
    {
        builder.ConfigureGroupFrom();
    }
}