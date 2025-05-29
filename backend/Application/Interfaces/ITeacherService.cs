using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Teacher;

namespace Application.Interfaces;

public interface ITeacherService
{
    Task<BasePaginatedResult<TeacherGetDto>> GetPaginated(TeacherPaginatedRequest request);
    Task Create(TeacherPostDto dto);
    Task Update(TeacherPatchDto dto);
    Task Delete(int id);
}