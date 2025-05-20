using Application.DTOs.Metrics;

namespace Application.Interfaces;

public interface IUniqueChartsService
{
    Task<PerformanceHistogramDto> GetPerformanceHistogramAsync(PerformanceRequest request);
    
    Task<OrderHistogramDto> GetOrderHistogramAsync(OrderHistogramRequest request);
    Task<ContingentHistogramDto> GetContingentHistogramAsync(ContingentHistogramRequest request);
}