using System.Data;
using Domain.Enums;
using FluentValidation;

namespace Application.DTOs.Order;

public class OrderPostDtoValidator: AbstractValidator<OrderPostDto>
{
    public OrderPostDtoValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Номер приказа не может быть пустым");
        
        RuleFor(x => x.Date)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Дата приказа не может быть в будущем");

        RuleFor(x => x)
            .Custom((order, context) =>
            {
                switch (order.Type)
                {
                    case OrderTypes.Enrollment or
                        OrderTypes.TransferToOtherInstitution or 
                        OrderTypes.ReinstatementFromExpelled or 
                        OrderTypes.ReinstatementFromAcademy:
                        if (order.GroupTo == null) 
                            context.AddFailure($"Поле {nameof(order.GroupTo)} обязательно для приказов типа {order.Type}");
                        if (order.GroupFrom != null) 
                            context.AddFailure($"Поле {nameof(order.GroupFrom)} закрыто для приказов типа {order.Type}");
                        break;
                        
                    case OrderTypes.TransferFromOtherInstitution or
                        OrderTypes.AcademicLeave or 
                        OrderTypes.Expulsion or 
                        OrderTypes.Graduation:
                        if (order.GroupFrom == null) 
                            context.AddFailure($"Поле {nameof(order.GroupFrom)} обязательно для приказов типа {order.Type}");
                        if (order.GroupTo != null) 
                            context.AddFailure($"Поле {nameof(order.GroupTo)} закрыто для приказов типа {order.Type}");
                        break;
                    case OrderTypes.TransferBetweenGroups:
                        if (order.GroupFrom == null) 
                            context.AddFailure($"Поле {nameof(order.GroupFrom)} обязательно для приказов типа {order.Type}");
                        if (order.GroupTo == null) 
                            context.AddFailure($"Поле {nameof(order.GroupTo)} обязательно для приказов типа {order.Type}");
                        break;
                }
            });
    }
}