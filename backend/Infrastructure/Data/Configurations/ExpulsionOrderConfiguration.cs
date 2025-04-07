using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ExpulsionOrderConfiguration:IEntityTypeConfiguration<ExpulsionOrder>
{
    public void Configure(EntityTypeBuilder<ExpulsionOrder> builder)
    {
        builder.ConfigureGroupFrom();
    }
}