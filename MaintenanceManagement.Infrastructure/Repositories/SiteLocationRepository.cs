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
    public class SiteLocationRepository :
       GenericRepository<SiteLocation>,
       ISiteLocationRepository
    {
        public SiteLocationRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif konumları alfabetik sırayla getirir.
        /// </summary>
        public async Task<List<SiteLocation>>
            GetActiveLocationsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bütün konumları ilişkili iş talepleriyle
        /// birlikte getirir.
        ///
        /// Aktif ve pasif konumları kapsar.
        /// </summary>
        public async Task<List<SiteLocation>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.WorkRequests)
                .AsSplitQuery()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen konumu iş talepleri,
        /// iş alt türleri ve iş türleriyle getirir.
        /// </summary>
        public async Task<SiteLocation?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.WorkRequests)
                    .ThenInclude(x => x.WorkSubType)
                        .ThenInclude(x => x.WorkType)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Konum adına göre mevcut kaydı getirir.
        ///
        /// Aktif ve pasif konumlar sorgulanır.
        /// </summary>
        public async Task<SiteLocation?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            string normalizedName = name.Trim();

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Name == normalizedName,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen binaya ait bütün
        /// konum kayıtlarını getirir.
        /// </summary>
        public async Task<List<SiteLocation>>
            GetLocationsByBuildingAsync(
                string building,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(building))
            {
                return new List<SiteLocation>();
            }

            string normalizedBuilding = building.Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(x => x.Building == normalizedBuilding)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Adres bilgisinde belirtilen ifadeyi
        /// içeren konum kayıtlarını getirir.
        /// </summary>
        public async Task<List<SiteLocation>>
            SearchByAddressAsync(
                string address,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return new List<SiteLocation>();
            }

            string searchText = address.Trim();

            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.Address != null &&
                    x.Address.Contains(searchText))
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Enlem ve boylam bilgileri tanımlı
        /// aktif konumları getirir.
        ///
        /// Harita üzerinde gösterilecek
        /// konumları listelemek için kullanılır.
        /// </summary>
        public async Task<List<SiteLocation>>
            GetLocationsWithCoordinatesAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Latitude != null &&
                    x.Longitude != null)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen konuma ait bütün iş
        /// taleplerini ilişkili iş türleriyle getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetWorkRequestsByLocationIdAsync(
                int locationId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequests
                .AsNoTracking()
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType)
                .Where(x => x.SiteLocationId == locationId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Konuma bağlı Yeni (Draft)
        /// durumundaki iş taleplerini getirir.
        ///
        /// WorkRequestStatusEnum:
        /// Draft = 1
        /// Rejected = 2
        /// ConvertedToWorkOrder = 3
        /// Cancelled = 4
        /// </summary>
        public async Task<List<WorkRequest>>
            GetNewWorkRequestsByLocationIdAsync(
                int locationId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequests
                .AsNoTracking()
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType)
                .Where(x =>
                    x.SiteLocationId == locationId &&
                    x.Status == WorkRequestStatusEnum.Draft)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Konum adının başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
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
                    x.Name == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Konuma bağlı herhangi bir
        /// iş talebi olup olmadığını kontrol eder.
        ///
        /// Bütün WorkRequestStatusEnum
        /// durumları bu sorguya dahildir.
        /// </summary>
        public async Task<bool> HasWorkRequestsAsync(
            int locationId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequests
                .AsNoTracking()
                .AnyAsync(
                    x => x.SiteLocationId == locationId,
                    cancellationToken);
        }

        /// <summary>
        /// Konuma bağlı henüz sonuçlandırılmamış
        /// Yeni (Draft) durumundaki iş taleplerini
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasNewWorkRequestsAsync(
            int locationId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequests
                .AsNoTracking()
                .AnyAsync(x =>
                    x.SiteLocationId == locationId &&
                    x.Status == WorkRequestStatusEnum.Draft,
                    cancellationToken);
        }

        /// <summary>
        /// Konumun aktiflik durumunu günceller.
        ///
        /// Kayıt fiziksel olarak silinmez.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var location = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (location == null)
            {
                return false;
            }

            if (location.IsActive == isActive)
            {
                return true;
            }

            location.IsActive = isActive;
            location.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
