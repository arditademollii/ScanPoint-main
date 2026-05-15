using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface ILigjerataRepository
    {
        Task<List<Ligjerata>> GetAllAsync();                    // Të gjitha ligjeratat
        Task<List<Ligjerata>> GetByLigjeruesiAsync(int ligjeruesiId); // Ligjeratat e një ligjeruesi
        Task<Ligjerata?> GetByIdAsync(int id);                  // Ligjerata me ID
        Task<Ligjerata> CreateAsync(Ligjerata ligjerata);           // Shto ligjerata
        Task UpdateAsync(Ligjerata ligjerata);                     // Përditëso ligjerata
        Task DeleteAsync(int id);                              // Fshi nxënës
    }
}
