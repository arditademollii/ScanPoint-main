using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IPlayerRepository
    {
        Task<List<Player232470351>> GetAllAsync();                    // Të gjithë nxënësit
        Task<List<Player232470351>> GetByTeam232470351Async(int Team232470351Id); // Nxënësit e një shkolle
        Task<Player232470351?> GetByIdAsync(int id);                  // Nxënësi me ID
        Task<Player232470351> CreateAsync(Player232470351 Player232470351);           // Shto nxënës
        Task UpdateAsync(Player232470351 Player232470351);                     // Përditëso nxënës
        Task DeleteAsync(int id);                              // Fshi nxënës
    }
}
