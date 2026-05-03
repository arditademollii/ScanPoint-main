using ScanPoint.Models.DTOs.Managers;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IManagerService
    {
        Task<bool> EmailExistsGloballyAsync(string email, Guid? excludeId = null);
        Task<bool> UsernameExistsGloballyAsync(string username, Guid? excludeId = null);
        Task<ManagerReadDto> CreateManagerAsync(Guid adminId, ManagerCreateDto dto);
        Task<ManagerReadDto?> UpdateManagerAsync(Guid id, Guid adminId, ManagerUpdateDto dto);
    }
}