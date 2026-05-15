using Microsoft.EntityFrameworkCore;

using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;

namespace ScanPoint.Repositories.Repositories
{
    // ShkollaRepository = implementimi i logjikës së bazës së të dhënave për Shkollat
    // Kjo klasë di TË VETËN punë: të flasë me databazën

    public class EmployeeRepository : IEmployeeRepository   
    {
        // _context është lidhja me databazën
        private readonly ScanPointDbContext _context;


        // Dependency Injection — ASP.NET na jep _context automatikisht
        public EmployeeRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // ------------------- GET ALL — merr të gjitha punonjësit
        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.Contracts) // JOIN me tabelën e kontratave
                .ToListAsync();
        }

        // ------------------- GET BY ID — merr një punonjës specifik
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Contracts)
                .FirstOrDefaultAsync(e => e.ID_Employee == id);
        }

        // CREATE — shto punonjës të ri
        public async Task<Employee> CreateAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync(); // Ruaj në databazë
            return employee;
        }

        // UPDATE — përditëso punonjësin ekzistues
        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        // DELETE — fshi punonjësin (dhe kontratat automatikisht nëpërmjet Cascade)
        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"Punonjësi me ID {id} nuk u gjet.");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}
