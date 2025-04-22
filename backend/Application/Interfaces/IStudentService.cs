using Application.DTOs.Department;
using Application.DTOs.Student;

namespace Application.Interfaces;

public interface IStudentService
{
    Task<BasePaginatedResult<StudentGetDto>> GetPaginated(StudentPaginatedRequest request);
}