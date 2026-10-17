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
    public class AssetModelRepository :
      GenericRepository<AssetModel>,
      IAssetModelRepository
    {
        public AssetModelRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif markalara bağlı aktif demirbaş modellerini
        /// marka ve model adına göre sıralayarak getirir.
        /// </summary>
        public async Task<List<AssetModel>> GetActiveModelsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Brand)
                .Where(x =>
                    x.IsActive &&
                    x.Brand.IsActive)
                .OrderBy(x => x.Brand.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bütün demirbaş modellerini marka ve
        /// ilişkili demirbaşlarıyla birlikte getirir.
        /// </summary>
        public async Task<List<AssetModel>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Brand)
                .Include(x => x.Assets)
                .AsSplitQuery()
                .OrderBy(x => x.Brand.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen modeli marka ve demirbaş
        /// detaylarıyla birlikte getirir.
        /// </summary>
        public async Task<AssetModel?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Brand)
                .Include(x => x.Assets)
                    .ThenInclude(x => x.Unit)
                .Include(x => x.Assets)
                    .ThenInclude(x => x.AssetKind)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen markaya ait aktif ve pasif
        /// bütün modelleri getirir.
        /// </summary>
        public async Task<List<AssetModel>> GetModelsByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Brand)
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
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Brand)
                .Where(x =>
                    x.BrandId == brandId &&
                    x.IsActive &&
                    x.Brand.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Marka ve model adına göre mevcut
        /// kayıt bilgisini getirir.
        /// Pasif modeller de sorgulanır.
        /// </summary>
        public async Task<AssetModel?> GetByBrandAndNameAsync(
            int brandId,
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
                .Include(x => x.Brand)
                .FirstOrDefaultAsync(
                    x => x.BrandId == brandId &&
                         x.Name == normalizedName,
                    cancellationToken);
        }

        /// <summary>
        /// Aynı marka içerisinde model adının daha önce
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            int brandId,
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
                    x.BrandId == brandId &&
                    x.Name == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen modele ait demirbaşları
        /// organizasyon ve demirbaş türüyle getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByModelIdAsync(
            int modelId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .Include(x => x.Unit)
                .Include(x => x.AssetKind)
                .Where(x => x.AssetModelId == modelId)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen modele ait kayıtlı
        /// demirbaş olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssetsAsync(
            int modelId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .AnyAsync(
                    x => x.AssetModelId == modelId,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen modele bağlı aktif demirbaş
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssetsAsync(
            int modelId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Assets
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetModelId == modelId &&
                    x.Status == AssetStatusEnum.Active,
                    cancellationToken);
        }

        /// <summary>
        /// Modelin aktiflik durumunu günceller
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var assetModel = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (assetModel == null)
            {
                return false;
            }

            if (assetModel.IsActive == isActive)
            {
                return true;
            }

            assetModel.IsActive = isActive;

            assetModel.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
