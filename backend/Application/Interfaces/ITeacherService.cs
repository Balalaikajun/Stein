using Application.DTOs.Department;
using Application.DTOs.Teacher;

namespace Application.Interfaces;

public interface ITeacherService
{
    Task<BasePaginatedResult<TeacherGetDto>> GetPaginated(TeacherPaginatedRequest request);
}