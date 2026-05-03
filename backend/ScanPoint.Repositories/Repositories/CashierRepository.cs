using ScanPoint.Models.Data;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ScanPoint.Repositories.Repositories
{
    public class CashierRepository : ICashierRepository
    {
        private readonly ScanPointDbContext _context;

        public CashierRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // ===========================
        // READ
        // ===========================

        public async Task<List<Cashier>> GetByManagerAsync(Guid managerId)
        {
            return await _context.Cashiers
                .Include(c => c.Manager)
                .Include(c => c.Shop)
                .Where(c => c.ManagerId == managerId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Cashier?> GetByIdAsync(Guid id)
        {
            return await _context.Cashiers
                .Include(c => c.Manager)
                .Include(c => c.Shop)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<List<Cashier>> GetAllByAdminAsync(Guid adminId)
        {
            var adminShops = await _context.Shops
                .Where(s => s.AdminId == adminId)
                .Select(s => s.Id)
                .ToListAsync();

            return await _context.Cashiers
                .Include(c => c.Manager)
                .Include(c => c.Shop)
                .Where(c => c.ShopId.HasValue && adminShops.Contains(c.ShopId.Value) && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Cashier>> GetAllByAdminIncludeDeletedAsync(Guid adminId)
        {
            var adminShops = await _context.Shops
                .IgnoreQueryFilters()
                .Where(s => s.AdminId == adminId)
                .Select(s => s.Id)
                .ToListAsync();

            return await _context.Cashiers
                .IgnoreQueryFilters()
                .Include(c => c.Manager)
                .Include(c => c.Shop)
                .Where(c => c.ShopId.HasValue && adminShops.Contains(c.ShopId.Value))
                .ToListAsync();
        }

        public async Task<Cashier?> GetDeletedByIdAsync(Guid id)
        {
            return await _context.Cashiers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);
        }

        public async Task<Manager?> GetManagerByIdAsync(Guid managerId)
        {
            return await _context.Managers
                .Include(m => m.Shop)
                .FirstOrDefaultAsync(m => m.Id == managerId);
        }

        // ===========================
        // VALIDATION — logjika biznesi e centralizuar këtu
        // Kontrollon email/username ndërmjet GJITHA tabelave (Users, Managers, Cashiers)
        // ===========================

        public async Task<bool> EmailExistsGloballyAsync(string email, Guid? excludeId = null)
        {
            var lower = email.ToLower();

            var inAdmins = await _context.Users
                .AnyAsync(a => a.Email.ToLower() == lower);
            if (inAdmins) return true;

            var inManagers = await _context.Managers
                .IgnoreQueryFilters()
                .AnyAsync(m => m.Email.ToLower() == lower &&
                               (excludeId == null || m.Id != excludeId));
            if (inManagers) return true;

            var inCashiers = await _context.Cashiers
                .IgnoreQueryFilters()
                .AnyAsync(c => c.Email.ToLower() == lower &&
                               (excludeId == null || c.Id != excludeId));
            return inCashiers;
        }

        public async Task<bool> UsernameExistsGloballyAsync(string username, Guid? excludeId = null)
        {
            var lower = username.ToLower();

            var inAdmins = await _context.Users
                .AnyAsync(a => a.Username.ToLower() == lower);
            if (inAdmins) return true;

            var inManagers = await _context.Managers
                .IgnoreQueryFilters()
                .AnyAsync(m => m.Username.ToLower() == lower &&
                               (excludeId == null || m.Id != excludeId));
            if (inManagers) return true;

            var inCashiers = await _context.Cashiers
                .IgnoreQueryFilters()
                .AnyAsync(c => c.Username.ToLower() == lower &&
                               (excludeId == null || c.Id != excludeId));
            return inCashiers;
        }

        // ===========================
        // WRITE
        // ===========================

        public async Task AddAsync(Cashier cashier, string plainPassword)
        {
            if (cashier == null) throw new ArgumentNullException(nameof(cashier));
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password cannot be empty.");

            cashier.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            cashier.IsDeleted = false;
            await _context.Cashiers.AddAsync(cashier);
        }

        public Task UpdateAsync(Cashier cashier)
        {
            _context.Cashiers.Update(cashier);
            return Task.CompletedTask;
        }

        // ✅ Soft Delete — invalido refresh token automatikisht
        public async Task DeleteAsync(Cashier cashier)
        {
            cashier.IsDeleted = true;
            cashier.DeletedAt = DateTime.UtcNow;
            cashier.RefreshToken = null;
            cashier.RefreshTokenExpiryTime = null;

            _context.Cashiers.Update(cashier);
            await Task.CompletedTask;
        }

        public async Task RestoreAsync(Guid id)
        {
            var cashier = await _context.Cashiers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cashier == null)
                throw new KeyNotFoundException($"Cashier with ID {id} not found.");

            cashier.IsDeleted = false;
            cashier.DeletedAt = null;

            _context.Cashiers.Update(cashier);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}