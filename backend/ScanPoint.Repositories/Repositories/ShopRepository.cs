using ScanPoint.Models.Data;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ScanPoint.Repositories.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly ScanPointDbContext _context;

        public ShopRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        public async Task<Shop> CreateAsync(Shop shop)
        {
            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();
            return shop;
        }

        public async Task<Shop?> GetByIdAsync(Guid id)
        {
            return await _context.Shops
                .Include(s => s.Admin)
                .Include(s => s.Managers)
                .Include(s => s.Cashiers)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Shop>> GetAllAsync()
        {
            return await _context.Shops
                .Include(s => s.Admin)
                .ToListAsync();
        }

        public async Task<List<Shop>> GetByAdminIdAsync(Guid adminId)
        {
            // Vetëm shopet aktive
            return await _context.Shops
                .Where(s => s.AdminId == adminId)
                .Include(s => s.Admin)
                .ToListAsync();
        }

        // Të gjitha shopet përfshi të fshirat — për restore feature
        public async Task<List<Shop>> GetAllByAdminIdAsync(Guid adminId)
        {
            return await _context.Shops
                .IgnoreQueryFilters()
                .Where(s => s.AdminId == adminId)
                .Include(s => s.Admin)
                .ToListAsync();
        }

        public async Task UpdateAsync(Shop shop)
        {
            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var shop = await _context.Shops
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shop == null)
                throw new KeyNotFoundException($"Shop with ID {id} not found.");

            if (shop.IsDeleted)
                throw new InvalidOperationException("Shop është tashmë i fshirë.");

            shop.IsDeleted = true;
            shop.DeletedAt = DateTime.UtcNow;

            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
        }

        public async Task RestoreAsync(Guid id)
        {
            var shop = await _context.Shops
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shop == null)
                throw new KeyNotFoundException($"Shop with ID {id} not found.");

            shop.IsDeleted = false;
            shop.DeletedAt = null;

            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
        }

    

        public async Task<bool> ExistsByNameForAdminAsync(string name, Guid adminId)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Shops
                .AnyAsync(s => s.Name.ToLower() == normalizedName
                            && s.AdminId == adminId
                            && !s.IsDeleted);
        }

        public async Task<bool> ExistsByNameForAdminUpdateAsync(string name, Guid adminId, Guid id)
        {
            var normalizedName = name.Trim().ToLower();

            return await _context.Shops
                .AnyAsync(s => s.Name.ToLower() == normalizedName
                            && s.AdminId == adminId
                            && s.Id != id
                            && !s.IsDeleted);
        }
    }
}