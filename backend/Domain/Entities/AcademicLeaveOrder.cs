using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class AcademicLeaveOrder:Order,IHasGroupFrom
{
    public int FromSpecializationId { get; set; }
    public string FromYear { get; set; }
    public string FromGroupId { get; set; }
    
    public Group FromGroup { get; set; }

    public AcademicLeaveOrder()
    {
        OrderType = OrderTypes.AcademicLeave;
    }
}