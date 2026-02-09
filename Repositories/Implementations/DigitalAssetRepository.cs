using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Repositories.Implementations
{
    public class DigitalAssetRepository : IDigitalAssetRepository
    {
        private readonly ApplicationDbContext _context;

        public DigitalAssetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DigitalAsset> GetByIdAsync(int id)
        {
            return await _context.DigitalAssets
                .AsNoTracking()
                .FirstOrDefaultAsync(da => da.Id == id);
        }

        public async Task<List<DigitalAsset>> GetByReleaseVersionAsync(int releaseVersionId)
        {
            return await _context.DigitalAssets
                .AsNoTracking()
                .Where(da => da.ReleaseVersionId == releaseVersionId)
                .ToListAsync();
        }

        public async Task<DigitalAsset> CreateAsync(DigitalAsset asset)
        {
            _context.DigitalAssets.Add(asset);
            await _context.SaveChangesAsync();
            return asset;
        }

        public async Task UpdateAsync(DigitalAsset asset)
        {
            _context.DigitalAssets.Update(asset);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var asset = await _context.DigitalAssets.FindAsync(id);
            if (asset != null)
            {
                _context.DigitalAssets.Remove(asset);
                await _context.SaveChangesAsync();
            }
        }
    }
}
