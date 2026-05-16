using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    // Interface për Satelite
    public interface ISateliteRepository
    {
        // Merr të gjithë satelitët
        Task<List<Satelite>> GetAllAsync();

        // Merr satelitët e një planeti
        Task<List<Satelite>> GetByPlanetAsync(int planetId);

        // Merr satelitin sipas ID
        Task<Satelite?> GetByIdAsync(int id);

        // Krijo satelit
        Task<Satelite> CreateAsync(Satelite satelite);

        // Update satelit
        Task UpdateAsync(Satelite satelite);

        // Soft delete
        Task DeleteAsync(int id);
    }
}