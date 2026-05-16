using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    // Interface për Planet
    public interface IPlanetRepository
    {
        // Merr të gjithë planetët që nuk janë deleted
        Task<List<Planet>> GetAllAsync();

        // Merr planetin sipas ID
        Task<Planet?> GetByIdAsync(int id);

        // Krijo planet të ri
        Task<Planet> CreateAsync(Planet planet);

        // Update planet
        Task UpdateAsync(Planet planet);

        // Soft delete
        Task DeleteAsync(int id);
    }
}