using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScanPoint.Models.Data;
using ScanPoint.Models.DTOs.Managers;
using ScanPoint.Models.Models;
using ScanPoint.Repositories.Interfaces;

namespace ScanPoint.Repositories.Repositories
{
    /// <summary>
    /// Përmban të gjithë logjikën e biznesit për Manager:
    /// - Validimi global i email/username ndërmjet të gjitha tabelave
    /// - Krijimi dhe përditësimi i managerëve me rregulla biznesi
    /// </summary>
    public class ManagerService : IManagerService
    {
        private readonly ScanPointDbContext _context;
        private readonly IManagerRepository _repository;
        private readonly IMapper _mapper;

        public ManagerService(
            ScanPointDbContext context,
            IManagerRepository repository,
            IMapper mapper)
        {
            _context = context;
            _repository = repository;
            _mapper = mapper;
        }

        // ============================
        // VALIDIM GLOBAL I EMAIL-IT
        // Kontrollon: Users (Admin) + Managers + Cashiers
        // excludeId: përdoret gjatë Update — mos e konto vetë managerin
        // ============================
        public async Task<bool> EmailExistsGloballyAsync(string email, Guid? excludeId = null)
        {
            var lower = email.ToLower();

            var inAdmins = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == lower);
            if (inAdmins) return true;

            var inManagers = await _context.Managers
                .IgnoreQueryFilters()
                .AnyAsync(m => m.Email.ToLower() == lower &&
                               (excludeId == null || m.Id != excludeId));
            if (inManagers) return true;

            var inCashiers = await _context.Cashiers
                .IgnoreQueryFilters()
                .AnyAsync(c => c.Email.ToLower() == lower);

            return inCashiers;
        }

        // ============================
        // VALIDIM GLOBAL I USERNAME-IT
        // Kontrollon: Users (Admin) + Managers + Cashiers
        // ============================
        public async Task<bool> UsernameExistsGloballyAsync(string username, Guid? excludeId = null)
        {
            var lower = username.ToLower();

            var inAdmins = await _context.Users
                .AnyAsync(u => u.Username.ToLower() == lower);
            if (inAdmins) return true;

            var inManagers = await _context.Managers
                .IgnoreQueryFilters()
                .AnyAsync(m => m.Username.ToLower() == lower &&
                               (excludeId == null || m.Id != excludeId));
            if (inManagers) return true;

            var inCashiers = await _context.Cashiers
                .IgnoreQueryFilters()
                .AnyAsync(c => c.Username.ToLower() == lower);

            return inCashiers;
        }

        // ============================
        // KRIJO MANAGER
        // Logjika e biznesit: validim shop, email, username, hash password
        // ============================
        public async Task<ManagerReadDto> CreateManagerAsync(Guid adminId, ManagerCreateDto dto)
        {
            // 1. ShopId është i detyrueshëm
            if (dto.ShopId == null)
                throw new ArgumentException("ShopId është i detyrueshëm për të krijuar një Manager.");

            // 2. Shop ekziston dhe është aktiv
            var shop = await _context.Shops.FindAsync(dto.ShopId);
            if (shop == null)
                throw new ArgumentException("Shopi i zgjedhur nuk ekziston.");
            if (shop.IsDeleted)
                throw new ArgumentException($"Shopi '{shop.Name}' është i fshirë. Zgjidh një shop aktiv.");

            // 3. NumriFaqeve unik globalisht
            if (await EmailExistsGloballyAsync(dto.Email))
                throw new ArgumentException($"Email-i '{dto.Email}' është tashmë në përdorim.");

            // 4. Username unik globalisht
            if (await UsernameExistsGloballyAsync(dto.Username))
                throw new ArgumentException($"Username-i '{dto.Username}' është tashmë në përdorim.");

            // 5. Krijo manager
            var manager = _mapper.Map<Manager>(dto);
            manager.ShopId = dto.ShopId;
            manager.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            manager.Role = "Manager";

            var created = await _repository.CreateAsync(manager);
            return _mapper.Map<ManagerReadDto>(created);
        }

        // ============================
        // PËRDITËSO MANAGER
        // Logjika e biznesit: validim email/username vetëm nëse kanë ndryshuar
        // ============================
        public async Task<ManagerReadDto?> UpdateManagerAsync(Guid id, Guid adminId, ManagerUpdateDto dto)
        {
            var manager = await _repository.GetByIdAsync(id, adminId);
            if (manager == null) return null;

            if (dto.ShopId == null)
                throw new ArgumentException("ShopId është i detyrueshëm.");

            // Kontrollo email vetëm nëse ka ndryshuar
            if (!string.Equals(manager.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await EmailExistsGloballyAsync(dto.Email, excludeId: id))
                    throw new ArgumentException($"Email-i '{dto.Email}' është tashmë në përdorim.");
            }

            // Kontrollo username vetëm nëse ka ndryshuar
            if (!string.Equals(manager.Username, dto.Username, StringComparison.OrdinalIgnoreCase))
            {
                if (await UsernameExistsGloballyAsync(dto.Username, excludeId: id))
                    throw new ArgumentException($"Username-i '{dto.Username}' është tashmë në përdorim.");
            }

            // Nëse ShopId ka ndryshuar — verifiko shop-in e ri
            if (dto.ShopId != manager.ShopId)
            {
                var shop = await _context.Shops.FindAsync(dto.ShopId);
                if (shop == null)
                    throw new ArgumentException("Shopi i zgjedhur nuk ekziston.");
                if (shop.IsDeleted)
                    throw new ArgumentException($"Shopi '{shop.Name}' është i fshirë. Zgjidh një shop aktiv.");

                manager.ShopId = shop.Id;
                manager.Shop = shop;
            }

            _mapper.Map(dto, manager);

            if (!string.IsNullOrEmpty(dto.Password))
                manager.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var updated = await _repository.UpdateAsync(manager);
            return _mapper.Map<ManagerReadDto>(updated);
        }
    }
}