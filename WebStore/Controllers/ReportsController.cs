using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,advanced")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: api/reports?from=2025-11-01&to=2025-11-23
        [HttpGet]
        public async Task<ActionResult<Report>> GetRangeReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var report = await _reportService.GenerateReportAsync(from, to);
            return Ok(report);
        }

        // GET: api/reports/daily?date=2025-11-23
        [HttpGet("daily")]
        public async Task<ActionResult<Report>> GetDaily([FromQuery] DateTime date)
        {
            var report = await _reportService.GetDailyReportAsync(date);
            return Ok(report);
        }

        // GET: api/reports/monthly?year=2025&month=11
        [HttpGet("monthly")]
        public async Task<ActionResult<Report>> GetMonthly([FromQuery] int year, [FromQuery] int month)
        {
            var report = await _reportService.GetMonthlyReportAsync(year, month);
            return Ok(report);
        }

        // GET: api/reports/top-products?from=2025-11-01&to=2025-11-23&topN=5
        [HttpGet("top-products")]
        public async Task<ActionResult<IEnumerable<TopProductDto>>> GetTopProducts(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int topN = 5)
        {
            var result = await _reportService.GetTopProductsAsync(from, to, topN);
            return Ok(result);
        }
    }
}
