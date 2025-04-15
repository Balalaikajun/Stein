using Application.DTOs.Department;

namespace Application.Interfaces;

public interface IDepartmentService
{
    Task<DepartmentOptionResultDto> GetDepartments(DepartmentOptionRequestDto request);
}