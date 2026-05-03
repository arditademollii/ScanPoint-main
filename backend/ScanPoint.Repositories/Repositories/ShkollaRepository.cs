using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    // ShkollaRepository = implementimi i logjikës së bazës së të dhënave për Shkollat
    // Kjo klasë di TË VETËN punë: të flasë me databazën

    public class ShkollaRepository : IShkollaRepository
    {
        // _context është lidhja me databazën
        private readonly ScanPointDbContext _context;
       

        // Dependency Injection — ASP.NET na jep _context automatikisht
        public ShkollaRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — merr të gjitha shkollat me nxënësit e tyre
        public async Task<List<Shkolla>> GetAllAsync()
        {
            return await _context.Shkollat
                .Include(s => s.Nxenesit) // JOIN me tabelën e nxënësve
                .ToListAsync();
        }

        // GET BY ID — merr një shkollë specifike
        public async Task<Shkolla?> GetByIdAsync(int id)
        {
            return await _context.Shkollat
                .Include(s => s.Nxenesit)
                .FirstOrDefaultAsync(s => s.ID_Shkolla == id);
        }

        // CREATE — shto shkollë të re
        public async Task<Shkolla> CreateAsync(Shkolla shkolla)
        {
            _context.Shkollat.Add(shkolla);
            await _context.SaveChangesAsync(); // Ruaj në databazë
            return shkolla;
        }

        // UPDATE — përditëso shkollën ekzistuese
        public async Task UpdateAsync(Shkolla shkolla)
        {
            _context.Shkollat.Update(shkolla);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi shkollën (dhe nxënësit automatikisht nëpërmjet Cascade)
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
