using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LigjerataController : ControllerBase
    {
        private readonly ILigjerataRepository _ligjerataRepo;
        private readonly ILigjeruesiRepository _ligjeruesiRepo; // Na duhet për të kontrolluar ligjeruesin

        public LigjerataController(ILigjerataRepository ligjerataRepo, ILigjeruesiRepository ligjeruesiRepo)
        {
            _ligjerataRepo = ligjerataRepo;
            _ligjeruesiRepo = ligjeruesiRepo;
        }

        // Helper — mapim Ligjerata → LigjerataReadDto
        private static LigjerataReadDto MapToDto(Ligjerata l) => new LigjerataReadDto
        {
            ID = l.ID,
            EmriLigjerates = l.EmriLigjerates,
            ID_Ligjeruesi = l.ID_Ligjeruesi,
            EmriLigjeruesit = l.Ligjeruesi?.EmriLigjeruesit ?? ""
        };

        // ===========================
        // GET /api/Ligjerata
        // Merr të gjitha ligjeratat
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ligjeratat = await _ligjerataRepo.GetAllAsync();
            return Ok(ligjeratat.Select(MapToDto));
        }

        // ===========================
        // GET /api/Ligjerata/byLigjeruesi/{ligjeruesiId}
        // Filtro ligjeratat sipas ligjeruesit — për dropdown filtrim
        // ===========================
        [HttpGet("byLigjeruesi/{ligjeruesiId}")]
        public async Task<IActionResult> GetByLigjeruesi(int ligjeruesiId)
        {
            var ligjeratat = await _ligjerataRepo.GetByLigjeruesiAsync(ligjeruesiId);
            return Ok(ligjeratat.Select(MapToDto));
        }

        // ===========================
        // GET /api/Ligjerata/{id}
        // Merr ligjeratën me ID specifik
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ligjerata = await _ligjerataRepo.GetByIdAsync(id);
            if (ligjerata == null)
                return NotFound(new { message = "Ligjerata nuk u gjet." });

            return Ok(MapToDto(ligjerata));
        }

        // ===========================
        // POST /api/Ligjerata
        // Shto ligjeratë të re
        // Gjatë krijimit, dropdown zgjedh ligjeruesin ekzistues (EmriLigjeruesit   )
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LigjerataCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kontrollo nëse ligjeruesi ekziston
            var ligjeruesi = await _ligjeruesiRepo.GetByIdAsync(dto.ID_Ligjeruesi);
            if (ligjeruesi == null)
                return BadRequest(new { message = "Ligjeruesi i  zgjedhur nuk ekziston." });

            var ligjerata = new Ligjerata
            {
                EmriLigjerates = dto.EmriLigjerates,
                ID_Ligjeruesi = dto.ID_Ligjeruesi
            };

            var created = await _ligjerataRepo.CreateAsync(ligjerata);

            // Ringarko ligjeratën me ligjeruesin për ta kthyer të plotë
            var withLecturer = await _ligjerataRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withLecturer!));
        }

        // ===========================
        // PUT /api/Ligjerata/{id}
        // Përditëso të dhënat e ligjeratës
        // Forma mbushet paraprakisht me të dhënat aktuale
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LigjerataUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ligjerata = await _ligjerataRepo.GetByIdAsync(id);
            if (ligjerata == null)
                return NotFound(new { message = "Ligjerata nuk u gjet." });

            // Kontrollo nëse ligjeruesi i ri ekziston
            var ligjeruesi = await _ligjeruesiRepo.GetByIdAsync(dto.ID_Ligjeruesi);
            if (ligjeruesi == null)
                return BadRequest(new { message = "Ligjeruesi i zgjedhur nuk ekziston." });

            // Përditëso fushat
            ligjerata.EmriLigjerates = dto.EmriLigjerates;
            ligjerata.ID_Ligjeruesi = dto.ID_Ligjeruesi;

            await _ligjerataRepo.UpdateAsync(ligjerata);
            return Ok(new { message = "Ligjerata u përditësua me sukses." });
        }

        // ===========================
        // DELETE /api/Ligjerata/{id}
        // Fshi ligjeratën nga lista
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _ligjerataRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
