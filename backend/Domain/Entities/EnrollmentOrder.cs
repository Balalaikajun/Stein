using Domain.Interfaces;
using Domain.Enums;

namespace Domain.Entities;

public class EnrollmentOrder:Order,IHasGroupTo
{
    public int ToSpecializationId { get; set; }
    public int ToYear { get; set; }
    public string ToGroupId { get; set; }
    
    public Group ToGroup { get; set; }

    public EnrollmentOrder()
    {
        OrderType = OrderTypes.Enrollment;
    }
}