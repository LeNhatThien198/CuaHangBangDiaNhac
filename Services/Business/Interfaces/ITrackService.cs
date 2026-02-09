using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Services.Business.Interfaces
{
    public interface ITrackService
    {
        Task<Track> GetTrackByIdAsync(int id);
        Task<List<Track>> GetTracksByReleaseAsync(int releaseVersionId);
        Task<Track> CreateTrackAsync(Track track);
        Task UpdateTrackAsync(Track track);
        Task DeleteTrackAsync(int id);
    }
}
