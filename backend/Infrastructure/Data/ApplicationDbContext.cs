using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Data;

public class ApplicationDbContext: DbContext, IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Department> Departments { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    
    public DbSet<AcademicPerformance> AcademicPerformances { get; set; }
    
    // Хранит все виды приказов
    public DbSet<Order> Orders { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}