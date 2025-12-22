using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")] 
    [Authorize(Roles = "Admin,Staff")]
    public class DashboardController : Controller
    {
        private readonly CuaHangBangDiaNhac.Data.ApplicationDbContext _context;

        public DashboardController(CuaHangBangDiaNhac.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            // 1. Stats
            // Fix: Calculate in memory to avoid "Cannot perform aggregate on aggregate" SQL error
            var dailyOrders = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CreatedAt.Date == today && o.Status != Models.OrderStatus.Cancelled && o.Status != Models.OrderStatus.Returned)
                .ToList();
            
            var dailyRevenue = dailyOrders.Sum(o => o.Items.Sum(i => i.Quantity * i.UnitPrice) + o.ShippingFee - o.Discount);

            var monthlyOrders = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CreatedAt >= startOfMonth && o.Status != Models.OrderStatus.Cancelled && o.Status != Models.OrderStatus.Returned)
                .ToList();

            var monthlyRevenue = monthlyOrders.Sum(o => o.Items.Sum(i => i.Quantity * i.UnitPrice) + o.ShippingFee - o.Discount);

            var newOrdersCount = _context.Orders
                .Count(o => o.Status == Models.OrderStatus.Pending || o.Status == Models.OrderStatus.Processing);

            var totalProducts = _context.Products.Count();
            var totalUsers = _context.Users.Count();

            // 2. Recent Orders (Top 5)
            var recentOrders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items) // Include items to calculate total in memory if needed
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToList();

            // 3. Chart Data (Last 7 Days Revenue)
            var chartLabels = new List<string>();
            var chartData = new List<decimal>();

            // Optimize chart data fetching: Get all orders for last 7 days once
            var sevenDaysAgo = today.AddDays(-6);
            var lastWeekOrders = _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CreatedAt.Date >= sevenDaysAgo && o.Status != Models.OrderStatus.Cancelled && o.Status != Models.OrderStatus.Returned)
                .ToList();

            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var revenue = lastWeekOrders
                    .Where(o => o.CreatedAt.Date == date)
                    .Sum(o => o.Items.Sum(x => x.Quantity * x.UnitPrice) + o.ShippingFee - o.Discount);
                
                chartLabels.Add(date.ToString("dd/MM"));
                chartData.Add(revenue);
            }

            var model = new CuaHangBangDiaNhac.ViewModels.Admin.DashboardViewModel
            {
                DailyRevenue = dailyRevenue,
                MonthlyRevenue = monthlyRevenue,
                NewOrdersCount = newOrdersCount,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                RecentOrders = recentOrders,
                ChartLabels = chartLabels,
                ChartData = chartData
            };

            return View(model);
        }
    }
}
