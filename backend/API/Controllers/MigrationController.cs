using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MigrationController: ControllerBase
{
    private readonly IExcelMigrationService _migrationService;

    public MigrationController(IExcelMigrationService migrationService)
    {
        _migrationService = migrationService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран.");

        if (file.ContentType == "application/vnd.ms-excel")
            return BadRequest("Данный формат файла не поддерживается");

        await _migrationService.MigrateAsync(file);
        
        return Ok();
    }
}