using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs.Managers;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Models.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ScanPoint.Controllers.Auth
{
    /// <summary>
    /// Pranon HTTP kërkesa dhe kthen përgjigje.
    /// Nuk përmban logjikë biznesi — delegon te IManagerService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerRepository _repository;
        private readonly IManagerService _managerService;
        private readonly IMapper _mapper;
        private readonly ScanPointDbContext _context;

        public ManagerController(
            IManagerRepository repository,
            IManagerService managerService,
            IMapper mapper,
            ScanPointDbContext context)
        {
            _repository = repository;
            _managerService = managerService;
            _mapper = mapper;
            _context = context;
        }

        private Guid GetCurrentAdminId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ===========================
        // GET — vetëm aktivë
        // ===========================
        [HttpGet]
        public async Task<ActionResult<List<ManagerReadDto>>> GetAll()
        {
            var adminId = GetCurrentAdminId();
            var managers = await _repository.GetAllByAdminIdAsync(adminId);
            return Ok(_mapper.Map<List<ManagerReadDto>>(managers));
        }

        // ===========================
        // GET ALL — përfshi të fshirat
        // ===========================
        [HttpGet("all")]
        public async Task<ActionResult> GetAllIncludeDeleted()
        {
            var adminId = GetCurrentAdminId();
            var managers = await _repository.GetAllByAdminIdIncludeDeletedAsync(adminId);

            var result = managers.Select(m => new
            {
                id = m.Id,
                username = m.Username,
                email = m.Email,
                shopId = m.ShopId,
                shopName = m.Shop?.Name,
                shopIsDeleted = m.Shop?.IsDeleted ?? false,
                isDeleted = m.IsDeleted,
                deletedAt = m.DeletedAt
            });

            return Ok(result);
        }

        // ===========================
        // GET BY ID
        // ===========================
        [HttpGet("{id}")]
        public async Task<ActionResult<ManagerReadDto>> GetById(Guid id)
        {
            var adminId = GetCurrentAdminId();
            var manager = await _repository.GetByIdAsync(id, adminId);
            if (manager == null) return NotFound();
            return Ok(_mapper.Map<ManagerReadDto>(manager));
        }

        // ===========================
        // CREATE — logjika është në ManagerService
        // ===========================
        [HttpPost]
        public async Task<ActionResult<ManagerReadDto>> Create([FromBody] ManagerCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var adminId = GetCurrentAdminId();
                var result = await _managerService.CreateManagerAsync(adminId, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===========================
        // UPDATE — logjika është në ManagerService
        // ===========================
        [HttpPut("{id}")]
        public async Task<ActionResult<ManagerReadDto>> Update(Guid id, [FromBody] ManagerUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var adminId = GetCurrentAdminId();
                var result = await _managerService.UpdateManagerAsync(id, adminId, dto);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===========================
        // SOFT DELETE
        // ===========================
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var adminId = GetCurrentAdminId();
            var result = await _repository.DeleteAsync(id, adminId);
            if (!result) return NotFound();
            return NoContent();
        }

        // ===========================
        // RESTORE
        // ===========================
        [HttpPost("{id}/restore")]
        public async Task<ActionResult> Restore(Guid id)
        {
            var adminId = GetCurrentAdminId();
            var managers = await _repository.GetAllByAdminIdIncludeDeletedAsync(adminId);
            var manager = managers.FirstOrDefault(m => m.Id == id);

            if (manager == null)
                return NotFound("Manager nuk u gjet.");

            if (!manager.IsDeleted)
                return BadRequest("Manager është tashmë aktiv.");

            if (manager.ShopId.HasValue)
            {
                var shop = await _context.Shops
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(s => s.Id == manager.ShopId);

                if (shop == null)
                    return BadRequest("Shopi i managerit nuk ekziston.");

                if (shop.IsDeleted)
                    return BadRequest(new
                    {
                        message = $"Shopi '{shop.Name}' është i fshirë. Riktheje fillimisht shoping para se të rikthesh managerin."
                    });
            }

            var result = await _repository.RestoreAsync(id, adminId);
            if (!result) return BadRequest("Rikthimi dështoi.");

            return Ok(new { message = "Manager u rikthye me sukses." });
        }
    }
}