using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IProfesoriRepository
    {
        Task<List<Profesori>> GetAllAsync();                    // Të gjithë profesorët
        Task<List<Profesori>> GetByUniversitetAsync(int universitetId); // Profesorët e një universiteti
        Task<Profesori?> GetByIdAsync(int id);                  // Profesori me ID
        Task<Profesori> CreateAsync(Profesori profesori);           // Shto profesori
        Task UpdateAsync(Profesori profesori);                     // Përditëso profesori
        Task DeleteAsync(int id);                              // Fshi profesori
    }
}
