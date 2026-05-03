using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.DTOs.Shop;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System.Security.Claims;

namespace ScanPoint.Controllers.Shops
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ShopsController : ControllerBase
    {
        private readonly IShopRepository _shopRepository;

        public ShopsController(IShopRepository shopRepository)
        {
            _shopRepository = shopRepository;
        }

        // Helper — merr Admin ID nga token-i
        private Guid? GetAdminId()
        {
            var claim = User.FindFirst("Id")?.Value
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }

        // ===========================
        // CREATE SHOP
        // ===========================
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var adminId = GetAdminId();
            if (adminId == null) return Unauthorized();

            var exists = await _shopRepository.ExistsByNameForAdminAsync(dto.Name, adminId.Value);
            if (exists)
                return BadRequest(new { message = "Ti tashmë ke një shop me këtë emër!" });

            // Krijimi i entitetit — mbetet në Controller meqë ShopRepository pret Shop objekt
            var shop = new Shop
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                FiscalNumber = dto.FiscalNumber,
                VatNumber = dto.VatNumber,
                AdminId = adminId.Value
            };

            var createdShop = await _shopRepository.CreateAsync(shop);

            return CreatedAtAction(nameof(GetShopById), new { id = createdShop.Id },
                MapToDto(createdShop));
        }

        // ===========================
        // GET BY ID
        // ===========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShopById(Guid id)
        {
            var shop = await _shopRepository.GetByIdAsync(id);
            if (shop == null) return NotFound();

            return Ok(MapToDto(shop));
        }

        // ===========================
        // UPDATE SHOP
        // ===========================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShop(Guid id, [FromBody] UpdateShopDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var shop = await _shopRepository.GetByIdAsync(id);
            if (shop == null) return NotFound();

            var exists = await _shopRepository
                .ExistsByNameForAdminUpdateAsync(dto.Name, shop.AdminId, id);
            if (exists)
                return BadRequest(new { message = "Ti tashmë ke një shop tjetër me këtë emër!" });

            shop.Name = dto.Name;
            shop.Address = dto.Address;
            shop.FiscalNumber = dto.FiscalNumber;
            shop.VatNumber = dto.VatNumber;

            try
            {
                await _shopRepository.UpdateAsync(shop);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Një shop me këtë emër ekziston!" });
            }

            return Ok(MapToDto(shop));
        }

        // ===========================
        // GET MY SHOPS — vetëm aktive
        // ===========================
        [HttpGet("my-shops")]
        public async Task<IActionResult> GetMyShops()
        {
            var adminId = GetAdminId();
            if (adminId == null) return Unauthorized();

            var shops = await _shopRepository.GetByAdminIdAsync(adminId.Value);
            return Ok(shops.Select(MapToDto));
        }

        // ===========================
        // GET MY SHOPS ALL — përfshi të fshirat
        // ===========================
        [HttpGet("my-shops/all")]
        public async Task<IActionResult> GetMyShopsAll()
        {
            var adminId = GetAdminId();
            if (adminId == null) return Unauthorized();

            var shops = await _shopRepository.GetAllByAdminIdAsync(adminId.Value);

            var result = shops.Select(s => new
            {
                id = s.Id,
                name = s.Name,
                address = s.Address,
                vatNumber = s.VatNumber,
                fiscalNumber = s.FiscalNumber,
                adminName = s.Admin?.Username,
                isDeleted = s.IsDeleted,
                deletedAt = s.DeletedAt
            });

            return Ok(result);
        }

        // ===========================
        // SOFT DELETE
        // ===========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop(Guid id)
        {
            var shop = await _shopRepository.GetByIdAsync(id);
            if (shop == null) return NotFound(new { message = "Shop nuk u gjet." });

            try
            {
                await _shopRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===========================
        // RESTORE
        // ===========================
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreShop(Guid id)
        {
            try
            {
                await _shopRepository.RestoreAsync(id);
                return Ok(new { message = "Shop u rikthye me sukses." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ===========================
        // HELPER — mapim Shop → ShopDto
        // ===========================
        private static ShopDto MapToDto(Shop s) => new ShopDto
        {
            Id = s.Id,
            Name = s.Name,
            Address = s.Address,
            VatNumber = s.VatNumber,
            FiscalNumber = s.FiscalNumber,
            AdminName = s.Admin?.Username
        };
    }
}