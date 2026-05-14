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
    public class ShkollaController : ControllerBase
    {
        // Repository na jep qasjen te databaza
        private readonly IShkollaRepository _shkollaRepo;

        // Dependency Injection — ASP.NET Core na jep repository automatikisht
        public ShkollaController(IShkollaRepository shkollaRepo)
        {
            _shkollaRepo = shkollaRepo;
        }

        // ===========================
        // GET /api/Shkolla
        // Merr listën e të gjitha shkollave
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shkollat = await _shkollaRepo.
                GetAllAsync();

            // Mapim: Shkolla → ShkollaReadDto (nuk i kthejmë të gjitha fushat)
            var result = shkollat.Select(s => new ShkollaReadDto
            {
                ID_Shkolla = s.ID_Shkolla,
                EmriShkolles = s.EmriShkolles,
                Qyteti = s.Qyteti
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
            var shkolla = await _shkollaRepo.GetByIdAsync(id);
            if (shkolla == null)
                return NotFound(new { message = "Shkolla nuk u gjet." }); // 404

            return Ok(new ShkollaReadDto
            {
                ID_Shkolla = shkolla.ID_Shkolla,
                EmriShkolles = shkolla.EmriShkolles,
                Qyteti = shkolla.Qyteti
            });
        }

        // ===========================
        // POST /api/Shkolla
        // Shto shkollë të re
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShkollaCreateDto dto)
        {
            // ModelState.IsValid — kontrollon Data Annotations nga DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 nëse të dhënat janë të gabuara

            // Krijojmë entitetin nga DTO
            var shkolla = new Shkolla
            {
                EmriShkolles = dto.EmriShkolles,
                Qyteti = dto.Qyteti
            };

            var created = await _shkollaRepo.CreateAsync(shkolla);

            // 201 Created + URL ku mund ta gjesh entitetin e ri
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Shkolla },
                new ShkollaReadDto
                {
                    ID_Shkolla = created.ID_Shkolla,
                    EmriShkolles = created.EmriShkolles,
                    Qyteti = created.Qyteti
                });
        }

        // ===========================
        // PUT /api/Shkolla/{id}
        // Përditëso shkollën ekzistuese
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShkollaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var shkolla = await _shkollaRepo.GetByIdAsync(id);
            if (shkolla == null)
                return NotFound(new { message = "Shkolla nuk u gjet." });

            // Përditëso fushat
            shkolla.EmriShkolles = dto.EmriShkolles;
            shkolla.Qyteti = dto.Qyteti;

            await _shkollaRepo.UpdateAsync(shkolla);
            return Ok(new { message = "Shkolla u përditësua me sukses." }); // 200
        }

        // ===========================
        // DELETE /api/Shkolla/{id}
        // Fshi shkollën
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _shkollaRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses, pa body
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
        }
    }
}
