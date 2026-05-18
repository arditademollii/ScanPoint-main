using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
  
    public class PlanetRepository : IPlanetRepository
    {
        private readonly ScanPointDbContext _context;

        public PlanetRepository(ScanPointDbContext context)
        {
            _context = context;
        }

       
        public async Task<List<Planet>> GetAllAsync()
        {
            return await _context.Planetet
                .Include(p => p.Satelitet)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

      
        public async Task<Planet?> GetByIdAsync(int id)
        {
            return await _context.Planetet
                .Include(p => p.Satelitet)
                .FirstOrDefaultAsync(p =>
                    p.ID_Planet == id &&
                    !p.IsDeleted);
        }

     
        public async Task<Planet> CreateAsync(Planet planet)
        {
            _context.Planetet.Add(planet);

            await _context.SaveChangesAsync();

            return planet;
        }

      
        public async Task UpdateAsync(Planet planet)
        {
            _context.Planetet.Update(planet);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var planet = await _context.Planetet
                .FirstOrDefaultAsync(p =>
                    p.ID_Planet == id &&
                    !p.IsDeleted);

            if (planet == null)
                throw new KeyNotFoundException(
                    $"Planeti me ID {id} nuk u gjet."
                );

           
            planet.IsDeleted = true;

            await _context.SaveChangesAsync();
        }
    }
}