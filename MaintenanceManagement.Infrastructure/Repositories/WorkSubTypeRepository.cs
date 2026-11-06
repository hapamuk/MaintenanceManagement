using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class WorkSubTypeRepository :
       GenericRepository<WorkSubType>,
       IWorkSubTypeRepository
    {
        public WorkSubTypeRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif iş türlerine bağlı aktif
        /// iş alt türlerini getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetActiveWorkSubTypesAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.WorkType)
                .Where(x =>
                    x.IsActive &&
                    x.WorkType.IsActive)
                .OrderBy(x => x.WorkType.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş alt türlerini bağlı iş türü ve
        /// rol yetkileriyle birlikte getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.WorkType.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş alt türünü rol yetkileri, iş talepleri,
        /// iş emirleri ve planlı bakım kayıtlarıyla getirir.
        /// </summary>
        public async Task<WorkSubType?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Include(x => x.WorkRequests)
                .Include(x => x.WorkOrders)
                .Include(x => x.PlannedMaintenances)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş türüne ait bütün
        /// iş alt türlerini getirir.
        /// Pasif kayıtlar da dahildir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.WorkType)
                .Where(x => x.WorkTypeId == workTypeId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aktif iş türüne bağlı aktif
        /// iş alt türlerini getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetActiveByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.WorkType)
                .Where(x =>
                    x.WorkTypeId == workTypeId &&
                    x.IsActive &&
                    x.WorkType.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyonda kullanılabilecek
        /// aktif iş alt türlerini getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetAvailableByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetAvailableForUnitQuery(unitId)
                .Include(x => x.WorkType)
                .OrderBy(x => x.WorkType.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona tanımlanmış belirtilen
        /// iş türünün aktif alt türlerini getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetAvailableByUnitAndWorkTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            return await GetAvailableForUnitQuery(unitId)
                .Include(x => x.WorkType)
                .Where(x => x.WorkTypeId == workTypeId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen organizasyondaki rolün,
        /// verilen izin koduyla yetkili olduğu
        /// iş alt türlerini getirir.
        ///
        /// Örnek izin kodu: WORK_ORDER_EXECUTE
        /// </summary>
        public async Task<List<WorkSubType>>
            GetAuthorizedByRoleAsync(
                int roleId,
                int unitId,
                string permissionCode = "WORK_ORDER_EXECUTE",
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                throw new ArgumentException(
                    "Yetki kodu boş olamaz.",
                    nameof(permissionCode));
            }

            string normalizedCode = permissionCode.Trim();

            return await GetAvailableForUnitQuery(unitId)
                .Include(x => x.WorkType)
                .Where(x =>
                    x.RolePermissions.Any(rp =>
                        rp.RoleId == roleId &&
                        rp.IsActive &&
                        rp.Role.IsActive &&
                        rp.Role.UnitId == unitId &&
                        rp.Permission.IsActive &&
                        rp.Permission.Code == normalizedCode))
                .OrderBy(x => x.WorkType.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının organizasyondaki aktif
        /// rolleri üzerinden yetkili olduğu
        /// iş alt türlerini getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetAuthorizedByUserAsync(
                int userId,
                int unitId,
                string permissionCode = "WORK_ORDER_EXECUTE",
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                throw new ArgumentException(
                    "Yetki kodu boş olamaz.",
                    nameof(permissionCode));
            }

            string normalizedCode = permissionCode.Trim();

            return await GetAvailableForUnitQuery(unitId)
                .Include(x => x.WorkType)
                .Where(x =>
                    x.RolePermissions.Any(rp =>
                        rp.IsActive &&
                        rp.Role.IsActive &&
                        rp.Role.UnitId == unitId &&
                        rp.Permission.IsActive &&
                        rp.Permission.Code == normalizedCode &&

                        rp.Role.UserUnitRoles.Any(ur =>
                            ur.UserId == userId &&
                            ur.IsActive &&
                            ur.User.IsActive)))
                .OrderBy(x => x.WorkType.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş alt türünün organizasyonda aktif
        /// olarak kullanılabilir olup olmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> IsAvailableForUnitAsync(
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            return await GetAvailableForUnitQuery(unitId)
                .AnyAsync(
                    x => x.Id == workSubTypeId,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcının organizasyonda belirtilen
        /// iş alt türünü uygulama yetkisini kontrol eder.
        ///
        /// WORK_ORDER_EXECUTE yetkisini esas alır.
        /// </summary>
        public async Task<bool> CanUserExecuteAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            const string permissionCode = "WORK_ORDER_EXECUTE";

            return await GetAvailableForUnitQuery(unitId)
                .AnyAsync(x =>
                    x.Id == workSubTypeId &&

                    x.RolePermissions.Any(rp =>
                        rp.IsActive &&
                        rp.Role.IsActive &&
                        rp.Role.UnitId == unitId &&
                        rp.Permission.IsActive &&
                        rp.Permission.Code == permissionCode &&

                        rp.Role.UserUnitRoles.Any(ur =>
                            ur.UserId == userId &&
                            ur.IsActive &&
                            ur.User.IsActive)),
                    cancellationToken);
        }

        /// <summary>
        /// Aynı iş türü altında aynı alt tür
        /// adının kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            int workTypeId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string normalizedName = name.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkTypeId == workTypeId &&
                    x.Name == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// İş alt türüne bağlı herhangi bir
        /// iş talebi bulunup bulunmadığını kontrol eder.
        ///
        /// Bütün talep durumları dikkate alınır.
        /// </summary>
        public async Task<bool> HasWorkRequestsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequests
                .AsNoTracking()
                .AnyAsync(
                    x => x.WorkSubTypeId == workSubTypeId,
                    cancellationToken);
        }

        /// <summary>
        /// İş alt türünde Yeni (Draft)
        /// durumunda iş talebi bulunup
        /// bulunmadığını kontrol eder.
        ///
        /// Kullanıcının dört durumlu
        /// WorkRequestStatusEnum tanımına uygundur.
        /// </summary>
        public async Task<bool> HasNewWorkRequestsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequests
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkSubTypeId == workSubTypeId &&
                    x.Status == WorkRequestStatusEnum.Draft,
                    cancellationToken);
        }

        /// <summary>
        /// İş alt türüne bağlı tamamlanmamış
        /// veya iptal edilmemiş iş emri
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrders
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkSubTypeId == workSubTypeId &&
                    (
                        x.Status == WorkStatusEnum.New ||
                        x.Status == WorkStatusEnum.InProgress ||
                        x.Status == WorkStatusEnum.PendingApproval
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// İş alt türüne bağlı aktif planlı
        /// bakım bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveMaintenancePlansAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.PlannedMaintenances
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkSubTypeId == workSubTypeId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// İş alt türüne tanımlanmış aktif
        /// rol-yetki kaydı bulunup bulunmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveRolePermissionsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.RoleWorkSubTypePermissions
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkSubTypeId == workSubTypeId &&
                    x.IsActive &&
                    x.Role.IsActive &&
                    x.Permission.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// İş alt türünün aktiflik durumunu
        /// günceller ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var workSubType = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (workSubType == null)
            {
                return false;
            }

            if (workSubType.IsActive == isActive)
            {
                return true;
            }

            workSubType.IsActive = isActive;
            workSubType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// İş türü, iş türünün organizasyon
        /// eşleştirmeleri ve rol bazlı yetkilerini
        /// içeren ortak detay sorgusudur.
        /// </summary>
        private IQueryable<WorkSubType> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // Bağlı iş türü
                .Include(x => x.WorkType)
                    .ThenInclude(x => x.UnitWorkTypes)
                        .ThenInclude(x => x.Unit)

                // İş alt türüne tanımlanan roller
                .Include(x => x.RolePermissions)
                    .ThenInclude(x => x.Role)
                        .ThenInclude(x => x.Unit)

                // İş alt türüne tanımlanan izinler
                .Include(x => x.RolePermissions)
                    .ThenInclude(x => x.Permission)

                .AsSplitQuery();
        }

        /// <summary>
        /// Belirtilen organizasyonda kullanılabilir
        /// aktif iş alt türlerini sorgular.
        ///
        /// İş alt türü, iş türü, organizasyon,
        /// ve UnitWorkType eşleştirmesi aktif olmalıdır.
        /// Organizasyon türü Unit.OrganizationTypeEnum üzerinden tutulur.
        /// </summary>
        private IQueryable<WorkSubType> GetAvailableForUnitQuery(
            int unitId)
        {
            return _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.WorkType.IsActive &&

                    x.WorkType.UnitWorkTypes.Any(uwt =>
                        uwt.UnitId == unitId &&
                        uwt.IsActive &&
                        uwt.Unit.IsActive));
        }
    }
}
