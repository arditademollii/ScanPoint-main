using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ScanPointDbContext _context;

        public ContractRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // GET ALL — të gjithë kontratat
        public async Task<List<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Employee) // JOIN me tabelën Employeet
                .ToListAsync();
        }

        // GET BY Employee — filtro kontratat sipas shkollës
        // Kjo përdoret për filterin e listës së nxënësve
        public async Task<List<Contract>> GetByEmployeeAsync(int EmployeeId)
        {
            return await _context.Contracts
                .Where(n => n.ID_Employee == EmployeeId) // Filtro me WHERE
                .Include(n => n.Employee)
                .ToListAsync();
        }

        // GET BY ID — nxënësi me ID specifik
        public async Task<Contract?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(n => n.Employee)
                .FirstOrDefaultAsync(n => n.ID == id);
        }

        // CREATE — shto nxënës të ri
        public async Task<Contract> CreateAsync(Contract Contract)
        {
            _context.Contracts.Add(Contract);
            await _context.SaveChangesAsync();
            return Contract;
        }

        // UPDATE — përditëso të dhënat e nxënësit
        public async Task UpdateAsync(Contract Contract)
        {
            _context.Contracts.Update(Contract);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi nxënësin
        public async Task DeleteAsync(int id)
        {
            var Contract = await _context.Contracts.FindAsync(id);
            if (Contract == null)
                throw new KeyNotFoundException($"Kontrata me ID {id} nuk u gjet.");

            _context.Contracts.Remove(Contract);
            await _context.SaveChangesAsync();
        }
    }
}
