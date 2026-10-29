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
    public class RoleRepository :
        GenericRepository<Role>,
        IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        #region GetAllWithDetails

        /// <summary>
        /// Bütün rolleri bağlı organizasyon
        /// birimleriyle birlikte getirir.
        /// </summary>
        public async Task<IReadOnlyList<Role>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Unit)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetByIdWithDetails

        /// <summary>
        /// Rolün detaylarını ve ilişkili
        /// kullanıcı/yetki kayıtlarını getirir.
        /// </summary>
        public async Task<Role?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()

                // Rolün bağlı olduğu birim
                .Include(x => x.Unit)

                // Kullanıcı-rol atamaları
                .Include(x => x.UserUnitRoles)

                // Rolün genel yetkileri
                .Include(x => x.RolePermissions)

                // İş alt türü bazındaki yetkileri
                .Include(x => x.WorkSubTypePermissions)

                .AsSplitQuery()

                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        #endregion

        #region GetByUnit

        /// <summary>
        /// Belirtilen organizasyon biriminin
        /// aktif rollerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<Role>> GetByUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Unit)

                .Where(x =>
                    x.UnitId == unitId &&
                    x.IsActive)

                .OrderBy(x => x.Name)

                .ToListAsync(cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Organizasyon birimi bazında
        /// rol kodu benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var normalizedCode = code.Trim();

            return await _dbSet
                .AsNoTracking()

                .AnyAsync(x =>
                    x.UnitId == unitId &&
                    x.Code == normalizedCode &&
                    (
                        !excludedId.HasValue ||
                        x.Id != excludedId.Value
                    ),
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Rolün aktif/pasif durumunu günceller.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var role = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (role == null)
            {
                return false;
            }

            if (role.IsActive == isActive)
            {
                return true;
            }

            role.IsActive = isActive;

            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        #endregion
    }
}
