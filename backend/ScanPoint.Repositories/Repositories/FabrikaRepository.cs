using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Repositories
{
    public class FabrikaRepository : IFabrikaRepository 
    {
        // _context është lidhja me databazën
        private readonly ScanPointDbContext _context;


        // Dependency Injection — ASP.NET na jep _context automatikisht
        public FabrikaRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // ------------------- GET ALL — merr të gjitha fabrikat me punëtorët e tyre
        public async Task<List<Fabrika>> GetAllAsync()
        {
            return await _context.Fabrikat
                .Include(f => f.Punetoret) // JOIN me tabelën e punëtorëve
                .ToListAsync();
        }

        // ------------------- GET BY ID — merr një fabrikë specifike
        public async Task<Fabrika?> GetByIdAsync(int id)
        {
            return await _context.Fabrikat
                .Include(f => f.Punetoret)
                .FirstOrDefaultAsync(f => f.ID_Fabrika == id);
        }

        // CREATE — shto fabrikë të re
        public async Task<Fabrika> CreateAsync(Fabrika fabrika)
        {
            _context.Fabrikat.Add(fabrika);
            await _context.SaveChangesAsync(); // Ruaj në databazë
            return fabrika;
        }

        // UPDATE — përditëso fabrikën ekzistuese
        public async Task UpdateAsync(Fabrika fabrika)
        {
            _context.Fabrikat.Update(fabrika);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi fabrikën (dhe punëtorët automatikisht nëpërmjet Cascade)
        public async Task DeleteAsync(int id)
        {
            var fabrika = await _context.Fabrikat.FindAsync(id);
            if (fabrika == null)
                throw new KeyNotFoundException($"Fabrika me ID {id} nuk u gjet.");

            _context.Fabrikat.Remove(fabrika);
            await _context.SaveChangesAsync();
        }
    }
}
