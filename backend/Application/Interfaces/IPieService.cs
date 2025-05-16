using Application.DTOs.Metrics;
using Application.Enums.MetricTypes;

namespace Application.Interfaces;

public interface IPieService
{
    Task<PieDto> GetPieAsync(PieTypes type, PieRequest request);
}