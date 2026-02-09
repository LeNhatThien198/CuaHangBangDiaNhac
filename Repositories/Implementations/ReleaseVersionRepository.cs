using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Repositories.Implementations
{
    public class ReleaseVersionRepository : IReleaseVersionRepository
    {
        private readonly ApplicationDbContext _context;

        public ReleaseVersionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReleaseVersion> GetByIdAsync(int id)
        {
            return await _context.ReleaseVersions
                .Include(rv => rv.Tracks)
                .Include(rv => rv.DigitalAsset)
                .AsNoTracking()
                .FirstOrDefaultAsync(rv => rv.Id == id);
        }

        public async Task<List<ReleaseVersion>> GetByProductAsync(int productId)
        {
            return await _context.ReleaseVersions
                .Include(rv => rv.Tracks)
                .AsNoTracking()
                .Where(rv => rv.ProductId == productId)
                .OrderByDescending(rv => rv.ReleaseDate)
                .ToListAsync();
        }

        public async Task<List<ReleaseVersion>> GetByConditionAsync(ReleaseCondition condition)
        {
            return await _context.ReleaseVersions
                .Include(rv => rv.Tracks)
                .AsNoTracking()
                .Where(rv => rv.Condition == condition)
                .OrderByDescending(rv => rv.ReleaseDate)
                .ToListAsync();
        }

        public async Task<ReleaseVersion> CreateAsync(ReleaseVersion version)
        {
            _context.ReleaseVersions.Add(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task UpdateAsync(ReleaseVersion version)
        {
            _context.ReleaseVersions.Update(version);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var version = await _context.ReleaseVersions.FindAsync(id);
            if (version != null)
            {
                _context.ReleaseVersions.Remove(version);
                await _context.SaveChangesAsync();
            }
        }
    }
}
