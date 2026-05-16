using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    // Interface = kontrata — tregon ÇKA duhet të bëjë repository
    // Implementimi real ndodh në SpitaliRepository.cs
    public interface ISpitaliRepository
    {
        Task<List<Spitali>> GetAllAsync();           // Merr të gjitha Spitalit  shumes prandaj liste
        Task<Spitali?> GetByIdAsync(int id);         // Merr shkollën me ID
        Task<Spitali> CreateAsync(Spitali Spitali);  // Shto shkollë të re
        Task UpdateAsync(Spitali Spitali);            // Përditëso shkollën
        Task DeleteAsync(int id);                    // Fshi spitalin   
    }
}
