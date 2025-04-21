using Application.DTOs.Department;

namespace Application.Interfaces;

public interface IDepartmentService
{
    Task<BasePaginatedResultDto<DepartmentGetDto>> GetPaginated(DepartmentPaginatedRequestDto request);
}