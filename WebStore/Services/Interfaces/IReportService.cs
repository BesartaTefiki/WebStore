using WebStore.Models;

namespace WebStore.Services.Interfaces
{
    public interface IReportService
    {

        Task<Report> GenerateReportAsync(DateTime from, DateTime to);
        Task<Report> GetDailyReportAsync(DateTime date);
        Task<Report> GetMonthlyReportAsync(int year, int month);
        Task<IEnumerable<TopProductDto>> GetTopProductsAsync(DateTime from, DateTime to, int topN);
    }
}
