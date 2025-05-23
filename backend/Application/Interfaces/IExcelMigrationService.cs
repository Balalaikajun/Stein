using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IExcelMigrationService
{
    Task MigrateAsync(IFormFile file);
}