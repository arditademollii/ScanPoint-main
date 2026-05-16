using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    // Repository për Planet
    public class PlanetRepository : IPlanetRepository
    {
        private readonly ScanPointDbContext _context;

        // Dependency Injection
        public PlanetRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — merr të gjithë planetët që nuk janë deleted
        public async Task<List<Planet>> GetAllAsync()
        {
            return await _context.Planetet
                .Include(p => p.Satelitet)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        // GET BY ID — merr planetin sipas ID
        public async Task<Planet?> GetByIdAsync(int id)
        {
            return await _context.Planetet
                .Include(p => p.Satelitet)
                .FirstOrDefaultAsync(p =>
                    p.ID_Planet == id &&
                    !p.IsDeleted);
        }

        // CREATE — shto planet të ri
        public async Task<Planet> CreateAsync(Planet planet)
        {
            _context.Planetet.Add(planet);

            await _context.SaveChangesAsync();

            return planet;
        }

        // UPDATE — përditëso planetin
        public async Task UpdateAsync(Planet planet)
        {
            _context.Planetet.Update(planet);

            await _context.SaveChangesAsync();
        }

        // DELETE — Soft Delete
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

            // Soft Delete
            planet.IsDeleted = true;

            await _context.SaveChangesAsync();
        }
    }
}