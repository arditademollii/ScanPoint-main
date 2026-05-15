using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    // Interface = kontrata — tregon ÇKA duhet të bëjë repository
    // Implementimi real ndodh në Team232470351Repository.cs
    public interface ITeamRepository
    {
        Task<List<Team232470351>> GetAllAsync();           // Merr të gjitha Team232470351t  shumes prandaj liste
        Task<Team232470351?> GetByIdAsync(int id);         // Merr team me ID
        Task<Team232470351> CreateAsync(Team232470351 team);  // Shto team të ri
        Task UpdateAsync(Team232470351 team);            // Përditëso team
        Task DeleteAsync(int id);                    // Fshi team  
    }
}
