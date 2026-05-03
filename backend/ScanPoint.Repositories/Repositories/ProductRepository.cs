using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ScanPointDbContext _context;

        public ProductRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            product.IsDeleted = true; // soft delete
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllByShopAsync(Guid shopId)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Where(p => p.ShopId == shopId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllByAdminAsync(Guid adminId)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .Where(p => p.Shop.AdminId == adminId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Shop)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Product> GetByBarcodeAsync(string barcode, Guid shopId)
        {
            // Heq AsNoTracking që të mund të bëjmë update
            return await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.Barcode == barcode &&
                    p.ShopId == shopId &&
                    !p.IsDeleted);
        }
    }
}
