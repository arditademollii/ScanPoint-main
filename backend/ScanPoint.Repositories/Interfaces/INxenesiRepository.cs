using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface INxenesiRepository
    {
        Task<List<Nxenesi>> GetAllAsync();                    // Të gjithë nxënësit
        Task<List<Nxenesi>> GetByShkollaAsync(int shkollaId); // Nxënësit e një shkolle
        Task<Nxenesi?> GetByIdAsync(int id);                  // Nxënësi me ID
        Task<Nxenesi> CreateAsync(Nxenesi nxenesi);           // Shto nxënës
        Task UpdateAsync(Nxenesi nxenesi);                     // Përditëso nxënës
        Task DeleteAsync(int id);                              // Fshi nxënës
    }
}
