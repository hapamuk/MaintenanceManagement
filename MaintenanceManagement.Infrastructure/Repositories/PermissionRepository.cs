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
    public class PermissionRepository :
       GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif yetkileri modül ve yetki adına göre
        /// sıralayarak getirir.
        /// </summary>
        public async Task<List<Permission>> GetActivePermissionsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Module)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Yetkileri ilişkili roller ve iş alt türü
        /// yetkilendirmeleriyle birlikte getirir.
        /// </summary>
        public async Task<List<Permission>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.RolePermissions)
                    .ThenInclude(x => x.Role)
                .Include(x => x.WorkSubTypePermissions)
                    .ThenInclude(x => x.Role)
                .Include(x => x.WorkSubTypePermissions)
                    .ThenInclude(x => x.WorkSubType)
                .AsSplitQuery()
                .OrderBy(x => x.Module)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen yetkiyi ID bilgisine göre
        /// ilişkili rol ve iş alt türü bilgileriyle getirir.
        /// </summary>
        public async Task<Permission?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.RolePermissions)
                    .ThenInclude(x => x.Role)
                .Include(x => x.WorkSubTypePermissions)
                    .ThenInclude(x => x.Role)
                .Include(x => x.WorkSubTypePermissions)
                    .ThenInclude(x => x.WorkSubType)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen modüle ait aktif yetkileri getirir.
        /// </summary>
        public async Task<List<Permission>> GetPermissionsByModuleAsync(
            string module,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(module))
            {
                return new List<Permission>();
            }

            string normalizedModule = module.Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.Module == normalizedModule &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aynı modülde yetki adının daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            string module,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(module))
            {
                return false;
            }

            string normalizedName = name.Trim();
            string normalizedModule = module.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Name == normalizedName &&
                    x.Module == normalizedModule &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Yetki kodunun daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            string normalizedCode = code.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Code == normalizedCode &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Yetkinin genel rol yetkilerinde veya
        /// iş alt türü yetkilerinde kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssignmentsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            bool hasRolePermission = await _context.RolePermissions
                .AsNoTracking()
                .AnyAsync(
                    x => x.PermissionId == id,
                    cancellationToken);

            if (hasRolePermission)
            {
                return true;
            }

            bool hasWorkSubTypePermission =
                await _context.RoleWorkSubTypePermissions
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.PermissionId == id,
                        cancellationToken);

            return hasWorkSubTypePermission;
        }

        /// <summary>
        /// Yetkinin aktiflik durumunu değiştirir
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var permission = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (permission == null)
            {
                return false;
            }

            permission.IsActive = isActive;

            permission.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
