using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Specialization;

namespace Application.Interfaces;

public interface ISpecializationService
{
    Task<BasePaginatedResult<SpecializationGetDto>> GetPaginated(SpecializationPaginatedRequest request);
    Task Create(SpecializationPostDto dto);
    Task Update(SpecializationPatchDto dto);
    Task Delete(int id);
}