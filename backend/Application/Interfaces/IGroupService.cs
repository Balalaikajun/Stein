using Application.DTOs.Department;
using Application.DTOs.Group;

namespace Application.Interfaces;

public interface IGroupService
{
    Task<BasePaginatedResult<GroupGetDto>> GetPaginated(GroupPaginatedRequest request);
    Task Create(GroupPostDto dto);
    Task Update(GroupPatchDto dto);
    Task Delete(GroupKeyDto id);
}