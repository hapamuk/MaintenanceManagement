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
    public class OrganizationTypeRepository :
         GenericRepository<OrganizationType>,
         IOrganizationTypeRepository
    {
        public OrganizationTypeRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif organizasyon türlerini alfabetik sırayla getirir.
        /// </summary>
        public async Task<List<OrganizationType>>
            GetActiveOrganizationTypesAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyon türlerini ilişkili birimleriyle getirir.
        /// </summary>
        public async Task<List<OrganizationType>>
            GetAllWithUnitsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Units)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyon türünü ID bilgisine göre
        /// ilişkili birimleriyle getirir.
        /// </summary>
        public async Task<OrganizationType?>
            GetByIdWithUnitsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Units)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyon türü adının mevcut olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedName = name.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Name == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyon türü kodunun mevcut olup olmadığını kontrol eder.
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
        /// Organizasyon türüne bağlı birim olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasUnitsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Units
                .AsNoTracking()
                .AnyAsync(
                    x => x.OrganizationTypeId == id,
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyon türünün aktiflik durumunu günceller.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var organizationType = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (organizationType == null)
            {
                return false;
            }

            organizationType.IsActive = isActive;

            organizationType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
