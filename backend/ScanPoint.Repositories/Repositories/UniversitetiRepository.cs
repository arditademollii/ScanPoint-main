using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    // ShkollaRepository = implementimi i logjikës së bazës së të dhënave për Shkollat
    // Kjo klasë di TË VETËN punë: të flasë me databazën

    public class UniversitetiRepository : IUniversitetiRepository
    {
        // _context është lidhja me databazën
        private readonly ScanPointDbContext _context;


        // Dependency Injection — ASP.NET na jep _context automatikisht
        public UniversitetiRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // ------------------- GET ALL — merr të gjitha universitetet me nxënësit e tyre
        public async Task<List<Universiteti>> GetAllAsync()
        {
            return await _context.Universitetet
                .Include(u => u.Profesoret) // JOIN me tabelën e nxënësve
                .ToListAsync();
        }

        // ------------------- GET BY ID — merr një universitet specifik
        public async Task<Universiteti?> GetByIdAsync(int id)
        {
            return await _context.Universitetet
                .Include(u => u.Profesoret)
                .FirstOrDefaultAsync(u => u.ID_Universiteti == id);
        }

        // CREATE — shto universitet të ri
        public async Task<Universiteti> CreateAsync(Universiteti universiteti)
        {
            _context.Universitetet.Add(universiteti);
            await _context.SaveChangesAsync(); // Ruaj në databazë
            return universiteti;
        }

        // UPDATE — përditëso universitetin ekzistues
        public async Task UpdateAsync(Universiteti universiteti)
        {
            _context.Universitetet.Update(universiteti);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi universitetin (dhe profesorët automatikisht nëpërmjet Cascade)
        public async Task DeleteAsync(int id)
        {
            var universiteti = await _context.Universitetet.FindAsync(id);
            if (universiteti == null)
                throw new KeyNotFoundException($"Universiteti me ID {id} nuk u gjet.");

            _context.Universitetet.Remove(universiteti);
            await _context.SaveChangesAsync();
        }
    }
}
