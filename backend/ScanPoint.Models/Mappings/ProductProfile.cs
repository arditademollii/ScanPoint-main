using AutoMapper;
using ScanPoint.Models.DTOs.Products;
using ScanPoint.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPoint.Models.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // 🔹 POS mapping
            CreateMap<Product, PosProductDto>()
                .ForMember(dest => dest.AvailableQuantity,
                           opt => opt.MapFrom(src => src.Quantity));
        }
    }
}
