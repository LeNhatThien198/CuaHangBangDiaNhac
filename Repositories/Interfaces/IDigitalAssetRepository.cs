using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Repositories.Interfaces
{
    public interface IDigitalAssetRepository
    {
        Task<DigitalAsset> GetByIdAsync(int id);
        Task<List<DigitalAsset>> GetByReleaseVersionAsync(int releaseVersionId);
        Task<DigitalAsset> CreateAsync(DigitalAsset asset);
        Task UpdateAsync(DigitalAsset asset);
        Task DeleteAsync(int id);
    }
}
