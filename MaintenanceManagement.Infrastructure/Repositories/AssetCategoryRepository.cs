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
    public class AssetCategoryRepository :
         GenericRepository<AssetCategory>,
         IAssetCategoryRepository
    {
        public AssetCategoryRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif demirbaş kategorilerini alfabetik
        /// sırayla getirir.
        /// </summary>
        public async Task<List<AssetCategory>>
            GetActiveCategoriesAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bütün demirbaş kategorilerini ilişkili
        /// türleriyle birlikte getirir.
        /// </summary>
        public async Task<List<AssetCategory>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Kinds)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategoriyi ilişkili
        /// demirbaş türleriyle getirir.
        /// </summary>
        public async Task<AssetCategory?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Kinds)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategoriye ait aktif ve pasif
        /// bütün demirbaş türlerini getirir.
        /// </summary>
        public async Task<List<AssetKind>>
            GetKindsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            return await _context.AssetKinds
                .AsNoTracking()
                .Where(x =>
                    x.AssetCategoryId == categoryId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategoriye ait aktif türleri getirir.
        /// Kategorinin de aktif olması gerekir.
        /// </summary>
        public async Task<List<AssetKind>>
            GetActiveKindsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            return await _context.AssetKinds
                .AsNoTracking()
                .Where(x =>
                    x.AssetCategoryId == categoryId &&
                    x.IsActive &&
                    x.AssetCategory.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kategoriye ait demirbaş türlerine bağlı
        /// bütün demirbaşları getirir.
        /// </summary>
        public async Task<List<Asset>>
            GetAssetsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .Include(x => x.AssetKind)
                .Include(x => x.AssetModel)
                    .ThenInclude(x => x.Brand)
                .Include(x => x.Unit)
                .Where(x =>
                    x.AssetKind.AssetCategoryId == categoryId)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kategori adının başka bir kayıt tarafından
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
        /// Kategoriye bağlı herhangi bir
        /// demirbaş türü bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasKindsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AssetKinds
                .AsNoTracking()
                .AnyAsync(
                    x => x.AssetCategoryId == categoryId,
                    cancellationToken);
        }

        /// <summary>
        /// Kategoriye bağlı aktif demirbaş türü
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveKindsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.AssetKinds
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetCategoryId == categoryId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Kategoriye ait türler üzerinden kayıtlı
        /// herhangi bir demirbaş olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssetsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetKind.AssetCategoryId == categoryId,
                    cancellationToken);
        }

        /// <summary>
        /// Kategoriye bağlı aktif durumdaki
        /// demirbaşları kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssetsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetKind.AssetCategoryId == categoryId &&
                    x.Status == AssetStatusEnum.Active,
                    cancellationToken);
        }

        /// <summary>
        /// Kategorinin aktiflik durumunu değiştirir
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var category = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (category == null)
            {
                return false;
            }

            if (category.IsActive == isActive)
            {
                return true;
            }

            category.IsActive = isActive;

            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
