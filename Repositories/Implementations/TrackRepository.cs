using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Repositories.Implementations
{
    public class TrackRepository : ITrackRepository
    {
        private readonly ApplicationDbContext _context;

        public TrackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Track> GetByIdAsync(int id)
        {
            return await _context.Tracks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Track>> GetByReleaseVersionAsync(int releaseVersionId)
        {
            return await _context.Tracks
                .AsNoTracking()
                .Where(t => t.ReleaseVersionId == releaseVersionId)
                .OrderBy(t => t.TrackNumber)
                .ToListAsync();
        }

        public async Task<Track> CreateAsync(Track track)
        {
            _context.Tracks.Add(track);
            await _context.SaveChangesAsync();
            return track;
        }

        public async Task UpdateAsync(Track track)
        {
            _context.Tracks.Update(track);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var track = await _context.Tracks.FindAsync(id);
            if (track != null)
            {
                _context.Tracks.Remove(track);
                await _context.SaveChangesAsync();
            }
        }
    }
}
