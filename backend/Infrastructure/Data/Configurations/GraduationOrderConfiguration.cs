using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class GraduationOrderConfiguration: IEntityTypeConfiguration<GraduationOrder>
{
    public void Configure(EntityTypeBuilder<GraduationOrder> builder)
    {   
        builder.ConfigureGroupFrom();
    }
}