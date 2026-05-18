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
      
        private readonly IPlanetRepository _planetRepo;

    
        public PlanenteteController(IPlanetRepository planetRepo)
        {
            _planetRepo = planetRepo;
        }

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

           
            planet.EmriPlanetit =dto.EmriPlanetit;

            planet.Type = dto.Type;

            await _planetRepo.UpdateAsync(planet);

            return Ok(new
            {
                message =
                "Planeti u përditësua me sukses."
            });
        }

    
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