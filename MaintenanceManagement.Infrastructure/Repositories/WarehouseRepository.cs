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
    public class WarehouseRepository :
      GenericRepository<Warehouse>,
      IWarehouseRepository
    {
        public WarehouseRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün ambarları ilişkili
        /// organizasyon detaylarıyla getirir.
        /// </summary>
        public async Task<List<Warehouse>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambarı ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<Warehouse?>
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
        /// Ambar koduna göre mevcut kaydı getirir.
        /// </summary>
        public async Task<Warehouse?>
            GetByCodeAsync(
                string code,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            string normalizedCode = code.Trim();

            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Code == normalizedCode,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen ada sahip ambarları getirir.
        ///
        /// Farklı organizasyonlarda aynı
        /// ambar adı kullanılabilir.
        /// </summary>
        public async Task<List<Warehouse>>
            GetByNameAsync(
                string name,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<Warehouse>();
            }

            string normalizedName = name.Trim();

            return await GetDetailsQuery()
                .Where(x => x.Name == normalizedName)
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aktif ambarları getirir.
        /// </summary>
        public async Task<List<Warehouse>>
            GetActiveWarehousesAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Pasif ambarları getirir.
        /// </summary>
        public async Task<List<Warehouse>>
            GetInactiveWarehousesAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen organizasyona bağlı
        /// bütün ambarları getirir.
        ///
        /// Ortak ambarlar dahil değildir.
        /// </summary>
        public async Task<List<Warehouse>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UnitId == unitId)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona bağlı aktif
        /// ambarları getirir.
        /// </summary>
        public async Task<List<Warehouse>>
            GetActiveByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.UnitId == unitId &&
                    x.IsActive)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyondan bağımsız
        /// ortak ambarları getirir.
        ///
        /// onlyActive true ise yalnızca
        /// aktif ortak ambarlar listelenir.
        /// </summary>
        public async Task<List<Warehouse>>
            GetSharedWarehousesAsync(
                bool onlyActive = true,
                CancellationToken cancellationToken = default)
        {
            IQueryable<Warehouse> query =
                GetDetailsQuery()
                    .Where(x => x.UnitId == null);

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyonun kendi aktif
        /// ambarlarıyla ortak aktif
        /// ambarları birlikte getirir.
        /// </summary>
        public async Task<List<Warehouse>>
            GetAvailableForUnitAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.IsActive &&
                    (
                        x.UnitId == unitId ||
                        x.UnitId == null
                    ))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambar kodu, adı, konum veya açıklama
        /// bilgilerine göre arama yapar.
        ///
        /// Organizasyon belirtilirse o
        /// organizasyona ait ve ortak
        /// ambarlar sorgulanır.
        /// </summary>
        public async Task<List<Warehouse>>
            SearchWarehousesAsync(
                string searchText,
                int? unitId = null,
                bool onlyActive = true,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new List<Warehouse>();
            }

            string search = searchText.Trim();

            IQueryable<Warehouse> query = GetDetailsQuery();

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
                    x.Code.Contains(search) ||
                    x.Name.Contains(search) ||
                    (
                        x.Location != null &&
                        x.Location.Contains(search)
                    ) ||
                    (
                        x.Description != null &&
                        x.Description.Contains(search)
                    ))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambardaki bütün malzemelerin
        /// stok kayıtlarını getirir.
        ///
        /// Sıfır miktarlı stok kayıtları
        /// da listeye dahildir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await GetStocksQuery()
                .Where(x => x.WarehouseId == warehouseId)
                .OrderBy(x => x.Material.Name)
                .ThenBy(x => x.MaterialId)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambarda pozitif stok miktarına
        /// sahip kayıtları getirir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetPositiveStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await GetStocksQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.Quantity > 0m)
                .OrderBy(x => x.Material.Name)
                .ThenBy(x => x.MaterialId)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambar ve malzemeye ait
        /// mevcut stok kaydını getirir.
        /// </summary>
        public async Task<WarehouseStock?>
            GetStockByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await GetStocksQuery()
                .FirstOrDefaultAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId,
                    cancellationToken);
        }

        /// <summary>
        /// Ambarda pozitif stok bakiyesi bulunan
        /// aktif malzemeleri getirir.
        ///
        /// Ambar pasifse sonuç döndürülmez.
        /// </summary>
        public async Task<List<Material>>
            GetMaterialsInStockAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .Include(x => x.MeasurementUnit)
                .Include(x => x.MaterialCategory)
                .Where(material =>
                    material.IsActive &&

                    _context.Set<WarehouseStock>()
                        .Any(stock =>
                            stock.WarehouseId == warehouseId &&
                            stock.MaterialId == material.Id &&
                            stock.Quantity > 0m &&
                            stock.Warehouse.IsActive))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen malzemenin ilgili
        /// ambardaki stok miktarını getirir.
        ///
        /// Stok kaydı yoksa sıfır döndürür.
        /// </summary>
        public async Task<decimal>
            GetStockQuantityAsync(
                int warehouseId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            decimal? quantity =
                await _context.Set<WarehouseStock>()
                    .AsNoTracking()
                    .Where(x =>
                        x.WarehouseId == warehouseId &&
                        x.MaterialId == materialId)
                    .SumAsync(
                        x => (decimal?)x.Quantity,
                        cancellationToken);

            return quantity ?? 0m;
        }

        /// <summary>
        /// Ambardaki toplam stok
        /// kayıt sayısını getirir.
        ///
        /// Sıfır miktarlı kayıtlar dahildir.
        /// </summary>
        public async Task<int>
            GetStockRecordCountAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .CountAsync(
                    x => x.WarehouseId == warehouseId,
                    cancellationToken);
        }

        /// <summary>
        /// Ambarda pozitif stok bakiyesi
        /// bulunan farklı malzeme sayısını getirir.
        /// </summary>
        public async Task<int>
            GetMaterialInStockCountAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.Quantity > 0m)
                .Select(x => x.MaterialId)
                .Distinct()
                .CountAsync(cancellationToken);
        }

        /// <summary>
        /// Ambara ait son stok hareketlerini getirir.
        ///
        /// In ve Out türündeki bütün
        /// hareketler listeye dahildir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetStockMovementsByWarehouseIdAsync(
                int warehouseId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetMovementsQuery()
                .Where(x => x.WarehouseId == warehouseId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambardaki stok hareketlerini
        /// In veya Out türüne göre getirir.
        ///
        /// TransferGroupId bulunan transfer
        /// kayıtları da bu sorguya dahildir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetMovementsByTypeAsync(
                int warehouseId,
                StockMovementTypeEnum movementType,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateMovementType(movementType);
            ValidateTake(take);

            return await GetMovementsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MovementType == movementType)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambara ait transfer giriş ve
        /// çıkış hareketlerini getirir.
        ///
        /// TransferGroupId dolu olan In ve Out
        /// kayıtları transfer olarak kabul edilir.
        ///
        /// Enum içerisinde TransferIn veya
        /// TransferOut değeri kullanılmaz.
        /// </summary>
        public async Task<List<StockMovement>>
            GetTransferMovementsAsync(
                int warehouseId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetMovementsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.TransferGroupId.HasValue &&
                    (
                        x.MovementType == StockMovementTypeEnum.In ||
                        x.MovementType == StockMovementTypeEnum.Out
                    ))
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambardaki belirli malzemenin
        /// stok hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetMovementsByMaterialIdAsync(
                int warehouseId,
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetMovementsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambardaki belirtilen UTC tarih
        /// aralığındaki stok hareketlerini getirir.
        ///
        /// Başlangıç ve bitiş tarihleri dahildir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetMovementsByDateRangeAsync(
                int warehouseId,
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            return await GetMovementsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MovementDate >= startDateUtc &&
                    x.MovementDate <= endDateUtc)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambar kodunun başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        ///
        /// Güncelleme sırasında excludedId
        /// ile mevcut kayıt hariç tutulabilir.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            string normalizedCode = code.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Code == normalizedCode &&
                    (
                        !excludedId.HasValue ||
                        x.Id != excludedId.Value
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// Ambarda herhangi bir pozitif
        /// stok bakiyesi olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool>
            HasPositiveStockAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WarehouseStock>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.Quantity > 0m,
                    cancellationToken);
        }

        /// <summary>
        /// Ambarda geçmiş stok hareketi
        /// bulunup bulunmadığını kontrol eder.
        ///
        /// Geçmiş hareket kayıtları fiziksel
        /// olarak silinmemelidir.
        /// </summary>
        public async Task<bool>
            HasStockMovementsAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<StockMovement>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.WarehouseId == warehouseId,
                    cancellationToken);
        }

        /// <summary>
        /// Ambardan malzeme kullanılan açık
        /// iş emri bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool>
            HasOpenWorkOrderUsageAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrderMaterial>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WarehouseId == warehouseId &&
                    (
                        x.WorkOrder.Status == WorkStatusEnum.New ||
                        x.WorkOrder.Status ==
                            WorkStatusEnum.InProgress ||
                        x.WorkOrder.Status ==
                            WorkStatusEnum.PendingApproval
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// Ambarın aktiflik durumunu değiştirir.
        ///
        /// Pozitif stok bakiyesi veya açık iş
        /// emrinde malzeme kullanımı varsa
        /// pasife alma işlemi engellenir.
        ///
        /// Aktifleştirme sırasında bağlı
        /// organizasyon kontrol edilir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var warehouse = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (warehouse == null)
            {
                return false;
            }

            // Zaten istenen durumda ise
            // gereksiz güncelleme yapma.
            if (warehouse.IsActive == isActive)
            {
                return true;
            }

            if (!isActive)
            {
                // Ambarda pozitif stok bulunduğunda
                // pasife alma işlemine izin verilmez.
                bool hasPositiveStock =
                    await _context.Set<WarehouseStock>()
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.WarehouseId == id &&
                            x.Quantity > 0m,
                            cancellationToken);

                if (hasPositiveStock)
                {
                    throw new InvalidOperationException(
                        "Ambarda pozitif stok bakiyesi bulunmaktadır. " +
                        "Stoklar aktarılmadan ambar pasife alınamaz.");
                }

                // Açık iş emrinde malzeme
                // kullanımı bulunup bulunmadığını kontrol et.
                bool hasOpenWorkOrderUsage =
                    await _context.Set<WorkOrderMaterial>()
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.WarehouseId == id &&
                            (
                                x.WorkOrder.Status ==
                                    WorkStatusEnum.New ||
                                x.WorkOrder.Status ==
                                    WorkStatusEnum.InProgress ||
                                x.WorkOrder.Status ==
                                    WorkStatusEnum.PendingApproval
                            ),
                            cancellationToken);

                if (hasOpenWorkOrderUsage)
                {
                    throw new InvalidOperationException(
                        "Bu ambardan malzeme kullanılan açık " +
                        "iş emirleri bulunmaktadır. " +
                        "Ambar pasife alınamaz.");
                }
            }
            else
            {
                // Ortak ambarlarda UnitId null olabilir.
                // Organizasyon bağlıysa aktif olmalıdır.
                if (warehouse.UnitId.HasValue)
                {
                    bool validUnit =
                        await _context.Set<Unit>()
                            .AsNoTracking()
                            .AnyAsync(x =>
                                x.Id == warehouse.UnitId.Value &&
                                x.IsActive,
                                cancellationToken);

                    if (!validUnit)
                    {
                        throw new InvalidOperationException(
                            "Ambarın bağlı olduğu organizasyon " +
                            "bulunamadı veya aktif değildir.");
                    }
                }
            }

            warehouse.IsActive = isActive;

            warehouse.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Ambarları bağlı organizasyon
        /// detaylarıyla getiren ortak sorgudur.
        /// </summary>
        private IQueryable<Warehouse> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.Unit);
        }

        /// <summary>
        /// Ambar stok kayıtlarını malzeme,
        /// ölçü birimi ve kategori bilgileriyle
        /// birlikte getiren ortak sorgudur.
        /// </summary>
        private IQueryable<WarehouseStock> GetStocksQuery()
        {
            return _context.Set<WarehouseStock>()
                .AsNoTracking()

                // Malzeme ve ölçü birimi
                .Include(x => x.Material)
                    .ThenInclude(x => x.MeasurementUnit)

                // Malzeme kategorisi
                .Include(x => x.Material)
                    .ThenInclude(x => x.MaterialCategory);
        }

        /// <summary>
        /// Stok hareketlerini malzeme,
        /// ölçü birimi ve işlem yapan kullanıcı
        /// bilgileriyle getiren ortak sorgudur.
        /// </summary>
        private IQueryable<StockMovement> GetMovementsQuery()
        {
            return _context.Set<StockMovement>()
                .AsNoTracking()

                // Malzeme ve ölçü birimi
                .Include(x => x.Material)
                    .ThenInclude(x => x.MeasurementUnit)

                // Stok işlemini yapan kullanıcı
                .Include(x => x.PerformedByUser);
        }

        /// <summary>
        /// Stok hareket türünün yalnızca
        /// In veya Out olduğunu doğrular.
        /// </summary>
        private static void ValidateMovementType(
            StockMovementTypeEnum movementType)
        {
            if (movementType != StockMovementTypeEnum.In &&
                movementType != StockMovementTypeEnum.Out)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(movementType),
                    "Stok hareket türü yalnızca In veya Out olabilir.");
            }
        }

        /// <summary>
        /// Listeleme sınırının geçerli
        /// olduğunu kontrol eder.
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
        /// Başlangıç ve bitiş tarihlerinin
        /// UTC formatında ve kronolojik
        /// olarak geçerli olduğunu doğrular.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Başlangıç ve bitiş tarihleri UTC olmalıdır.");
            }

            if (startDateUtc > endDateUtc)
            {
                throw new ArgumentException(
                    "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }
        }
    }
}
