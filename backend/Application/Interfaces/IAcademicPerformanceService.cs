using Application.DTOs.AcademicPerformance;
using Application.DTOs.Department;

namespace Application.Interfaces;

public interface IAcademicPerformanceService
{
    Task<BasePaginatedResult<AcademicPerformanceGetDto?>> GetPaginated(AcademicPerformancePaginatedRequest request);
}