using Application.DTOs.Department;
using Application.DTOs.Group;
using Application.DTOs.Order;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<BasePaginatedResult<OrderGetDto>> GetPaginated(OrderPaginatedRequest dto);
    Task Create(OrderPostDto dto);
    
    Task Delete(int id);
}