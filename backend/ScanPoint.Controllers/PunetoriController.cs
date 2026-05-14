using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PunetoriController : ControllerBase
    {
        private readonly IPunetoriRepository _punetoriRepo;
        private readonly IFabrikaRepository _fabrikaRepo; // Na duhet për të kontrolluar shkollën

        public PunetoriController(IPunetoriRepository punetoriRepo, IFabrikaRepository fabrikaRepo)
        {
            _punetoriRepo = punetoriRepo;
            _fabrikaRepo = fabrikaRepo;
        }

        // Helper — mapim Punetori → PunetoriReadDto
        private static PunetoriReadDto MapToDto(Punetori p) => new PunetoriReadDto
        {
            ID = p.ID,
            EmriPunetorit = p.EmriPunetorit,
            MbiemriPunetorit= p.MbiemriPunetorit,
            Pozita = p.Pozita,
            ID_Fabrika = p.ID_Fabrika,
            EmriFabrikes = p.Fabrika?.EmriFabrikes ?? ""
        };

        // ===========================
        // GET /api/Punetori
        // Merr të gjithë punëtorët
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var punetoret = await _punetoriRepo.GetAllAsync();
            return Ok(punetoret.Select(MapToDto));
        }

        // ===========================
        // GET /api/Punetori/byFabrika/{fabrikaId }
        // Filtro nxënësit sipas shkollës — për dropdown filtrim
        // ===========================
        [HttpGet("byFabrika/{fabrikaId}")]
        public async Task<IActionResult> GetByFabrika(int fabrikaId)
        {
            var punetoret = await _punetoriRepo.GetByFabrikaAsync(fabrikaId);
            return Ok(punetoret.Select(MapToDto));
        }

        // ===========================
        // GET /api/Nxenesi/{id}
        // Merr nxënësin me ID specifik
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var punetori = await _punetoriRepo.GetByIdAsync(id);
            if (punetori == null)
                return NotFound(new { message = "Punëtori  nuk u gjet." });

            return Ok(MapToDto(punetori));
        }

        // ===========================
        // POST /api/Punetori
        // Shto punëtor të ri
        // Gjatë krijimit, dropdown zgjedh fabrikën ekzistuese (EmriFabrikes)
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PunetoriCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kontrollo nëse fabrika ekziston
            var fabrika = await _fabrikaRepo.GetByIdAsync(dto.ID_Fabrika);
            if (fabrika == null)
                return BadRequest(new { message = "Fabrika e zgjedhur nuk ekziston." });

            var punetori = new Punetori
            {
                EmriPunetorit = dto.EmriPunetorit,
                MbiemriPunetorit = dto.MbiemriPunetorit,
                Pozita = dto.Pozita,
                ID_Fabrika = dto.ID_Fabrika
            };

            var created = await _punetoriRepo.CreateAsync(punetori);
            // Ringarko punëtori me fabrikën për ta kthyer të plotë
            var withFabrika = await _punetoriRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withFabrika!));
        }

        // ===========================
        // PUT /api/Punetori/{id}
        // Përditëso të dhënat e punëtorit
        // Forma mbushet paraprakisht me të dhënat aktuale
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PunetoriUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var punetori = await _punetoriRepo.GetByIdAsync(id);
            if (punetori == null)
                return NotFound(new { message = "Punëtori nuk u gjet." });

            // Kontrollo nëse fabrika e re ekziston
            var fabrika = await _fabrikaRepo.GetByIdAsync(dto.ID_Fabrika);
            if (fabrika == null)
                return BadRequest(new { message = "Fabrika e zgjedhur nuk ekziston." });

            // Përditëso fushat
            punetori.EmriPunetorit = dto.EmriPunetorit;
            punetori.MbiemriPunetorit = dto.MbiemriPunetorit; 
            punetori.Pozita = dto.Pozita;
            punetori.ID_Fabrika = dto.ID_Fabrika;

            await _punetoriRepo.UpdateAsync(punetori);
            return Ok(new { message = "Punëtori u përditësua me sukses." });
        }

        // ===========================
        // DELETE /api/Punetori/{id}
        // Fshi punëtorin nga lista
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _punetoriRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
