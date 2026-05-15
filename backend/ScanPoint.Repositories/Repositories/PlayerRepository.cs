using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ScanPointDbContext _context;

        public PlayerRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — të gjithë nxënësit me emrin e shkollës
        public async Task<List<Player232470351>> GetAllAsync()
        {
            return await _context.Players232470351
                .Include(n => n.Team232470351) // JOIN me tabelën Shkollat
                .ToListAsync();
        }

        // GET BY SHKOLLA — filtro nxënësit sipas shkollës
        // Kjo përdoret për filterin e listës së nxënësve
        public async Task<List<Player232470351>> GetByTeam232470351Async(int Team232470351Id)
        {
            return await _context.Players232470351
                .Where(n => n.ID_Team232470351 == Team232470351Id) // Filtro me WHERE
                .Include(n => n.Team232470351)
                .ToListAsync();
        }

        // GET BY ID — nxënësi me ID specifik
        public async Task<Player232470351?> GetByIdAsync(int id)
        {
            return await _context.Players232470351
                .Include(n => n.Team232470351)
                .FirstOrDefaultAsync(n => n.ID == id);
        }

        // CREATE — shto nxënës të ri
        public async Task<Player232470351> CreateAsync(Player232470351 player)
        {
            _context.Players232470351.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        // UPDATE — përditëso të dhënat e nxënësit
        public async Task UpdateAsync(Player232470351 player)
        {
            _context.Players232470351.Update(player);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi nxënësin
        public async Task DeleteAsync(int id)
        {
            var player = await _context.Players232470351.FindAsync(id);
            if (player == null)
                throw new KeyNotFoundException($"Nxënësi me ID {id} nuk u gjet.");

            _context.Players232470351.Remove(player);
            await _context.SaveChangesAsync();
        }

       
    }
}
