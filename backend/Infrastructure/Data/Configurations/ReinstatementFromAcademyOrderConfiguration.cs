using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ReinstatementFromAcademyOrderConfiguration:IEntityTypeConfiguration<ReinstatementFromAcademyOrder>
{
    public void Configure(EntityTypeBuilder<ReinstatementFromAcademyOrder> builder)
    {
        builder.ConfigureGroupFrom();
    }
}