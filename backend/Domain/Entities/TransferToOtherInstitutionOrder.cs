using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class TransferToOtherInstitutionOrder:Order,IHasGroupTo
{
    public string ToInstituteAcronym { get; set; }
    public int ToSpecializationId { get; set; }
    public string ToYear { get; set; }
    public string ToGroupId { get; set; }
    
    public Group ToGroup { get; set; }

    public TransferToOtherInstitutionOrder()
    {
        OrderType = OrderTypes.TransferToOtherInstitution;
    }
}