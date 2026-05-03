using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.DTOs.User;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileDto = ScanPoint.Models.DTOs.User.UserProfileDto;

namespace ScanPoint.Repositories.Repositories
{
    public class UserManagementService : IUserManagementService
    {
        private readonly ScanPointDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Hierarkia e roleve: çdo rol mund të krijojë vetëm rolet nën të
        private static readonly Dictionary<string, List<string>> _allowedToCreate = new()
        {
            { "Admin",   new List<string> { "Manager", "Cashier" } },
            { "Manager", new List<string> { "Cashier" } },
            { "Cashier", new List<string>() }
        };

        // Rolet që janë të dukshme për secilin rol
        private static readonly Dictionary<string, List<string>> _allowedToView = new()
        {
            { "Admin",   new List<string> { "Admin", "Manager", "Cashier" } },
            { "Manager", new List<string> { "Manager", "Cashier" } },
            { "Cashier", new List<string>() }
        };

        public UserManagementService(ScanPointDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ============================
        // KRIJO USER
        // ============================
        public async Task<ServiceResult<UserProfileDto>> CreateUserAsync(CreateUserDto dto, string callerRole)
        {
            // 1. Kontrollo hierarkinë e roleve
            if (!_allowedToCreate.TryGetValue(callerRole, out var creatableRoles) ||
                !creatableRoles.Contains(dto.Role))
            {
                return ServiceResult<UserProfileDto>.Fail(
                    $"Roli '{callerRole}' nuk ka leje të krijojë user me rol '{dto.Role}'.");
            }

            // 2. Kontrollo duplikatin e email-it
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (emailExists)
                return ServiceResult<UserProfileDto>.Fail(
                    $"Email-i '{dto.Email}' është tashmë në përdorim.");

            // 3. Kontrollo duplikatin e username-it
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower());

            if (usernameExists)
                return ServiceResult<UserProfileDto>.Fail(
                    $"Username-i '{dto.Username}' është tashmë në përdorim.");

            // 4. Krijo userin
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            // 5. Ngarko foton nëse ka
            if (dto.File != null && dto.File.Length > 0)
            {
                user.ProfileImagePath = await SaveProfileImageAsync(dto.File);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ServiceResult<UserProfileDto>.Ok(MapToDto(user));
        }

        // ============================
        // PËRDITËSO USER
        // ============================
        public async Task<ServiceResult<UserProfileDto>> UpdateUserAsync(
            Guid targetUserId, UpdateUserDto dto, Guid callerId, string callerRole)
        {
            var targetUser = await _context.Users.FindAsync(targetUserId);
            if (targetUser == null)
                return ServiceResult<UserProfileDto>.Fail("User-i nuk u gjet.");

            // Nëse nuk po përditëson veten, kontrollo hierarkinë
            bool isSelfUpdate = targetUserId == callerId;

            if (!isSelfUpdate)
            {
                if (!_allowedToCreate.TryGetValue(callerRole, out var manageableRoles) ||
                    !manageableRoles.Contains(targetUser.Role))
                {
                    return ServiceResult<UserProfileDto>.Fail(
                        $"Nuk ke leje të ndryshosh userin me rol '{targetUser.Role}'.");
                }
            }

            // Kontrollo duplikatin e email-it (nëse po e ndryshon)
            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                dto.Email.ToLower() != targetUser.Email.ToLower())
            {
                var emailTaken = await _context.Users
                    .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != targetUserId);

                if (emailTaken)
                    return ServiceResult<UserProfileDto>.Fail(
                        $"Email-i '{dto.Email}' është tashmë në përdorim.");

                targetUser.Email = dto.Email.ToLower();
            }

            // Kontrollo duplikatin e username-it (nëse po e ndryshon)
            if (!string.IsNullOrWhiteSpace(dto.Username) &&
                dto.Username.ToLower() != targetUser.Username.ToLower())
            {
                var usernameTaken = await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == dto.Username.ToLower() && u.Id != targetUserId);

                if (usernameTaken)
                    return ServiceResult<UserProfileDto>.Fail(
                        $"Username-i '{dto.Username}' është tashmë në përdorim.");

                targetUser.Username = dto.Username;
            }

            // Ndrysho password-in nëse ka
            if (!string.IsNullOrWhiteSpace(dto.Password))
                targetUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Ngarko foton nëse ka
            if (dto.File != null && dto.File.Length > 0)
                targetUser.ProfileImagePath = await SaveProfileImageAsync(dto.File);

            _context.Users.Update(targetUser);
            await _context.SaveChangesAsync();

            return ServiceResult<UserProfileDto>.Ok(MapToDto(targetUser));
        }

        // ============================
        // FSHI USER
        // ============================
        public async Task<ServiceResult<bool>> DeleteUserAsync(
            Guid targetUserId, Guid callerId, string callerRole)
        {
            // Nuk mund të fshish veten
            if (targetUserId == callerId)
                return ServiceResult<bool>.Fail("Nuk mund të fshish llogarinë tënde.");

            var targetUser = await _context.Users.FindAsync(targetUserId);
            if (targetUser == null)
                return ServiceResult<bool>.Fail("User-i nuk u gjet.");

            if (!_allowedToCreate.TryGetValue(callerRole, out var manageableRoles) ||
                !manageableRoles.Contains(targetUser.Role))
            {
                return ServiceResult<bool>.Fail(
                    $"Nuk ke leje të fshish userin me rol '{targetUser.Role}'.");
            }

            _context.Users.Remove(targetUser);
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(true);
        }

        // ============================
        // LISTO USERAT
        // ============================
        public async Task<ServiceResult<IEnumerable<UserProfileDto>>> GetUsersAsync(string callerRole)
        {
            if (!_allowedToView.TryGetValue(callerRole, out var visibleRoles) || !visibleRoles.Any())
                return ServiceResult<IEnumerable<UserProfileDto>>.Fail("Nuk ke leje të shohësh userat.");

            var users = await _context.Users
                .Where(u => visibleRoles.Contains(u.Role))
                .Select(u => MapToDto(u))
                .ToListAsync();

            return ServiceResult<IEnumerable<UserProfileDto>>.Ok(users);
        }

        // ============================
        // HELPERS
        // ============================
        private async Task<string> SaveProfileImageAsync(IFormFile file)
        {
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "profiles");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"uploads/profiles/{fileName}";
        }

        private static UserProfileDto MapToDto(User u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Role = u.Role,
            ProfileImagePath = u.ProfileImagePath
        };
    }
}
