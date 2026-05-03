using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NxenesiController : ControllerBase
    {
        private readonly INxenesiRepository _nxenesiRepo;
        private readonly IShkollaRepository _shkollaRepo; // Na duhet për të kontrolluar shkollën

        public NxenesiController(INxenesiRepository nxenesiRepo, IShkollaRepository shkollaRepo)
        {
            _nxenesiRepo = nxenesiRepo;
            _shkollaRepo = shkollaRepo;
        }

        // Helper — mapim Nxenesi → NxenesiReadDto
        private static NxenesiReadDto MapToDto(Nxenesi n) => new NxenesiReadDto
        {
            ID = n.ID,
            EmriNxenesit = n.EmriNxenesit,
            Klasa = n.Klasa,
            ID_Shkolla = n.ID_Shkolla,
            EmriShkolles = n.Shkolla?.EmriShkolles ?? ""
        };

        // ===========================
        // GET /api/Nxenesi
        // Merr të gjithë nxënësit
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var nxenesit = await _nxenesiRepo.GetAllAsync();
            return Ok(nxenesit.Select(MapToDto));
        }

        // ===========================
        // GET /api/Nxenesi/byShkolla/{shkollaId}
        // Filtro nxënësit sipas shkollës — për dropdown filtrim
        // ===========================
        [HttpGet("byShkolla/{shkollaId}")]
        public async Task<IActionResult> GetByShkolla(int shkollaId)
        {
            var nxenesit = await _nxenesiRepo.GetByShkollaAsync(shkollaId);
            return Ok(nxenesit.Select(MapToDto));
        }

        // ===========================
        // GET /api/Nxenesi/{id}
        // Merr nxënësin me ID specifik
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var nxenesi = await _nxenesiRepo.GetByIdAsync(id);
            if (nxenesi == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            return Ok(MapToDto(nxenesi));
        }

        // ===========================
        // POST /api/Nxenesi
        // Shto nxënës të ri
        // Gjatë krijimit, dropdown zgjedh shkollën ekzistuese (EmriShkolles)
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NxenesiCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kontrollo nëse shkolla ekziston
            var shkolla = await _shkollaRepo.GetByIdAsync(dto.ID_Shkolla);
            if (shkolla == null)
                return BadRequest(new { message = "Shkolla e zgjedhur nuk ekziston." });

            var nxenesi = new Nxenesi
            {
                EmriNxenesit = dto.EmriNxenesit,
                Klasa = dto.Klasa,
                ID_Shkolla = dto.ID_Shkolla
            };

            var created = await _nxenesiRepo.CreateAsync(nxenesi);

            // Ringarko nxënësin me shkollën për ta kthyer të plotë
            var withSchool = await _nxenesiRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withSchool!));
        }

        // ===========================
        // PUT /api/Nxenesi/{id}
        // Përditëso të dhënat e nxënësit
        // Forma mbushet paraprakisht me të dhënat aktuale
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] NxenesiUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nxenesi = await _nxenesiRepo.GetByIdAsync(id);
            if (nxenesi == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            // Kontrollo nëse shkolla e re ekziston
            var shkolla = await _shkollaRepo.GetByIdAsync(dto.ID_Shkolla);
            if (shkolla == null)
                return BadRequest(new { message = "Shkolla e zgjedhur nuk ekziston." });

            // Përditëso fushat
            nxenesi.EmriNxenesit = dto.EmriNxenesit;
            nxenesi.Klasa = dto.Klasa;
            nxenesi.ID_Shkolla = dto.ID_Shkolla;

            await _nxenesiRepo.UpdateAsync(nxenesi);
            return Ok(new { message = "Nxënësi u përditësua me sukses." });
        }

        // ===========================
        // DELETE /api/Nxenesi/{id}
        // Fshi nxënësin nga lista
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _nxenesiRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
