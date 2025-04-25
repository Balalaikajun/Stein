using Application.DTOs.Department;

namespace Application.Interfaces;

public interface IDepartmentService
{
    Task<BasePaginatedResult<DepartmentGetDto>> GetPaginated(DepartmentPaginatedRequest request);
    Task Create(DepartmentPostDto dto);
    Task Update(DepartmentPatchDto dto);
    
    Task Delete(int id);
}