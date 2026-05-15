using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    // [ApiController] — e bën këtë klasë controller të API-t
    // [Route("api/[controller]")] — URL bëhet: /api/Shkolla
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        // Repository na jep qasjen te databaza
        private readonly IEmployeeRepository _employeeRepo;

        // Dependency Injection — ASP.NET Core na jep repository automatikisht
        public EmployeeController(IEmployeeRepository employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        // ===========================
        // GET /api/Employee
        // Merr listën e të gjitha employee
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeRepo.
                GetAllAsync();

            // Mapim: Employee → EmployeeReadDto (nuk i kthejmë të gjitha fushat)
            var result = employees.Select(e => new EmployeeReadDto
            {
                ID_Employee = e.ID_Employee,
                EmriEmployee = e.EmriEmployee   ,
                MbiemriEmployee = e.MbiemriEmployee
            });

            return Ok(result); // 200 OK + JSON
        }

        // ===========================
        // GET /api/Shkolla/{id}
        // Merr një shkollë specifike
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeRepo.GetByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee nuk u gjet." }); // 404

            return Ok(new EmployeeReadDto
            {
                ID_Employee = employee.ID_Employee,
                EmriEmployee = employee.EmriEmployee,
                MbiemriEmployee = employee.MbiemriEmployee
            });
        }

        // ===========================
        // POST /api/Employee
        // Shto shkollë të re
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeCreateDto dto)
        {
            // ModelState.IsValid — kontrollon Data Annotations nga DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 nëse të dhënat janë të gabuara

            // Krijojmë entitetin nga DTO
            var employee = new Employee
            {
                EmriEmployee = dto.EmriEmployee,
                MbiemriEmployee = dto.MbiemriEmployee
            };

            var created = await _employeeRepo.CreateAsync(employee);
            // 201 Created + URL ku mund ta gjesh entitetin e ri
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Employee },
                new EmployeeReadDto
                {
                    ID_Employee = created.ID_Employee,
                    EmriEmployee = created.EmriEmployee,
                    MbiemriEmployee = created.MbiemriEmployee
                });
        }

        // ===========================
        // PUT /api/Employee/{id}
        // Përditëso employee ekzistues
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = await _employeeRepo.GetByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = "Employee nuk u gjet." });

            // Përditëso fushat
            employee.EmriEmployee = dto.EmriEmployee;
            employee.MbiemriEmployee = dto.MbiemriEmployee;
            await _employeeRepo.UpdateAsync(employee);
            return Ok(new { message = "Employee u përditësua me sukses." }); // 200
        }

        // ===========================
        // DELETE /api/Employee/{id}
        // Fshi employee
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _employeeRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses, pa body
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
        }
    }
}
