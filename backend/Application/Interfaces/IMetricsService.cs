namespace Application.Interfaces;

public interface IMetricsService
{
    Task<int> GetStudentsCountAsync();
    Task<int> GetOrdersCountAsync();
    Task<int> GetForeignersCountAsync();
    
    
}