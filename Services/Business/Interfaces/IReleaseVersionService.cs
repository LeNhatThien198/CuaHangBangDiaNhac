using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Services.Business.Interfaces
{
    public interface IReleaseVersionService
    {
        Task<ReleaseVersion> GetVersionByIdAsync(int id);
        Task<List<ReleaseVersion>> GetVersionsByProductAsync(int productId);
        Task<decimal> GetPriceByConditionAsync(int productId, ReleaseCondition condition);
        Task<ReleaseVersion> CreateVersionAsync(ReleaseVersion version);
        Task UpdateVersionAsync(ReleaseVersion version);
        Task DeleteVersionAsync(int id);
        Task<bool> IsOutOfStockAsync(int id);
    }
}
