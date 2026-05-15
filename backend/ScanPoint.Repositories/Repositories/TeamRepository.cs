using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    // ShkollaRepository = implementimi i logjikës së bazës së të dhënave për Shkollat
    // Kjo klasë di TË VETËN punë: të flasë me databazën

    public class TeamRepository : ITeamRepository
    {
        // _context është lidhja me databazën
        private readonly ScanPointDbContext _context;


        // Dependency Injection — ASP.NET na jep _context automatikisht
        public TeamRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // ------------------- GET ALL — merr të gjitha team-et me nxënësit e tyre
        public async Task<List<Team232470351>> GetAllAsync()
        {
            return await _context.Teams232470351
                .Include(s => s.Players232470351) // JOIN me tabelën e nxënësve
                .ToListAsync();
        }

        // ------------------- GET BY ID — merr një team specifik
        public async Task<Team232470351?> GetByIdAsync(int id)
        {
            return await _context.Teams232470351
                .Include(s => s.Players232470351)
                .FirstOrDefaultAsync(s => s.ID_Team232470351 == id);
        }

        // CREATE — shto team të ri
        public async Task<Team232470351> CreateAsync(Team232470351 team)
        {
            _context.Teams232470351.Add(team);
            await _context.SaveChangesAsync(); // Ruaj në databazë
            return team;
        }

        // UPDATE — përditëso team ekzistues
        public async Task UpdateAsync(Team232470351 team)
        {
            _context.Teams232470351.Update(team);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi team (dhe nxënësit automatikisht nëpërmjet Cascade)
        public async Task DeleteAsync(int id)
        {
            var team = await _context.Teams232470351.FindAsync(id);
            if (team == null)
                throw new KeyNotFoundException($"Team232470351 me ID {id} nuk u gjet.");

            _context.Teams232470351.Remove(team);
            await _context.SaveChangesAsync();
        }
    }
}
