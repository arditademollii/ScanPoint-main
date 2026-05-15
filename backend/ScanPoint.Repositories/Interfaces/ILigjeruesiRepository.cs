using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    // Interface = kontrata — tregon ÇKA duhet të bëjë repository
    // Implementimi real ndodh në LigjeruesiRepository.cs
    public interface ILigjeruesiRepository
    {
        Task<List<Ligjeruesi>> GetAllAsync();           // Merr të gjitha ligjeruesit  shumes prandaj liste
        Task<Ligjeruesi?> GetByIdAsync(int id);         // Merr ligjeruesin me ID
        Task<Ligjeruesi> CreateAsync(Ligjeruesi ligjeruesi);  // Shto ligjerues të ri
        Task UpdateAsync(Ligjeruesi ligjeruesi);            // Përditëso ligjeruesin    
        Task DeleteAsync(int id);                    // Fshi shkollën
    }
}
