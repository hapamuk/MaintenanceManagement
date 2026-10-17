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
    public class BrandRepository :
        GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif markaları alfabetik sırayla getirir.
        /// </summary>
        public async Task<List<Brand>> GetActiveBrandsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bütün markaları ilişkili demirbaş modelleriyle
        /// birlikte getirir.
        /// </summary>
        public async Task<List<Brand>> GetAllWithModelsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Models)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen markayı ID bilgisine göre
        /// ilişkili modelleriyle birlikte getirir.
        /// </summary>
        public async Task<Brand?> GetByIdWithModelsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Models)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen markaya ait aktif ve pasif
        /// bütün demirbaş modellerini getirir.
        /// </summary>
        public async Task<List<AssetModel>> GetModelsByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AssetModels
                .AsNoTracking()
                .Where(x => x.BrandId == brandId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen markaya ait aktif modelleri getirir.
        /// Markanın da aktif olması gerekir.
        /// </summary>
        public async Task<List<AssetModel>>
            GetActiveModelsByBrandIdAsync(
                int brandId,
                CancellationToken cancellationToken = default)
        {
            return await _context.AssetModels
                .AsNoTracking()
                .Where(x =>
                    x.BrandId == brandId &&
                    x.IsActive &&
                    x.Brand.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Marka adının başka bir kayıt tarafından
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
        /// Markaya bağlı herhangi bir demirbaş modeli
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasModelsAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AssetModels
                .AsNoTracking()
                .AnyAsync(
                    x => x.BrandId == brandId,
                    cancellationToken);
        }

        /// <summary>
        /// Markaya bağlı aktif demirbaş modeli
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveModelsAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AssetModels
                .AsNoTracking()
                .AnyAsync(
                    x => x.BrandId == brandId &&
                         x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Markanın herhangi bir modeline bağlı
        /// kayıtlı demirbaş bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssetsAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .AnyAsync(
                    x => x.AssetModel.BrandId == brandId,
                    cancellationToken);
        }

        /// <summary>
        /// Markanın aktiflik durumunu günceller
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var brand = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (brand == null)
            {
                return false;
            }

            if (brand.IsActive == isActive)
            {
                return true;
            }

            brand.IsActive = isActive;

            brand.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
