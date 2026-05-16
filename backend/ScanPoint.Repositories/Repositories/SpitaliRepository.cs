using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    // SpitaliRepository = implementimi i logjikës së bazës së të dhënave për Spitalet
    // Kjo klasë di TË VETËN punë: të flasë me databazën

    public class SpitaliRepository : ISpitaliRepository
    {
        // _context është lidhja me databazën
        private readonly ScanPointDbContext _context;


        // Dependency Injection — ASP.NET na jep _context automatikisht
        public SpitaliRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // ------------------- GET ALL — merr të gjitha Spitalit me nxënësit e tyre
        public async Task<List<Spitali>> GetAllAsync()
        {
            return await _context.Spitalet
                .Include(s => s.Mjeket) // JOIN me tabelën e mjekëve    
                .ToListAsync();
        }

        // ------------------- GET BY ID — merr një shkollë specifike
        public async Task<Spitali?> GetByIdAsync(int id)
        {
            return await _context.Spitalet
                .Include(s => s.Mjeket)
                .FirstOrDefaultAsync(s => s.ID_Spitali == id);
        }

        // CREATE — shto shkollë të re
        public async Task<Spitali> CreateAsync(Spitali Spitali)
        {
            _context.Spitalet.Add(Spitali);
            await _context.SaveChangesAsync(); // Ruaj në databazë
            return Spitali;
        }

        // UPDATE — përditëso shkollën ekzistuese
        public async Task UpdateAsync(Spitali Spitali)
        {
            _context.Spitalet.Update(Spitali);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi shkollën (dhe nxënësit automatikisht nëpërmjet Cascade)
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
