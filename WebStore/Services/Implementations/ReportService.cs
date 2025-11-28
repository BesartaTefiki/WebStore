using Microsoft.EntityFrameworkCore;
using WebStore.Data;
using WebStore.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Services
{
    public class ReportService : IReportService
    {
        private readonly WebStoreContext _context;

        public ReportService(WebStoreContext context)
        {
            _context = context;
        }

        public async Task<Report> GenerateReportAsync(DateTime from, DateTime to)
        {
            var orders = await _context.Orders
                .Where(o => o.Status == "Confirmed"
                            && o.CreatedAt >= from
                            && o.CreatedAt <= to)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();

            decimal totalEarnings = 0m;
            var productSales = new Dictionary<int, (int quantity, decimal revenue)>();

            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    var product = item.Product;
                    if (product == null) continue;

                    var price = product.Price;
                    var discountPercent = product.DiscountPercent ?? 0m;
                    var finalPrice = price * (1 - discountPercent / 100m);

                    var lineRevenue = finalPrice * item.Quantity;
                    totalEarnings += lineRevenue;

                    if (!productSales.ContainsKey(product.Id))
                    {
                        productSales[product.Id] = (0, 0m);
                    }

                    var current = productSales[product.Id];
                    productSales[product.Id] = (
                        current.quantity + item.Quantity,
                        current.revenue + lineRevenue
                    );
                }
            }

            int? topProductId = null;
            if (productSales.Any())
            {
                topProductId = productSales
                    .OrderByDescending(p => p.Value.quantity)
                    .First().Key;
            }

            return new Report
            {
                FromDate = from,
                ToDate = to,
                TotalEarnings = totalEarnings,
                TopProductId = topProductId
            };
        }

        public Task<Report> GetDailyReportAsync(DateTime date)
        {
            var from = date.Date;
            var to = from.AddDays(1).AddTicks(-1);
            return GenerateReportAsync(from, to);
        }

        public Task<Report> GetMonthlyReportAsync(int year, int month)
        {
            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1).AddTicks(-1);
            return GenerateReportAsync(from, to);
        }

        public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync(DateTime from, DateTime to, int topN)
        {
            var orders = await _context.Orders
                .Where(o => o.Status == "Confirmed"
                            && o.CreatedAt >= from
                            && o.CreatedAt <= to)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .ToListAsync();

            var productSales = new Dictionary<int, (string name, int quantity, decimal revenue)>();

            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    var product = item.Product;
                    if (product == null) continue;

                    var price = product.Price;
                    var discountPercent = product.DiscountPercent ?? 0m;
                    var finalPrice = price * (1 - discountPercent / 100m);
                    var lineRevenue = finalPrice * item.Quantity;

                    if (!productSales.ContainsKey(product.Id))
                    {
                        productSales[product.Id] = (product.Name, 0, 0m);
                    }

                    var current = productSales[product.Id];
                    productSales[product.Id] = (
                        current.name,
                        current.quantity + item.Quantity,
                        current.revenue + lineRevenue
                    );
                }
            }

            return productSales
                .OrderByDescending(p => p.Value.quantity)
                .Take(topN)
                .Select(p => new TopProductDto
                {
                    ProductId = p.Key,
                    ProductName = p.Value.name,
                    QuantitySold = p.Value.quantity,
                    TotalEarnings = p.Value.revenue
                })
                .ToList();
        }
    }
}
