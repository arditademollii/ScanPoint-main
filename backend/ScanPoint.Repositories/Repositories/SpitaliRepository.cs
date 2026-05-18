using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
   

    public class SpitaliRepository : ISpitaliRepository
    {
      
        private readonly ScanPointDbContext _context;


        public SpitaliRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        
        public async Task<List<Spitali>> GetAllAsync()
        {
            return await _context.Spitalet
                .Include(s => s.Mjeket)   
                .ToListAsync();
        }

      
        public async Task<Spitali?> GetByIdAsync(int id)
        {
            return await _context.Spitalet
                .Include(s => s.Mjeket)
                .FirstOrDefaultAsync(s => s.ID_Spitali == id);
        }

      
        public async Task<Spitali> CreateAsync(Spitali Spitali)
        {
            _context.Spitalet.Add(Spitali);
            await _context.SaveChangesAsync(); 
            return Spitali;
        }

      
        public async Task UpdateAsync(Spitali Spitali)
        {
            _context.Spitalet.Update(Spitali);
            await _context.SaveChangesAsync();
        }

       
        public async Task DeleteAsync(int id)
        {
            var Spitali = await _context.Spitalet.FindAsync(id);
            if (Spitali == null)
                throw new KeyNotFoundException($"Spitali me ID {id} nuk u gjet.");

            _context.Spitalet.Remove(Spitali);
            await _context.SaveChangesAsync();
        }
    }
}
