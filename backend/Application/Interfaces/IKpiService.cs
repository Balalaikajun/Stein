using Application.DTOs.Metrics;
using Application.Enums.MetricTypes;

namespace Application.Interfaces;

public interface IKpiService
{
    Task<CountDto> GetKpiAsync(KpiTypes type, KpiRequest request);
}