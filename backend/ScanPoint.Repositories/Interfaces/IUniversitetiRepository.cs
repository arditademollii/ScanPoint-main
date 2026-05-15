using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    // Interface = kontrata — tregon ÇKA duhet të bëjë repository
    // Implementimi real ndodh në UniveristetiRepository.cs
    public interface IUniversitetiRepository
    {
        Task<List<Universiteti>> GetAllAsync();           // Merr të gjitha Univeristetit  shumes prandaj liste
        Task<Universiteti?> GetByIdAsync(int id);         // Merr shkollën me ID
        Task<Universiteti> CreateAsync(Universiteti Universiteti);  // Shto shkollë të re
        Task UpdateAsync(Universiteti Universiteti);            // Përditëso shkollën
        Task DeleteAsync(int id);                    // Fshi shkollën
    }
}
