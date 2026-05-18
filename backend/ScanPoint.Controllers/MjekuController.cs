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
        private readonly ISpitaliRepository _SpitaliRepo; 

        public MjekuController(IMjekuRepository MjekuRepo, ISpitaliRepository SpitaliRepo)
        {
            _MjekuRepo = MjekuRepo;
            _SpitaliRepo = SpitaliRepo;
        }

      
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

       
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Mjekut = await _MjekuRepo.GetAllAsync();
            return Ok(Mjekut.Select(MapToDto));
        }

      
        [HttpGet("bySpitali/{SpitaliId}")]
        public async Task<IActionResult> GetBySpitali(int SpitaliId)
        {
            var Mjekut = await _MjekuRepo.GetBySpitaliAsync(SpitaliId);
            return Ok(Mjekut.Select(MapToDto));
        }

     
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Mjeku = await _MjekuRepo.GetByIdAsync(id);
            if (Mjeku == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

            return Ok(MapToDto(Mjeku));
        }

      
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MjekuCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            
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

            
            var withSchool = await _MjekuRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withSchool!));
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MjekuUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Mjeku = await _MjekuRepo.GetByIdAsync(id);
            if (Mjeku == null)
                return NotFound(new { message = "Nxënësi nuk u gjet." });

          
            var Spitali = await _SpitaliRepo.GetByIdAsync(dto.ID_Spitali);
            if (Spitali == null)
                return BadRequest(new { message = "Spitali e zgjedhur nuk ekziston." });

          
            Mjeku.EmriMjekut = dto.EmriMjekut;
            Mjeku.Paga = dto.Paga;
            Mjeku.DataPunesimit = dto.DataPunesimit;
            Mjeku.EshteSpecialist = dto.EshteSpecialist;
            Mjeku.ID_Spitali = dto.ID_Spitali;

            await _MjekuRepo.UpdateAsync(Mjeku);
            return Ok(new { message = "Mjeku u përditësua me sukses." });
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _MjekuRepo.DeleteAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
