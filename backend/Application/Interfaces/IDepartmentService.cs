using Application.DTOs.Department;

namespace Application.Interfaces;

public interface IDepartmentService
{
    Task<BasePaginatedResult<DepartmentGetDto>> GetPaginated(DepartmentPaginatedRequest request);
    Task Create(DepartmentPostDto dto);
}