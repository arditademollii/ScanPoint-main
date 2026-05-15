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
    public class LigjeruesiController : ControllerBase
    {
        // Repository na jep qasjen te databaza
        private readonly ILigjeruesiRepository _ligjeruesiRepo;

        // Dependency Injection — ASP.NET Core na jep repository automatikisht
        public LigjeruesiController(ILigjeruesiRepository ligjeruesiRepo)
        {
            _ligjeruesiRepo = ligjeruesiRepo;
        }

        // ===========================
        // GET /api/Ligjeruesi
        // Merr listën e të gjitha ligjeruesve
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ligjeruesit = await _ligjeruesiRepo.
                GetAllAsync();

            // Mapim: Ligjeruesi → LigjeruesiReadDto (nuk i kthejmë të gjitha fushat)
            var result = ligjeruesit.Select(l => new LigjeruesiReadDto
            {
                ID_Ligjeruesi = l.ID_Ligjeruesi,
                EmriLigjeruesit = l.EmriLigjeruesit,
               Departamenti = l.Departamenti,
               Email= l.Email   
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
            var ligjeruesi = await _ligjeruesiRepo.GetByIdAsync(id);
            if (ligjeruesi == null)
                return NotFound(new { message = "Ligjeruesi nuk u gjet." }); // 404

            return Ok(new LigjeruesiReadDto
            {
                ID_Ligjeruesi = ligjeruesi.ID_Ligjeruesi,
                EmriLigjeruesit = ligjeruesi.EmriLigjeruesit,
                Departamenti = ligjeruesi.Departamenti,
                Email = ligjeruesi.Email
            });
        }

        // ===========================
        // POST /api/Ligjeruesi
        // Shto ligjerues të ri
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LigjeruesiCreateDto dto)
        {
            // ModelState.IsValid — kontrollon Data Annotations nga DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 nëse të dhënat janë të gabuara

            // Krijojmë entitetin nga DTO
            var ligjeruesi = new Ligjeruesi
            {
                EmriLigjeruesit = dto.EmriLigjeruesit,
                Departamenti = dto.Departamenti,
                Email = dto.Email
            };

            var created = await _ligjeruesiRepo.CreateAsync(ligjeruesi);

            // 201 Created + URL ku mund ta gjesh entitetin e ri
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Ligjeruesi },
                new LigjeruesiReadDto
                {
                    ID_Ligjeruesi = created.ID_Ligjeruesi,
                    EmriLigjeruesit = created.EmriLigjeruesit,
                    Departamenti = created.Departamenti,
                    Email = created.Email
                });
        }

        // ===========================
        // PUT /api/Ligjeruesi/{id}
        // Përditëso ligjeruesin ekzistues
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LigjeruesiCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ligjeruesi = await _ligjeruesiRepo.GetByIdAsync(id);
            if (ligjeruesi == null)
                return NotFound(new { message = "Ligjeruesi nuk u gjet." });

            // Përditëso fushat
            ligjeruesi.EmriLigjeruesit = dto.EmriLigjeruesit;
            ligjeruesi.Departamenti = dto.Departamenti;
            ligjeruesi.Email = dto.Email;
            await _ligjeruesiRepo.UpdateAsync(ligjeruesi);
            return Ok(new { message = "Ligjeruesi u përditësua me sukses." }); // 200
        }

        // ===========================
        // DELETE /api/Ligjeruesi/{id}
        // Fshi ligjeruesin
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _ligjeruesiRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses, pa body
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
        }
    }
}
