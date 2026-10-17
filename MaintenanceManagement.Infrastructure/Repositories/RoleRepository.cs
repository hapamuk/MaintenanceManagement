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
        GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Organizasyona ait aktif rolleri getirir.
        /// </summary>
        public async Task<List<Role>> GetRolesByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.UnitId == unitId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyon içerisinde rol kodunu kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(x =>
                    x.UnitId == unitId &&
                    x.Code == code &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Rolün aktiflik durumunu günceller.
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

            role.IsActive = isActive;
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
