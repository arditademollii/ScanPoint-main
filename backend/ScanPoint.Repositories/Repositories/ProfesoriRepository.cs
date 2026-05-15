using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    public class ProfesoriRepository : IProfesoriRepository
    {
        private readonly ScanPointDbContext _context;

        public ProfesoriRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — të gjithë nxënësit me emrin e universitetit
        public async Task<List<Profesori>> GetAllAsync()
        {
            return await _context.Profesoret
                .Include(n => n.Universiteti) // JOIN me tabelën Universitetit
                .ToListAsync();
        }

        // GET BY Universiteti — filtro nxënësit sipas universitetit
        // Kjo përdoret për filterin e listës së nxënësve
        public async Task<List<Profesori>> GetByUniversitetAsync(int universitetiId)
        {
            return await _context.Profesoret
                .Where(n => n.ID_Universiteti == universitetiId) // Filtro me WHERE
                .Include(n => n.Universiteti)
                .ToListAsync();
        }

        // GET BY ID — nxënësi me ID specifik
        public async Task<Profesori?> GetByIdAsync(int id)
        {
            return await _context.Profesoret
                .Include(n => n.Universiteti)
                .FirstOrDefaultAsync(n => n.ID == id);
        }

        // CREATE — shto nxënës të ri
        public async Task<Profesori> CreateAsync(Profesori Profesori)
        {
            _context.Profesoret.Add(Profesori);
            await _context.SaveChangesAsync();
            return Profesori;
        }

        // UPDATE — përditëso të dhënat e nxënësit
        public async Task UpdateAsync(Profesori Profesori)
        {
            _context.Profesoret.Update(Profesori);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi nxënësin
        public async Task DeleteAsync(int id)
        {
            var Profesori = await _context.Profesoret.FindAsync(id);
            if (Profesori == null)
                throw new KeyNotFoundException($"Nxënësi me ID {id} nuk u gjet.");

            _context.Profesoret.Remove(Profesori);
            await _context.SaveChangesAsync();
        }
    }
}
