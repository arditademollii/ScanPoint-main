using ScanPoint.Models.DTOs;
using ScanPoint.Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileDto = ScanPoint.Models.DTOs.User.UserProfileDto;

namespace ScanPoint.Repositories.Interfaces
{
    public interface IUserManagementService
    {
        Task<ServiceResult<UserProfileDto>> CreateUserAsync(CreateUserDto dto, string callerRole);
        Task<ServiceResult<UserProfileDto>> UpdateUserAsync(Guid targetUserId, UpdateUserDto dto, Guid callerId, string callerRole);
        Task<ServiceResult<bool>> DeleteUserAsync(Guid targetUserId, Guid callerId, string callerRole);
        Task<ServiceResult<IEnumerable<UserProfileDto>>> GetUsersAsync(string callerRole);
    }
}
