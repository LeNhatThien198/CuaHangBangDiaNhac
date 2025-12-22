using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CuaHangBangDiaNhac.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order/History
        [HttpGet]
        public async Task<IActionResult> History(string searchString, string sortOrder)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Images)
                .Include(o => o.ShippingAddress)
                .Where(o => o.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                if (int.TryParse(searchString, out int id))
                {
                    query = query.Where(o => o.Id == id);
                }
                else 
                {
                     // Search by product name in items or status
                     query = query.Where(o => o.Items.Any(i => i.Product.Name.ToLower().Contains(searchString)));
                }
            }

            switch (sortOrder)
            {
                case "date_asc": query = query.OrderBy(o => o.CreatedAt); break;
                case "total_asc": query = query.OrderBy(o => o.Items.Sum(i => i.UnitPrice * i.Quantity) + o.ShippingFee - o.Discount); break;
                case "total_desc": query = query.OrderByDescending(o => o.Items.Sum(i => i.UnitPrice * i.Quantity) + o.ShippingFee - o.Discount); break;
                case "status_asc": query = query.OrderBy(o => o.Status); break;
                case "status_desc": query = query.OrderByDescending(o => o.Status); break;
                default: query = query.OrderByDescending(o => o.CreatedAt); break;
            }

            var orders = await query.ToListAsync();

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.ShippingAddress)
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Artist)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.Items) // Include items to return stock if needed (optional logic)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            // Only allow cancellation if Pending or Processing
            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Processing)
            {
                order.Status = OrderStatus.Cancelled;
                
                _context.Update(order);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng này do đơn hàng đã được vận chuyển hoặc hoàn tất.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
