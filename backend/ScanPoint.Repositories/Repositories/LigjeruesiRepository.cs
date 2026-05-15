using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    // ShkollaRepository = implementimi i logjikës së bazës së të dhënave për Shkollat
    // Kjo klasë di TË VETËN punë: të flasë me databazën

    public class LigjeruesiRepository : ILigjeruesiRepository   
    {
        // _context është lidhja me databazën
        private readonly ScanPointDbContext _context;


        // Dependency Injection — ASP.NET na jep _context automatikisht
        public LigjeruesiRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // ------------------- GET ALL — merr të gjitha ligjeruesit
        public async Task<List<Ligjeruesi>> GetAllAsync()
        {
            return await _context.Ligjeruesit
                .Include(l => l.Ligjeratat) // JOIN me tabelën e ligjeratave    
                .ToListAsync();
        }

        // ------------------- GET BY ID — merr një ligjerues specifik
        public async Task<Ligjeruesi?> GetByIdAsync(int id)
        {
            return await _context.Ligjeruesit
                .Include(l => l.Ligjeratat)
                .FirstOrDefaultAsync(l => l.ID_Ligjeruesi == id);
        }

        // CREATE — shto ligjerues të ri
        public async Task<Ligjeruesi> CreateAsync(Ligjeruesi ligjeruesi)
        {
            _context.Ligjeruesit.Add(ligjeruesi);
            await _context.SaveChangesAsync(); // Ruaj në databazë
            return ligjeruesi;
        }

        // UPDATE — përditëso ligjeruesin ekzistues
        public async Task UpdateAsync(Ligjeruesi ligjeruesi)
        {
            _context.Ligjeruesit.Update(ligjeruesi);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi ligjeruesin (dhe ligjeratat automatikisht nëpërmjet Cascade)
        public async Task DeleteAsync(int id)
        {
            var ligjeruesi = await _context.Ligjeruesit.FindAsync(id);
            if (ligjeruesi == null)
                throw new KeyNotFoundException($"Ligjeruesi me ID {id} nuk u gjet.");

            _context.Ligjeruesit.Remove(ligjeruesi);
            await _context.SaveChangesAsync();
        }
    }
}
