using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.ViewModels.Admin
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int NewOrdersCount { get; set; } // e.g. Pending/Processing
        public decimal DailyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        
        // Lists
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        
        // Chart Data
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<decimal> ChartData { get; set; } = new List<decimal>();
    }
}
