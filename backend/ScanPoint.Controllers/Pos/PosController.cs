using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs.Pos;
using ScanPoint.Models.DTOs.Products;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ScanPoint.Controllers.Pos
{
    [ApiController]
    [Route("api/pos")]
    public class PosController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public PosController(IProductRepository productRepository, IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [HttpGet("products/{barcode}")]
        [Authorize(Roles = "Cashier,Manager,Admin")]
        public async Task<IActionResult> GetProductByBarcode(string barcode)
        {
            var shopIdClaim = User.FindFirst("ShopId")?.Value;
            if (shopIdClaim == null) return Forbid();

            var shopId = Guid.Parse(shopIdClaim);
            var product = await _productRepository.GetByBarcodeAsync(barcode, shopId);

            if (product == null || product.Price <= 0)
                return NotFound("Produkti nuk ekziston ose ka çmim jo-valid.");

            return Ok(_mapper.Map<PosProductDto>(product));
        }

        [HttpPost("checkout")]
        [Authorize(Roles = "Cashier,Manager,Admin")]
        public async Task<IActionResult> Checkout([FromBody] PosCheckoutRequestDto request)
        {
            if (request.Items == null || request.Items.Count == 0)
                return BadRequest("Nuk keni shtuar asnjë produkt.");

            var shopIdClaim = User.FindFirst("ShopId")?.Value;
            var cashierIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (shopIdClaim == null || cashierIdClaim == null) return Forbid();

            var shopId = Guid.Parse(shopIdClaim);
            var cashierId = Guid.Parse(cashierIdClaim);
            var invoiceItems = new List<InvoiceItem>();
            decimal totalAmount = 0;

            foreach (var itemDto in request.Items)
            {
                if (itemDto.Quantity <= 0)
                    return BadRequest($"Sasia për produktin {itemDto.Barcode} duhet të jetë > 0.");

                var product = await _productRepository.GetByBarcodeAsync(itemDto.Barcode, shopId);
                if (product == null)
                    return BadRequest($"Produkti me barcode {itemDto.Barcode} nuk u gjet.");

                if (product.Price <= 0)
                    return BadRequest($"Produkti {product.Name} ka çmim jo-valid.");

                if (product.Quantity < itemDto.Quantity)
                    return BadRequest($"Nuk ka stok të mjaftueshëm për {product.Name}.");

                product.Quantity -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product);

                invoiceItems.Add(new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = itemDto.Quantity
                });

                totalAmount += product.Price * itemDto.Quantity;
            }

            if (totalAmount <= 0)
                return BadRequest("Totali i faturës nuk mund të jetë 0.");

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                SerialNumber = $"INV-{DateTime.Now:yyyyMMddHHmmss}",
                CreatedAt = DateTime.UtcNow,
                ShopId = shopId,
                CashierId = cashierId,
                TotalAmount = totalAmount,
                Items = invoiceItems
            };

            await _invoiceRepository.AddAsync(invoice);

            var response = new PosInvoiceDto
            {
                InvoiceId = invoice.Id,
                SerialNumber = invoice.SerialNumber,
                CreatedAt = invoice.CreatedAt,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = request.AmountPaid,
                Change = request.AmountPaid - totalAmount,
                Items = invoiceItems.Select(i => new PosInvoiceItemDto
                {
                    Name = i.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    SubTotal = i.Price * i.Quantity
                }).ToList()
            };

            return Ok(response);
        }
    }
}
