using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TransferFromOtherInstitutionOrderConfiguration:
    IEntityTypeConfiguration<TransferFromOtherInstitutionOrder>
{
    public void Configure(EntityTypeBuilder<TransferFromOtherInstitutionOrder> builder)
    {
        builder.Property(o => o.FromInstituteAcronym)
            .IsRequired()
            .HasMaxLength(25);
        
        builder.ConfigureGroupFrom();
    }
}