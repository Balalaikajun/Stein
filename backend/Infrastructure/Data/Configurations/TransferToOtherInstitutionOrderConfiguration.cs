using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TransferToOtherInstitutionOrderConfiguration:IEntityTypeConfiguration<TransferToOtherInstitutionOrder>
{
    public void Configure(EntityTypeBuilder<TransferToOtherInstitutionOrder> builder)
    {
        builder.Property(o => o.ToInstituteAcronym)
            .IsRequired()
            .HasMaxLength(25);
        
       builder.ConfigureGroupTo();
    }
}