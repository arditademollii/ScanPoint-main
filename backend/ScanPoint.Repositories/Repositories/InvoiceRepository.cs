using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;
using ScanPoint.Models.DTOs.Cashier;
using ScanPoint.Models.DTOs.Pos;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ScanPointDbContext _context;

        public InvoiceRepository(ScanPointDbContext context)
        {
            _context = context;
        }

        // ✅ Helper për me largu invoices me total 0
        private IQueryable<Invoice> FilterValidInvoices(IQueryable<Invoice> query)
        {
            return query.Where(i => i.TotalAmount > 0);
        }

        private IQueryable<Invoice> IncludeAll(IQueryable<Invoice> query)
        {
            return query
                .Include(i => i.Cashier)
                .Include(i => i.Shop)
                .Include(i => i.Items)
                    .ThenInclude(item => item.Product);
        }

        public async Task AddAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllByAdminShopsAsync(List<Guid> adminShopIds)
        {
            var query = FilterValidInvoices(_context.Invoices)
                .Where(i => adminShopIds.Contains(i.ShopId));

            query = IncludeAll(query)
                    .OrderByDescending(i => i.CreatedAt);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByShopAsync(Guid shopId, List<Guid> adminShopIds)
        {
            if (!adminShopIds.Contains(shopId))
                return new List<Invoice>();

            var query = FilterValidInvoices(_context.Invoices)
                .Where(i => i.ShopId == shopId);

            query = IncludeAll(query)
                    .OrderByDescending(i => i.CreatedAt);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByCashierForAdminShopsAsync(Guid cashierId, List<Guid> adminShopIds)
        {
            var query = FilterValidInvoices(_context.Invoices)
                .Where(i => i.CashierId == cashierId && adminShopIds.Contains(i.ShopId));

            query = IncludeAll(query)
                    .OrderByDescending(i => i.CreatedAt);

            return await query.ToListAsync();
        }

        public async Task<Invoice> GetByIdAsync(Guid id, List<Guid> adminShopIds)
        {
            var invoice = await IncludeAll(
                FilterValidInvoices(_context.Invoices)
            ).FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null || !adminShopIds.Contains(invoice.ShopId))
                return null;

            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetByDateRangeForAdminShopsAsync(
            DateTime start,
            DateTime end,
            List<Guid> adminShopIds)
        {
            end = end.Date.AddDays(1).AddTicks(-1);

            var query = FilterValidInvoices(_context.Invoices)
                .Where(i =>
                    i.CreatedAt >= start &&
                    i.CreatedAt <= end &&
                    adminShopIds.Contains(i.ShopId));

            query = IncludeAll(query)
                    .OrderByDescending(i => i.CreatedAt);

            return await query.ToListAsync();
        }

        // =========================
        // Summary / Total i arkës
        // =========================
        public async Task<IEnumerable<InvoiceSummaryDto>> GetInvoiceSummaryAsync(
            DateTime startDate,
            DateTime endDate,
            List<Guid> adminShopIds,
            Guid? shopId = null,
            Guid? cashierId = null)
        {
            endDate = endDate.Date.AddDays(1).AddTicks(-1);

            var query = FilterValidInvoices(_context.Invoices).AsQueryable();

            query = query.Where(i => adminShopIds.Contains(i.ShopId));

            if (shopId.HasValue)
                query = query.Where(i => i.ShopId == shopId.Value);

            if (cashierId.HasValue)
                query = query.Where(i => i.CashierId == cashierId.Value);

            query = query.Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate);

            var summary = await query
                .Include(i => i.Cashier)
                .Include(i => i.Shop)
                .GroupBy(i => new
                {
                    i.CreatedAt.Date,
                    i.ShopId,
                    i.CashierId,
                    i.Shop.Name,
                    i.Cashier.Username
                })
                .Select(g => new InvoiceSummaryDto
                {
                    Date = g.Key.Date,
                    ShopName = g.Key.Name,
                    CashierName = g.Key.Username,
                    TotalAmount = g.Sum(i => i.TotalAmount),
                    InvoiceCount = g.Count()
                })
                .OrderBy(s => s.Date)
                .ToListAsync();

            return summary;
        }

        public async Task<List<BestSellerProductDto>> GetBestSellingProductsAsync(
            DateTime startDate,
            DateTime endDate,
            List<Guid> adminShopIds)
        {
            if (adminShopIds == null || adminShopIds.Count == 0)
                return new List<BestSellerProductDto>();

            return await _context.InvoiceItems
                .Include(ii => ii.Invoice)
                .Include(ii => ii.Product)
                .Where(ii =>
                    ii.Invoice.CreatedAt >= startDate &&
                    ii.Invoice.CreatedAt <= endDate &&
                    adminShopIds.Contains(ii.Invoice.ShopId) &&
                    ii.Invoice.TotalAmount > 0)
                .GroupBy(ii => new
                {
                    ii.ProductId,
                    ProductName = ii.Product != null ? ii.Product.Name : ii.Name
                })
                .Select(g => new BestSellerProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x =>
                        (x.Product != null ? x.Product.Price : x.Price) * x.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .ToListAsync();
        }

        public async Task<List<CashierPerformanceDto>> GetCashierPerformanceAsync(
            DateTime start,
            DateTime end,
            List<Guid> adminShopIds)
        {
            var query = await FilterValidInvoices(_context.Invoices)
                .Where(i =>
                    i.CreatedAt >= start &&
                    i.CreatedAt <= end &&
                    adminShopIds.Contains(i.ShopId))
                .Include(i => i.Cashier)
                .Include(i => i.Items)
                .ToListAsync();

            var result = query
                .GroupBy(i => new { i.CashierId, i.Cashier.Username })
                .Select(g => new CashierPerformanceDto
                {
                    CashierId = g.Key.CashierId,
                    CashierName = g.Key.Username,
                    TotalInvoices = g.Count(),
                    TotalRevenue = g.Sum(i => i.TotalAmount),
                    AveragePerInvoice = g.Count() == 0 ? 0 : g.Sum(i => i.TotalAmount) / g.Count(),
                    TotalItemsSold = g.Sum(i => i.Items.Sum(x => x.Quantity))
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            return result;
        }

        public async Task<List<PeakHourDto>> GetPeakHoursAsync(
            DateTime start,
            DateTime end,
            List<Guid> adminShopIds)
        {
            return await FilterValidInvoices(_context.Invoices)
                .Where(i =>
                    i.CreatedAt >= start &&
                    i.CreatedAt <= end &&
                    adminShopIds.Contains(i.ShopId))
                .GroupBy(i => i.CreatedAt.Hour)
                .Select(g => new PeakHourDto
                {
                    Hour = g.Key,
                    TotalInvoices = g.Count(),
                    TotalRevenue = g.Sum(i => i.TotalAmount)
                })
                .OrderByDescending(x => x.TotalInvoices)
                .ToListAsync();
        }
    }
}