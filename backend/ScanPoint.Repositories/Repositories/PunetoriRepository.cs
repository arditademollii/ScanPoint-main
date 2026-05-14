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
    public class PunetoriRepository : IPunetoriRepository               
    {       
        private readonly ScanPointDbContext _context;

        public PunetoriRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — të gjithë punëtorët me emrin e fabrikës        

        public async Task<List<Punetori>> GetAllAsync()
        {
            return await _context.Punetoret
                .Include(p => p.Fabrika) // JOIN me tabelën Fabrikat    
                .ToListAsync();
        }

        // GET BY FABRIKA — filtro punëtorët sipas fabrikës
        // Kjo përdoret për filterin e listës së punëtorëve
        public async Task<List<Punetori>> GetByFabrikaAsync(int fabrikaId)
        {
            return await _context.Punetoret
                .Where(p => p.ID_Fabrika == fabrikaId) // Filtro me WHERE
                .Include(p => p.Fabrika)
                .ToListAsync();
        }

        // GET BY ID — punëtori me ID specifik
        public async Task<Punetori?> GetByIdAsync(int id)
        {
            return await _context.Punetoret
                .Include(p => p.Fabrika)
                .FirstOrDefaultAsync(p => p.ID == id);
        }

        // CREATE — shto punëtor të ri
        public async Task<Punetori> CreateAsync(Punetori punetori)
        {
            _context.Punetoret.Add(punetori);
            await _context.SaveChangesAsync();
            return punetori;
        }

        // UPDATE — përditëso të dhënat e punëtorit
        public async Task UpdateAsync(Punetori punetori)
        {
            _context.Punetoret.Update(punetori);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi punëtorin  
        public async Task DeleteAsync(int id)
        {
            var punetori = await _context.Punetoret.FindAsync(id);
            if (punetori == null)
                throw new KeyNotFoundException($"Punëtori me ID {id} nuk u gjet.");

            _context.Punetoret.Remove(punetori);
            await _context.SaveChangesAsync();
        }
    }
}
