using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CuaHangBangDiaNhac.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index([FromQuery] List<int> selectedIds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Artist)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            var itemsToCheckout = cart.Items;

            // If selections provided, filter
            if (selectedIds != null && selectedIds.Any())
            {
                itemsToCheckout = itemsToCheckout.Where(i => selectedIds.Contains(i.ProductId)).ToList();
            }

            if (!itemsToCheckout.Any())
            {
                TempData["ErrorMessage"] = "Không có sản phẩm nào để thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            // 2. Get User Addresses
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId && !a.IsDeleted)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();

            var shipments = new List<CheckoutShipmentViewModel>();
            int shipmentCounter = 1;

            var inStockItems = itemsToCheckout.Where(i => i.Product != null && !i.Product.IsPreOrder).ToList();
            if (inStockItems.Any())
            {
                shipments.Add(new CheckoutShipmentViewModel
                {
                    ShipmentId = shipmentCounter++,
                    Title = "Đơn hàng có sẵn",
                    IsPreOrder = false,
                    Items = MapToViewModels(inStockItems)
                });
            }

            // Group 2..n: Pre-Order Items (Grouped by ReleaseDate)
            var preOrderItems = itemsToCheckout.Where(i => i.Product != null && i.Product.IsPreOrder).ToList();
            var preOrderGroups = preOrderItems
                .GroupBy(i => i.Product.ReleaseDate?.Date)
                .OrderBy(g => g.Key);

            foreach (var group in preOrderGroups)
            {
                var releaseDate = group.Key;
                shipments.Add(new CheckoutShipmentViewModel
                {
                    ShipmentId = shipmentCounter++,
                    Title = releaseDate.HasValue
                        ? $"Đơn đặt trước (Dự kiến: {releaseDate.Value:dd/MM/yyyy})"
                        : "Đơn đặt trước",
                    IsPreOrder = true,
                    ReleaseDate = releaseDate,
                    Items = MapToViewModels(group.ToList())
                });
            }

            // Calculate Totals per Shipment
            foreach (var shipment in shipments)
            {
                shipment.ShippingFee = 30000; // Fixed Fee
            }

            var viewModel = new CheckoutViewModel
            {
                Addresses = addresses,
                SelectedAddressId = addresses.FirstOrDefault(a => a.IsDefault)?.Id ?? 0,
                Shipments = shipments,
                PaymentMethod = shipments.Any(s => s.IsPreOrder) ? "Transfer" : "COD",
                SelectedIds = selectedIds ?? new List<int>()
            };

            return View(viewModel);
        }

        private List<CartItemViewModel> MapToViewModels(List<CartItem> items)
        {
            return items.Select(ci => new CartItemViewModel
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product?.Name ?? "Unknown",
                ProductImage = ci.Product?.Images?.FirstOrDefault(i => i.IsPrimary)?.Url ?? "/images/no-image.png",
                ArtistName = ci.Product?.Artist?.Name ?? "Unknown Artist",
                Price = ci.Product?.Price ?? 0,
                PromotionPrice = ci.Product?.PromotionPrice,
                Quantity = ci.Quantity
            }).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // 1. Validate Address
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == model.SelectedAddressId && a.UserId == userId && !a.IsDeleted);
            
            if (address == null)
            {
                TempData["ErrorMessage"] = "Địa chỉ không hợp lệ. Vui lòng chọn lại.";
                return RedirectToAction("Index");
            }

            // 2. Get Cart (Source of Truth)
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống.";
                return RedirectToAction("Index", "Cart");
            }

            // 3. Execution Strategy (Transaction)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var itemsToProcess = cart.Items.ToList();
                if (model.SelectedIds != null && model.SelectedIds.Any())
                {
                    itemsToProcess = itemsToProcess.Where(i => model.SelectedIds.Contains(i.ProductId)).ToList();
                }

                if (!itemsToProcess.Any())
                {
                     throw new Exception("Không có sản phẩm nào được chọn để thanh toán.");
                }

                var orderGroups = new List<(bool IsPreOrder, DateTime? ReleaseDate, List<CartItem> Items)>();
                
                var inStockItems = itemsToProcess.Where(i => i.Product != null && !i.Product.IsPreOrder).ToList();
                if (inStockItems.Any())
                {
                    orderGroups.Add((false, null, inStockItems));
                }

                var preOrderItems = itemsToProcess.Where(i => i.Product != null && i.Product.IsPreOrder).ToList();
                var preOrderGroups = preOrderItems
                    .GroupBy(i => i.Product.ReleaseDate?.Date)
                    .OrderBy(g => g.Key);
                
                foreach(var group in preOrderGroups)
                {
                    orderGroups.Add((true, group.Key, group.ToList()));
                }

                var hasPreOrder = orderGroups.Any(g => g.IsPreOrder);
                
                PaymentMethod paymentMethod = PaymentMethod.COD;
                if (model.PaymentMethod == "Transfer")
                {
                    paymentMethod = PaymentMethod.Deposit;
                }
                else
                {
                     Enum.TryParse<PaymentMethod>(model.PaymentMethod, out paymentMethod);
                }

                if (hasPreOrder && paymentMethod != PaymentMethod.Deposit) // Assuming Transfer maps to Deposit enum
                {
                    // Force Deposit/Transfer if user tried to bypass UI
                    paymentMethod = PaymentMethod.Deposit;
                }

                var createdOrderIds = new List<int>();

                foreach (var group in orderGroups)
                {
                    var order = new Order
                    {
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                        Status = OrderStatus.Pending,
                        ShippingFee = 30000,
                        Discount = 0,
                        DepositAmount = 0,
                        PaymentMethod = paymentMethod,
                        IsPreOrderDeposit = group.IsPreOrder,
                        // Snapshot Address Info
                        ShippingAddressId = address.Id
                    };
                    
                    // Add Items
                    foreach(var item in group.Items)
                    {
                        var orderItem = new OrderItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = item.Product.PromotionPrice ?? item.Product.Price
                        };
                        order.Items.Add(orderItem);

                        // Deduct Stock
                        if (item.Product != null)
                        {
                            item.Product.Quantity -= item.Quantity;
                        }
                    }

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync(); // Save per order to get ID
                    createdOrderIds.Add(order.Id);
                }

                // 5. Clear Cart (ONLY PROCESSED ITEMS)
                _context.CartItems.RemoveRange(itemsToProcess);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Redirect to Success Page
                TempData["PaymentMethod"] = paymentMethod.ToString(); // "COD" or "Deposit"
                return RedirectToAction("Success", new { orderIds = createdOrderIds }); 
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xử lý đơn hàng: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Success(List<int> orderIds)
        {
            if (orderIds == null || !orderIds.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => orderIds.Contains(o.Id))
                .ToListAsync();

            ViewBag.PaymentMethod = TempData["PaymentMethod"]?.ToString() == "Deposit" ? "Transfer" : "COD";
            // Also check from the orders themselves if TempData is lost
            if (ViewBag.PaymentMethod == null && orders.Any())
            {
                ViewBag.PaymentMethod = orders.First().PaymentMethod == PaymentMethod.Deposit ? "Transfer" : "COD";
            }

            ViewBag.GrandTotal = orders.Sum(o => o.Total);

            return View(orders);
        }
    }
}
