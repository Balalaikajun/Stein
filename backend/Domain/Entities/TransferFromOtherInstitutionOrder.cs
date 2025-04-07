using Domain.Enums;
using Domain.Interfaces;

namespace Domain.Entities;

public class TransferFromOtherInstitutionOrder:Order,IHasGroupFrom
{
    public string FromInstituteAcronym { get; set; }
    public int FromSpecializationId { get; set; }
    public string FromYear { get; set; }
    public string FromGroupId { get; set; }
    
    public Group FromGroup { get; set; }

    public TransferFromOtherInstitutionOrder()
    {
        OrderType = OrderTypes.TransferFromOtherInstitution;
    }
}