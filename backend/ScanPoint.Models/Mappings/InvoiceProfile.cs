using AutoMapper;
using ScanPoint.Models.DTOs.Pos;
using ScanPoint.Models.Models;
using System.Linq;

namespace ScanPoint.Models.Mappings
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            // =========================
            // Invoice LIST (Table view)
            // =========================
            CreateMap<Invoice, InvoiceListDto>()
                .ForMember(d => d.ShopName,
                    o => o.MapFrom(s => s.Shop != null ? s.Shop.Name : "Unknown"))
                .ForMember(d => d.CashierName,
                    o => o.MapFrom(s => s.Cashier != null ? s.Cashier.Username : "Unknown"));

            // =========================
            // Invoice Item
            // =========================
            CreateMap<InvoiceItem, InvoiceItemReadDto>()
                .ForMember(d => d.SubTotal,
                    o => o.MapFrom(s => s.Price * s.Quantity));

            // =========================
            // Invoice DETAILS
            // =========================
            CreateMap<Invoice, InvoiceDetailsDto>()
                .ForMember(d => d.Items,
                    o => o.MapFrom(s => s.Items))
                .ForMember(d => d.Cashier,
                    o => o.MapFrom(s => s.Cashier != null ? new CashierBasicDto
                    {
                        Id = s.Cashier.Id,
                        Name = s.Cashier.Username
                    } : null))
                .ForMember(d => d.Shop,
                    o => o.MapFrom(s => s.Shop != null ? new ShopBasicDto
                    {
                        Id = s.Shop.Id,
                        Name = s.Shop.Name
                    } : null));

            // =========================
            // POS Invoice DTO (Checkout)
            // =========================
            CreateMap<Invoice, PosInvoiceDto>()
                .ForMember(d => d.InvoiceId,
                    o => o.MapFrom(s => s.Id))
                .ForMember(d => d.SerialNumber,
                    o => o.MapFrom(s => s.SerialNumber))
                .ForMember(d => d.CreatedAt,
                    o => o.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.TotalAmount,
                    o => o.MapFrom(s => s.TotalAmount))
                .ForMember(d => d.Items,
                    o => o.MapFrom(s => s.Items))
                .ForMember(d => d.AmountPaid,
                    o => o.MapFrom(s => s.TotalAmount))
                .ForMember(d => d.Change,
                    o => o.MapFrom(s => s.TotalAmount - s.TotalAmount));

            CreateMap<InvoiceItem, PosInvoiceItemDto>()
                .ForMember(d => d.SubTotal,
                    o => o.MapFrom(s => s.Price * s.Quantity));

            // =========================
            // Invoice SUMMARY (Total per shop/cashier/date)
            // =========================
            CreateMap<Invoice, InvoiceSummaryDto>()
                .ForMember(d => d.Date,
                    o => o.MapFrom(s => s.CreatedAt.Date))
                .ForMember(d => d.ShopName,
                    o => o.MapFrom(s => s.Shop != null ? s.Shop.Name : "Unknown"))
                .ForMember(d => d.CashierName,
                    o => o.MapFrom(s => s.Cashier != null ? s.Cashier.Username : "Unknown"))
                .ForMember(d => d.TotalAmount,
                    o => o.MapFrom(s => s.TotalAmount))
                .ForMember(d => d.InvoiceCount,
                    o => o.MapFrom(s => 1)); // do të bëhet sum/count në query
        }
    }
}
