using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetPublishedAsync(int page = 1, int pageSize = 12);
        Task<List<Product>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 12);
        Task<List<Product>> SearchAsync(string keyword, int page = 1, int pageSize = 12);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<int> GetTotalCountAsync();
    }
}
