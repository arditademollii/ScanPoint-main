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

        
        public async Task<List<Mjeku>> GetAllAsync()
        {
            return await _context.Mjeket
                .Include(n => n.Spitali) 
                .ToListAsync();
        }

  
        public async Task<List<Mjeku>> GetBySpitaliAsync(int SpitaliId)
        {
            return await _context.Mjeket
                .Where(n => n.ID_Spitali == SpitaliId) 
                .Include(n => n.Spitali)
                .ToListAsync();
        }

        public async Task<Mjeku?> GetByIdAsync(int id)
        {
            return await _context.Mjeket
                .Include(n => n.Spitali)
                .FirstOrDefaultAsync(n => n.ID == id);
        }

        
        public async Task<Mjeku> CreateAsync(Mjeku Mjeku)
        {
            _context.Mjeket.Add(Mjeku);
            await _context.SaveChangesAsync();
            return Mjeku;
        }

        
        public async Task UpdateAsync(Mjeku Mjeku)
        {
            _context.Mjeket.Update(Mjeku);
            await _context.SaveChangesAsync();
        }

       
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
