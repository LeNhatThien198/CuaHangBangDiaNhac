using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Services.Business.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<List<Product>> GetPublishedProductsAsync(int page = 1, int pageSize = 12);
        Task<List<Product>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pageSize = 12);
        Task<List<Product>> SearchProductsAsync(string keyword, int page = 1, int pageSize = 12);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task<decimal> GetFinalPriceAsync(int productId);
    }
}
