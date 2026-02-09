using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Artist)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Genre)
                .Include(p => p.Style)
                .Include(p => p.Images)
                .Include(p => p.ReleaseVersions)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetPublishedAsync(int page = 1, int pageSize = 12)
        {
            return await _context.Products
                .Include(p => p.Artist)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsPublished)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 12)
        {
            return await _context.Products
                .Include(p => p.Artist)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsPublished && p.CategoryId == categoryId)
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Product>> SearchAsync(string keyword, int page = 1, int pageSize = 12)
        {
            keyword = keyword?.ToLower() ?? "";
            return await _context.Products
                .Include(p => p.Artist)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Where(p => p.IsPublished && 
                    (p.Name.ToLower().Contains(keyword) || p.Artist.Name.ToLower().Contains(keyword)))
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Products
                .Where(p => p.IsPublished)
                .CountAsync();
        }
    }
}
