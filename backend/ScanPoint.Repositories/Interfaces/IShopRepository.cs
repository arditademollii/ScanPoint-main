using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IShopRepository
    {
        // --- READ ---
        Task<Shop> CreateAsync(Shop shop);
        Task<Shop?> GetByIdAsync(Guid id);
        Task<IEnumerable<Shop>> GetAllAsync();
        Task<List<Shop>> GetByAdminIdAsync(Guid adminId);
        Task<List<Shop>> GetAllByAdminIdAsync(Guid adminId);

        // --- WRITE ---
        Task UpdateAsync(Shop shop);
        Task DeleteAsync(Guid id);
        Task RestoreAsync(Guid id);

        // --- VALIDATION (logjika biznesi — jo në Controller) ---
        Task<bool> ExistsByNameForAdminAsync(string name, Guid adminId);
        Task<bool> ExistsByNameForAdminUpdateAsync(string name, Guid adminId, Guid excludeId);
    }
}