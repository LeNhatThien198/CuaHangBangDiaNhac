using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReleaseVersionController : ControllerBase
    {
        private readonly IReleaseVersionService _versionService;
        private readonly IProductService _productService;
        private readonly IAuditLogService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReleaseVersionController(IReleaseVersionService versionService, IProductService productService, IAuditLogService auditService, IHttpContextAccessor httpContextAccessor)
        {
            _versionService = versionService;
            _productService = productService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVersionAsync(int id)
        {
            try
            {
                var version = await _versionService.GetVersionByIdAsync(id);
                if (version == null)
                    return NotFound(new { message = "Version not found" });
                return Ok(new { success = true, data = version });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetVersionsByProductAsync(int productId)
        {
            try
            {
                var versions = await _versionService.GetVersionsByProductAsync(productId);
                return Ok(new { success = true, data = versions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}/price")]
        public async Task<IActionResult> GetPriceAsync(int id)
        {
            try
            {
                var version = await _versionService.GetVersionByIdAsync(id);
                if (version == null)
                    return NotFound(new { message = "Version not found" });
                
                var product = await _productService.GetProductByIdAsync(version.ProductId);
                var price = await _versionService.GetPriceByConditionAsync(product.Id, version.Condition);
                
                return Ok(new { success = true, data = new { price, condition = version.Condition.ToString() } });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateVersionAsync([FromBody] ReleaseVersion version)
        {
            try
            {
                var createdVersion = await _versionService.CreateVersionAsync(version);
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                await _auditService.LogActionAsync("CREATE", "ReleaseVersion", createdVersion.Id, userId, null, createdVersion.VersionType.ToString());
                
                return CreatedAtAction(nameof(GetVersionAsync), new { id = createdVersion.Id }, new { success = true, data = createdVersion });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateVersionAsync(int id, [FromBody] ReleaseVersion version)
        {
            try
            {
                version.Id = id;
                await _versionService.UpdateVersionAsync(version);
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                await _auditService.LogActionAsync("UPDATE", "ReleaseVersion", id, userId);
                
                return Ok(new { success = true, message = "Version updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteVersionAsync(int id)
        {
            try
            {
                await _versionService.DeleteVersionAsync(id);
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                await _auditService.LogActionAsync("DELETE", "ReleaseVersion", id, userId);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
