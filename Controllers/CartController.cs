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

        public async Task<IActionResult> Index()
        {
            var cart = await GetCart();
            var sortedItems = cart.Items
                .OrderBy(ci => ci.Product?.Artist?.Name)
                .ThenBy(ci => ci.Product?.Name);

            var viewModel = new CartViewModel
            {
                Items = sortedItems.Select(ci => new CartItemViewModel
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
                    MaxStock = ci.Product?.Quantity ?? 0
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
                }
                else
                {
                    _context.CartItems.Remove(item);
                }
                await _context.SaveChangesAsync();
            }

            // Calculate totals for response (optional, but good for AJAX)
            // But since we reload page in JS, simple success is enough.
            // But let's verify if `item` is null (removed) for Total calculation.
            
            // Re-fetch or calculate from memory
            return Json(new { success = true });
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
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Artist)
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
