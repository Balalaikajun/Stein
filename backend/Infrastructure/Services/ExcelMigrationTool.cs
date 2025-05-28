using System.Globalization;
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


            var specializationId = int.Parse(groupsWorksheet.Cells[row, 1].Text);
            var year = int.Parse(groupsWorksheet.Cells[row, 2].Text);
            var index = groupsWorksheet.Cells[row, 3].Text;
            var acronym = groupsWorksheet.Cells[row, 4].Text;
            var teacherId = int.Parse(groupsWorksheet.Cells[row, 5].Text);
            var isActive = bool.Parse(groupsWorksheet.Cells[row, 6].Text);

            var group = new Group()
            {
                SpecializationId = specializationId,
                Year = year,
                Index = index,
                Acronym = acronym,
                TeacherId = teacherId,
                IsActive = isActive,
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
            var id = int.Parse(studentsWorksheet.Cells[row, 1].Text);
            var surname = studentsWorksheet.Cells[row, 2].Text;
            var name = studentsWorksheet.Cells[row, 3].Text;
            var patronymic = studentsWorksheet.Cells[row, 4].Text;
            var isCitizen = studentsWorksheet.Cells[row, 8].Text == "РФ";
            var gender = studentsWorksheet.Cells[row, 9].Text switch
            {
                "Мужской" => Gender.Male,
                "Женский" => Gender.Female,
            };
            var dateOfBirth = ParseNullableDate(studentsWorksheet.Cells[row, 10].Value,
                studentsWorksheet.Cells[row, 10].Text);


            var student = new Student()
            {
                Id = id,
                Surname = surname,
                Name = name,
                Patronymic = patronymic,
                IsCitizen = isCitizen,
                Gender = gender,
                DateOfBirth = dateOfBirth
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

        // 1) Получаем из БД “текущий статус” студентов (если они уже есть в БД, 
        //    иначе можно заполнять словарь null-значениями).
        var allStudentIdsInDb = await _context.Students
            .Select(s => s.Id)
            .ToListAsync();

        // Словарь: StudentId → (SpecializationId, Year, GroupId)
        var currentStatus =
            new Dictionary<int, (int? SpecId, int? Year, string? GroupId)>(capacity: allStudentIdsInDb.Count);
        // Заполняем из БД текущее состояние (можно выбирать только нужные поля через проекцию).
        var studentsFromDb = await _context.Students
            .Select(s => new
            {
                s.Id,
                s.GroupSpecializationId,
                s.GroupYear,
                s.GroupId
            })
            .ToListAsync();

        foreach (var s in studentsFromDb)
        {
            currentStatus[s.Id] = (s.GroupSpecializationId, s.GroupYear, s.GroupId);
        }

        // 2) Читаем заказы из Excel в промежуточный список
        var parsedOrders = new List<Order>(capacity: ordersWorksheet.Dimension.Rows - 1);

        for (var row = 2; row <= ordersWorksheet.Dimension.Rows; row++)
        {
            // Парсим поля заказа (предполагаем, что используем ваш метод ParseNullableDate)
            var order = new Order
            {
                Id = int.Parse(ordersWorksheet.Cells[row, 1].Text),
                OrderType = ordersWorksheet.Cells[row, 2].Text switch
                {
                    "Зачисление" => OrderTypes.Enrollment,
                    "Перевод из других образовательных организаций с программой того же уровня"
                        => OrderTypes.TransferFromOtherInstitution,
                    "Перевод в другие образовательные организации на программы того же уровня"
                        => OrderTypes.TransferToOtherInstitution,
                    "Востановленные из числа ранее отчисленных"
                        => OrderTypes.ReinstatementFromExpelled,
                    "По другим причинам (в т.ч. Перевод из группы в группу внутри колледжа)"
                        => OrderTypes.ReinstatementFromAcademy,
                    "В связи с академическим отпуском (уход в ВС РФ, по болезни, семейным)"
                        => OrderTypes.AcademicLeave,
                    "Перевод внутри колледжа с программ того же уровня"
                        => OrderTypes.TransferBetweenGroups,
                    "Отчислены" => OrderTypes.Expulsion,
                    "Выпущен" => OrderTypes.Graduation,
                    _ => throw new InvalidOperationException(
                        $"Неизвестный тип приказа: {ordersWorksheet.Cells[row, 2].Text}")
                },
                Number = ordersWorksheet.Cells[row, 3].Text,
                StudentId = int.Parse(ordersWorksheet.Cells[row, 4].Text),
                Date = ParseNullableDate(ordersWorksheet.Cells[row, 5].Value, ordersWorksheet.Cells[row, 5].Text),
                ToSpecializationId = ParseNullableInt(ordersWorksheet.Cells[row, 6].Text),
                ToYear = ParseNullableInt(ordersWorksheet.Cells[row, 7].Text),
                ToGroupId = ParseNullableString(ordersWorksheet.Cells[row, 8].Text),
                FromSpecializationId = ParseNullableInt(ordersWorksheet.Cells[row, 9].Text),
                FromYear = ParseNullableInt(ordersWorksheet.Cells[row, 10].Text),
                FromGroupId = ParseNullableString(ordersWorksheet.Cells[row, 11].Text)
            };

            parsedOrders.Add(order);
        }

        // 3) Сортируем по дате, чтобы обработка шла хронологически:
        parsedOrders.Sort((o1, o2) => o1.Date.CompareTo(o2.Date));

        // 4) Перебираем и валидируем/вставляем
        foreach (var order in parsedOrders)
        {
            // Если нет в словаре (студента ещё нет в БД), можно считать, что студента раньше не было:
            if (!currentStatus.TryGetValue(order.StudentId, out var status))
            {
                // Можно либо считать, что до первого приказа студент не был ни в какой группе (null,null,null),
                // либо вставить дефолтное значение:
                status = (null, null, null);
                currentStatus[order.StudentId] = status;
            }


            // Если текущая группа (status.GroupId) не совпадает с заявленной FromGroupId — это ошибка
            if (status.SpecId != order.FromSpecializationId
                || status.Year != order.FromYear
                || status.GroupId != order.FromGroupId)
            {
                // Варианты обработки:
                //  - Пропустить этот приказ: continue
                //  - Логировать предупреждение: Console.WriteLine(...)
                //  - Бросить исключение, чтобы сразу остановиться и править файл
                //  - Принять “как есть” (если решения поочередные загадочны)
                Console.WriteLine($"[Warning] Студент {order.StudentId}: приказ {order.Id} " +
                                  $"указывает FromGroup ({order.FromSpecializationId},{order.FromYear},{order.FromGroupId}), " +
                                  $"но текущий статус ({status.SpecId},{status.Year},{status.GroupId}).");

                // Если хотим пропускать:
                // continue;
                // Если всё-таки вставляем, просто идём дальше без continue.
            }


            // 5) Вставляем или обновляем сам приказ:
            var existingOrder = await _context.Orders.FindAsync(order.Id);
            if (existingOrder == null)
                _context.Orders.Add(order);
            else
                _context.Orders.Entry(existingOrder).CurrentValues.SetValues(order);

            // 6) Обновляем словарь на “последнюю” группу (приказы типа Enrollment/Transfer/Restor..., 
            //    Graduation/Expulsion меняют группу у студента).
            currentStatus[order.StudentId] = order.OrderType switch
            {
                OrderTypes.Enrollment 
                    or OrderTypes.TransferFromOtherInstitution 
                    or OrderTypes.ReinstatementFromAcademy
                    or OrderTypes.ReinstatementFromExpelled 
                    or OrderTypes.TransferBetweenGroups =>
                    // Переходим в новую группу ToGroupId
                    (order.ToSpecializationId, order.ToYear, order.ToGroupId),
                OrderTypes.AcademicLeave
                    or OrderTypes.Graduation
                    or OrderTypes.Expulsion =>
                    // После академического отпуска / выпуска / отчисления студент вообще “нигде не числится”
                    (null, null, null),
                _ => currentStatus[order.StudentId]
            };
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

            var existing =
                await _context.AcademicPerformances.FindAsync(performance.Year, performance.Month,
                    performance.StudentId);

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

    private DateOnly ParseNullableDate(object? cellValue, string? cellText)
    {
        // Если текст ячейки пустой или состоящий только из пробелов — сразу возвращаем null
        if (string.IsNullOrWhiteSpace(cellText))
            throw new FormatException("Invalid date");

        // 1. Если в ячейке лежит double (OLE-число) — пробуем конвертировать
        if (cellValue is double oaDate)
        {
            try
            {
                return DateOnly.FromDateTime(DateTime.FromOADate(oaDate));
            }
            catch
            {
                // Если по какой-то причине не удалось преобразовать OADate — возвращаем null
                throw new FormatException("Invalid date");
            }
        }

        // 2. Если в ячейке строка — пробуем распарсить в нескольких форматах
        //    Список форматов, которые хотим поддерживать:
        var formats = new[]
        {
            "dd.MM.yyyy",
            "d.M.yyyy",
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "MM/dd/yyyy",
            "M/d/yyyy",
            "dd-MM-yyyy",
            "d-M-yyyy",
            "dd/MM/yyyy",
            "d/M/yyyy"
            // При необходимости можно добавить другие паттерны
        };

        // Попытаемся выполнить TryParseExact по указанным форматам, с учётом текущей культуры или invariant:
        if (DateTime.TryParseExact(cellText,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsedDateTime))
        {
            return DateOnly.FromDateTime(parsedDateTime);
        }

        // 3. Если ни один из строгих форматов не подошёл, попробуем гибкий TryParse
        if (DateTime.TryParse(cellText,
                CultureInfo.CurrentCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out parsedDateTime))
        {
            return DateOnly.FromDateTime(parsedDateTime);
        }

        // 4. Если ничего не сработало — возвращаем null
        throw new FormatException("Invalid date");
    }
}