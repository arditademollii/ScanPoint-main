using ScanPoint.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);

        Task<IEnumerable<Product>> GetAllByShopAsync(Guid shopId);
        Task<IEnumerable<Product>> GetAllByAdminAsync(Guid adminId);

        Task<Product> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<Product> GetByBarcodeAsync(string barcode, Guid shopId);
    }
}
