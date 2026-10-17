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
    public class AssetRepository :
         GenericRepository<Asset>, IAssetRepository
    {
        public AssetRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif demirbaşları organizasyon, marka, model
        /// ve tür bilgileriyle getirir.
        /// </summary>
        public async Task<List<Asset>> GetActiveAssetsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .Where(x => x.Status == AssetStatusEnum.Active)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bütün demirbaşları organizasyon, marka,
        /// model, kategori ve tür bilgileriyle getirir.
        /// </summary>
        public async Task<List<Asset>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Demirbaşı detayları, kullanıcı atamaları,
        /// hareketleri, iş talepleri, iş emirleri
        /// ve bakım planlarıyla getirir.
        /// </summary>
        public async Task<Asset?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .Include(x => x.AssetUsers)
                    .ThenInclude(x => x.User)
                .Include(x => x.AssetMovements)
                .Include(x => x.WorkRequests)
                .Include(x => x.WorkOrders)
                .Include(x => x.PlannedMaintenances)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaş koduna göre mevcut kaydı getirir.
        /// </summary>
        public async Task<Asset?> GetByAssetCodeAsync(
            string assetCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                return null;
            }

            string normalizedCode = assetCode.Trim();

            return await GetAssetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.AssetCode == normalizedCode,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen organizasyona ait bütün
        /// demirbaşları getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .Where(x => x.UnitId == unitId)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen markaya bağlı modellerin
        /// demirbaşlarını getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .Where(x => x.AssetModel.BrandId == brandId)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen modele ait demirbaşları getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByModelIdAsync(
            int modelId,
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .Where(x => x.AssetModelId == modelId)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kategoriye ait demirbaşları,
        /// tür üzerinden filtreleyerek getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .Where(x =>
                    x.AssetKind.AssetCategoryId == categoryId)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen demirbaş türüne ait
        /// bütün kayıtları getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByKindIdAsync(
            int kindId,
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .Where(x => x.AssetKindId == kindId)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen AssetStatusEnum değerindeki
        /// demirbaşları getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByStatusAsync(
            AssetStatusEnum status,
            CancellationToken cancellationToken = default)
        {
            return await GetAssetDetailsQuery()
                .Where(x => x.Status == status)
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcıya halen atanmış demirbaşları
        /// kullanıcı atama kayıtları üzerinden getirir.
        /// </summary>
        public async Task<List<Asset>> GetAssetsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await GetAssetDetailsQuery()
                .Where(x => x.AssetUsers.Any(au =>
                    au.UserId == userId &&
                    au.StartDate <= now &&
                    au.EndDate == null))
                .OrderBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen gün içerisinde garanti süresi
        /// sona erecek demirbaşları getirir.
        /// Garantisi zaten bitmiş kayıtlar dahil edilmez.
        /// </summary>
        public async Task<List<Asset>>
            GetAssetsWithExpiringWarrantyAsync(
                int days,
                CancellationToken cancellationToken = default)
        {
            if (days < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(days),
                    "Gün sayısı negatif olamaz.");
            }

            DateTime today = DateTime.UtcNow.Date;
            DateTime endDate = today.AddDays(days);

            return await GetAssetDetailsQuery()
                .Where(x =>
                    x.WarrantyEndDate.HasValue &&
                    x.WarrantyEndDate.Value.Date >= today &&
                    x.WarrantyEndDate.Value.Date <= endDate)
                .OrderBy(x => x.WarrantyEndDate)
                .ThenBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Garanti bitiş tarihi geçmiş
        /// demirbaşları getirir.
        /// </summary>
        public async Task<List<Asset>>
            GetExpiredWarrantyAssetsAsync(
                CancellationToken cancellationToken = default)
        {
            DateTime today = DateTime.UtcNow.Date;

            return await GetAssetDetailsQuery()
                .Where(x =>
                    x.WarrantyEndDate.HasValue &&
                    x.WarrantyEndDate.Value.Date < today)
                .OrderBy(x => x.WarrantyEndDate)
                .ThenBy(x => x.AssetCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Demirbaş kodunun daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsAssetCodeAsync(
            string assetCode,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                return false;
            }

            string normalizedCode = assetCode.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetCode == normalizedCode &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Seri numarasının başka bir demirbaş
        /// tarafından kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsSerialNumberAsync(
            string serialNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                return false;
            }

            string normalizedSerialNumber = serialNumber.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.SerialNumber == normalizedSerialNumber &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Envanter numarasının başka bir demirbaşta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsInventoryNumberAsync(
            string inventoryNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(inventoryNumber))
            {
                return false;
            }

            string normalizedInventoryNumber = inventoryNumber.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.InventoryNumber == normalizedInventoryNumber &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaşa bağlı tamamlanmamış veya iptal
        /// edilmemiş iş emri bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrders
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetId == assetId &&
                    (
                        x.Status == WorkStatusEnum.New ||
                        x.Status == WorkStatusEnum.InProgress ||
                        x.Status == WorkStatusEnum.PendingApproval
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaşın halen bir kullanıcıya atanmış
        /// olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssignmentsAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await _context.AssetUsers
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetId == assetId &&
                    x.StartDate <= now &&
                    x.EndDate == null,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaşa ait aktif bir planlı bakım
        /// kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveMaintenancePlansAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            return await _context.PlannedMaintenances
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetId == assetId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaş durumunu günceller
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            AssetStatusEnum status,
            CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(typeof(AssetStatusEnum), status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    "Geçersiz demirbaş durumu.");
            }

            var asset = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (asset == null)
            {
                return false;
            }

            if (asset.Status == status)
            {
                return true;
            }

            asset.Status = status;
            asset.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Organizasyon, marka, model, kategori
        /// ve tür bilgilerini içeren ortak sorgudur.
        /// </summary>
        private IQueryable<Asset> GetAssetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.Unit)
                .Include(x => x.AssetModel)
                    .ThenInclude(x => x.Brand)
                .Include(x => x.AssetKind)
                    .ThenInclude(x => x.AssetCategory);
        }
    }
}



