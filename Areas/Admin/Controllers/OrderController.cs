using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBangDiaNhac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")] // Staff can also manage orders
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index(OrderStatus? status, string searchString, string sortOrder, int page = 1)
        {
            int pageSize = 10;
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Artist)
                .Include(o => o.ShippingAddress)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                if (int.TryParse(searchString, out int id))
                {
                    query = query.Where(o => o.Id == id);
                }
                else
                {
                    query = query.Where(o => (o.User != null && (o.User.FullName.ToLower().Contains(searchString) || o.User.UserName.ToLower().Contains(searchString) || o.User.Email.ToLower().Contains(searchString)))
                                          || (o.ShippingAddress != null && (o.ShippingAddress.ReceiverName.ToLower().Contains(searchString) || o.ShippingAddress.PhoneNumber.Contains(searchString))));
                }
            }

            switch (sortOrder)
            {
                case "date_asc": query = query.OrderBy(o => o.CreatedAt); break;
                case "total_asc": query = query.OrderBy(o => o.Items.Sum(i => i.UnitPrice * i.Quantity) + o.ShippingFee - o.Discount); break;
                case "total_desc": query = query.OrderByDescending(o => o.Items.Sum(i => i.UnitPrice * i.Quantity) + o.ShippingFee - o.Discount); break;
                case "status_asc": query = query.OrderBy(o => o.Status); break;
                case "status_desc": query = query.OrderByDescending(o => o.Status); break;
                default: query = query.OrderByDescending(o => o.CreatedAt); break; // Default date_desc
            }

            int totalItems = await query.CountAsync();
            var orders = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.Pagination = new CuaHangBangDiaNhac.ViewModels.Shared.Pagination { PageNumber = page, TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize) };
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            return View(orders);
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.ShippingAddress)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Admin/Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus, string? returnUrl = null)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            // Basic validation logic (optional: prevent reverting from Delivered to Pending etc.)
            order.Status = newStatus;
            
            // If Delivered, maybe set a DeliveredAt timestamp? (If added to model)
            // If Cancelled, maybe restore stock? (Complex logic, skipping for now unless requested)

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cập nhật trạng thái đơn hàng #{id} thành công!";
            
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
