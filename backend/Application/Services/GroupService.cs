using System.Linq.Expressions;
using Application.DTOs.Base;
using Application.DTOs.Department;
using Application.DTOs.Group;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class GroupService : IGroupService
{
    private static readonly Dictionary<string, Expression<Func<Group, object>>> _sortSelectors = new()
    {
        ["Acronym"] = g => g.Acronym,
        ["IsActive"] = g => g.IsActive,
        ["Year"] = g => g.Year,
        ["Specialization"] = g => g.Specialization.Title,
        ["Department"] = g => g.Specialization.Department.Title,
        ["Teacher"] = g => g.Teacher.Surname,
        ["Students"] = g => g.Students.Count
    };

    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GroupService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasePaginatedResult<GroupGetDto>> GetPaginated(GroupPaginatedRequest request)
    {
        var query = _context.Groups.AsQueryable();

        // Фильтрация
        if (request.ActiveFilter.HasValue)
            query = query.Where(d => d.IsActive == request.ActiveFilter.Value);

        if (!string.IsNullOrWhiteSpace(request.SearchText))
            query = query.Where(d => EF.Functions.Like(d.Acronym, $"%{request.SearchText}%"));

        query = query
            .Join(_context.Specializations,
                g => g.SpecializationId,
                s => s.Id,
                (g, s) => new { Group = g, Specialization = s })
            .Where(x => (request.DepartmentIds == null || !request.DepartmentIds.Any() ||
                         request.DepartmentIds.Contains(x.Specialization.DepartmentId)) &&
                        (request.SpecializationIds == null || !request.SpecializationIds.Any() ||
                         request.SpecializationIds.Contains(x.Specialization.Id)))
            .Select(x => x.Group);

        if (request.Years != null && request.Years.Any()) query = query.Where(g => request.Years.Contains(g.Year));

        // Сортировка
        var sortBy = _sortSelectors.ContainsKey(request.SortBy) ? request.SortBy : "Acronym";
        var sortSelector = _sortSelectors[sortBy];

        query = request.Descending
            ? query.OrderByDescending(sortSelector).ThenByDescending(d => d.Index)
            : query.OrderBy(sortSelector).ThenBy(d => d.Index);

        int? total = null;
        if (request.Skip == 0) total = await query.CountAsync();

        // Пагинацияs
        var items = await query
            .Skip(request.Skip)
            .Take(request.Take + 1) // берём на 1 больше, чтобы узнать, есть ли ещё
            .ProjectTo<GroupGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();


        var hasMore = items.Count > request.Take;
        var resultItems = hasMore ? items.Take(request.Take) : items;

        return new BasePaginatedResult<GroupGetDto>(
            resultItems,
            hasMore,
            total);
    }

    public async Task Create(GroupPostDto dto)
    {
        if (!await _context.Specializations.AnyAsync(x => x.Id == dto.SpecializationId))
            throw new NotFoundException($"Specialization with id - {dto.SpecializationId} was not found");

        var group = _mapper.Map<Group>(dto);

        _context.Groups.Add(group);

        await _context.SaveChangesAsync();
    }


    public async Task Update(GroupPatchDto dto)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(x =>
                        x.SpecializationId == dto.Id.SpecializationId &&
                        x.Year == dto.Id.Year &&
                        x.Index == dto.Id.Index) ??
                    throw new NotFoundException($"Department with id - {dto.Id} was not found");

        if (dto.NewSpecializationId.HasValue && dto.NewSpecializationId != group.SpecializationId)
        {
            if (!await _context.Specializations.AnyAsync(x => x.Id == dto.NewSpecializationId))
                throw new NotFoundException($"Specialization with id - {dto.NewSpecializationId} was not found");

            group.SpecializationId = dto.NewSpecializationId.Value;
        }

        if (dto.NewYear.HasValue && dto.NewYear != group.Year)
            group.Year = dto.NewYear.Value;

        if (!string.IsNullOrWhiteSpace(dto.NewIndex) && dto.NewIndex != group.Index)
            group.Index = dto.NewIndex;

        if (!string.IsNullOrWhiteSpace(dto.NewAcronym) && dto.NewAcronym != group.Acronym)
            group.Acronym = dto.NewAcronym;

        if (dto.NewIsActive.HasValue && dto.NewIsActive != group.IsActive)
            group.IsActive = dto.NewIsActive.Value;

        if (dto.NewTeacherId.HasValue && dto.NewTeacherId != group.TeacherId)
        {
            if (!await _context.Teachers.AnyAsync(x => x.Id == dto.NewTeacherId))
                throw new NotFoundException($"Teacher with id - {dto.NewTeacherId} was not found");

            group.TeacherId = dto.NewTeacherId.Value;
        }

        await _context.SaveChangesAsync();
    }

    public async Task Delete(GroupKeyDto id)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(x =>
            x.SpecializationId == id.SpecializationId &&
            x.Year == id.Year &&
            x.Index == id.Index);

        if (group == null)
            throw new NotFoundException($"Group with id - {id} was not found");

        _context.Groups.Remove(group);

        await _context.SaveChangesAsync();
    }
}