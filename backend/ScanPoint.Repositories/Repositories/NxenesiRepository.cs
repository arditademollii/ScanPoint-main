using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    public class NxenesiRepository : INxenesiRepository
    {
        private readonly ScanPointDbContext _context;

        public NxenesiRepository(ScanPointDbContext context)
        {
            _context = context;
        }

      
        public async Task<List<Nxenesi>> GetAllAsync()
        {
            return await _context.Nxenesit
                .Include(n => n.Shkolla) 
                .ToListAsync();
        }

      
     
        public async Task<List<Nxenesi>> GetByShkollaAsync(int shkollaId)
        {
            return await _context.Nxenesit
                .Where(n => n.ID_Shkolla == shkollaId) 
                .Include(n => n.Shkolla)
                .ToListAsync();
        }

       
        public async Task<Nxenesi?> GetByIdAsync(int id)
        {
            return await _context.Nxenesit
                .Include(n => n.Shkolla)
                .FirstOrDefaultAsync(n => n.ID == id);
        }

        public async Task<Nxenesi> CreateAsync(Nxenesi nxenesi)
        {
            _context.Nxenesit.Add(nxenesi);
            await _context.SaveChangesAsync();
            return nxenesi;
        }

       
        public async Task UpdateAsync(Nxenesi nxenesi)
        {
            _context.Nxenesit.Update(nxenesi);
            await _context.SaveChangesAsync();
        }

      
        public async Task DeleteAsync(int id)
        {
            var nxenesi = await _context.Nxenesit.FindAsync(id);
            if (nxenesi == null)
                throw new KeyNotFoundException($"Nxënësi me ID {id} nuk u gjet.");

            _context.Nxenesit.Remove(nxenesi);
            await _context.SaveChangesAsync();
        }
    }
}
