using MaintenanceManagement.Application.Interfaces;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class RoleWorkSubTypePermissionRepository :
       GenericRepository<RoleWorkSubTypePermission>,
       IRoleWorkSubTypePermissionRepository
    {
        public RoleWorkSubTypePermissionRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün eşleştirmeleri rol, organizasyon,
        /// iş türü, iş alt türü ve yetki bilgileriyle getirir.
        /// </summary>
        public async Task<List<RoleWorkSubTypePermission>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Role.Unit.Name)
                .ThenBy(x => x.Role.Name)
                .ThenBy(x => x.WorkSubType.Name)
                .ThenBy(x => x.Permission.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// ID bilgisine göre eşleştirmeyi
        /// bütün ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<RoleWorkSubTypePermission?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen role ait aktif ve pasif
        /// bütün eşleştirmeleri getirir.
        /// </summary>
        public async Task<List<RoleWorkSubTypePermission>>
            GetByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.RoleId == roleId)
                .OrderBy(x => x.WorkSubType.Name)
                .ThenBy(x => x.Permission.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Rolün fiilen kullanılabilir durumdaki
        /// aktif iş alt türü yetkilerini getirir.
        /// </summary>
        public async Task<List<RoleWorkSubTypePermission>>
            GetActiveByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.RoleId == roleId &&
                    x.IsActive &&
                    x.Role.IsActive &&
                    x.Role.Unit.IsActive &&
                    x.WorkSubType.IsActive &&
                    x.WorkSubType.WorkType.IsActive &&
                    x.Permission.IsActive &&
                    x.WorkSubType.WorkType.UnitWorkTypes.Any(uwt =>
                        uwt.UnitId == x.Role.UnitId &&
                        uwt.IsActive))
                .OrderBy(x => x.WorkSubType.Name)
                .ThenBy(x => x.Permission.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş alt türüne ait rol ve
        /// yetki eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<RoleWorkSubTypePermission>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkSubTypeId == workSubTypeId)
                .OrderBy(x => x.Role.Unit.Name)
                .ThenBy(x => x.Role.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen organizasyonun rollerine ait
        /// bütün iş alt türü yetkilendirmelerini getirir.
        /// </summary>
        public async Task<List<RoleWorkSubTypePermission>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.Role.UnitId == unitId)
                .OrderBy(x => x.Role.Name)
                .ThenBy(x => x.WorkSubType.Name)
                .ThenBy(x => x.Permission.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Rol, iş alt türü ve yetki ID değerleriyle
        /// eşleştirme kaydını getirir.
        /// Pasif kayıtlar da sorgulanır.
        /// </summary>
        public async Task<RoleWorkSubTypePermission?>
            GetByRoleWorkSubTypeAndPermissionAsync(
                int roleId,
                int workSubTypeId,
                int permissionId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(x =>
                    x.RoleId == roleId &&
                    x.WorkSubTypeId == workSubTypeId &&
                    x.PermissionId == permissionId,
                    cancellationToken);
        }

        /// <summary>
        /// Aynı rol, iş alt türü ve yetki
        /// eşleştirmesinin varlığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsAssignmentAsync(
            int roleId,
            int workSubTypeId,
            int permissionId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.RoleId == roleId &&
                    x.WorkSubTypeId == workSubTypeId &&
                    x.PermissionId == permissionId &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Rolün belirli bir iş alt türündeki aktif
        /// yetkiye sahip olup olmadığını kontrol eder.
        /// Organizasyonun iş türünü kabul etmesi de gerekir.
        /// </summary>
        public async Task<bool> HasActivePermissionAsync(
            int roleId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                return false;
            }

            string normalizedCode = permissionCode.Trim();

            return await GetEffectivePermissionsQuery()
                .AnyAsync(x =>
                    x.RoleId == roleId &&
                    x.WorkSubTypeId == workSubTypeId &&
                    x.Permission.Code == normalizedCode,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirtilen organizasyonda ve
        /// iş alt türünde aktif yetkisinin olup olmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasUserActivePermissionAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                return false;
            }

            string normalizedCode = permissionCode.Trim();

            return await GetEffectivePermissionsQuery()
                .AnyAsync(x =>
                    x.Role.UnitId == unitId &&
                    x.WorkSubTypeId == workSubTypeId &&
                    x.Permission.Code == normalizedCode &&
                    x.Role.UserUnitRoles.Any(ur =>
                        ur.UserId == userId &&
                        ur.IsActive &&
                        ur.User.IsActive),
                    cancellationToken);
        }

        /// <summary>
        /// Rol-iş alt türü-yetki eşleştirmesinin
        /// aktiflik durumunu değiştirir ve kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (entity == null)
            {
                return false;
            }

            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Rol, organizasyon, iş türü, iş alt türü
        /// ve yetki bilgilerini içeren ortak sorgu.
        /// </summary>
        private IQueryable<RoleWorkSubTypePermission>
            GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.Role)
                    .ThenInclude(x => x.Unit)
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType)
                .Include(x => x.Permission);
        }

        /// <summary>
        /// Yalnızca kullanılabilir durumdaki
        /// rol ve iş alt türü yetkilerini sorgular.
        /// </summary>
        private IQueryable<RoleWorkSubTypePermission>
            GetEffectivePermissionsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Role.IsActive &&
                    x.Role.Unit.IsActive &&
                    x.WorkSubType.IsActive &&
                    x.WorkSubType.WorkType.IsActive &&
                    x.Permission.IsActive &&
                    x.WorkSubType.WorkType.UnitWorkTypes.Any(uwt =>
                        uwt.UnitId == x.Role.UnitId &&
                        uwt.IsActive));
        }
    }
}
