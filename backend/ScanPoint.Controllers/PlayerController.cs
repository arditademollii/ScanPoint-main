using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepo;
        private readonly ITeamRepository _teamRepo; // Na duhet për të kontrolluar shkollën

        public PlayerController(IPlayerRepository playerRepo, ITeamRepository teamRepo)
        {
            _playerRepo = playerRepo;
            _teamRepo = teamRepo;
        }

        // Helper — mapim Player → PlayerReadDto
        private static Player232470351ReadDto MapToDto(Player232470351 p) => new Player232470351ReadDto
        {
            ID = p.ID,
            EmriPlayer232470351 = p.EmriPlayer232470351,
            Number = p.Number,
            ID_Team232470351 = p.ID_Team232470351,
            EmriTeam232470351 = p.Team232470351?.EmriTeam232470351 ?? ""
        };

        // ===========================
        // GET /api/Player
        // Merr të gjithë lojtarët  
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lojtarët = await _playerRepo.GetAllAsync();
            return Ok(lojtarët.Select(MapToDto));
        }

        // ===========================
        // GET /api/Player/byTeam/{teamId}
        // Filtro lojtarët sipas team-it — për dropdown filtrim
        // ===========================
        [HttpGet("byTeam/{teamId}")]
        public async Task<IActionResult> GetByTeam(int teamId)
        {
            var lojtarët = await _playerRepo.GetByTeam232470351Async(teamId);
            return Ok(lojtarët.Select(MapToDto));
        }

        // ===========================
        // GET /api/Player/{id}
        // Merr lojtarin me ID specifik
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var lojtar = await _playerRepo.GetByIdAsync(id);
            if (lojtar == null)
                return NotFound(new { message = "Lojtari nuk u gjet." });

            return Ok(MapToDto(lojtar));
        }

        // ===========================
        // POST /api/Player
        // Shto lojtar të ri
        // Gjatë krijimit, dropdown zgjedh team-in ekzistues (EmriTeam)
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Player232470351CreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kontrollo nëse team ekziston
            var team = await _teamRepo.GetByIdAsync(dto.ID_Team232470351);
            if (team == null)
                return BadRequest(new { message = "Team-i i zgjedhur nuk ekziston." });

            var lojtar = new Player232470351
            {
                EmriPlayer232470351 = dto.EmriPlayer232470351,
                Number = dto.Number,
                ID_Team232470351 = dto.ID_Team232470351
            };

            var created = await _playerRepo.CreateAsync(lojtar);

            // Ringarko lojtarin me team-in për ta kthyer të plotë
            var withTeam = await _playerRepo.GetByIdAsync(created.ID);
            return CreatedAtAction(nameof(GetById), new { id = created.ID }, MapToDto(withTeam!));
        }

        // ===========================
        // PUT /api/Player/{id}
        // Përditëso të dhënat e lojtarit
        // Forma mbushet paraprakisht me të dhënat aktuale
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Player232470351UpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lojtar = await _playerRepo.GetByIdAsync(id);
            if (lojtar == null)
                return NotFound(new { message = "Lojtari  nuk u gjet." });

            // Kontrollo nëse team-i i ri ekziston
            var team = await _teamRepo.GetByIdAsync(dto.ID_Team232470351);
            if (team == null)
                return BadRequest(new { message = "Team-i i  zgjedhur nuk ekziston." });

            // Përditëso fushat
            lojtar.EmriPlayer232470351 = dto.EmriPlayer232470351;
            lojtar.Number = dto.Number;
            lojtar.ID_Team232470351 = dto.ID_Team232470351;

            await _playerRepo.UpdateAsync(lojtar);
            return Ok(new { message = "Lojtari u përditësua me sukses." });
        }

        // ===========================
        // DELETE /api/Player/{id}
        // Fshi lojtarin nga lista
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _playerRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
