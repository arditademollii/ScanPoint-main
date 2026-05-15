using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    public class LigjeratatRepository : ILigjerataRepository
    {
        private readonly ScanPointDbContext _context;

        public LigjeratatRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — të gjitha ligjeratat
        public async Task<List<Ligjerata>> GetAllAsync()
        {
            return await _context.Ligjeratat
                .Include(l => l.Ligjeruesi) // JOIN me tabelën Ligjeruesit  
                .ToListAsync();
        }

        // GET BY LIGJERUESI — filtro ligjeratat sipas ligjeruesit
        // Kjo përdoret për filterin e listës së ligjeratave
        public async Task<List<Ligjerata>> GetByLigjeruesiAsync(int ligjeruesiId)
        {
            return await _context.Ligjeratat
                .Where(l => l.ID_Ligjeruesi == ligjeruesiId) // Filtro me WHERE
                .Include(l => l.Ligjeruesi)
                .ToListAsync();
        }

        // GET BY ID — nxënësi me ID specifik
        public async Task<Ligjerata?> GetByIdAsync(int id)
        {
            return await _context.Ligjeratat
                .Include(l => l.Ligjeruesi)
                .FirstOrDefaultAsync(l => l.ID == id);
        }

        // CREATE — shto ligjeratë të re
        public async Task<Ligjerata> CreateAsync(Ligjerata ligjerata)
        {
            _context.Ligjeratat.Add(ligjerata);
            await _context.SaveChangesAsync();
            return ligjerata;
        }

       

        // DELETE — fshi ligjeratën
        public async Task DeleteAsync(int id)
        {
            var ligjerata = await _context.Ligjeratat.FindAsync(id);
            if (ligjerata == null)
                throw new KeyNotFoundException($"Ligjerata me ID {id} nuk u gjet.");

            _context.Ligjeratat.Remove(ligjerata);
            await _context.SaveChangesAsync();
        }
 // UPDATE — përditëso të dhënat e ligjeratës
        public async Task UpdateAsync(Ligjerata ligjerata)
        {
            _context.Ligjeratat.Update(ligjerata);
            await _context.SaveChangesAsync();
        }

       
    }
}
