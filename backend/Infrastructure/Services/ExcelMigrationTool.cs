using System.Text.Json;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Infrastructure.Services;

public class ExcelMigrationService : IExcelMigrationService
{
    private IApplicationDbContext _context;

    public ExcelMigrationService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task MigrateAsync(IFormFile file)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        using var package = new ExcelPackage(stream);

        await DepartmentsUpsert(package);
        await SpecializationsUpsert(package);
        await TeachersUpsert(package);
        await GroupsUpsert(package);
        await StudentsUpsert(package);
        await OrdersUpsert(package);
        await PerformanceUpsert(package);

        await _context.SaveChangesAsync();
        
        await UpdateStudentStatuses();
        
        await _context.SaveChangesAsync();
    }

    private async Task DepartmentsUpsert(ExcelPackage package)
    {
        var departmentsWorksheet = package.Workbook
            .Worksheets.FirstOrDefault(w => w.Name == "Departments");

        if (departmentsWorksheet == null)
            return;

        for (var row = 2; row <= departmentsWorksheet.Dimension.Rows; row++)
        {
            var department = new Department
            {
                Id = int.Parse(departmentsWorksheet.Cells[row, 1].Text),
                Title = departmentsWorksheet.Cells[row, 2].Text,
                IsActive = bool.Parse(departmentsWorksheet.Cells[row, 3].Text),
            };

            var existing = await _context.Departments.FindAsync(department.Id);

            if (existing == null)
            {
                _context.Departments.Add(department);
            }
            else
            {
                _context.Departments.Entry(existing).CurrentValues.SetValues(department);
            }
        }
    }

    private async Task SpecializationsUpsert(ExcelPackage package)
    {
        var specializationsWorksheet = package.Workbook
            .Worksheets.FirstOrDefault(w => w.Name == "Specializations");

        if (specializationsWorksheet == null)
            return;

        for (var row = 2; row <= specializationsWorksheet.Dimension.Rows; row++)
        {
            var specialization = new Specialization
            {
                Id = int.Parse(specializationsWorksheet.Cells[row, 1].Text),
                Code = specializationsWorksheet.Cells[row, 2].Text,
                Title = specializationsWorksheet.Cells[row, 3].Text,
                Acronym = specializationsWorksheet.Cells[row, 4].Text,
                DepartmentId = int.Parse(specializationsWorksheet.Cells[row, 5].Text),
                IsActive = bool.Parse(specializationsWorksheet.Cells[row, 6].Text),
            };

            var existing = await _context.Specializations.FindAsync(specialization.Id);

            if (existing == null)
            {
                _context.Specializations.Add(specialization);
            }
            else
            {
                _context.Specializations.Entry(existing).CurrentValues.SetValues(specialization);
            }
        }
    }

    private async Task TeachersUpsert(ExcelPackage package)
    {
        var teachersWorksheet = package.Workbook
            .Worksheets.FirstOrDefault(w => w.Name == "Teachers");

        if (teachersWorksheet == null)
            return;


        for (var row = 2; row <= teachersWorksheet.Dimension.Rows; row++)
        {
            var teacher = new Teacher()
            {
                Id = int.Parse(teachersWorksheet.Cells[row, 1].Text),
                Surname = teachersWorksheet.Cells[row, 2].Text,
                Name = teachersWorksheet.Cells[row, 3].Text,
                Patronymic = teachersWorksheet.Cells[row, 4].Text,
                IsActive = bool.Parse(teachersWorksheet.Cells[row, 5].Text),
            };

            var existing = await _context.Teachers.FindAsync(teacher.Id);

            if (existing == null)
            {
                _context.Teachers.Add(teacher);
            }
            else
            {
                _context.Teachers.Entry(existing).CurrentValues.SetValues(teacher);
            }
        }
    }

    private async Task GroupsUpsert(ExcelPackage package)
    {
        var groupsWorksheet = package.Workbook
            .Worksheets.FirstOrDefault(w => w.Name == "Groups");

        if (groupsWorksheet == null)
            return;

        for (var row = 2; row <= groupsWorksheet.Dimension.Rows; row++)
        {
            DateOnly? releaseDate = groupsWorksheet.Cells[row, 7].Text == ""
                ? null
                : DateOnly.FromDateTime(DateTime.FromOADate((double)groupsWorksheet.Cells[row, 7].Value));
            
            
            var group = new Group()
            {
                SpecializationId = int.Parse(groupsWorksheet.Cells[row, 1].Text),
                Year = int.Parse(groupsWorksheet.Cells[row, 2].Text),
                Index = groupsWorksheet.Cells[row, 3].Text,
                Acronym = groupsWorksheet.Cells[row, 4].Text,
                TeacherId = int.Parse(groupsWorksheet.Cells[row, 5].Text),
                IsActive = bool.Parse(groupsWorksheet.Cells[row, 6].Text),
                ReleaseDate = releaseDate
            };

            var existing = await _context.Groups.FindAsync(group.SpecializationId, group.Year, group.Index);

            if (existing == null)
            {
                _context.Groups.Add(group);
            }
            else
            {
                _context.Groups.Entry(existing).CurrentValues.SetValues(group);
            }
        }
    }

    private async Task StudentsUpsert(ExcelPackage package)
    {
        var studentsWorksheet = package.Workbook
            .Worksheets.FirstOrDefault(w => w.Name == "Students");

        if (studentsWorksheet == null)
            return;

        for (var row = 2; row <= studentsWorksheet.Dimension.Rows; row++)
        {
            var student = new Student()
            {
                Id = int.Parse(studentsWorksheet.Cells[row, 1].Text),
                Surname = studentsWorksheet.Cells[row, 2].Text,
                Name = studentsWorksheet.Cells[row, 3].Text,
                Patronymic = studentsWorksheet.Cells[row, 4].Text,
                IsCitizen = studentsWorksheet.Cells[row, 8].Text == "РФ",
                Gender = studentsWorksheet.Cells[row, 9].Text switch
                {
                    "Мужской" => Gender.Male,
                    "Женский" => Gender.Female,
                },
                DateOfBirth =
                    DateOnly.FromDateTime(DateTime.FromOADate((double)studentsWorksheet.Cells[row, 10].Value))
            };

            var existing = await _context.Students.FindAsync(student.Id);

            if (existing == null)
            {
                _context.Students.Add(student);
            }
            else
            {
                _context.Students.Entry(existing).CurrentValues.SetValues(student);
            }
        }
    }

    private async Task OrdersUpsert(ExcelPackage package)
    {
        var ordersWorksheet = package.Workbook
            .Worksheets.FirstOrDefault(w => w.Name == "Orders");

        if (ordersWorksheet == null)
            return;

        for (var row = 2; row <= ordersWorksheet.Dimension.Rows; row++)
        {
            var order = new Order
            {
                Id = int.Parse(ordersWorksheet.Cells[row, 1].Text),
                OrderType = ordersWorksheet.Cells[row, 2].Text switch
                {
                    "Зачисление" => OrderTypes.Enrollment,
                    "Перевод из других образовательных организаций с программой того же уровня" => OrderTypes
                        .TransferFromOtherInstitution,
                    "Перевод в другие образовательные организации на программы того же уровня" => OrderTypes
                        .TransferToOtherInstitution,
                    "Востановленные из числа ранее отчисленных" => OrderTypes.ReinstatementFromExpelled,
                    "По другим причинам (в т.ч. Перевод из группы в группу внутри колледжа)" => OrderTypes
                        .ReinstatementFromAcademy,
                    "В связи с академическим отпуском (уход в ВС РФ, по болезни, семейным)" => OrderTypes
                        .AcademicLeave,
                    "Перевод внутри колледжа с программ того же уровня" => OrderTypes.TransferBetweenGroups,
                    "Отчислены" => OrderTypes.Expulsion,
                    "Выпущен" => OrderTypes.Graduation
                },
                Number = ordersWorksheet.Cells[row, 3].Text,
                StudentId = int.Parse(ordersWorksheet.Cells[row, 4].Text),
                Date = DateOnly.FromDateTime(DateTime.FromOADate((double)ordersWorksheet.Cells[row, 5].Value)),
                ToSpecializationId = ParseNullableInt(ordersWorksheet.Cells[row, 6].Text),
                ToYear = ParseNullableInt(ordersWorksheet.Cells[row, 7].Text),
                ToGroupId = ParseNullableString(ordersWorksheet.Cells[row, 8].Text),
                FromSpecializationId = ParseNullableInt(ordersWorksheet.Cells[row, 9].Text),
                FromYear = ParseNullableInt(ordersWorksheet.Cells[row, 10].Text),
                FromGroupId = ParseNullableString(ordersWorksheet.Cells[row, 11].Text)
            };

            var existing = await _context.Orders.FindAsync(order.Id);

            if (existing == null)
            {
                _context.Orders.Add(order);
            }
            else
            {
                _context.Orders.Entry(existing).CurrentValues.SetValues(order);
            }
        }
    }

    private async Task PerformanceUpsert(ExcelPackage package)
    {
        var performanceWorksheet = package.Workbook
            .Worksheets.FirstOrDefault(w => w.Name == "AcademicPerformances");

        if (performanceWorksheet == null)
            return;


        for (var row = 2; row <= performanceWorksheet.Dimension.Rows; row++)
        {
            var performance = new AcademicPerformance()
            {
                Year = int.Parse(performanceWorksheet.Cells[row, 1].Text),
                Month = int.Parse(performanceWorksheet.Cells[row, 2].Text),
                StudentId = int.Parse(performanceWorksheet.Cells[row, 3].Text),
                Performance = (PerformanceCategory)Enum.Parse(typeof(PerformanceCategory),
                    performanceWorksheet.Cells[row, 4].Text)
            };

            var existing = await _context.AcademicPerformances.FindAsync(performance.Year, performance.Month, performance.StudentId);

            if (existing == null)
            {
                _context.AcademicPerformances.Add(performance);
            }
            else
            {
                _context.AcademicPerformances.Entry(existing).CurrentValues.SetValues(performance);
            }
        }
    }

    private async Task UpdateStudentStatuses()
    {
        var lastOrders = await _context.Orders
            .GroupBy(o => o.StudentId)
            .Select(g => g
                .OrderByDescending(o => o.Date)
                .First())
            .ToListAsync();
        
        var studentsIds = lastOrders.Select(o => o.StudentId);

        var studentIds = lastOrders.Select(o => o.StudentId).ToList();
        var students = await _context.Students
            .Where(s => studentIds.Contains(s.Id))
            .ToListAsync();

        foreach (var student in students)
        {
            var lastOrder = lastOrders.FirstOrDefault(o => o.StudentId == student.Id);
            
            if (lastOrder == null) continue;
            
            UpdateStudentFromOrder(student, lastOrder);

            _context.Students.Entry(student).State = EntityState.Modified;
        }
    }

    private static void UpdateStudentFromOrder(Student student, Order order)
    {
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
    
    private int? ParseNullableInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "0")
            return null;

        return int.TryParse(value, out var result) ? result : null;
    }

    private string? ParseNullableString(string value)
    {
        return string.IsNullOrWhiteSpace(value) || value == "0" ? null : value;
    }
}