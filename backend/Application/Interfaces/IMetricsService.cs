using Application.DTOs.Metrics;

namespace Application.Interfaces;

public interface IMetricsService
{
    Task<CountDto> GetStudentCountAsync();
    Task<CountDto> GetOrderCountAsync();
    Task<CountDto> GetForeignCountAsync();
    Task<CountsDto> GetAllCountsAsync();
    
    
    Task<PieDto> GetGenderDistributionAsync();
    Task<PieDto> GetAgeDistributionAsync();
    Task<PieDto> GetCitizenshipDistributionAsync();
    Task<PiesDto> GetAllPieDistributionsAsync();
    
    
}