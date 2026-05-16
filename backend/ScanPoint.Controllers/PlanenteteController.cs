using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanenteteController : ControllerBase
    {
        // Repository për databazë
        private readonly IPlanetRepository _planetRepo;

        // Dependency Injection
        public PlanenteteController(IPlanetRepository planetRepo)
        {
            _planetRepo = planetRepo;
        }

        // ===========================
        // GET /api/Planentete
        // Merr të gjithë planetët
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var planetet = await _planetRepo.GetAllAsync();

            var result = planetet.Select(p => new PlanetReadDto
            {
                ID_Planet = p.ID_Planet,
                EmriPlanetit = p.EmriPlanetit,
                Type = p.Type,
                IsDeleted = p.IsDeleted
            });

            return Ok(result);
        }

        // ===========================
        // GET /api/Planentete/{id}
        // Merr planetin sipas ID
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var planet = await _planetRepo.GetByIdAsync(id);

            if (planet == null)
                return NotFound(new
                {
                    message = "Planeti nuk u gjet."
                });

            return Ok(new PlanetReadDto
            {
                ID_Planet = planet.ID_Planet,
                EmriPlanetit = planet.EmriPlanetit,
                Type = planet.Type,
                IsDeleted = planet.IsDeleted
            });
        }

        // ===========================
        // POST /api/Planentete
        // Krijo planet të ri
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] PlanetCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var planet = new Planet
            {
                EmriPlanetit = dto.EmriPlanetit,
                Type = dto.Type
            };

            var created =
                await _planetRepo.CreateAsync(planet);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.ID_Planet },

                new PlanetReadDto
                {
                    ID_Planet = created.ID_Planet,
                    EmriPlanetit = created.EmriPlanetit,
                    Type = created.Type,
                    IsDeleted = created.IsDeleted
                });
        }

        // ===========================
        // PUT /api/Planentete/{id}
        // Update planet
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] PlanetCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var planet =
                await _planetRepo.GetByIdAsync(id);

            if (planet == null)
                return NotFound(new
                {
                    message = "Planeti nuk u gjet."
                });

            // Përditëso fushat
            planet.EmriPlanetit =
                dto.EmriPlanetit;

            planet.Type =
                dto.Type;

            await _planetRepo.UpdateAsync(planet);

            return Ok(new
            {
                message =
                "Planeti u përditësua me sukses."
            });
        }

        // ===========================
        // DELETE /api/Planentete/{id}
        // Soft Delete
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _planetRepo.DeleteAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}