using System.Configuration;
using System.Data;
using System.Reflection;
using System.Windows;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPanel;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public static ServiceProvider ServiceProvider { get; private set; }

    public App()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
    
    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddDbContext<IApplicationDbContext,ApplicationDbContext>(options =>
            options.UseNpgsql("Host=localhost;Port=5433;Database=SteinDB;Username=admin;Password=password"));

        services.AddAutoMapper(typeof(UserProfile));

        services.AddScoped<IUserService, UserService>();
    }
}