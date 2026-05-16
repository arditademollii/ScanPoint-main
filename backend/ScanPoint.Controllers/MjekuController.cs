using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MjekuController : ControllerBase
    {
        private readonly IMjekuRepository _MjekuRepo;
        private readonly ISpitaliRepository _SpitaliRepo; // Na duhet për të kontrolluar shkollën

        public MjekuController(IMjekuRepository MjekuRepo, ISpitaliRepository SpitaliRepo)
        {
            _MjekuRepo = MjekuRepo;
            _SpitaliRepo = SpitaliRepo;
        }

        // Helper — mapim Mjeku → MjekuReadDto
        private static MjekuReadDto MapToDto(Mjeku n) => new MjekuReadDto
        {
            ID = n.ID,
            EmriMjekut = n.EmriMjekut,
            Paga = n.Paga,
            DataPunesimit = n.DataPunesimit,
            EshteSpecialist=n.EshteSpecialist,
            ID_Spitali = n.ID_Spitali,
            EmriSpitalit = n.Spitali?.EmriSpitalit ?? ""
        };

        // ===========================
        // GET /api/Mjeku
        // Merr të gjithë nxënësit
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Mjekut = await _MjekuRepo.GetAllAsync();
            return Ok(Mjekut.Select(MapToDto));
        }

        // ===========================
        // GET /api/Mjeku/bySpitali/{SpitaliId}
        // Filtro nxënësit sipas shkollës — për dropdown filtrim
        // ===========================
        [HttpGet("bySpitali/{SpitaliId}")]
        public async Task<IActionResult> GetBySpitali(int SpitaliId)
        {
            var Mjekut = await _MjekuRepo.GetBySpitaliAsync(SpitaliId);
            return Ok(Mjekut.Select(MapToDto));
        }

        // ===========================
        // GET /api/Mjeku/{id}
        // Merr nxënësin me ID specifik
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Mjeku = await _MjekuRepo.GetByIdAsync(id);
            if (Mjeku == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            return Ok(MapToDto(Mjeku));
        }

        // ===========================
        // POST /api/Mjeku
        // Shto nxënës të ri
        // Gjatë krijimit, dropdown zgjedh shkollën ekzistuese (EmriShkolles)
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MjekuCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kontrollo nëse Spitali ekziston
            var Spitali = await _SpitaliRepo.GetByIdAsync(dto.ID_Spitali);
            if (Spitali == null)
                return BadRequest(new { message = "Spitali e zgjedhur nuk ekziston." });

            var Mjeku = new Mjeku
            {
                EmriMjekut = dto.EmriMjekut,
                Paga = dto.Paga,
                DataPunesimit = dto.DataPunesimit,
                EshteSpecialist = dto.EshteSpecialist,
                ID_Spitali = dto.ID_Spitali
            };

            var created = await _MjekuRepo.CreateAsync(Mjeku);

            // Ringarko nxënësin me shkollën për ta kthyer të plotë
            var withSchool = await _MjekuRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withSchool!));
        }

        // ===========================
        // PUT /api/Mjeku/{id}
        // Përditëso të dhënat e nxënësit
        // Forma mbushet paraprakisht me të dhënat aktuale
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MjekuUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Mjeku = await _MjekuRepo.GetByIdAsync(id);
            if (Mjeku == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            // Kontrollo nëse Spitali e re ekziston
            var Spitali = await _SpitaliRepo.GetByIdAsync(dto.ID_Spitali);
            if (Spitali == null)
                return BadRequest(new { message = "Spitali e zgjedhur nuk ekziston." });

            // Përditëso fushat
            Mjeku.EmriMjekut = dto.EmriMjekut;
            Mjeku.Paga = dto.Paga;
            Mjeku.DataPunesimit = dto.DataPunesimit;
            Mjeku.EshteSpecialist = dto.EshteSpecialist;
            Mjeku.ID_Spitali = dto.ID_Spitali;

            await _MjekuRepo.UpdateAsync(Mjeku);
            return Ok(new { message = "Mjeku u përditësua me sukses." });
        }

        // ===========================
        // DELETE /api/Mjeku/{id}
        // Fshi mjekun nga lista
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _MjekuRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
