using Application.DTOs.Department;
using Application.DTOs.Specialization;

namespace Application.Interfaces;

public interface ISpecializationService
{
    Task<BasePaginatedResult<SpecializationGetDto>>GetPaginated(SpecializationPaginatedRequest request);
}