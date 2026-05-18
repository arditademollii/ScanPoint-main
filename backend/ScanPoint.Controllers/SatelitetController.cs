using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SatelitetController : ControllerBase
    {
        private readonly ISateliteRepository _sateliteRepo;

       
        private readonly IPlanetRepository _planetRepo;

        public SatelitetController(
            ISateliteRepository sateliteRepo,
            IPlanetRepository planetRepo)
        {
            _sateliteRepo = sateliteRepo;
            _planetRepo = planetRepo;
        }

     
        private static SateliteReadDto MapToDto(
            Satelite s)
        {
            return new SateliteReadDto
            {
                ID = s.ID,
                EmriSatelitit = s.EmriSatelitit,
                IsDeleted = s.IsDeleted,
                ID_Planet = s.ID_Planet,

                EmriPlanetit =s.Planet?.EmriPlanetit ?? ""
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var satelitet =
                await _sateliteRepo.GetAllAsync();

            return Ok(
                satelitet.Select(MapToDto));
        }

        [HttpGet("byPlanet/{planetId}")]
        public async Task<IActionResult>
            GetByPlanet(int planetId)
        {
            var satelitet =
                await _sateliteRepo
                .GetByPlanetAsync(planetId);

            return Ok(
                satelitet.Select(MapToDto));
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult>
            GetById(int id)
        {
            var satelite =
                await _sateliteRepo.GetByIdAsync(id);

            if (satelite == null)
                return NotFound(new
                {
                    message =
                    "Sateliti nuk u gjet."
                });

            return Ok(MapToDto(satelite));
        }

     
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] SateliteCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            var planet =
                await _planetRepo
                .GetByIdAsync(dto.ID_Planet);

            if (planet == null)
                return BadRequest(new
                {
                    message =
                    "Planeti nuk ekziston."
                });

            var satelite = new Satelite
            {
                EmriSatelitit = dto.EmriSatelitit,

                ID_Planet = dto.ID_Planet
            };

            var created =
                await _sateliteRepo
                .CreateAsync(satelite);

            var withPlanet =
                await _sateliteRepo
                .GetByIdAsync(created.ID);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.ID },
                MapToDto(withPlanet!));
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] SateliteUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var satelite =
                await _sateliteRepo.GetByIdAsync(id);

            if (satelite == null)
                return NotFound(new
                {
                    message =
                    "Sateliti nuk u gjet."
                });

           
            var planet =
                await _planetRepo
                .GetByIdAsync(dto.ID_Planet);

            if (planet == null)
                return BadRequest(new
                {
                    message =
                    "Planeti nuk ekziston."
                });

         
            satelite.EmriSatelitit = dto.EmriSatelitit;

            satelite.ID_Planet = dto.ID_Planet;

            await _sateliteRepo
                .UpdateAsync(satelite);

            return Ok(new
            {
                message =
                "Sateliti u përditësua me sukses."
            });
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult>
            Delete(int id)
        {
            try
            {
                await _sateliteRepo
                    .DeleteAsync(id);

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