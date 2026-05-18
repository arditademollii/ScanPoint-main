using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NxenesiController : ControllerBase
    {
        private readonly INxenesiRepository _nxenesiRepo;
        private readonly IShkollaRepository _shkollaRepo; 

        public NxenesiController(INxenesiRepository nxenesiRepo, IShkollaRepository shkollaRepo)
        {
            _nxenesiRepo = nxenesiRepo;
            _shkollaRepo = shkollaRepo;
        }

        private static NxenesiReadDto MapToDto(Nxenesi n) => new NxenesiReadDto
        {
            ID = n.ID,
            EmriNxenesit = n.EmriNxenesit,
            Klasa = n.Klasa,
            ID_Shkolla = n.ID_Shkolla,
            EmriShkolles = n.Shkolla?.EmriShkolles ?? ""
        };

     
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var nxenesit = await _nxenesiRepo.GetAllAsync();
            return Ok(nxenesit.Select(MapToDto));
        }

    
        [HttpGet("byShkolla/{shkollaId}")]
        public async Task<IActionResult> GetByShkolla(int shkollaId)
        {
            var nxenesit = await _nxenesiRepo.GetByShkollaAsync(shkollaId);
            return Ok(nxenesit.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var nxenesi = await _nxenesiRepo.GetByIdAsync(id);
            if (nxenesi == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            return Ok(MapToDto(nxenesi));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NxenesiCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
            var shkolla = await _shkollaRepo.GetByIdAsync(dto.ID_Shkolla);
            if (shkolla == null)
                return BadRequest(new { message = "Shkolla e zgjedhur nuk ekziston." });

            var nxenesi = new Nxenesi
            {
                EmriNxenesit = dto.EmriNxenesit,
                Klasa = dto.Klasa,
                ID_Shkolla = dto.ID_Shkolla
            };

            var created = await _nxenesiRepo.CreateAsync(nxenesi);

          
            var withSchool = await _nxenesiRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withSchool!));
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] NxenesiUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nxenesi = await _nxenesiRepo.GetByIdAsync(id);
            if (nxenesi == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

          
            var shkolla = await _shkollaRepo.GetByIdAsync(dto.ID_Shkolla);
            if (shkolla == null)
                return BadRequest(new { message = "Shkolla e zgjedhur nuk ekziston." });

           
            nxenesi.EmriNxenesit = dto.EmriNxenesit;
            nxenesi.Klasa = dto.Klasa;
            nxenesi.ID_Shkolla = dto.ID_Shkolla;

            await _nxenesiRepo.UpdateAsync(nxenesi);
            return Ok(new { message = "Nxënësi u përditësua me sukses." });
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _nxenesiRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
