using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShkollaController : ControllerBase
    {
        
        private readonly IShkollaRepository _shkollaRepo;

     
        public ShkollaController(IShkollaRepository shkollaRepo)
        {
            _shkollaRepo = shkollaRepo;
        }

      
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shkollat = await _shkollaRepo.
                GetAllAsync();

            
            var result = shkollat.Select(s => new ShkollaReadDto
            {
                ID_Shkolla = s.ID_Shkolla,
                EmriShkolles = s.EmriShkolles,
                Qyteti = s.Qyteti
            });

            return Ok(result); 
        }

      
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

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShkollaCreateDto dto)
        {
           
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

           
            var shkolla = new Shkolla
            {
                EmriShkolles = dto.EmriShkolles,
                Qyteti = dto.Qyteti
            };

            var created = await _shkollaRepo.CreateAsync(shkolla);

           
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Shkolla },
                new ShkollaReadDto
                {
                    ID_Shkolla = created.ID_Shkolla,
                    EmriShkolles = created.EmriShkolles,
                    Qyteti = created.Qyteti
                });
        }

    
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShkollaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var shkolla = await _shkollaRepo.GetByIdAsync(id);
            if (shkolla == null)
                return NotFound(new { message = "Shkolla nuk u gjet." });

          
            shkolla.EmriShkolles = dto.EmriShkolles;
            shkolla.Qyteti = dto.Qyteti;

            await _shkollaRepo.UpdateAsync(shkolla);
            return Ok(new { message = "Shkolla u përditësua me sukses." }); 
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _shkollaRepo.DeleteAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); 
            }
        }
    }
}
