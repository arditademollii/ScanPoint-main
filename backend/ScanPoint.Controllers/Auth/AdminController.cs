using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.Data;
using ScanPoint.Repositories.Interfaces;
using System.Security.Claims;

namespace ScanPoint.Controllers.Auth
{
    /// <summary>
    /// Endpoint-et administrative të Admin-it.
    /// ITokenService u hoq — refresh token menaxhohet nga AuthController /api/auth/refresh.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ScanPointDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(
            IUserRepository userRepository,
            ScanPointDbContext context,
            IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _context = context;
            _env = env;
        }

        // Helper — merr ID-në e adminit nga token-i
        private Guid GetCurrentAdminId()
        {
            var idStr = User.FindFirstValue("Id")
                        ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(idStr, out var id))
                throw new UnauthorizedAccessException("Id i pavlefshëm në token.");

            return id;
        }

        // ============================
        // GET PROFILE
        // Kthen informacionin e adminit të kyçur
        // ============================
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var adminId = GetCurrentAdminId();
            var user = await _userRepository.GetByIdAsync(adminId);

            if (user == null)
                return NotFound(new { message = "Admin nuk u gjet." });

            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                role = user.Role
            });
        }

        // ============================
        // SHËNIM: regenerate-token u hoq
        // ============================
        // Refresh token menaxhohet tani nga:
        //   POST /api/auth/refresh   — gjeneron access token + refresh token të ri
        //   POST /api/auth/logout    — invalido refresh token në DB
        //
        // Klienti (frontend) e bën këtë automatikisht nëpërmjet axiosInstance interceptor.
        // ============================

        // ... endpoint-et e tjera të AdminController (shop, etj.) shtohen këtu
    }
}