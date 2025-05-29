using Domain.Entities;

namespace Domain.Interfaces;

public interface IHasGroupTo
{
    int ToSpecializationId { get; set; }
    int ToYear { get; set; }
    string ToGroupId { get; set; }

    Group ToGroup { get; set; }
}