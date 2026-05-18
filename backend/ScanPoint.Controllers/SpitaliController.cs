using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class SpitaliController : ControllerBase
    {
       
        private readonly ISpitaliRepository _SpitaliRepo;

       
        public SpitaliController(ISpitaliRepository SpitaliRepo)
        {
            _SpitaliRepo = SpitaliRepo;
        }

      
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Spitalit = await _SpitaliRepo.
                GetAllAsync();

           
            var result = Spitalit.Select(s => new SpitaliReadDto
            {
                ID_Spitali = s.ID_Spitali,
                EmriSpitalit = s.EmriSpitalit,
                NumriKateve = s.NumriKateve,
                KaUrgjence = s.KaUrgjence,
                DataHapjes = s.DataHapjes   
            });

            return Ok(result);
        }

       
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

     
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SpitaliCreateDto dto)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

           
            var Spitali = new Spitali
            {
                EmriSpitalit = dto.EmriSpitalit,
                NumriKateve = dto.NumriKateve,
                KaUrgjence = dto.KaUrgjence,
                DataHapjes = dto.DataHapjes
            };

            var created = await _SpitaliRepo.CreateAsync(Spitali);

           
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

       
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SpitaliCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var Spitali = await _SpitaliRepo.GetByIdAsync(id);
            if (Spitali == null)
                return NotFound(new { message = "Spitali nuk u gjet." });

            Spitali.EmriSpitalit = dto.EmriSpitalit;
            Spitali.NumriKateve = dto.NumriKateve;
            Spitali.KaUrgjence = dto.KaUrgjence;
            Spitali.DataHapjes = dto.DataHapjes;
            

            await _SpitaliRepo.UpdateAsync(Spitali);
            return Ok(new { message = "Spitali u përditësua me sukses." }); // 200
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _SpitaliRepo.DeleteAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); 
            }
        }
    }
}
