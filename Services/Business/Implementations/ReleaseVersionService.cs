using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Services.Business.Implementations
{
    public class ReleaseVersionService : IReleaseVersionService
    {
        private readonly IReleaseVersionRepository _versionRepo;
        private readonly ApplicationDbContext _context;

        public ReleaseVersionService(IReleaseVersionRepository versionRepo, ApplicationDbContext context)
        {
            _versionRepo = versionRepo;
            _context = context;
        }

        public async Task<ReleaseVersion> GetVersionByIdAsync(int id)
        {
            return await _versionRepo.GetByIdAsync(id);
        }

        public async Task<List<ReleaseVersion>> GetVersionsByProductAsync(int productId)
        {
            return await _versionRepo.GetByProductAsync(productId);
        }

        public async Task<decimal> GetPriceByConditionAsync(int productId, ReleaseCondition condition)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Sản phẩm không tồn tại");

            // Calculate base price (after promotion if applicable)
            decimal basePrice = product.PromotionPrice.HasValue && product.PromotionPrice > 0 
                ? product.PromotionPrice.Value 
                : product.Price;
            
            // Apply condition multiplier
            decimal multiplier = condition switch
            {
                ReleaseCondition.Mint => 1.00m,              // 100%
                ReleaseCondition.NearMint => 0.95m,          // 95%
                ReleaseCondition.VeryGoodPlus => 0.90m,      // 90%
                ReleaseCondition.VeryGood => 0.85m,          // 85%
                ReleaseCondition.Good => 0.75m,              // 75%
                ReleaseCondition.Fair => 0.60m,              // 60%
                ReleaseCondition.Poor => 0.50m,              // 50%
                _ => 1.00m
            };

            return Math.Round(basePrice * multiplier, 2);
        }

        public async Task<ReleaseVersion> CreateVersionAsync(ReleaseVersion version)
        {
            if (version.ReleaseDate == default)
                throw new ArgumentException("Ngày phát hành không hợp lệ", nameof(version.ReleaseDate));

            return await _versionRepo.CreateAsync(version);
        }

        public async Task UpdateVersionAsync(ReleaseVersion version)
        {
            if (version.ReleaseDate == default)
                throw new ArgumentException("Ngày phát hành không hợp lệ", nameof(version.ReleaseDate));

            await _versionRepo.UpdateAsync(version);
        }

        public async Task DeleteVersionAsync(int id)
        {
            await _versionRepo.DeleteAsync(id);
        }

        public async Task<bool> IsOutOfStockAsync(int id)
        {
            var version = await _versionRepo.GetByIdAsync(id);
            if (version?.Product == null)
                return true;

            return version.Product.Quantity <= 0;
        }
    }
}
