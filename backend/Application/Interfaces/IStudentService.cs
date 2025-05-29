using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Student;

namespace Application.Interfaces;

public interface IStudentService
{
    Task<BasePaginatedResult<StudentGetDto>> GetPaginated(StudentPaginatedRequest request);
    Task Create(StudentPostDto dto);
    Task Update(StudentPatchDto dto);
    Task Delete(int id);
}