using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    // [ApiController] — e bën këtë klasë controller të API-t
    // [Route("api/[controller]")] — URL bëhet: /api/Universiteti
    [ApiController]
    [Route("api/[controller]")]
    public class UniversitetiController : ControllerBase
    {
        // Repository na jep qasjen te databaza
        private readonly IUniversitetiRepository _UniversitetiRepo;

        // Dependency Injection — ASP.NET Core na jep repository automatikisht
        public UniversitetiController(IUniversitetiRepository UniversitetiRepo)
        {
            _UniversitetiRepo = UniversitetiRepo;
        }

        // ===========================
        // GET /api/Universiteti
        // Merr listën e të gjitha Universitetive
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Universitetit = await _UniversitetiRepo.
                GetAllAsync();

            // Mapim: Universiteti → UniversitetiReadDto (nuk i kthejmë të gjitha fushat)
            var result = Universitetit.Select(s => new UniversitetiReadDto
            {
                ID_Universiteti = s.ID_Universiteti,
                EmriUniversitetit = s.EmriUniversitetit,
                Shteti = s.Shteti
            });

            return Ok(result); // 200 OK + JSON
        }

        // ===========================
        // GET /api/Universiteti/{id}
        // Merr një shkollë specifike
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Universiteti = await _UniversitetiRepo.GetByIdAsync(id);
            if (Universiteti == null)
                return NotFound(new { message = "Universiteti nuk u gjet." }); // 404

            return Ok(new UniversitetiReadDto
            {
                ID_Universiteti = Universiteti.ID_Universiteti,
                EmriUniversitetit = Universiteti.EmriUniversitetit,
                Shteti = Universiteti.Shteti
            });
        }

        // ===========================
        // POST /api/Universiteti
        // Shto shkollë të re
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UniversitetiCreateDto dto)
        {
            // ModelState.IsValid — kontrollon Data Annotations nga DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 nëse të dhënat janë të gabuara

            // Krijojmë entitetin nga DTO
            var Universiteti = new Universiteti
            {
                EmriUniversitetit = dto.EmriUniversitetit,
                Shteti = dto.Shteti
            };

            var created = await _UniversitetiRepo.CreateAsync(Universiteti);

            // 201 Created + URL ku mund ta gjesh entitetin e ri
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Universiteti },
                new UniversitetiReadDto
                {
                    ID_Universiteti = created.ID_Universiteti,
                    EmriUniversitetit = created.EmriUniversitetit,
                    Shteti = created.Shteti
                });
        }

        // ===========================
        // PUT /api/Universiteti/{id}
        // Përditëso shkollën ekzistuese
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UniversitetiCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Universiteti = await _UniversitetiRepo.GetByIdAsync(id);
            if (Universiteti == null)
                return NotFound(new { message = "Universiteti nuk u gjet." });

            // Përditëso fushat
            Universiteti.EmriUniversitetit = dto.EmriUniversitetit;
            Universiteti.Shteti = dto.Shteti;

            await _UniversitetiRepo.UpdateAsync(Universiteti);
            return Ok(new { message = "Universiteti u përditësua me sukses." }); // 200
        }

        // ===========================
        // DELETE /api/Universiteti/{id}
        // Fshi shkollën
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _UniversitetiRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses, pa body
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
        }
    }
}
