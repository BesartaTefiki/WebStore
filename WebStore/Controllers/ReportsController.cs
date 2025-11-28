using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Controllers
{
    /// <summary>
    /// Provides earnings and top product reports.
    /// </summary>
    /// <remarks>
    /// Base route: /api/reports  
    /// Only admin and advanced users can access reports.
    /// </remarks>
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

        /// <summary>
        /// Returns earnings report for a custom date range.
        /// </summary>
        /// <remarks>
        /// GET /api/reports?from=YYYY-MM-DD&amp;to=YYYY-MM-DD
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<Report>> GetRangeReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var report = await _reportService.GenerateReportAsync(from, to);
            return Ok(report);
        }

        /// <summary>
        /// Returns daily earnings report.
        /// </summary>
        /// <remarks>
        /// GET /api/reports/daily?date=YYYY-MM-DD
        /// </remarks>
        [HttpGet("daily")]
        public async Task<ActionResult<Report>> GetDaily([FromQuery] DateTime date)
        {
            var report = await _reportService.GetDailyReportAsync(date);
            return Ok(report);
        }

        /// <summary>
        /// Returns monthly earnings report.
        /// </summary>
        /// <remarks>
        /// GET /api/reports/monthly?year=YYYY&amp;month=MM
        /// </remarks>
        [HttpGet("monthly")]
        public async Task<ActionResult<Report>> GetMonthly([FromQuery] int year, [FromQuery] int month)
        {
            var report = await _reportService.GetMonthlyReportAsync(year, month);
            return Ok(report);
        }

        /// <summary>
        /// Returns top selling products for a given period.
        /// </summary>
        /// <remarks>
        /// GET /api/reports/top-products?from=YYYY-MM-DD&amp;to=YYYY-MM-DD&amp;topN=5
        /// </remarks>
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
