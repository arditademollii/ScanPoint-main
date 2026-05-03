using ScanPoint.Models.DTOs.Cashier;
using ScanPoint.Models.DTOs.Pos;
using ScanPoint.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice);

        Task<IEnumerable<Invoice>> GetAllByAdminShopsAsync(List<Guid> adminShopIds);

        Task<IEnumerable<Invoice>> GetByShopAsync(Guid shopId, List<Guid> adminShopIds);

        Task<IEnumerable<Invoice>> GetByCashierForAdminShopsAsync(Guid cashierId, List<Guid> adminShopIds);

        Task<Invoice> GetByIdAsync(Guid id, List<Guid> adminShopIds);

        Task<IEnumerable<Invoice>> GetByDateRangeForAdminShopsAsync(DateTime start, DateTime end, List<Guid> adminShopIds);

        // =========================
        // Summary / Total i arkës
        // =========================
        Task<IEnumerable<InvoiceSummaryDto>> GetInvoiceSummaryAsync(
            DateTime startDate,
            DateTime endDate,
            List<Guid> adminShopIds,
            Guid? shopId = null,
            Guid? cashierId = null
        );

        Task<List<BestSellerProductDto>> GetBestSellingProductsAsync(
            DateTime startDate,
            DateTime endDate,
            List<Guid> adminShopIds
        );

        Task<List<CashierPerformanceDto>> GetCashierPerformanceAsync(
    DateTime start,
    DateTime end,
    List<Guid> adminShopIds);
    
    Task<List<PeakHourDto>> GetPeakHoursAsync(
    DateTime start,
    DateTime end,
    List<Guid> adminShopIds);
    }
}
