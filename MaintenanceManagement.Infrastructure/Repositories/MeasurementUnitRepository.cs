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
    public class MeasurementUnitRepository :
       GenericRepository<MeasurementUnit>,
       IMeasurementUnitRepository
    {
        public MeasurementUnitRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün ölçü birimlerini ilişkili
        /// malzemeleriyle birlikte getirir.
        /// </summary>
        public async Task<List<MeasurementUnit>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ölçü birimini ID bilgisine göre
        /// malzemeleriyle birlikte getirir.
        /// </summary>
        public async Task<MeasurementUnit?>
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
        /// Aktif ölçü birimlerini
        /// isimlerine göre sıralı getirir.
        /// </summary>
        public async Task<List<MeasurementUnit>>
            GetActiveMeasurementUnitsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Pasif ölçü birimlerini getirir.
        /// </summary>
        public async Task<List<MeasurementUnit>>
            GetInactiveMeasurementUnitsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ölçü biriminin adına göre
        /// mevcut kaydı getirir.
        /// </summary>
        public async Task<MeasurementUnit?>
            GetByNameAsync(
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
        /// Ölçü biriminin sembolüne göre
        /// mevcut kaydı getirir.
        /// </summary>
        public async Task<MeasurementUnit?>
            GetBySymbolAsync(
                string symbol,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return null;
            }

            string normalizedSymbol = symbol.Trim();

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Symbol == normalizedSymbol,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen malzemenin
        /// ölçü birimini getirir.
        /// </summary>
        public async Task<MeasurementUnit?>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Materials.Any(
                        material => material.Id == materialId),
                    cancellationToken);
        }

        /// <summary>
        /// Ölçü birimi adı veya sembolü
        /// içinde belirtilen ifadeyi arar.
        ///
        /// onlyActive true ise yalnızca
        /// aktif ölçü birimleri listelenir.
        /// </summary>
        public async Task<List<MeasurementUnit>>
            SearchByNameOrSymbolAsync(
                string searchText,
                bool onlyActive = false,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new List<MeasurementUnit>();
            }

            string normalizedSearchText = searchText.Trim();

            IQueryable<MeasurementUnit> query = _dbSet
                .AsNoTracking();

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            query = query.Where(x =>
                x.Name.Contains(normalizedSearchText) ||
                x.Symbol.Contains(normalizedSearchText));

            return await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// En az bir malzemeyle ilişkili
        /// ölçü birimlerini getirir.
        ///
        /// Aktif ve pasif malzemeler
        /// ilişki kontrolüne dahildir.
        /// </summary>
        public async Task<List<MeasurementUnit>>
            GetUnitsWithMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.Materials.Any())
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Hiçbir malzeme kaydıyla ilişkili
        /// olmayan ölçü birimlerini getirir.
        /// </summary>
        public async Task<List<MeasurementUnit>>
            GetUnitsWithoutMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => !x.Materials.Any())
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen ölçü birimini kullanan
        /// aktif ve pasif bütün malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>>
            GetMaterialsByMeasurementUnitIdAsync(
                int measurementUnitId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .Where(x =>
                    x.MeasurementUnitId == measurementUnitId)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen ölçü birimini kullanan
        /// aktif malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>>
            GetActiveMaterialsByMeasurementUnitIdAsync(
                int measurementUnitId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .Where(x =>
                    x.MeasurementUnitId == measurementUnitId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ölçü birimi adının başka bir
        /// kayıtta kullanılıp kullanılmadığını kontrol eder.
        ///
        /// Güncelleme sırasında excludedId
        /// ile mevcut kayıt hariç tutulabilir.
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
        /// Ölçü birimi sembolünün başka bir
        /// kayıtta kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsSymbolAsync(
            string symbol,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                return false;
            }

            string normalizedSymbol = symbol.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Symbol == normalizedSymbol &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Ölçü birimini kullanan herhangi
        /// bir malzeme olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasMaterialsAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MeasurementUnitId == measurementUnitId,
                    cancellationToken);
        }

        /// <summary>
        /// Ölçü birimini kullanan aktif
        /// malzeme olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveMaterialsAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MeasurementUnitId == measurementUnitId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Ölçü birimini kullanan aktif ve pasif
        /// bütün malzemelerin sayısını getirir.
        /// </summary>
        public async Task<int> GetMaterialCountAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .CountAsync(x =>
                    x.MeasurementUnitId == measurementUnitId,
                    cancellationToken);
        }

        /// <summary>
        /// Ölçü birimini kullanan aktif
        /// malzemelerin sayısını getirir.
        /// </summary>
        public async Task<int> GetActiveMaterialCountAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .CountAsync(x =>
                    x.MeasurementUnitId == measurementUnitId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Ölçü biriminin aktiflik durumunu değiştirir.
        ///
        /// Ölçü birimi aktif malzemeler tarafından
        /// kullanılıyorsa pasife alma işlemi engellenir.
        ///
        /// Kayıt fiziksel olarak silinmez.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var measurementUnit = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (measurementUnit == null)
            {
                return false;
            }

            // Zaten istenen durumda ise
            // tekrar veritabanı işlemi yapma.
            if (measurementUnit.IsActive == isActive)
            {
                return true;
            }

            // Pasife alınacak ölçü birimi
            // aktif malzemelerde kullanılıyor mu?
            if (!isActive)
            {
                bool hasActiveMaterials =
                    await _context.Set<Material>()
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.MeasurementUnitId == id &&
                            x.IsActive,
                            cancellationToken);

                if (hasActiveMaterials)
                {
                    throw new InvalidOperationException(
                        "Bu ölçü birimi aktif malzemeler " +
                        "tarafından kullanılmaktadır. " +
                        "Ölçü birimi pasife alınamaz.");
                }
            }

            measurementUnit.IsActive = isActive;
            measurementUnit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Ölçü birimlerini ilgili malzemelerle
        /// birlikte getiren ortak sorgudur.
        ///
        /// Koleksiyon yüklemesinde AsSplitQuery
        /// kullanılarak ayrı SQL sorguları oluşturulur.
        /// </summary>
        private IQueryable<MeasurementUnit> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.Materials)
                .AsSplitQuery();
        }
    }
}
