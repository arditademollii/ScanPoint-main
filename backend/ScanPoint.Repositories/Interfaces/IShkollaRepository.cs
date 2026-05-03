using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    // Interface = kontrata — tregon ÇKA duhet të bëjë repository
    // Implementimi real ndodh në ShkollaRepository.cs
    public interface IShkollaRepository
    {
        Task<List<Shkolla>> GetAllAsync();           // Merr të gjitha shkollat
        Task<Shkolla?> GetByIdAsync(int id);         // Merr shkollën me ID
        Task<Shkolla> CreateAsync(Shkolla shkolla);  // Shto shkollë të re
        Task UpdateAsync(Shkolla shkolla);            // Përditëso shkollën
        Task DeleteAsync(int id);                    // Fshi shkollën
    }
}
