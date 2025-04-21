using Application.DTOs.Department;
using Application.DTOs.Group;

namespace Application.Interfaces;

public interface IGroupService
{
    Task<BasePaginatedResult<GroupGetDto>> GetPaginated(GroupPaginatedRequest request);
}