using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ============================
        // REGISTER
        // ============================
        [HttpPost("register")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Register([FromForm] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // ============================
        // LOGIN
        // ============================
        [HttpPost("login")]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        // ============================
        // REFRESH TOKEN
        // Nuk kërkon [Authorize] — access token mund të jetë skaduar.
        // Refresh token valid në body është e mjaftueshme.
        // ============================
        [HttpPost("refresh")]
        [Consumes("application/json")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(new { message = "Refresh token mungon." });

            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

            if (!result.Success)
                return Unauthorized(new { message = "Refresh token i pavlefshëm ose skaduar." });

            return Ok(result);
        }

        // ============================
        // LOGOUT — fshin refresh token nga DB
        // ============================
        [HttpPost("logout")]
        [Authorize]
        [Consumes("application/json")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(new { message = "Refresh token mungon." });

            var success = await _authService.LogoutAsync(dto.RefreshToken);

            if (!success)
                return BadRequest(new { message = "Logout dështoi. Token jo i vlefshëm." });

            return Ok(new { message = "Logout i suksesshëm." });
        }
    }
}
