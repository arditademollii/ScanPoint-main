using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.DTOs.User;
using ScanPoint.Repositories.Interfaces;
using System.Security.Claims;

namespace ScanPoint.Controllers.Auth
{
    [ApiController]
    [Route("api/profile")] // Kjo është rruga kryesore
    [Authorize] // Çdo përdorues i kyçur mund ta thërrasë
    
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ScanPointDbContext _context;

        public UserManagementController(IUserManagementService userManagementService, ScanPointDbContext context)
        {
            _userManagementService = userManagementService;
            _context = context;
        }

        // ============================
        // GET /api/UserManagement/me
        // ⚠️ DUHET të jetë PARA {id:guid}
        // ============================
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var (callerId, _) = GetCallerInfo();

            if (callerId == null)
                return Unauthorized(new { message = "User Id nuk gjendet në token." });

            var user = await _context.Users.FindAsync(callerId.Value);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new Models.DTOs.User.UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ProfileImagePath = user.ProfileImagePath
            });
        }

        // ============================
        // PUT /api/UserManagement/me
        // ⚠️ DUHET të jetë PARA {id:guid}
        // ============================
        [HttpPut("me")]
        
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (callerId, callerRole) = GetCallerInfo();
            if (callerId == null || callerRole == null) return Unauthorized();

            var result = await _userManagementService.UpdateUserAsync(
                callerId.Value, dto, callerId.Value, callerRole);

            if (!result.Success)
                return BadRequest(new { message = result.Error });

            return Ok(result.Data);
        }

        // ============================
        // GET /api/UserManagement
        // Vetëm Admin dhe Manager
        // ============================
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUsers()
        {
            var callerRole = GetCallerRole();
            if (callerRole == null) return Unauthorized();

            var result = await _userManagementService.GetUsersAsync(callerRole);
            return result.Success ? Ok(result.Data) : Forbid();
        }

        // ============================
        // POST /api/UserManagement
        // Vetëm Admin dhe Manager
        // ============================
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var callerRole = GetCallerRole();
            if (callerRole == null) return Unauthorized();

            var result = await _userManagementService.CreateUserAsync(dto, callerRole);

            if (!result.Success)
                return BadRequest(new { message = result.Error });

            return CreatedAtAction(nameof(GetUsers), result.Data);
        }

        // ============================
        // PUT /api/UserManagement/{id}
        // ⚠️ DUHET të jetë PAS "me"
        // ============================
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (callerId, callerRole) = GetCallerInfo();
            if (callerId == null || callerRole == null) return Unauthorized();

            var result = await _userManagementService.UpdateUserAsync(id, dto, callerId.Value, callerRole);

            if (!result.Success)
                return BadRequest(new { message = result.Error });

            return Ok(result.Data);
        }

        // ============================
        // DELETE /api/UserManagement/{id}
        // ============================
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var (callerId, callerRole) = GetCallerInfo();
            if (callerId == null || callerRole == null) return Unauthorized();

            var result = await _userManagementService.DeleteUserAsync(id, callerId.Value, callerRole);

            if (!result.Success)
                return BadRequest(new { message = result.Error });

            return Ok(new { message = "User-i u fshi me sukses." });
        }

        // ============================
        // HELPERS
        // ============================
        private string? GetCallerRole() =>
            User.FindFirst(ClaimTypes.Role)?.Value;

        private (Guid? id, string? role) GetCallerInfo()
        {
            var idStr = User.FindFirst("Id")?.Value
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(idStr, out var id))
                return (null, null);

            return (id, GetCallerRole());
        }

        [HttpGet("test")]
        public IActionResult Test() => Ok("funksionon");
    }
}
