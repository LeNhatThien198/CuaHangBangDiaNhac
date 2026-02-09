using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Repositories.Interfaces
{
    public interface IReleaseVersionRepository
    {
        Task<ReleaseVersion> GetByIdAsync(int id);
        Task<List<ReleaseVersion>> GetByProductAsync(int productId);
        Task<List<ReleaseVersion>> GetByConditionAsync(ReleaseCondition condition);
        Task<ReleaseVersion> CreateAsync(ReleaseVersion version);
        Task UpdateAsync(ReleaseVersion version);
        Task DeleteAsync(int id);
    }
}
