using ScanPoint.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IPunetoriRepository
    {
        Task<List<Punetori>> GetAllAsync();                    // Të gjithë punëtorët
        Task<List<Punetori>> GetByFabrikaAsync(int fabrikaId); // Punëtorët e një fabrike
        Task<Punetori?> GetByIdAsync(int id);                  // Punëtori me ID
        Task<Punetori> CreateAsync(Punetori punetori);           // Shto punëtor
        Task UpdateAsync(Punetori punetori);                     // Përditëso punëtor
        Task DeleteAsync(int id);                              // Fshi punëtor
    }
}
