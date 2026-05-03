using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface ICashierRepository
    {
        // --- READ ---
        Task<List<Cashier>> GetByManagerAsync(Guid managerId);
        Task<List<Cashier>> GetAllByAdminAsync(Guid adminId);
        Task<List<Cashier>> GetAllByAdminIncludeDeletedAsync(Guid adminId);
        Task<Cashier?> GetByIdAsync(Guid id);
        Task<Cashier?> GetDeletedByIdAsync(Guid id);
        Task<Manager?> GetManagerByIdAsync(Guid managerId);

        // --- VALIDATION (logjika biznesi — jo në Controller) ---
        Task<bool> EmailExistsGloballyAsync(string email, Guid? excludeId = null);
        Task<bool> UsernameExistsGloballyAsync(string username, Guid? excludeId = null);

        // --- WRITE ---
        Task AddAsync(Cashier cashier, string plainPassword);
        Task UpdateAsync(Cashier cashier);
        Task DeleteAsync(Cashier cashier);
        Task RestoreAsync(Guid id);
        Task SaveChangesAsync();
    }
}