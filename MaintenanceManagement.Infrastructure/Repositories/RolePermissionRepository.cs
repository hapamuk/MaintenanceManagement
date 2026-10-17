using MaintenanceManagement.Application.Interfaces.Repositories;
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
    public class RolePermissionRepository :
       GenericRepository<RolePermission>,
       IRolePermissionRepository
    {
        public RolePermissionRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Tüm rol-yetki kayıtlarını rol, organizasyon
        /// ve yetki detaylarıyla getirir.
        /// </summary>
        public async Task<List<RolePermission>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Role)
                    .ThenInclude(x => x.Unit)
                .Include(x => x.Permission)
                .OrderBy(x => x.Role.Name)
                .ThenBy(x => x.Permission.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen rol-yetki kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<RolePermission?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Role)
                    .ThenInclude(x => x.Unit)
                .Include(x => x.Permission)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen role tanımlanan aktif
        /// ve pasif bütün yetkileri getirir.
        /// </summary>
        public async Task<List<RolePermission>> GetByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Permission)
                .Where(x => x.RoleId == roleId)
                .OrderBy(x => x.Permission.Module)
                .ThenBy(x => x.Permission.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen role ait yalnızca kullanılabilir
        /// durumdaki aktif genel yetkileri getirir.
        /// </summary>
        public async Task<List<RolePermission>>
            GetActiveByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Permission)
                .Where(x =>
                    x.RoleId == roleId &&
                    x.IsActive &&
                    x.Role.IsActive &&
                    x.Role.Unit.IsActive &&
                    x.Permission.IsActive)
                .OrderBy(x => x.Permission.Module)
                .ThenBy(x => x.Permission.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen yetkinin tanımlandığı
        /// bütün rolleri getirir.
        /// </summary>
        public async Task<List<RolePermission>>
            GetByPermissionIdAsync(
                int permissionId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Role)
                    .ThenInclude(x => x.Unit)
                .Include(x => x.Permission)
                .Where(x => x.PermissionId == permissionId)
                .OrderBy(x => x.Role.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Rol ve yetki eşleştirmesini getirir.
        /// Pasif kayıtlar da sorgulanır.
        /// </summary>
        public async Task<RolePermission?>
            GetByRoleAndPermissionAsync(
                int roleId,
                int permissionId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Role)
                .Include(x => x.Permission)
                .FirstOrDefaultAsync(
                    x => x.RoleId == roleId &&
                         x.PermissionId == permissionId,
                    cancellationToken);
        }

        /// <summary>
        /// Aynı rol ve yetki eşleştirmesinin
        /// daha önce oluşturulup oluşturulmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsAssignmentAsync(
            int roleId,
            int permissionId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.RoleId == roleId &&
                    x.PermissionId == permissionId &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Rolün belirtilen genel sistem yetkisine
        /// aktif olarak sahip olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActivePermissionAsync(
            int roleId,
            string permissionCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                return false;
            }

            string normalizedCode = permissionCode.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.RoleId == roleId &&
                    x.IsActive &&
                    x.Role.IsActive &&
                    x.Role.Unit.IsActive &&
                    x.Permission.IsActive &&
                    x.Permission.Code == normalizedCode,
                    cancellationToken);
        }

        /// <summary>
        /// Rol-yetki eşleştirmesinin aktiflik
        /// durumunu günceller ve kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var rolePermission = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (rolePermission == null)
            {
                return false;
            }

            rolePermission.IsActive = isActive;

            rolePermission.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
