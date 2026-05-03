using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs.Products;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ScanPoint.Controllers.Shops
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProducts()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var shopIdClaim = User.FindFirst("ShopId")?.Value;
            IEnumerable<Product> products;

            if (role == "Admin")
            {
                var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                products = await _repository.GetAllByAdminAsync(adminId);
            }
            else if ((role == "Manager" || role == "Cashier") && shopIdClaim != null)
            {
                var shopId = Guid.Parse(shopIdClaim);
                products = await _repository.GetAllByShopAsync(shopId);
            }
            else
            {
                return Forbid();
            }

            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var shopIdClaim = User.FindFirst("ShopId")?.Value;

            Guid shopId;
            if (role == "Manager" && shopIdClaim != null)
                shopId = Guid.Parse(shopIdClaim);
            else if (role == "Admin" && dto.ShopId.HasValue)
                shopId = dto.ShopId.Value;
            else
                return BadRequest("ShopId required");

            // ✅ Kontrollo nëse barkodi ekziston në të njëjtin shop
            var existingProduct = await _repository.GetByBarcodeAsync(dto.Barcode, shopId);
            if (existingProduct != null)
            {
                return BadRequest("Ky barkod ekziston tashmë në këtë shop. Zgjidh një barkod tjetër.");
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = _mapper.Map<Product>(dto);
            product.Id = Guid.NewGuid();
            product.ShopId = shopId;

            await _repository.AddAsync(product);
            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var shopIdClaim = User.FindFirst("ShopId")?.Value;

            if (role == "Manager")
            {
                if (shopIdClaim == null)
                    return Forbid();

                if (product.ShopId != Guid.Parse(shopIdClaim))
                    return Forbid();
            }

            if (role == "Admin" && dto.ShopId.HasValue)
            {
                product.ShopId = dto.ShopId.Value;
            }

            // ✅ Kontrollo barkodin unik në shop për update
            if (!string.IsNullOrEmpty(dto.Barcode))
            {
                var existingProduct = await _repository.GetByBarcodeAsync(dto.Barcode, product.ShopId);
                if (existingProduct != null && existingProduct.Id != product.Id)
                {
                    return BadRequest("Ky barkod është i përdorur nga një produkt tjetër në këtë shop.");
                }
                product.Barcode = dto.Barcode;
            }
            if (!ModelState.IsValid) return BadRequest(ModelState);
            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Unit = dto.Unit;
            product.ExpiryDate = dto.ExpiryDate;
            product.Category = dto.Category;
            product.Quantity = dto.Quantity;

            await _repository.UpdateAsync(product);

            return Ok(_mapper.Map<ProductDto>(product));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var shopIdClaim = User.FindFirst("ShopId")?.Value;
            if (role == "Manager" && shopIdClaim != null && product.ShopId != Guid.Parse(shopIdClaim))
                return Forbid();

            await _repository.DeleteAsync(product);
            return NoContent();
        }
    }
}
