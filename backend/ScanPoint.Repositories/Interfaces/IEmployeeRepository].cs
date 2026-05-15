using ScanPoint.Models.Models;

namespace ScanPoint.Repositories.Interfaces
{
    // Interface = kontrata — tregon ÇKA duhet të bëjë repository
    // Implementimi real ndodh në EmployeeRepository.cs
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync();           // Merr të gjitha punonjësit  shumes prandaj liste
        Task<Employee?> GetByIdAsync(int id);         // Merr punonjësin me ID
        Task<Employee> CreateAsync(Employee employee);  // Shto punonjës të ri
        Task UpdateAsync(Employee employee);            // Përditëso punonjësin
        Task DeleteAsync(int id);                    // Fshi punonjësin
    }
}
