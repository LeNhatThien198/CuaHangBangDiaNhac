using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Services.Business.Implementations
{
    public class TrackService : ITrackService
    {
        private readonly ITrackRepository _trackRepo;

        public TrackService(ITrackRepository trackRepo)
        {
            _trackRepo = trackRepo;
        }

        public async Task<Track> GetTrackByIdAsync(int id)
        {
            return await _trackRepo.GetByIdAsync(id);
        }

        public async Task<List<Track>> GetTracksByReleaseAsync(int releaseVersionId)
        {
            return await _trackRepo.GetByReleaseVersionAsync(releaseVersionId);
        }

        public async Task<Track> CreateTrackAsync(Track track)
        {
            if (track.TrackNumber <= 0)
                throw new ArgumentException("Số thứ tự bài hát phải lớn hơn 0", nameof(track.TrackNumber));

            if (string.IsNullOrWhiteSpace(track.Title))
                throw new ArgumentException("Tên bài hát không được để trống", nameof(track.Title));

            return await _trackRepo.CreateAsync(track);
        }

        public async Task UpdateTrackAsync(Track track)
        {
            if (track.TrackNumber <= 0)
                throw new ArgumentException("Số thứ tự bài hát phải lớn hơn 0", nameof(track.TrackNumber));

            if (string.IsNullOrWhiteSpace(track.Title))
                throw new ArgumentException("Tên bài hát không được để trống", nameof(track.Title));

            await _trackRepo.UpdateAsync(track);
        }

        public async Task DeleteTrackAsync(int id)
        {
            await _trackRepo.DeleteAsync(id);
        }
    }
}
