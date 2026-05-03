using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace ScanPoint.Repositories.Repositories
{
    public class AuthRepository : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        // Access token: 30 minuta | Refresh token: 7 ditë
        private const int AccessTokenMinutes = 30;
        private const int RefreshTokenDays = 7;

        public AuthRepository(
            IUserRepository userRepository,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _config = config;
            _env = env;
        }

        // ============================
        // REGISTER (Vetëm Admin)
        // ============================
        public async Task<RegisterUserResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                return new RegisterUserResponseDto { Success = false, Message = "Email already exists." };

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = "Admin",
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.ProfileImage != null && dto.ProfileImage.Length > 0)
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadDir, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.ProfileImage.CopyToAsync(stream);

                newUser.ProfileImagePath = $"uploads/profiles/{fileName}";
            }

            await _userRepository.AddAsync(newUser);

            return new RegisterUserResponseDto
            {
                Success = true,
                Message = "Admin registered successfully.",
                UserId = newUser.Id
            };
        }

        // ============================
        // LOGIN
        // ============================
        public async Task<LoginResponseDto> LoginAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new LoginResponseDto { Success = false, Message = "Email ose fjalëkalimi është i gabuar." };

            if (user.IsDeleted)
                return new LoginResponseDto { Success = false, Message = "Llogaria juaj është joaktive. Kontaktoni administratorin." };

            var accessToken = await GenerateJwtTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(RefreshTokenDays);
            await _userRepository.UpdateAsync(user);

            return new LoginResponseDto
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                Role = user.Role
            };
        }

        // ============================
        // REFRESH TOKEN
        // ✅ Token rotation: gjenerohet refresh token i ri me çdo kërkesë
        // ============================
        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return new LoginResponseDto { Success = false };

            if (user.IsDeleted)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userRepository.UpdateAsync(user);
                return new LoginResponseDto { Success = false, Message = "Llogaria juaj është joaktive." };
            }

            var newAccessToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();

            // ✅ Token rotation — refresh token i vjetër invalidohet
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(RefreshTokenDays);
            await _userRepository.UpdateAsync(user);

            return new LoginResponseDto
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                Role = user.Role
            };
        }

        // ============================
        // LOGOUT — fshin refresh token nga DB
        // ============================
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        // ============================
        // JWT GENERATOR (privat)
        // Algoritmi: HmacSha256 — konsistent në të gjithë sistemin
        // Expiry: 30 minuta
        // ============================
        private async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            if (user.Role == "Manager" || user.Role == "Cashier")
            {
                if (user.ShopId.HasValue)
                    claims.Add(new Claim("ShopId", user.ShopId.Value.ToString()));
                else
                    throw new InvalidOperationException("Manager/Cashier must have a ShopId assigned.");
            }

            if (user.Role == "Admin")
            {
                var adminShopIds = await _userRepository.GetAdminShopIdsAsync(user.Id);
                if (adminShopIds != null && adminShopIds.Any())
                    claims.Add(new Claim("AdminShopIds", string.Join(",", adminShopIds)));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            // ✅ HmacSha256 — i vetmi algoritëm në sistem (TokenService u hoq)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ============================
        // REFRESH TOKEN GENERATOR (privat)
        // 64 bytes kriptografikë të rastësishëm
        // ============================
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
