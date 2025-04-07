using Domain.Entities;

namespace Domain.Interfaces;

public interface IHasGroupFrom
{
    int FromSpecializationId { get; set; }
    string FromYear { get; set; }
    string FromGroupId { get; set; }
    
    Group FromGroup { get; set; }
}