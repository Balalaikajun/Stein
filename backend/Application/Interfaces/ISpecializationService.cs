using Application.DTOs.Department;
using Application.DTOs.Specialization;

namespace Application.Interfaces;

public interface ISpecializationService
{
    Task<BasePaginatedResultDto<SpecializationGetDto>>GetPaginated(SpecializationPaginatedRequestDto request);
}