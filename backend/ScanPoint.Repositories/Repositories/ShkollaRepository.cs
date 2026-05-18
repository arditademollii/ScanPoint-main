using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
   

    public class ShkollaRepository : IShkollaRepository
    {
      
        private readonly ScanPointDbContext _context;
       
        public ShkollaRepository(ScanPointDbContext context)
        {
            _context = context;
        }

      
        public async Task<List<Shkolla>> GetAllAsync()
        {
            return await _context.Shkollat
                .Include(s => s.Nxenesit)
                .ToListAsync();
        }

       
        public async Task<Shkolla?> GetByIdAsync(int id)
        {
            return await _context.Shkollat
                .Include(s => s.Nxenesit)
                .FirstOrDefaultAsync(s => s.ID_Shkolla == id);
        }

       
        public async Task<Shkolla> CreateAsync(Shkolla shkolla)
        {
            _context.Shkollat.Add(shkolla);
            await _context.SaveChangesAsync(); 
            return shkolla;
        }

      
        public async Task UpdateAsync(Shkolla shkolla)
        {
            _context.Shkollat.Update(shkolla);
            await _context.SaveChangesAsync();
        }

       
        public async Task DeleteAsync(int id)
        {
            var shkolla = await _context.Shkollat.FindAsync(id);
            if (shkolla == null)
                throw new KeyNotFoundException($"Shkolla me ID {id} nuk u gjet.");

            _context.Shkollat.Remove(shkolla);
            await _context.SaveChangesAsync();
        }
    }
}
