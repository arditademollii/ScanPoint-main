using AutoMapper;
using ScanPoint.Models.DTOs.Managers;
using ScanPoint.Models.Models;

namespace ScanPoint.Models.Mappings
{
    public class ManagerProfile : Profile
    {
        public ManagerProfile()
        {
            // Entity → ReadDTO
            CreateMap<Manager, ManagerReadDto>()
                .ForMember(dest => dest.ShopName,
                           opt => opt.MapFrom(src => src.Shop != null ? src.Shop.Name : null));

            // CreateDTO → Entity
            CreateMap<ManagerCreateDto, Manager>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => "Manager"))
                .ForMember(dest => dest.ShopId, opt => opt.MapFrom(src => src.ShopId)); // ✅ ShopId

            // UpdateDTO → Entity
            CreateMap<ManagerUpdateDto, Manager>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ShopId, opt => opt.MapFrom(src => src.ShopId));
        }
    }
}