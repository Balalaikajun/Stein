using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TransferBetweenGroupsOrderConfiguration:IEntityTypeConfiguration<TransferBetweenGroupsOrder>
{
    public void Configure(EntityTypeBuilder<TransferBetweenGroupsOrder> builder)
    {
        builder.ConfigureGroupFrom();
        builder.ConfigureGroupTo();
    }
}