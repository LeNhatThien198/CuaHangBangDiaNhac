using CuaHangBangDiaNhac.Models;
using CuaHangBangDiaNhac.Repositories.Interfaces;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CuaHangBangDiaNhac.Services.Business.Implementations
{
    public class DigitalAssetService : IDigitalAssetService
    {
        private readonly IDigitalAssetRepository _assetRepo;
        private readonly IConfiguration _config;
        private static readonly Dictionary<string, string> TokenCache = new();

        public DigitalAssetService(IDigitalAssetRepository assetRepo, IConfiguration config)
        {
            _assetRepo = assetRepo;
            _config = config;
        }

        public async Task<DigitalAsset> GetAssetByIdAsync(int id)
        {
            return await _assetRepo.GetByIdAsync(id);
        }

        public async Task<List<DigitalAsset>> GetAssetsByReleaseAsync(int releaseVersionId)
        {
            return await _assetRepo.GetByReleaseVersionAsync(releaseVersionId);
        }

        public async Task<DigitalAsset> CreateAssetAsync(DigitalAsset asset)
        {
            if (string.IsNullOrWhiteSpace(asset.FileUrl))
                throw new ArgumentException("URL tệp không được để trống", nameof(asset.FileUrl));

            return await _assetRepo.CreateAsync(asset);
        }

        public async Task UpdateAssetAsync(DigitalAsset asset)
        {
            if (string.IsNullOrWhiteSpace(asset.FileUrl))
                throw new ArgumentException("URL tệp không được để trống", nameof(asset.FileUrl));

            await _assetRepo.UpdateAsync(asset);
        }

        public async Task DeleteAssetAsync(int id)
        {
            await _assetRepo.DeleteAsync(id);
        }

        public async Task<string> GenerateTokenUrlAsync(int assetId, int expiryMinutes = 60)
        {
            var jwtSecret = _config["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(jwtSecret))
                throw new InvalidOperationException("JWT Secret Key không được cấu hình");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("assetId", assetId.ToString()),
                new Claim("type", "asset_access")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            try
            {
                var jwtSecret = _config["Jwt:SecretKey"];
                if (string.IsNullOrEmpty(jwtSecret))
                    return false;

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GenerateDownloadTokenAsync(int assetId, string userId, int expiryMinutes = 60)
        {
            var asset = await _assetRepo.GetByIdAsync(assetId);
            if (asset == null)
                throw new Exception("Asset not found");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"] ?? "DefaultSecretKeyFor256BitHmacSha256");

            var claims = new List<Claim>
            {
                new Claim("assetId", assetId.ToString()),
                new Claim("userId", userId),
                new Claim("type", "download"),
                new Claim(JwtRegisteredClaimNames.Exp, ((long)(DateTime.UtcNow.AddMinutes(expiryMinutes) - new DateTime(1970, 1, 1)).TotalSeconds).ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = "CuaHangBangDiaNhac",
                Audience = "DigitalAssetDownload",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            TokenCache[tokenString] = assetId.ToString();

            return tokenString;
        }

        public async Task<bool> VerifyDownloadTokenAsync(string token, int assetId, string userId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"] ?? "DefaultSecretKeyFor256BitHmacSha256");

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "CuaHangBangDiaNhac",
                    ValidateAudience = true,
                    ValidAudience = "DigitalAssetDownload",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var tokenAssetId = principal.FindFirst("assetId")?.Value;
                var tokenUserId = principal.FindFirst("userId")?.Value;

                if (tokenAssetId != assetId.ToString() || tokenUserId != userId)
                    return false;

                var asset = await _assetRepo.GetByIdAsync(assetId);
                return asset != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GenerateDownloadUrlAsync(int assetId, string userId)
        {
            var token = await GenerateDownloadTokenAsync(assetId, userId);
            return $"/api/DigitalAsset/download/{token}";
        }
    }
}
