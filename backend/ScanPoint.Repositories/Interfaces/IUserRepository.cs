using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByRefreshTokenAsync(string token);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);

        // Add this method to fix CS1061
        Task UpdateProfileImageAsync(Guid id, string imagePath);

        Task<UserProfileDto?> GetProfileByIdAsync(Guid userId);


        Task UpdateProfileAsync(User user, string? newPassword = null);

        // 🔹 Kjo metodë kthehet lista e Guid-eve të shop-eve që admin menaxhon
        Task<List<Guid>> GetAdminShopIdsAsync(Guid adminId);

        Task SaveRefreshTokenAsync(Guid userId, string refreshToken, DateTime expires);
    }
}
