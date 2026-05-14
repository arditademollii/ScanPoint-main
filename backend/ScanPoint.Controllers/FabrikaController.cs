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
    public class FabrikaController : ControllerBase
    {
        // Repository na jep qasjen te databaza
        private readonly IFabrikaRepository _fabrikaRepo;

        // Dependency Injection — ASP.NET Core na jep repository automatikisht
        public FabrikaController(IFabrikaRepository fabrikaRepo)
        {
            _fabrikaRepo = fabrikaRepo;
        }

        // ===========================
        // GET /api/Fabrika
        // Merr listën e të gjitha fabrikave
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var fabrikat = await _fabrikaRepo.
                GetAllAsync();

            // Mapim: Fabrika → FabrikaReadDto (nuk i kthejmë të gjitha fushat)
            var result = fabrikat.Select(f => new FabrikaReadDto
            {
                ID_Fabrika = f.ID_Fabrika,
                EmriFabrikes = f.EmriFabrikes,
                Lokacioni = f.Lokacioni
            });

            return Ok(result); // 200 OK + JSON
        }

        // ===========================
        // GET /api/Fabrika/{id}
        // Merr një fabrikë specifike
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var fabrika = await _fabrikaRepo.GetByIdAsync(id);
            if (fabrika == null)
                return NotFound(new { message = "Fabrika nuk u gjet." }); // 404

            return Ok(new FabrikaReadDto
            {
                ID_Fabrika = fabrika.ID_Fabrika,
                EmriFabrikes = fabrika.EmriFabrikes,
                Lokacioni = fabrika.Lokacioni
            });
        }

        // ===========================
        // POST /api/Fabrika
        // Shto fabrikë të re
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FabrikaCreateDto dto)
        {
            // ModelState.IsValid — kontrollon Data Annotations nga DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 nëse të dhënat janë të gabuara

            // Krijojmë entitetin nga DTO
            var fabrika = new Fabrika
            {
                EmriFabrikes = dto.EmriFabrikes,
                Lokacioni = dto.Lokacioni   
            };

            var created = await _fabrikaRepo.CreateAsync(fabrika);
            // 201 Created + URL ku mund ta gjesh entitetin e ri
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Fabrika },
                new FabrikaReadDto
                {
                    ID_Fabrika = created.ID_Fabrika,
                    EmriFabrikes = created.EmriFabrikes,
                    Lokacioni = created.Lokacioni
                });
        }

        // ===========================
        // PUT /api/Shkolla/{id}
        // Përditëso shkollën ekzistuese
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FabrikaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fabrika = await _fabrikaRepo.GetByIdAsync(id);
            if (fabrika == null)
                return NotFound(new { message = "Fabrika nuk u gjet." });

            // Përditëso fushat
            fabrika.EmriFabrikes = dto.EmriFabrikes;
            fabrika.Lokacioni = dto.Lokacioni;

            await _fabrikaRepo.UpdateAsync(fabrika);
            return Ok(new { message = "Fabrika u përditësua me sukses." }); // 200
        }

        // ===========================
        // DELETE /api/Fabrika/{id}
        // Fshi fabrikën
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _fabrikaRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses, pa body
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
        }
    }
}
