using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Repositories.Interfaces
{
    public interface ITrackRepository
    {
        Task<Track> GetByIdAsync(int id);
        Task<List<Track>> GetByReleaseVersionAsync(int releaseVersionId);
        Task<Track> CreateAsync(Track track);
        Task UpdateAsync(Track track);
        Task DeleteAsync(int id);
    }
}
