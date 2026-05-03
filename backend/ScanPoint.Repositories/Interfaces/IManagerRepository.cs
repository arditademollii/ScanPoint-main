using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IManagerRepository
    {
        Task<List<Manager>> GetAllByAdminIdAsync(Guid adminId);
        Task<List<Manager>> GetAllByAdminIdIncludeDeletedAsync(Guid adminId); // ✅ për restore
        Task<Manager> GetByIdAsync(Guid id, Guid adminId);
        Task<Manager> CreateAsync(Manager manager);
        Task<Manager> UpdateAsync(Manager manager);
        Task<bool> DeleteAsync(Guid id, Guid adminId);
        Task<bool> RestoreAsync(Guid id, Guid adminId);           // ✅ restore
        Task<bool> ExistsAsync(string username, string email);
    }
}