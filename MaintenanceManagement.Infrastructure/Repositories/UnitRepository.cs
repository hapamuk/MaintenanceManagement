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
    public class UnitRepository :
       GenericRepository<Unit>, IUnitRepository
    {
        public UnitRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif organizasyon ve birimleri
        /// alfabetik sırayla getirir.
        /// </summary>
        public async Task<List<Unit>> GetActiveUnitsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Tüm organizasyon ve birimleri
        /// organizasyon türü, üst birim ve alt birim
        /// bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<List<Unit>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.OrganizationType)
                .Include(x => x.ParentUnit)
                .Include(x => x.ChildUnits)
                .AsSplitQuery()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen birimi organizasyon türü,
        /// üst birim, alt birimler, kullanıcılar,
        /// takımlar, roller ve iş türleriyle getirir.
        /// </summary>
        public async Task<Unit?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.OrganizationType)
                .Include(x => x.ParentUnit)
                .Include(x => x.ChildUnits)
                .Include(x => x.Users)
                .Include(x => x.Teams)
                .Include(x => x.Roles)
                .Include(x => x.UnitWorkTypes)
                    .ThenInclude(x => x.WorkType)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen organizasyon türüne ait
        /// aktif birimleri getirir.
        /// </summary>
        public async Task<List<Unit>> GetUnitsByOrganizationTypeIdAsync(
            int organizationTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.OrganizationType)
                .Where(x =>
                    x.OrganizationTypeId == organizationTypeId &&
                    x.IsActive &&
                    x.OrganizationType.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen üst birime bağlı
        /// aktif alt birimleri getirir.
        /// </summary>
        public async Task<List<Unit>> GetChildUnitsAsync(
            int parentUnitId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.ParentUnitId == parentUnitId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aynı üst birim altında belirtilen
        /// birim adının kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? parentUnitId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedName = name.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Name == normalizedName &&
                    x.ParentUnitId == parentUnitId &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Birim kodunun daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
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
        /// Birime bağlı alt birim bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasChildUnitsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(
                    x => x.ParentUnitId == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen organizasyona bağlı
        /// aktif takım bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveTeamsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Teams
                .AsNoTracking()
                .AnyAsync(
                    x => x.UnitId == id && x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyon veya birimin
        /// aktiflik durumunu günceller.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var unit = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (unit == null)
            {
                return false;
            }

            unit.IsActive = isActive;

            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
