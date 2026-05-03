using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Repositories
{
    /// <summary>
    /// Menaxhon vetëm komunikimin me databazën.
    /// Nuk përmban logjikë biznesi — ajo është në ManagerService.
    /// </summary>
    public class ManagerRepository : IManagerRepository
    {
        private readonly ScanPointDbContext _context;

        public ManagerRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // Vetëm managerët aktivë të këtij admini
        public async Task<List<Manager>> GetAllByAdminIdAsync(Guid adminId)
        {
            return await _context.Managers
                .Include(m => m.Shop)
                .Where(m => m.Shop.AdminId == adminId && !m.IsDeleted)
                .ToListAsync();
        }

        // Të gjithë (përfshi të fshirat) — për admin panel / restore
        public async Task<List<Manager>> GetAllByAdminIdIncludeDeletedAsync(Guid adminId)
        {
            return await _context.Managers
                .IgnoreQueryFilters()
                .Include(m => m.Shop)
                .Where(m => m.Shop.AdminId == adminId)
                .ToListAsync();
        }

        public async Task<Manager?> GetByIdAsync(Guid id, Guid adminId)
        {
            return await _context.Managers
                .Include(m => m.Shop)
                .FirstOrDefaultAsync(m =>
                    m.Id == id &&
                    m.Shop.AdminId == adminId &&
                    !m.IsDeleted);
        }

        // ✅ Repository NUK bën validim biznesi — vetëm e ruan në DB
        // Të gjitha rregullat (shop ekziston, email unik, etj.) janë në ManagerService
        public async Task<Manager> CreateAsync(Manager manager)
        {
            manager.Role = "Manager";
            manager.IsDeleted = false;

            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            // Ringo me Shop të ngarkuar për mapper
            await _context.Entry(manager)
                .Reference(m => m.Shop)
                .LoadAsync();

            return manager;
        }

        public async Task<Manager> UpdateAsync(Manager manager)
        {
            _context.Managers.Update(manager);
            await _context.SaveChangesAsync();
            return manager;
        }

        // ✅ Soft Delete — invalido edhe refresh token
        public async Task<bool> DeleteAsync(Guid id, Guid adminId)
        {
            var manager = await GetByIdAsync(id, adminId);
            if (manager == null) return false;

            manager.IsDeleted = true;
            manager.DeletedAt = DateTime.UtcNow;
            manager.RefreshToken = null;
            manager.RefreshTokenExpiryTime = null;

            _context.Managers.Update(manager);
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Restore — vetëm hiq soft delete flags
        public async Task<bool> RestoreAsync(Guid id, Guid adminId)
        {
            var manager = await _context.Managers
                .IgnoreQueryFilters()
                .Include(m => m.Shop)
                .FirstOrDefaultAsync(m => m.Id == id && m.Shop.AdminId == adminId);

            if (manager == null) return false;

            manager.IsDeleted = false;
            manager.DeletedAt = null;

            _context.Managers.Update(manager);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string username, string email)
        {
            return await _context.Managers
                .AnyAsync(m => m.Username == username || m.Email == email);
        }
    }
}