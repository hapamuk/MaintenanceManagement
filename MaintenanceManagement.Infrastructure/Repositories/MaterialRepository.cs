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
    public class MaterialRepository :
        GenericRepository<Material>,
        IMaterialRepository
    {
        public MaterialRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün malzemeleri ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<List<Material>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemeyi ID bilgisine göre detaylarıyla getirir.
        /// </summary>
        public async Task<Material?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Malzeme koduna göre mevcut kaydı getirir.
        /// </summary>
        public async Task<Material?> GetByMaterialCodeAsync(
            string materialCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(materialCode))
            {
                return null;
            }

            string normalizedCode = materialCode.Trim();

            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.MaterialCode == normalizedCode,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen ada sahip malzemeleri getirir.
        /// Malzeme isimlerinin aynı olmasına izin verilir.
        /// </summary>
        public async Task<List<Material>> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<Material>();
            }

            string normalizedName = name.Trim();

            return await GetDetailsQuery()
                .Where(x => x.Name == normalizedName)
                .OrderBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aktif malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>> GetActiveMaterialsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Pasif malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>> GetInactiveMaterialsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kategoriye bağlı aktif ve pasif
        /// bütün malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>> GetByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.MaterialCategoryId == categoryId)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen ölçü birimine bağlı
        /// bütün malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>> GetByMeasurementUnitIdAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.MeasurementUnitId == measurementUnitId)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona bağlı malzemeleri getirir.
        /// Ortak malzemeler dahil değildir.
        /// </summary>
        public async Task<List<Material>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UnitId == unitId)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Herhangi bir organizasyona özel
        /// olarak bağlanmamış ortak malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>> GetSharedMaterialsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UnitId == null)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona özel aktif malzemelerle
        /// ortak aktif malzemeleri birlikte getirir.
        /// </summary>
        public async Task<List<Material>> GetAvailableForUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.IsActive &&
                    (x.UnitId == unitId || x.UnitId == null))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kategori atanmamış malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>> GetWithoutCategoryAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.MaterialCategoryId == null)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzeme kodu, adı veya açıklamasında arama yapar.
        /// Organizasyon belirtilirse o birime ait
        /// malzemeler ve ortak malzemeler sorgulanır.
        /// </summary>
        public async Task<List<Material>> SearchMaterialsAsync(
            string searchText,
            int? unitId = null,
            bool onlyActive = true,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new List<Material>();
            }

            string search = searchText.Trim();

            IQueryable<Material> query = GetDetailsQuery();

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            if (unitId.HasValue)
            {
                query = query.Where(x =>
                    x.UnitId == unitId.Value ||
                    x.UnitId == null);
            }

            return await query
                .Where(x =>
                    x.MaterialCode.Contains(search) ||
                    x.Name.Contains(search) ||
                    (x.Description != null &&
                     x.Description.Contains(search)))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemenin aktif ve pasif bütün
        /// ambarlardaki stok kayıtlarını getirir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetWarehouseStocksByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .Where(x => x.MaterialId == materialId)
                .OrderBy(x => x.WarehouseId)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemenin belirtilen ambardaki
        /// stok kaydını getirir.
        /// </summary>
        public async Task<WarehouseStock?> GetWarehouseStockAsync(
            int materialId,
            int warehouseId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.MaterialId == materialId &&
                    x.WarehouseId == warehouseId,
                    cancellationToken);
        }

        /// <summary>
        /// Malzemeye ait son stok hareketlerini getirir.
        /// Varsayılan olarak en son 100 kayıt döndürülür.
        /// </summary>
        public async Task<List<StockMovement>>
            GetStockMovementsByMaterialIdAsync(
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await _context.Set<StockMovement>()
                .AsNoTracking()
                .Where(x => x.MaterialId == materialId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemenin iş emirlerindeki kullanım
        /// kayıtlarını en yeniden eskiye getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetWorkOrderMaterialsByMaterialIdAsync(
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await _context.Set<WorkOrderMaterial>()
                .AsNoTracking()
                .Include(x => x.WorkOrder)
                .Include(x => x.Warehouse)
                .Where(x => x.MaterialId == materialId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemenin bütün ambarlardaki toplam
        /// fiziksel stok bakiyesini hesaplar.
        ///
        /// Pasif ambarlar da toplama dahildir.
        /// </summary>
        public async Task<decimal> GetTotalStockQuantityAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            decimal? total = await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .Where(x => x.MaterialId == materialId)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return total ?? 0m;
        }

        /// <summary>
        /// Malzemenin yalnızca aktif ambarlardaki
        /// kullanılabilir stok miktarını hesaplar.
        /// </summary>
        public async Task<decimal> GetAvailableStockQuantityAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            decimal? total = await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .Where(stock =>
                    stock.MaterialId == materialId &&
                    _context.Set<Warehouse>().Any(warehouse =>
                        warehouse.Id == stock.WarehouseId &&
                        warehouse.IsActive))
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return total ?? 0m;
        }

        /// <summary>
        /// Malzemenin belirtilen ambardaki
        /// toplam stok miktarını getirir.
        ///
        /// Ambar pasif olsa bile kayıtlı
        /// stok miktarı sorgulanabilir.
        /// </summary>
        public async Task<decimal> GetStockQuantityInWarehouseAsync(
            int materialId,
            int warehouseId,
            CancellationToken cancellationToken = default)
        {
            decimal? total = await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .Where(x =>
                    x.MaterialId == materialId &&
                    x.WarehouseId == warehouseId)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return total ?? 0m;
        }

        /// <summary>
        /// Aktif malzemeler arasından kullanılabilir
        /// stok miktarı minimum seviyeye eşit veya
        /// altında olanları getirir.
        /// </summary>
        public async Task<List<Material>>
            GetCriticalStockMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetCriticalStockQuery()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona özel veya ortak aktif
        /// malzemeler arasındaki kritik stokları getirir.
        /// </summary>
        public async Task<List<Material>>
            GetCriticalStockMaterialsByUnitIdAsync(
                int unitId,
                bool includeShared = true,
                CancellationToken cancellationToken = default)
        {
            IQueryable<Material> query = GetCriticalStockQuery();

            if (includeShared)
            {
                query = query.Where(x =>
                    x.UnitId == unitId || x.UnitId == null);
            }
            else
            {
                query = query.Where(x => x.UnitId == unitId);
            }

            return await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aktif ambarlarda kullanılabilir
        /// stok miktarı sıfır veya altında olan
        /// aktif malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>>
            GetOutOfStockMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(material =>
                    material.IsActive &&

                    (
                        _context.Set<WarehouseStock>()
                            .Where(stock =>
                                stock.MaterialId == material.Id &&
                                _context.Set<Warehouse>()
                                    .Any(warehouse =>
                                        warehouse.Id ==
                                            stock.WarehouseId &&
                                        warehouse.IsActive))
                            .Sum(stock =>
                                (decimal?)stock.Quantity) ?? 0m
                    ) <= 0m)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kritik stok seviyesinde bulunan
        /// aktif malzemelerin sayısını getirir.
        /// </summary>
        public async Task<int> GetCriticalStockCountAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetCriticalStockQuery()
                .CountAsync(cancellationToken);
        }

        /// <summary>
        /// Malzeme kodunun başka bir kayıt
        /// tarafından kullanılıp kullanılmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> ExistsMaterialCodeAsync(
            string materialCode,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(materialCode))
            {
                return false;
            }

            string normalizedCode = materialCode.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MaterialCode == normalizedCode &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Malzemenin herhangi bir ambarda
        /// pozitif stok bakiyesi olup olmadığını
        /// kontrol eder.
        ///
        /// Pasif ambarlar da kontrol edilir.
        /// </summary>
        public async Task<bool> HasStockAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MaterialId == materialId &&
                    x.Quantity > 0m,
                    cancellationToken);
        }

        /// <summary>
        /// Malzemenin geçmiş stok hareketi
        /// olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasStockMovementsAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<StockMovement>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.MaterialId == materialId,
                    cancellationToken);
        }

        /// <summary>
        /// Malzemenin herhangi bir iş emrinde
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasWorkOrderUsageAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrderMaterial>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.MaterialId == materialId,
                    cancellationToken);
        }

        /// <summary>
        /// Ölçü birimi değişikliğinin geçmiş
        /// stok miktarlarının anlamını bozup
        /// bozmayacağını kontrol eder.
        ///
        /// Herhangi bir ambar stok kaydı,
        /// stok hareketi veya iş emri kullanım
        /// kaydı varsa değişikliğe izin verilmez.
        /// </summary>
        public async Task<bool> CanChangeMeasurementUnitAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            bool materialExists = await _dbSet
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == materialId,
                    cancellationToken);

            if (!materialExists)
            {
                return false;
            }

            bool hasStockRecord = await _context
                .Set<WarehouseStock>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.MaterialId == materialId,
                    cancellationToken);

            if (hasStockRecord)
            {
                return false;
            }

            bool hasMovements = await _context
                .Set<StockMovement>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.MaterialId == materialId,
                    cancellationToken);

            if (hasMovements)
            {
                return false;
            }

            bool hasWorkOrderUsage = await _context
                .Set<WorkOrderMaterial>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.MaterialId == materialId,
                    cancellationToken);

            return !hasWorkOrderUsage;
        }

        /// <summary>
        /// Malzemenin aktiflik durumunu değiştirir.
        ///
        /// Pozitif stok bakiyesi veya açık iş emrinde
        /// malzeme kullanımı bulunan kayıtlar
        /// pasife alınamaz.
        ///
        /// Aktifleştirme sırasında ölçü birimi,
        /// kategori ve organizasyon kontrol edilir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var material = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (material == null)
            {
                return false;
            }

            if (material.IsActive == isActive)
            {
                return true;
            }

            if (!isActive)
            {
                bool hasStock = await _context
                    .Set<WarehouseStock>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.MaterialId == id &&
                        x.Quantity > 0m,
                        cancellationToken);

                if (hasStock)
                {
                    throw new InvalidOperationException(
                        "Malzemenin ambarlarda stok bakiyesi " +
                        "bulunmaktadır. Pasife alınamaz.");
                }

                bool hasOpenWorkOrderUsage = await _context
                    .Set<WorkOrderMaterial>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.MaterialId == id &&
                        (
                            x.WorkOrder.Status == WorkStatusEnum.New ||
                            x.WorkOrder.Status ==
                                WorkStatusEnum.InProgress ||
                            x.WorkOrder.Status ==
                                WorkStatusEnum.PendingApproval
                        ),
                        cancellationToken);

                if (hasOpenWorkOrderUsage)
                {
                    throw new InvalidOperationException(
                        "Malzeme açık bir iş emrinde " +
                        "kullanılmaktadır. Pasife alınamaz.");
                }
            }
            else
            {
                bool validMeasurementUnit = await _context
                    .Set<MeasurementUnit>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == material.MeasurementUnitId &&
                        x.IsActive,
                        cancellationToken);

                if (!validMeasurementUnit)
                {
                    throw new InvalidOperationException(
                        "Malzemenin ölçü birimi aktif değildir.");
                }

                if (material.MaterialCategoryId.HasValue)
                {
                    bool validCategory = await _context
                        .Set<MaterialCategory>()
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.Id == material.MaterialCategoryId.Value &&
                            x.IsActive,
                            cancellationToken);

                    if (!validCategory)
                    {
                        throw new InvalidOperationException(
                            "Malzemenin kategorisi aktif değildir.");
                    }
                }

                if (material.UnitId.HasValue)
                {
                    bool validUnit = await _context
                        .Set<Unit>()
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.Id == material.UnitId.Value &&
                            x.IsActive,
                            cancellationToken);

                    if (!validUnit)
                    {
                        throw new InvalidOperationException(
                            "Malzemenin organizasyonu aktif değildir.");
                    }
                }
            }

            material.IsActive = isActive;
            material.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Malzemenin minimum stok
        /// seviyesini günceller.
        ///
        /// Güncel stok bakiyesini değiştirmez.
        /// </summary>
        public async Task<bool> UpdateMinimumStockLevelAsync(
            int id,
            decimal minimumStockLevel,
            CancellationToken cancellationToken = default)
        {
            if (minimumStockLevel < 0m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minimumStockLevel),
                    "Minimum stok seviyesi negatif olamaz.");
            }

            var material = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (material == null)
            {
                return false;
            }

            if (material.MinimumStockLevel == minimumStockLevel)
            {
                return true;
            }

            material.MinimumStockLevel = minimumStockLevel;
            material.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Aktif malzemeler arasında kullanılabilir
        /// stok miktarı minimum stok seviyesine
        /// eşit veya altında olan kayıtları sorgular.
        ///
        /// Stok yalnızca aktif ambarlardan hesaplanır.
        /// </summary>
        private IQueryable<Material> GetCriticalStockQuery()
        {
            return GetDetailsQuery()
                .Where(material =>
                    material.IsActive &&

                    (
                        _context.Set<WarehouseStock>()
                            .Where(stock =>
                                stock.MaterialId == material.Id &&
                                _context.Set<Warehouse>()
                                    .Any(warehouse =>
                                        warehouse.Id ==
                                            stock.WarehouseId &&
                                        warehouse.IsActive))
                            .Sum(stock =>
                                (decimal?)stock.Quantity) ?? 0m
                    ) <= material.MinimumStockLevel);
        }

        /// <summary>
        /// Malzemeleri ölçü birimi, kategori
        /// ve organizasyon detaylarıyla getirir.
        ///
        /// Stok kayıtları ayrıca sorgulanır.
        /// </summary>
        private IQueryable<Material> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.MeasurementUnit)
                .Include(x => x.MaterialCategory)
                .Include(x => x.Unit);
        }

        /// <summary>
        /// Listeleme sınırının geçerli olduğunu kontrol eder.
        /// </summary>
        private static void ValidateTake(int take)
        {
            if (take <= 0 || take > 1000)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(take),
                    "Kayıt sayısı 1 ile 1000 arasında olmalıdır.");
            }
        }

        /// <summary>
        /// Tarih değerlerinin UTC formatında
        /// ve kronolojik olarak geçerli
        /// olduğunu doğrular.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Tarih değerleri UTC formatında olmalıdır.");
            }

            if (startDateUtc > endDateUtc)
            {
                throw new ArgumentException(
                    "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }
        }
    }
}
