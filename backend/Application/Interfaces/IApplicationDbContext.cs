using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Department> Departments { get; set; }
    DbSet<Specialization> Specializations { get; set; }
    DbSet<Group> Groups { get; set; }
    DbSet<Student> Students { get; set; }
    DbSet<Teacher> Teachers { get; set; }
    DbSet<AcademicPerformance> AcademicPerformances { get; set; }
    DbSet<Order> Orders { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}