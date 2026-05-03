using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ScanPointDbContext _context;

        public UserRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public Task<List<User>> GetAllAsync()
        {
            return _context.Users.ToListAsync();
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User> GetByIdAsync(Guid id)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<User> GetByRefreshTokenAsync(string token)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.RefreshToken == token);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProfileImageAsync(Guid userId, string imagePath)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.ProfileImagePath = imagePath;
            await _context.SaveChangesAsync();
        }

        public async Task<UserProfileDto?> GetProfileByIdAsync(Guid userId)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileDto
                {
                    Username = u.Username,
                    Email = u.Email,
                    ProfileImagePath = u.ProfileImagePath
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateProfileAsync(User user, string? newPassword = null)
        {
            if (!string.IsNullOrEmpty(newPassword))
            {
                // BCrypt - i njëjti algoritëm si RegisterAsync
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // 🔹 Metoda e re për marrjen e shop-eve që admin menaxhon
        // 🔹 Merr GUID-et e shop-eve që admin menaxhon
        public async Task<List<Guid>> GetAdminShopIdsAsync(Guid adminId)
        {
            // Lexon direkt nga tabela Shops ku AdminId = adminId
            return await _context.Shops
                .Where(s => s.AdminId == adminId)
                .Select(s => s.Id)
                .ToListAsync();
        }

        public async Task SaveRefreshTokenAsync(Guid userId, string refreshToken, DateTime expires)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expires; // ← jo RefreshTokenExpiry
            await _context.SaveChangesAsync();
        }

    }
}
