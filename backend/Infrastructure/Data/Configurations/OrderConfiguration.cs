using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.HasOne<Student>(o => o.Student)
            .WithMany()
            .HasForeignKey(o => o.StudentID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasDiscriminator<OrderTypes>("OrderType")
            .HasValue<EnrollmentOrder>(OrderTypes.Enrollment)
            .HasValue<TransferFromOtherInstitutionOrder>(OrderTypes.TransferFromOtherInstitution)
            .HasValue<TransferToOtherInstitutionOrder>(OrderTypes.TransferToOtherInstitution)
            .HasValue<ReinstatementFromExpelledOrder>(OrderTypes.ReinstatementFromExpelled)
            .HasValue<ReinstatementFromAcademyOrder>(OrderTypes.ReinstatementFromAcademy)
            .HasValue<AcademicLeaveOrder>(OrderTypes.AcademicLeave)
            .HasValue<TransferBetweenGroupsOrder>(OrderTypes.TransferBetweenGroups)
            .HasValue<ExpulsionOrder>(OrderTypes.Expulsion)
            .HasValue<GraduationOrder>(OrderTypes.Graduation);
    }
    
}