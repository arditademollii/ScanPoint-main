using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    // Repository për Satelite
    public class SateliteRepository : ISateliteRepository
    {
        private readonly ScanPointDbContext _context;

        public SateliteRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — të gjithë satelitët
        public async Task<List<Satelite>> GetAllAsync()
        {
            return await _context.Satelitet
                .Include(s => s.Planet)
                .Where(s =>
                    !s.IsDeleted &&
                    !s.Planet.IsDeleted)
                .ToListAsync();
        }

        // GET BY PLANET — satelitët e një planeti
        public async Task<List<Satelite>> GetByPlanetAsync(int planetId)
        {
            return await _context.Satelitet
                .Include(s => s.Planet)
                .Where(s =>
                    s.ID_Planet == planetId &&
                    !s.IsDeleted &&
                    !s.Planet.IsDeleted)
                .ToListAsync();
        }

        // GET BY ID
        public async Task<Satelite?> GetByIdAsync(int id)
        {
            return await _context.Satelitet
                .Include(s => s.Planet)
                .FirstOrDefaultAsync(s =>
                    s.ID == id &&
                    !s.IsDeleted &&
                    !s.Planet.IsDeleted);
        }

        // CREATE
        public async Task<Satelite> CreateAsync(Satelite satelite)
        {
            _context.Satelitet.Add(satelite);

            await _context.SaveChangesAsync();

            return satelite;
        }

        // UPDATE
        public async Task UpdateAsync(Satelite satelite)
        {
            _context.Satelitet.Update(satelite);

            await _context.SaveChangesAsync();
        }

        // DELETE — Soft Delete
        public async Task DeleteAsync(int id)
        {
            var satelite = await _context.Satelitet
                .FirstOrDefaultAsync(s =>
                    s.ID == id &&
                    !s.IsDeleted);

            if (satelite == null)
                throw new KeyNotFoundException(
                    $"Sateliti me ID {id} nuk u gjet."
                );

            // Soft Delete
            satelite.IsDeleted = true;

            await _context.SaveChangesAsync();
        }
    }
}