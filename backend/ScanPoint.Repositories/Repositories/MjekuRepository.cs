using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    public class MjekuRepository : IMjekuRepository
    {
        private readonly ScanPointDbContext _context;

        public MjekuRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — të gjithë nxënësit me emrin e shkollës
        public async Task<List<Mjeku>> GetAllAsync()
        {
            return await _context.Mjeket
                .Include(n => n.Spitali) // JOIN me tabelën Spitalit
                .ToListAsync();
        }

        // GET BY Spitali — filtro nxënësit sipas shkollës
        // Kjo përdoret për filterin e listës së nxënësve
        public async Task<List<Mjeku>> GetBySpitaliAsync(int SpitaliId)
        {
            return await _context.Mjeket
                .Where(n => n.ID_Spitali == SpitaliId) // Filtro me WHERE
                .Include(n => n.Spitali)
                .ToListAsync();
        }

        // GET BY ID — nxënësi me ID specifik
        public async Task<Mjeku?> GetByIdAsync(int id)
        {
            return await _context.Mjeket
                .Include(n => n.Spitali)
                .FirstOrDefaultAsync(n => n.ID == id);
        }

        // CREATE — shto nxënës të ri
        public async Task<Mjeku> CreateAsync(Mjeku Mjeku)
        {
            _context.Mjeket.Add(Mjeku);
            await _context.SaveChangesAsync();
            return Mjeku;
        }

        // UPDATE — përditëso të dhënat e nxënësit
        public async Task UpdateAsync(Mjeku Mjeku)
        {
            _context.Mjeket.Update(Mjeku);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi nxënësin
        public async Task DeleteAsync(int id)
        {
            var Mjeku = await _context.Mjeket.FindAsync(id);
            if (Mjeku == null)
                throw new KeyNotFoundException($"Nxënësi me ID {id} nuk u gjet.");

            _context.Mjeket.Remove(Mjeku);
            await _context.SaveChangesAsync();
        }
    }
}
