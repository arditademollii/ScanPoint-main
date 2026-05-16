using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    // [ApiController] — e bën këtë klasë controller të API-t
    // [Route("api/[controller]")] — URL bëhet: /api/Spitali
    [ApiController]
    [Route("api/[controller]")]
    public class SpitaliController : ControllerBase
    {
        // Repository na jep qasjen te databaza
        private readonly ISpitaliRepository _SpitaliRepo;

        // Dependency Injection — ASP.NET Core na jep repository automatikisht
        public SpitaliController(ISpitaliRepository SpitaliRepo)
        {
            _SpitaliRepo = SpitaliRepo;
        }

        // ===========================
        // GET /api/Spitali
        // Merr listën e të gjitha Spitalive
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Spitalit = await _SpitaliRepo.
                GetAllAsync();

            // Mapim: Spitali → SpitaliReadDto (nuk i kthejmë të gjitha fushat)
            var result = Spitalit.Select(s => new SpitaliReadDto
            {
                ID_Spitali = s.ID_Spitali,
                EmriSpitalit = s.EmriSpitalit,
                NumriKateve = s.NumriKateve,
                KaUrgjence = s.KaUrgjence,
                DataHapjes = s.DataHapjes   
            });

            return Ok(result); // 200 OK + JSON
        }

        // ===========================
        // GET /api/Spitali/{id}
        // Merr një shkollë specifike
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Spitali = await _SpitaliRepo.GetByIdAsync(id);
            if (Spitali == null)
                return NotFound(new { message = "Spitali nuk u gjet." }); // 404

            return Ok(new SpitaliReadDto
            {
                ID_Spitali = Spitali.ID_Spitali,
                EmriSpitalit = Spitali.EmriSpitalit,
                NumriKateve = Spitali.NumriKateve,
                KaUrgjence = Spitali.KaUrgjence,
                DataHapjes = Spitali.DataHapjes
            });
        }

        // ===========================
        // POST /api/Spitali
        // Shto shkollë të re
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpitaliCreateDto dto)
        {
            // ModelState.IsValid — kontrollon Data Annotations nga DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 nëse të dhënat janë të gabuara

            // Krijojmë entitetin nga DTO
            var Spitali = new Spitali
            {
                EmriSpitalit = dto.EmriSpitalit,
                NumriKateve = dto.NumriKateve,
                KaUrgjence = dto.KaUrgjence,
                DataHapjes = dto.DataHapjes
            };

            var created = await _SpitaliRepo.CreateAsync(Spitali);

            // 201 Created + URL ku mund ta gjesh entitetin e ri
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Spitali },
                new SpitaliReadDto
                {
                    ID_Spitali = created.ID_Spitali,
                    EmriSpitalit = created.EmriSpitalit,
                    NumriKateve = dto.NumriKateve,
                    KaUrgjence = dto.KaUrgjence,
                    DataHapjes = dto.DataHapjes
                });
        }

        // ===========================
        // PUT /api/Spitali/{id}
        // Përditëso shkollën ekzistuese
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SpitaliCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Spitali = await _SpitaliRepo.GetByIdAsync(id);
            if (Spitali == null)
                return NotFound(new { message = "Spitali nuk u gjet." });

            // Përditëso fushat
            Spitali.EmriSpitalit = dto.EmriSpitalit;
            Spitali.NumriKateve = dto.NumriKateve;
            Spitali.KaUrgjence = dto.KaUrgjence;
            Spitali.DataHapjes = dto.DataHapjes;
            

            await _SpitaliRepo.UpdateAsync(Spitali);
            return Ok(new { message = "Spitali u përditësua me sukses." }); // 200
        }

        // ===========================
        // DELETE /api/Spitali/{id}
        // Fshi shkollën
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _SpitaliRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses, pa body
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
        }
    }
}
