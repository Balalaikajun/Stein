using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class TransferBetweenGroupsOrder:Order,IHasGroupFrom,IHasGroupTo
{
    public int FromSpecializationId { get; set; }
    public string FromYear { get; set; }
    public string FromGroupId { get; set; }
    public int ToSpecializationId { get; set; }
    public string ToYear { get; set; }
    public string ToGroupId { get; set; }

    public Group FromGroup { get; set; }
    public Group ToGroup { get; set; }

    public TransferBetweenGroupsOrder()
    {
        OrderType = OrderTypes.TransferBetweenGroups;
    }
}