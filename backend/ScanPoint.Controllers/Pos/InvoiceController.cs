using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPoint.Models.DTOs.Cashier;
using ScanPoint.Models.DTOs.Pos;
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
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public InvoiceController(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var adminShopIds = GetAdminShopIds();
            if (adminShopIds.Count == 0)
                return Ok(new List<InvoiceListDto>());

            var invoices = await _invoiceRepository.GetAllByAdminShopsAsync(adminShopIds);
            return Ok(_mapper.Map<List<InvoiceListDto>>(invoices));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var adminShopIds = GetAdminShopIds();
            var invoice = await _invoiceRepository.GetByIdAsync(id, adminShopIds);
            if (invoice == null)
                return NotFound();

            return Ok(_mapper.Map<InvoiceDetailsDto>(invoice));
        }

        [HttpGet("by-shop/{shopId}")]
        public async Task<IActionResult> GetByShop(Guid shopId)
        {
            var adminShopIds = GetAdminShopIds();
            var invoices = await _invoiceRepository.GetByShopAsync(shopId, adminShopIds);
            return Ok(_mapper.Map<List<InvoiceListDto>>(invoices));
        }

        [HttpGet("by-cashier/{cashierId}")]
        public async Task<IActionResult> GetByCashier(Guid cashierId)
        {
            var adminShopIds = GetAdminShopIds();
            var invoices = await _invoiceRepository.GetByCashierForAdminShopsAsync(cashierId, adminShopIds);
            return Ok(_mapper.Map<List<InvoiceListDto>>(invoices));
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var adminShopIds = GetAdminShopIds();
            var invoices = await _invoiceRepository.GetByDateRangeForAdminShopsAsync(start, end, adminShopIds);
            return Ok(_mapper.Map<List<InvoiceListDto>>(invoices));
        }

        // =========================
        // Endpoint për totalin e arkës
        // =========================
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] Guid? shopId = null,
            [FromQuery] Guid? cashierId = null)
        {
            var adminShopIds = GetAdminShopIds();
            if (adminShopIds.Count == 0)
                return Ok(new List<InvoiceSummaryDto>());

            var summary = await _invoiceRepository.GetInvoiceSummaryAsync(
                startDate: start,
                endDate: end,
                adminShopIds: adminShopIds,
                shopId: shopId,
                cashierId: cashierId
            );

            return Ok(summary);
        }

        // =========================
        // Merr shopId-të vetëm për adminin aktual
        // =========================
        private List<Guid> GetAdminShopIds()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "AdminShopIds")?.Value;
            if (string.IsNullOrEmpty(claim))
                return new List<Guid>();

            return claim.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(Guid.Parse)
                        .ToList();
        }

        // Opsionale: Merr adminId nga token nëse dëshiron të filtroni më shumë
        private Guid GetCurrentAdminId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return claim != null ? Guid.Parse(claim) : Guid.Empty;
        }

        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
        {
            var adminShopIds = GetAdminShopIds();
            if (adminShopIds.Count == 0)
                return Ok(new List<BestSellerProductDto>());

            var bestSellers = await _invoiceRepository.GetBestSellingProductsAsync(start, end, adminShopIds);
            return Ok(bestSellers);
        }

        [HttpGet("cashier-performance")]
        public async Task<IActionResult> GetCashierPerformance(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
        {
            if (start > end)
                return BadRequest("Start date must be before End date");

            var adminShopIds = GetAdminShopIds();

            if (adminShopIds.Count == 0)
                return Ok(new List<CashierPerformanceDto>());

            var result = await _invoiceRepository
                .GetCashierPerformanceAsync(start, end, adminShopIds);

            return Ok(result);
        }

        [HttpGet("peak-hours")]
        public async Task<IActionResult> GetPeakHours(
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
        {
            if (start > end)
                return BadRequest("Start must be before End");

            var adminShopIds = GetAdminShopIds();

            if (adminShopIds.Count == 0)
                return Ok(new List<PeakHourDto>());

            var result = await _invoiceRepository
                .GetPeakHoursAsync(start, end, adminShopIds);

            return Ok(result);
        }
    }
}