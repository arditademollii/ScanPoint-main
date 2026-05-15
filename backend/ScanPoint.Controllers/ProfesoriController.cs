using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfesoriController : ControllerBase
    {
        private readonly IProfesoriRepository _ProfesoriRepo;
        private readonly IUniversitetiRepository _UniversitetiRepo; // Na duhet për të kontrolluar shkollën

        public ProfesoriController(IProfesoriRepository ProfesoriRepo, IUniversitetiRepository UniversitetiRepo)
        {
            _ProfesoriRepo = ProfesoriRepo;
            _UniversitetiRepo = UniversitetiRepo;
        }

        // Helper — mapim Profesori → ProfesoriReadDto
        private static ProfesoriReadDto MapToDto(Profesori n) => new ProfesoriReadDto
        {
            ID = n.ID,
            EmriProfesorit = n.EmriProfesorit,
            Lenda = n.Lenda,
            ID_Universiteti = n.ID_Universiteti,
            EmriUniversitetit = n.Universiteti?.EmriUniversitetit ?? ""
        };

        // ===========================
        // GET /api/Profesori
        // Merr të gjithë nxënësit
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Profesorit = await _ProfesoriRepo.GetAllAsync();
            return Ok(Profesorit.Select(MapToDto));
        }

        // ===========================
        // GET /api/Profesori/byUniversiteti/{UniversitetiId}
        // Filtro nxënësit sipas shkollës — për dropdown filtrim
        // ===========================
        [HttpGet("byUniversiteti/{UniversitetiId}")]
        public async Task<IActionResult> GetByUniversiteti(int UniversitetiId)
        {
            var Profesorit = await _ProfesoriRepo.GetByUniversitetAsync(UniversitetiId);
            return Ok(Profesorit.Select(MapToDto));
        }

        // ===========================
        // GET /api/Profesori/{id}
        // Merr nxënësin me ID specifik
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Profesori = await _ProfesoriRepo.GetByIdAsync(id);
            if (Profesori == null)
                return NotFound(new { message = "Profesori nuk u gjet." });

            return Ok(MapToDto(Profesori));
        }

        // ===========================
        // POST /api/Profesori
        // Shto nxënës të ri
        // Gjatë krijimit, dropdown zgjedh shkollën ekzistuese (EmriUniversitetit)
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProfesoriCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kontrollo nëse Universiteti ekziston
            var Universiteti = await _UniversitetiRepo.GetByIdAsync(dto.ID_Universiteti);
            if (Universiteti == null)
                return BadRequest(new { message = "Universiteti e zgjedhur nuk ekziston." });

            var Profesori = new Profesori
            {
                EmriProfesorit = dto.EmriProfesorit,
                Lenda = dto.Lenda,
                ID_Universiteti = dto.ID_Universiteti
            };

            var created = await _ProfesoriRepo.CreateAsync(Profesori);

            // Ringarko nxënësin me shkollën për ta kthyer të plotë
            var withSchool = await _ProfesoriRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withSchool!));
        }

        // ===========================
        // PUT /api/Profesori/{id}
        // Përditëso të dhënat e nxënësit
        // Forma mbushet paraprakisht me të dhënat aktuale
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProfesoriUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Profesori = await _ProfesoriRepo.GetByIdAsync(id);
            if (Profesori == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            // Kontrollo nëse Universiteti e re ekziston
            var Universiteti = await _UniversitetiRepo.GetByIdAsync(dto.ID_Universiteti);
            if (Universiteti == null)
                return BadRequest(new { message = "Universiteti e zgjedhur nuk ekziston." });

            // Përditëso fushat
            Profesori.EmriProfesorit = dto.EmriProfesorit;
            Profesori.Lenda = dto.Lenda;
            Profesori.ID_Universiteti = dto.ID_Universiteti;

            await _ProfesoriRepo.UpdateAsync(Profesori);
            return Ok(new { message = "Profesori u përditësua me sukses." });
        }

        // ===========================
        // DELETE /api/Profesori/{id}
        // Fshi profesorins nga lista
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _ProfesoriRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
