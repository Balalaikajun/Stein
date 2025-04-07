using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ReinstatementFromExpelledOrderConfiguration:IEntityTypeConfiguration<ReinstatementFromExpelledOrder>
{
    public void Configure(EntityTypeBuilder<ReinstatementFromExpelledOrder> builder)
    {
        builder.ConfigureGroupFrom();
    }
}