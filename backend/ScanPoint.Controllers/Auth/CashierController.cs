using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs.Cashier;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;

namespace ScanPoint.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class CashierController : ControllerBase
    {
        private readonly ICashierRepository _cashierRepository;
        private readonly IMapper _mapper;
        private readonly ScanPointDbContext _context;

        public CashierController(
            ICashierRepository cashierRepository,
            IMapper mapper,
            ScanPointDbContext context)
        {
            _cashierRepository = cashierRepository;
            _mapper = mapper;
            _context = context;
        }

        // Helper — ID e përdoruesit aktual nga token-i
        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ===========================
        // GET — vetëm aktivë
        // ===========================
        [HttpGet]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<List<CashierReadDto>>> GetCashiers()
        {
            var cashiers = User.IsInRole("Manager")
                ? await _cashierRepository.GetByManagerAsync(CurrentUserId)
                : await _cashierRepository.GetAllByAdminAsync(CurrentUserId);

            return Ok(_mapper.Map<List<CashierReadDto>>(cashiers));
        }

        // ===========================
        // GET ALL — përfshi të fshirat
        // ===========================
        [HttpGet("all")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult> GetAllIncludeDeleted()
        {
            IEnumerable<Cashier> cashiers;

            if (User.IsInRole("Manager"))
            {
                cashiers = await _context.Cashiers
                    .IgnoreQueryFilters()
                    .Where(c => c.ManagerId == CurrentUserId)
                    .Include(c => c.Shop)
                    .Include(c => c.Manager)
                    .ToListAsync();
            }
            else
            {
                cashiers = await _cashierRepository.GetAllByAdminIncludeDeletedAsync(CurrentUserId);
            }

            var result = cashiers.Select(c => new
            {
                id = c.Id,
                username = c.Username,
                email = c.Email,
                shopName = c.Shop?.Name,
                managerUsername = c.Manager?.Username,
                managerId = c.ManagerId,
                isDeleted = c.IsDeleted,
                deletedAt = c.DeletedAt
            });

            return Ok(result);
        }

        // ===========================
        // GET BY ID
        // ===========================
        [HttpGet("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<CashierReadDto>> GetById(Guid id)
        {
            var cashier = await _cashierRepository.GetByIdAsync(id);
            if (cashier == null) return NotFound();
            if (cashier.ManagerId != CurrentUserId) return Forbid();

            return Ok(_mapper.Map<CashierReadDto>(cashier));
        }

        // ===========================
        // CREATE
        // ===========================
        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Create([FromBody] CashierCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var manager = await _cashierRepository.GetManagerByIdAsync(CurrentUserId);
            if (manager == null)
                return Unauthorized(new { message = "Manager nuk ekziston." });

            // Validimi i duplikatëve — logjika është në Repository
            if (await _cashierRepository.EmailExistsGloballyAsync(dto.Email))
                return BadRequest(new { message = $"Email-i '{dto.Email}' është tashmë në përdorim." });

            if (await _cashierRepository.UsernameExistsGloballyAsync(dto.Username))
                return BadRequest(new { message = $"Username-i '{dto.Username}' është tashmë në përdorim." });

            var cashier = new Cashier
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email.ToLower(),
                Role = "Cashier",
                ShopId = manager.ShopId,
                ManagerId = manager.Id
            };

            try
            {
                await _cashierRepository.AddAsync(cashier, dto.Password);
                await _cashierRepository.SaveChangesAsync();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { cashier.Id, cashier.Username, cashier.Email });
        }

        // ===========================
        // UPDATE
        // ===========================
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CashierUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (id == CurrentUserId)
                return BadRequest(new { message = "Nuk mund të editosh llogarinë tënde nga ky endpoint." });

            var cashier = await _cashierRepository.GetByIdAsync(id);
            if (cashier == null) return NotFound();
            if (cashier.ManagerId != CurrentUserId) return Forbid();

            // Validimi — kontrollojmë vetëm nëse vlera ka ndryshuar
            if (!string.Equals(cashier.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _cashierRepository.EmailExistsGloballyAsync(dto.Email, excludeId: id))
                    return BadRequest(new { message = $"Email-i '{dto.Email}' është tashmë në përdorim." });
            }

            if (!string.Equals(cashier.Username, dto.Username, StringComparison.OrdinalIgnoreCase))
            {
                if (await _cashierRepository.UsernameExistsGloballyAsync(dto.Username, excludeId: id))
                    return BadRequest(new { message = $"Username-i '{dto.Username}' është tashmë në përdorim." });
            }

            cashier.Username = dto.Username;
            cashier.Email = dto.Email.ToLower();
            cashier.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                cashier.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            try
            {
                await _cashierRepository.UpdateAsync(cashier);
                await _cashierRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Dështoi përditësimi: {ex.Message}" });
            }

            return NoContent();
        }

        // ===========================
        // DELETE (Soft)
        // ===========================
        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cashier = await _cashierRepository.GetByIdAsync(id);
            if (cashier == null) return NotFound();
            if (cashier.ManagerId != CurrentUserId) return Forbid();

            await _cashierRepository.DeleteAsync(cashier);
            await _cashierRepository.SaveChangesAsync();

            return NoContent();
        }

        // ===========================
        // RESTORE
        // ===========================
        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Restore(Guid id)
        {
            var cashier = await _cashierRepository.GetDeletedByIdAsync(id);

            if (cashier == null)
                return NotFound(new { message = "Cashier nuk u gjet." });

            if (!cashier.IsDeleted)
                return BadRequest(new { message = "Cashier është tashmë aktiv." });

            if (User.IsInRole("Manager") && cashier.ManagerId != CurrentUserId)
                return Forbid();

            try
            {
                await _cashierRepository.RestoreAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            return Ok(new { message = "Cashier u rikthye me sukses." });
        }
    }
}