using ScanPoint.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IFabrikaRepository
    {
        Task<List<Fabrika>> GetAllAsync();           // Merr të gjitha fabrikat  shumes prandaj liste
        Task<Fabrika?> GetByIdAsync(int id);         // Merr fabrikën me ID
        Task<Fabrika> CreateAsync(Fabrika fabrika);  // Shto fabrikë të re
        Task UpdateAsync(Fabrika fabrika);            // Përditëso fabrikën 
        Task DeleteAsync(int id);                    // Fshi shkollën
    }
}
