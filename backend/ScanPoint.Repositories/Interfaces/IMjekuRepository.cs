using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IMjekuRepository
    {
        Task<List<Mjeku>> GetAllAsync();                    // Të gjithë nxënësit
        Task<List<Mjeku>> GetBySpitaliAsync(int SpitaliId); // Nxënësit e një shkolle
        Task<Mjeku?> GetByIdAsync(int id);                  // Nxënësi me ID
        Task<Mjeku> CreateAsync(Mjeku Mjeku);           // Shto nxënës
        Task UpdateAsync(Mjeku Mjeku);                     // Përditëso nxënës
        Task DeleteAsync(int id);                              // Fshi nxënës
    }
}
