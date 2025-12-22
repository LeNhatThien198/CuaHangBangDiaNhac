using Microsoft.AspNetCore.Mvc;
using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CuaHangBangDiaNhac.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentSearch"] = searchString;

            var cart = await GetCart();
            IEnumerable<CartItem> query = cart.Items;

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Product.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                      || (s.Product.Artist?.Name ?? "").Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(s => s.Product.Name);
                    break;
                case "price_asc":
                    query = query.OrderBy(s => (s.Product.PromotionPrice ?? s.Product.Price));
                    break;
                case "price_desc":
                    query = query.OrderByDescending(s => (s.Product.PromotionPrice ?? s.Product.Price));
                    break;
                default:
                    // Default: Group by Artist, then Product Name
                    query = query.OrderBy(ci => ci.Product?.Artist?.Name).ThenBy(ci => ci.Product?.Name);
                    break;
            }

            var viewModel = new CartViewModel
            {
                Items = query.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.Name ?? "Unknown",
                    ProductImage = ci.Product?.Images?.FirstOrDefault(i => i.IsPrimary)?.Url ?? "/images/no-image.png",
                    ArtistName = ci.Product?.Artist?.Name ?? "Unknown Artist",
                    ArtistId = ci.Product?.ArtistId ?? 0,
                    IsPreOrder = ci.Product?.IsPreOrder ?? false,
                    IsUsed = ci.Product?.IsUsed ?? false,
                    CreatedAt = ci.Product?.CreatedAt ?? DateTime.MinValue,
                    Price = ci.Product?.Price ?? 0,
                    PromotionPrice = ci.Product?.PromotionPrice,
                    Quantity = ci.Quantity,
                    MaxStock = ci.Product?.Quantity ?? 0,
                    // Pass current search down if needed, or total logic
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var cart = await GetCart();
            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    CartId = cart.Id
                });
            }

            await _context.SaveChangesAsync();

            // Recalculate count for badge
            var totalItems = cart.Items.Sum(x => x.Quantity);

            return Json(new { success = true, message = "Đã thêm vào giỏ hàng!", cartCount = totalItems });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var cart = await GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                    await _context.SaveChangesAsync();

                    // Recalculate totals
                    var itemTotal = (item.Product.PromotionPrice ?? item.Product.Price) * item.Quantity;
                    var cartCount = cart.Items.Sum(x => x.Quantity);

                    return Json(new { 
                        success = true, 
                        itemTotal = itemTotal, 
                        formattedItemTotal = itemTotal.ToString("N0") + " ₫", 
                        cartCount = cartCount
                    });
                }
                else
                {
                    _context.CartItems.Remove(item);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, shouldReload = true });
                }
            }

            return Json(new { success = false, message = "Item not found" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var cart = await GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMultipleItems(List<int> productIds)
        {
            var cart = await GetCart();
            var itemsToRemove = cart.Items.Where(x => productIds.Contains(x.ProductId)).ToList();
            if (itemsToRemove.Any())
            {
                _context.CartItems.RemoveRange(itemsToRemove);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        public async Task<IActionResult> GetCartCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Optimized query: Don't load full products
            var count = await _context.Carts
                .Where(c => c.UserId == userId)
                .SelectMany(c => c.Items)
                .SumAsync(i => i.Quantity);
            
            return Json(new { count = count });
        }

        private async Task<Cart> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product!)
                        .ThenInclude(p => p.Artist!)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            return cart;
        }
    }
}
