using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly IContractRepository _ContractRepo;
        private readonly IEmployeeRepository _EmployeeRepo; // Na duhet për të kontrolluar Employee

        public ContractController(IContractRepository ContractRepo, IEmployeeRepository EmployeeRepo)
        {
            _ContractRepo = ContractRepo;
            _EmployeeRepo = EmployeeRepo;
        }

        // Helper — mapim Contract → ContractReadDto
        private static ContractReadDto MapToDto(Contract n) => new ContractReadDto
        {
            ID = n.ID,
            Title = n.Title,
            Description = n.Description,
            ID_Employee = n.ID_Employee,
            EmriEmployee = n.Employee?.EmriEmployee ?? ""
        };

        // ===========================
        // GET /api/Contract
        // Merr të gjithë nxënësit
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Contractt = await _ContractRepo.GetAllAsync();
            return Ok(Contractt.Select(MapToDto));
        }

        // ===========================
        // GET /api/Contract/byEmployee/{EmployeeId}
        // Filtro nxënësit sipas shkollës — për dropdown filtrim
        // ===========================
        [HttpGet("byEmployee/{EmployeeId}")]
        public async Task<IActionResult> GetByEmployee(int EmployeeId)
        {
            var Contractt = await _ContractRepo.GetByEmployeeAsync(EmployeeId);
            return Ok(Contractt.Select(MapToDto));
        }

        // ===========================
        // GET /api/Contract/{id}
        // Merr nxënësin me ID specifik
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Contract = await _ContractRepo.GetByIdAsync(id);
            if (Contract == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            return Ok(MapToDto(Contract));
        }

        // ===========================
        // POST /api/Contract
        // Shto nxënës të ri
        // Gjatë krijimit, dropdown zgjedh shkollën ekzistuese (EmriShkolles)
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ContractCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kontrollo nëse Employee ekziston
            var Employee = await _EmployeeRepo.GetByIdAsync(dto.ID_Employee);
            if (Employee == null)
                return BadRequest(new { message = "Employee e zgjedhur nuk ekziston." });

            var Contract = new Contract
            {
                Title = dto.Title,
                Description = dto.Description,
                ID_Employee = dto.ID_Employee
            };

            var created = await _ContractRepo.CreateAsync(Contract);

            // Ringarko nxënësin me shkollën për ta kthyer të plotë
            var withSchool = await _ContractRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withSchool!));
        }

        // ===========================
        // PUT /api/Contract/{id}
        // Përditëso të dhënat e nxënësit
        // Forma mbushet paraprakisht me të dhënat aktuale
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ContractUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Contract = await _ContractRepo.GetByIdAsync(id);
            if (Contract == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            // Kontrollo nëse Employee e re ekziston
            var Employee = await _EmployeeRepo.GetByIdAsync(dto.ID_Employee);
            if (Employee == null)
                return BadRequest(new { message = "Employee e zgjedhur nuk ekziston." });

            // Përditëso fushat
            Contract.Title = dto.Title;
            Contract.Description = dto.Description;
            Contract.ID_Employee = dto.ID_Employee;

            await _ContractRepo.UpdateAsync(Contract);
            return Ok(new { message = "Contract u përditësua me sukses." });
        }

        // ===========================
        // DELETE /api/Contract/{id}
        // Fshi Contract     nga lista
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _ContractRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
