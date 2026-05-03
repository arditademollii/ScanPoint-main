using AutoMapper;
using ScanPoint.Models.DTOs.Cashier;
using ScanPoint.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Mappings
{
    public class CashierProfile : Profile
    {
        public CashierProfile()
        {
            CreateMap<Cashier, CashierReadDto>()
                .ForMember(d => d.ManagerUsername,
                    o => o.MapFrom(s => s.Manager.Username))
                .ForMember(d => d.ShopName,
                    o => o.MapFrom(s => s.Shop.Name));

            CreateMap<CashierCreateDto, Cashier>();
            CreateMap<CashierUpdateDto, Cashier>();
        }
    }
}
