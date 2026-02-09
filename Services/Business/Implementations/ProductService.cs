using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Services.Business.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IReleaseVersionService _versionService;
        private readonly IAuditLogService _auditService;
        private readonly IHttpContextAccessor _httpContext;

        public ProductService(IProductRepository productRepo, IReleaseVersionService versionService, 
            IAuditLogService auditService, IHttpContextAccessor httpContext)
        {
            _productRepo = productRepo;
            _versionService = versionService;
            _auditService = auditService;
            _httpContext = httpContext;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepo.GetByIdAsync(id);
        }

        public async Task<List<Product>> GetPublishedProductsAsync(int page = 1, int pageSize = 12)
        {
            return await _productRepo.GetPublishedAsync(page, pageSize);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pageSize = 12)
        {
            return await _productRepo.GetByCategoryAsync(categoryId, page, pageSize);
        }

        public async Task<List<Product>> SearchProductsAsync(string keyword, int page = 1, int pageSize = 12)
        {
            return await _productRepo.SearchAsync(keyword, page, pageSize);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Tên sản phẩm không được để trống", nameof(product.Name));

            var result = await _productRepo.CreateAsync(product);

            var userId = _httpContext?.HttpContext?.User?.FindFirst("sub")?.Value ?? "system";
            await _auditService.LogActionAsync("CREATE", "Product", result.Id, userId, 
                null, $"Tạo sản phẩm: {result.Name}");

            return result;
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Tên sản phẩm không được để trống", nameof(product.Name));

            var existingProduct = await _productRepo.GetByIdAsync(product.Id);
            if (existingProduct == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại");

            await _productRepo.UpdateAsync(product);

            var userId = _httpContext?.HttpContext?.User?.FindFirst("sub")?.Value ?? "system";
            await _auditService.LogActionAsync("UPDATE", "Product", product.Id, userId,
                $"Giá: {existingProduct.Price}", $"Giá: {product.Price}");
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại");

            // Validation: check if product has release versions
            var versions = await _versionService.GetVersionsByProductAsync(id);
            if (versions != null && versions.Count > 0)
                throw new InvalidOperationException($"Không thể xóa sản phẩm có {versions.Count} phiên bản phát hành. Vui lòng xóa phiên bản trước.");

            var userId = _httpContext?.HttpContext?.User.FindFirst("sub")?.Value ?? "system";
            await _productRepo.DeleteAsync(id);
            
            await _auditService.LogActionAsync("DELETE", "Product", id, userId, 
                $"Xóa sản phẩm: {product.Name}", null);
        }

        public async Task<decimal> GetFinalPriceAsync(int productId)
        {
            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại");

            if (product.PromotionPrice.HasValue && product.PromotionPrice > 0)
                return product.PromotionPrice.Value;

            return product.Price;
        }
    }
}
