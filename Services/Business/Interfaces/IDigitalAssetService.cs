using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Services.Business.Interfaces
{
    public interface IDigitalAssetService
    {
        Task<DigitalAsset> GetAssetByIdAsync(int id);
        Task<List<DigitalAsset>> GetAssetsByReleaseAsync(int releaseVersionId);
        Task<DigitalAsset> CreateAssetAsync(DigitalAsset asset);
        Task UpdateAssetAsync(DigitalAsset asset);
        Task DeleteAssetAsync(int id);
        Task<string> GenerateTokenUrlAsync(int assetId, int expiryMinutes = 60);
        Task<bool> IsTokenValidAsync(string token);
    }
}
