using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IContractRepository
    {
        Task<List<Contract>> GetAllAsync();                    // Të gjitha kontratat
        Task<List<Contract>> GetByEmployeeAsync(int employeeId); // Kontratat e një punonjësi
        Task<Contract?> GetByIdAsync(int id);                  // Kontrata me ID
        Task<Contract> CreateAsync(Contract contract);           // Shto kontratë
        Task UpdateAsync(Contract contract);                     // Përditëso kontratë
        Task DeleteAsync(int id);                              // Fshi kontratë
    }
}
