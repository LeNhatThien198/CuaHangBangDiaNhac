using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CuaHangBangDiaNhac.Services.Business.Interfaces;
using CuaHangBangDiaNhac.Models;

namespace CuaHangBangDiaNhac.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackController : ControllerBase
    {
        private readonly ITrackService _trackService;
        private readonly IAuditLogService _auditService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TrackController(ITrackService trackService, IAuditLogService auditService, IHttpContextAccessor httpContextAccessor)
        {
            _trackService = trackService;
            _auditService = auditService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrackAsync(int id)
        {
            try
            {
                var track = await _trackService.GetTrackByIdAsync(id);
                if (track == null)
                    return NotFound(new { message = "Track not found" });
                return Ok(new { success = true, data = track });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("release/{releaseVersionId}")]
        public async Task<IActionResult> GetTracksByReleaseAsync(int releaseVersionId)
        {
            try
            {
                var tracks = await _trackService.GetTracksByReleaseAsync(releaseVersionId);
                return Ok(new { success = true, data = tracks });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateTrackAsync([FromBody] Track track)
        {
            try
            {
                if (track.TrackNumber <= 0 || string.IsNullOrWhiteSpace(track.Title))
                    return BadRequest(new { success = false, message = "Invalid track data" });

                var createdTrack = await _trackService.CreateTrackAsync(track);
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                await _auditService.LogActionAsync("CREATE", "Track", createdTrack.Id, userId, null, createdTrack.Title);
                
                return CreatedAtAction(nameof(GetTrackAsync), new { id = createdTrack.Id }, new { success = true, data = createdTrack });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateTrackAsync(int id, [FromBody] Track track)
        {
            try
            {
                track.Id = id;
                await _trackService.UpdateTrackAsync(track);
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                await _auditService.LogActionAsync("UPDATE", "Track", id, userId, null, track.Title);
                
                return Ok(new { success = true, message = "Track updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteTrackAsync(int id)
        {
            try
            {
                await _trackService.DeleteTrackAsync(id);
                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value ?? "System";
                await _auditService.LogActionAsync("DELETE", "Track", id, userId);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
