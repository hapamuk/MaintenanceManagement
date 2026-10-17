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
    public class AssetKindRepository :
      GenericRepository<AssetKind>,
      IAssetKindRepository
    {
        public AssetKindRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif kategorilere bağlı aktif demirbaş
        /// türlerini kategori ve tür adına göre getirir.
        /// </summary>
        public async Task<List<AssetKind>> GetActiveKindsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.AssetCategory)
                .Where(x =>
                    x.IsActive &&
                    x.AssetCategory.IsActive)
                .OrderBy(x => x.AssetCategory.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Tüm demirbaş türlerini kategori ve
        /// bağlı demirbaşlarıyla birlikte getirir.
        /// </summary>
        public async Task<List<AssetKind>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.AssetCategory)
                .Include(x => x.Assets)
                .AsSplitQuery()
                .OrderBy(x => x.AssetCategory.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen demirbaş türünü kategori,
        /// demirbaş, model, marka ve birim
        /// bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<AssetKind?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.AssetCategory)
                .Include(x => x.Assets)
                    .ThenInclude(x => x.AssetModel)
                        .ThenInclude(x => x.Brand)
                .Include(x => x.Assets)
                    .ThenInclude(x => x.Unit)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategoriye ait aktif ve
        /// pasif bütün demirbaş türlerini getirir.
        /// </summary>
        public async Task<List<AssetKind>> GetKindsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.AssetCategory)
                .Where(x => x.AssetCategoryId == categoryId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategoriye ait yalnızca
        /// aktif demirbaş türlerini getirir.
        /// Kategori de aktif olmalıdır.
        /// </summary>
        public async Task<List<AssetKind>>
            GetActiveKindsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.AssetCategory)
                .Where(x =>
                    x.AssetCategoryId == categoryId &&
                    x.IsActive &&
                    x.AssetCategory.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kategori ve demirbaş türü adına göre
        /// mevcut kaydı getirir.
        /// Pasif kayıtlar da sorgulanır.
        /// </summary>
        public async Task<AssetKind?> GetByCategoryAndNameAsync(
            int categoryId,
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
                .Include(x => x.AssetCategory)
                .FirstOrDefaultAsync(
                    x => x.AssetCategoryId == categoryId &&
                         x.Name == normalizedName,
                    cancellationToken);
        }

        /// <summary>
        /// Aynı kategoride aynı demirbaş türü adının
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            int categoryId,
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
                    x.AssetCategoryId == categoryId &&
                    x.Name == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen demirbaş türüne bağlı
        /// bütün demirbaşları ilişkili bilgileriyle getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByKindIdAsync(
            int kindId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .Include(x => x.Unit)
                .Include(x => x.AssetModel)
                    .ThenInclude(x => x.Brand)
                .Where(x => x.AssetKindId == kindId)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen demirbaş türüne ait aktif durumdaki
        /// demirbaşları getirir.
        /// </summary>
        public async Task<List<Asset>>
            GetActiveAssetsByKindIdAsync(
                int kindId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .Include(x => x.Unit)
                .Include(x => x.AssetModel)
                    .ThenInclude(x => x.Brand)
                .Where(x =>
                    x.AssetKindId == kindId &&
                    x.Status == AssetStatusEnum.Active)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Demirbaş türüne bağlı kayıtlı herhangi
        /// bir demirbaş olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssetsAsync(
            int kindId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .AnyAsync(
                    x => x.AssetKindId == kindId,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaş türüne bağlı aktif durumda
        /// bir demirbaş olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssetsAsync(
            int kindId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetKindId == kindId &&
                    x.Status == AssetStatusEnum.Active,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaş türünün aktiflik durumunu
        /// günceller ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var assetKind = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (assetKind == null)
            {
                return false;
            }

            if (assetKind.IsActive == isActive)
            {
                return true;
            }

            assetKind.IsActive = isActive;
            assetKind.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
