using System.Linq.Expressions;
using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Order;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrderService : IOrderService
{
    private static readonly Dictionary<string, Expression<Func<Order, object>>> _sortSelectors = new()
    {
        ["OrderType"] = o => o.OrderType.ToString(),
        ["OrderNumber"] = o => o.Number,
        ["Student"] = o => o.Student.Surname,
        ["Date"] = o => o.Date
    };

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public OrderService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasePaginatedResult<OrderGetDto>> GetPaginated(OrderPaginatedRequest dto)
    {
        var query = _context.Orders.AsQueryable();

        // Фильтрация
        if (dto.OrderTypesFilter != null && dto.OrderTypesFilter.Any())
            query = query.Where(d => dto.OrderTypesFilter.Contains(d.OrderType));

        if (!string.IsNullOrWhiteSpace(dto.SearchText))
            query = query.Where(d => EF.Functions.Like(d.Number, $"%{dto.SearchText}%"));

        if (dto.DateRangeFilter != null && dto.DateRangeFilter.FromDate.HasValue)
            query = query.Where(s => s.Date >= dto.DateRangeFilter.FromDate);

        if (dto.DateRangeFilter != null && dto.DateRangeFilter.ToDate.HasValue)
            query = query.Where(s => s.Date <= dto.DateRangeFilter.ToDate);

        // Сортировка
        var sortBy = _sortSelectors.ContainsKey(dto.SortBy) ? dto.SortBy : "Date";
        var sortSelector = _sortSelectors[sortBy];

        query = dto.Descending
            ? query.OrderByDescending(sortSelector).ThenByDescending(d => d.Id)
            : query.OrderBy(sortSelector).ThenBy(d => d.Id);

        int? total = null;
        if (dto.Skip == 0) total = await query.CountAsync();

        // Пагинация
        var items = await query
            .Skip(dto.Skip)
            .Take(dto.Take + 1)
            .ProjectTo<OrderGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var hasMore = items.Count > dto.Take;
        var resultItems = hasMore ? items.Take(dto.Take) : items;

        return new BasePaginatedResult<OrderGetDto>(
            resultItems,
            hasMore,
            total);
    }

    public async Task Create(OrderPostDto dto)
    {
        var student = await _context.Students.FindAsync(dto.StudentId);

        if (student == null)
            throw new NotFoundException("Student was not found");

        if (dto.GroupTo != null && !await _context.Groups
                .AnyAsync(g =>
                    g.SpecializationId == dto.GroupTo.SpecializationId &&
                    g.Year == dto.GroupTo.Year &&
                    g.Index == dto.GroupTo.Index))
            throw new NotFoundException("GroupTo was not found");

        if (dto.GroupFrom != null && !await _context.Groups
                .AnyAsync(g =>
                    g.SpecializationId == dto.GroupFrom.SpecializationId &&
                    g.Year == dto.GroupFrom.Year &&
                    g.Index == dto.GroupFrom.Index))
            throw new NotFoundException("GroupTo was not found");

        var order = _mapper.Map<Order>(dto);

        _context.Orders.Add(order);

        switch (order.OrderType)
        {
            case OrderTypes.Enrollment
                or OrderTypes.TransferFromOtherInstitution
                or OrderTypes.ReinstatementFromAcademy:
                student.Status = StudentStatuses.Active;
                student.GroupSpecializationId = order.ToSpecializationId;
                student.GroupYear = order.ToYear;
                student.GroupId = order.ToGroupId;
                break;

            case OrderTypes.TransferBetweenGroups:
                student.GroupSpecializationId = order.ToSpecializationId;
                student.GroupYear = order.ToYear;
                student.GroupId = order.ToGroupId;
                break;

            case OrderTypes.AcademicLeave:
                student.Status = StudentStatuses.Vacation;
                student.GroupSpecializationId = null;
                student.GroupYear = null;
                student.GroupId = null;
                break;

            case OrderTypes.Graduation:
                student.Status = StudentStatuses.Released;
                student.GroupSpecializationId = null;
                student.GroupYear = null;
                student.GroupId = null;
                break;

            case OrderTypes.Expulsion:
                student.Status = StudentStatuses.Expelled;
                student.GroupSpecializationId = null;
                student.GroupYear = null;
                student.GroupId = null;
                break;

            case OrderTypes.TransferToOtherInstitution:
                student.Status = StudentStatuses.TransferToOtherInstitution;
                student.GroupSpecializationId = null;
                student.GroupYear = null;
                student.GroupId = null;
                break;
        }

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
            throw new NotFoundException("Order was not found");

        var student = await _context.Students.FindAsync(order.StudentId);

        if (student == null)
            throw new NotFoundException("Student was not found");

        var realyLastOrder = await _context.Orders.Where(o => o.StudentId == order.StudentId)
            .OrderByDescending(o => o.Date).FirstAsync();
        if (order.Id != realyLastOrder.Id)
            throw new InvalidOperationException("Order was not last");

        _context.Orders.Remove(order);

        var lastOrder = await _context.Orders.Where(o => o.StudentId == order.StudentId).OrderByDescending(o => o.Date)
            .Skip(1).FirstOrDefaultAsync();

        UpdateStudentFromOrder(student, lastOrder);

        await _context.SaveChangesAsync();
    }

    private static void UpdateStudentFromOrder(Student student, Order? order)
    {
        if (order == null)
        {
            student.Status = StudentStatuses.NotActive;
            student.GroupSpecializationId = null;
            student.GroupYear = null;
            student.GroupId = null;
            return;
        }

        switch (order.OrderType)
        {
            case OrderTypes.Enrollment
                or OrderTypes.TransferFromOtherInstitution
                or OrderTypes.ReinstatementFromAcademy
                or OrderTypes.ReinstatementFromExpelled:
                student.Status = StudentStatuses.Active;
                student.GroupSpecializationId = order.ToSpecializationId;
                student.GroupYear = order.ToYear;
                student.GroupId = order.ToGroupId;
                break;

            case OrderTypes.TransferBetweenGroups:
                student.GroupSpecializationId = order.ToSpecializationId;
                student.GroupYear = order.ToYear;
                student.GroupId = order.ToGroupId;
                break;

            case OrderTypes.AcademicLeave:
                student.Status = StudentStatuses.Vacation;
                student.GroupSpecializationId = null;
                student.GroupYear = null;
                student.GroupId = null;
                break;

            case OrderTypes.Graduation:
                student.Status = StudentStatuses.Released;
                student.GroupSpecializationId = null;
                student.GroupYear = null;
                student.GroupId = null;
                break;

            case OrderTypes.Expulsion:
                student.Status = StudentStatuses.Expelled;
                student.GroupSpecializationId = null;
                student.GroupYear = null;
                student.GroupId = null;
                break;
        }
    }
}