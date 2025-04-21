using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class ExpulsionOrder:Order,IHasGroupFrom
{
    public int FromSpecializationId { get; set; }
    public int FromYear { get; set; }
    public string FromGroupId { get; set; }
    
    public Group FromGroup { get; set; }

    public ExpulsionOrder()
    {
        OrderType = OrderTypes.Expulsion;
    }
}