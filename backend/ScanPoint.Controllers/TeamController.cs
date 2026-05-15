using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Controllers
{
    // [ApiController] — e bën këtë klasë controller të API-t
    // [Route("api/[controller]")] — URL bëhet: /api/Team
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        // Repository na jep qasjen te databaza
        private readonly ITeamRepository _teamRepo;

        // Dependency Injection — ASP.NET Core na jep repository automatikisht
        public TeamController(ITeamRepository teamRepo)
        {
            _teamRepo = teamRepo;
        }

        // ===========================
        // GET /api/Team
        // Merr listën e të gjitha shkollave
        // ===========================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamRepo.
                GetAllAsync();

            // Mapim: Team → TeamReadDto (nuk i kthejmë të gjitha fushat)
            var result = teams.Select(s => new TeamReadDto
            {
                ID_Team232470351 = s.ID_Team232470351,
                EmriTeam232470351 = s.EmriTeam232470351,
                
            });

            return Ok(result); // 200 OK + JSON
        }

        // ===========================
        // GET /api/Team/{id}
        // Merr një shkollë specifike
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var team = await _teamRepo.GetByIdAsync(id);
            if (team == null)
                return NotFound(new { message = "Team nuk u gjet." }); // 404

            return Ok(new TeamReadDto
            {
                ID_Team232470351 = team.ID_Team232470351,
                EmriTeam232470351 = team.EmriTeam232470351,
                
            });
        }

        // ===========================
        // POST /api/Team
        // Shto shkollë të re
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TeamCreateDto dto)
        {
            // ModelState.IsValid — kontrollon Data Annotations nga DTO
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 nëse të dhënat janë të gabuara

            // Krijojmë entitetin nga DTO
            var team = new Team232470351
            {
                EmriTeam232470351 = dto.EmriTeam232470351
            };

            var created = await _teamRepo.CreateAsync(team);

            // 201 Created + URL ku mund ta gjesh entitetin e ri
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Team232470351 },
                new TeamReadDto
                {
                    ID_Team232470351 = created.ID_Team232470351,
                    EmriTeam232470351 = created.EmriTeam232470351   
                });
        }

        // ===========================
        // PUT /api/Team/{id}
        // Përditëso shkollën ekzistuese
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TeamCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var team = await _teamRepo.GetByIdAsync(id);
            if (team == null)
                return NotFound(new { message = "Team nuk u gjet." });

            // Përditëso fushat
            team.EmriTeam232470351 = dto.EmriTeam232470351;

            await _teamRepo.UpdateAsync(team);
            return Ok(new { message = "Team u përditësua me sukses." }); // 200
        }

        // ===========================
        // DELETE /api/Team/{id}
        // Fshi shkollën
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _teamRepo.DeleteAsync(id);
                return NoContent(); // 204 — sukses, pa body
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404
            }
        }
    }
}
