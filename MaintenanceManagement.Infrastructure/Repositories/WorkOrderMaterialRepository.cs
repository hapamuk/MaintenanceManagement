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
    public class WorkOrderMaterialRepository :
        GenericRepository<WorkOrderMaterial>,
        IWorkOrderMaterialRepository
    {
        public WorkOrderMaterialRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün malzeme kullanım kayıtlarını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen malzeme kullanım kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkOrderMaterial?>
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
        /// İş emrine ait bütün malzeme
        /// kullanım kayıtlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.Material.Name)
                .ThenBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen malzemenin kullanıldığı
        /// iş emri kayıtlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.MaterialId == materialId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen ambardan iş emirlerine
        /// verilen malzemeleri getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WarehouseId == warehouseId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde kullanılan belirli
        /// malzemeye ait kayıtları getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetByWorkOrderAndMaterialIdAsync(
                int workOrderId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.MaterialId == materialId)
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde belirli ambardan
        /// kullanılan malzemeleri getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetByWorkOrderAndWarehouseIdAsync(
                int workOrderId,
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.WarehouseId == warehouseId)
                .OrderBy(x => x.Material.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Stok hareketine bağlı malzeme
        /// kullanım kaydını getirir.
        /// </summary>
        public async Task<WorkOrderMaterial?>
            GetByStockMovementIdAsync(
                int stockMovementId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.StockMovementId == stockMovementId,
                    cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerinde
        /// kullanılan malzemeleri getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrder.ResponsibleUnitId == unitId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen takıma atanmış iş emirlerinde
        /// kullanılan malzemeleri getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrder.TeamId == teamId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen UTC tarih aralığında
        /// oluşturulmuş kullanım kayıtlarını getirir.
        ///
        /// Filtreleme BaseEntity.CreatedAt
        /// alanına göre yapılır.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(startDateUtc, endDateUtc);

            return await GetDetailsQuery()
                .Where(x =>
                    x.CreatedAt >= startDateUtc &&
                    x.CreatedAt <= endDateUtc)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrine ait malzeme kullanım
        /// kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasMaterialsByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(
                    x => x.WorkOrderId == workOrderId,
                    cancellationToken);
        }

        /// <summary>
        /// Stok hareketinin başka bir malzeme
        /// kullanım kaydına bağlanıp bağlanmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> ExistsStockMovementLinkAsync(
            int stockMovementId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (stockMovementId <= 0)
            {
                return false;
            }

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.StockMovementId == stockMovementId &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// İş emrindeki toplam malzeme
        /// kullanım kayıt sayısını getirir.
        ///
        /// Aynı malzeme farklı çıkışlarla
        /// birden fazla kayıtta bulunabilir.
        /// </summary>
        public async Task<int>
            GetMaterialRecordCountByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(
                    x => x.WorkOrderId == workOrderId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinde belirtilen malzemenin
        /// toplam kullanım miktarını getirir.
        ///
        /// Aynı malzemenin farklı ambarlardan
        /// yapılan çıkışları birlikte toplanır.
        /// </summary>
        public async Task<decimal>
            GetTotalQuantityByWorkOrderAndMaterialIdAsync(
                int workOrderId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            decimal? totalQuantity = await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.MaterialId == materialId)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return totalQuantity ?? 0m;
        }

        /// <summary>
        /// Belirtilen malzemenin bütün iş emirlerinde
        /// kaydedilmiş toplam kullanımını getirir.
        /// </summary>
        public async Task<decimal>
            GetTotalQuantityByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            decimal? totalQuantity = await _dbSet
                .AsNoTracking()
                .Where(x => x.MaterialId == materialId)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return totalQuantity ?? 0m;
        }

        /// <summary>
        /// İş emrinde maliyeti bilinen malzemelerin
        /// toplam tutarını hesaplar.
        ///
        /// Hesaplama:
        /// Quantity * UnitCost
        ///
        /// UnitCost null olan kayıtlar toplam
        /// maliyete dahil edilmez.
        /// </summary>
        public async Task<decimal>
            GetTotalCostByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            decimal? totalCost = await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.UnitCost.HasValue)
                .SumAsync(
                    x => (decimal?)(x.Quantity *
                        x.UnitCost!.Value),
                    cancellationToken);

            return totalCost ?? 0m;
        }

        /// <summary>
        /// İş emrindeki belirli malzemenin
        /// toplam bilinen maliyetini hesaplar.
        /// </summary>
        public async Task<decimal>
            GetTotalCostByWorkOrderAndMaterialIdAsync(
                int workOrderId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            decimal? totalCost = await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.MaterialId == materialId &&
                    x.UnitCost.HasValue)
                .SumAsync(
                    x => (decimal?)(x.Quantity *
                        x.UnitCost!.Value),
                    cancellationToken);

            return totalCost ?? 0m;
        }

        /// <summary>
        /// İş emrinde birim maliyeti
        /// bulunmayan malzeme var mı?
        /// </summary>
        public async Task<bool> HasMaterialsWithoutUnitCostAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    !x.UnitCost.HasValue,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinde henüz birim maliyeti
        /// belirlenmemiş malzemeleri getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetMaterialsWithoutUnitCostAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    !x.UnitCost.HasValue)
                .OrderBy(x => x.Material.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Daha önce oluşturulmuş geçerli bir
        /// stok çıkış hareketine bağlı iş emri
        /// malzeme kullanım kaydını oluşturur.
        ///
        /// Bu metot WarehouseStock.Quantity
        /// değerini değiştirmez.
        /// </summary>
        public async Task<WorkOrderMaterial>
            RegisterIssuedMaterialAsync(
                WorkOrderMaterial workOrderMaterial,
                CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(workOrderMaterial);

            if (workOrderMaterial.Id != 0)
            {
                throw new ArgumentException(
                    "Yeni malzeme kullanım kaydının " +
                    "ID değeri sıfır olmalıdır.",
                    nameof(workOrderMaterial));
            }

            if (workOrderMaterial.WorkOrderId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(workOrderMaterial.WorkOrderId),
                    "Geçerli bir iş emri belirtilmelidir.");
            }

            if (workOrderMaterial.MaterialId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(workOrderMaterial.MaterialId),
                    "Geçerli bir malzeme belirtilmelidir.");
            }

            if (workOrderMaterial.WarehouseId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(workOrderMaterial.WarehouseId),
                    "Geçerli bir ambar belirtilmelidir.");
            }

            if (workOrderMaterial.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(workOrderMaterial.Quantity),
                    "Kullanılan malzeme miktarı " +
                    "sıfırdan büyük olmalıdır.");
            }

            if (workOrderMaterial.UnitCost.HasValue &&
                workOrderMaterial.UnitCost.Value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(workOrderMaterial.UnitCost),
                    "Malzeme birim maliyeti negatif olamaz.");
            }

            // Kullanım kaydı mevcut bir stok
            // çıkış hareketine bağlanmalıdır.
            if (!workOrderMaterial.StockMovementId.HasValue ||
                workOrderMaterial.StockMovementId.Value <= 0)
            {
                throw new InvalidOperationException(
                    "Malzeme kullanım kaydı için " +
                    "geçerli bir stok çıkış hareketi zorunludur.");
            }

            // İş emrinin mevcut durumunu kontrol et.
            var workOrder = await _context.Set<WorkOrder>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == workOrderMaterial.WorkOrderId,
                    cancellationToken);

            if (workOrder == null)
            {
                throw new KeyNotFoundException(
                    "İş emri bulunamadı.");
            }

            if (workOrder.Status != WorkStatusEnum.InProgress)
            {
                throw new InvalidOperationException(
                    "Yalnızca devam eden iş emirlerine " +
                    "malzeme kullanım kaydı eklenebilir.");
            }

            // Malzemenin aktif olduğunu kontrol et.
            bool materialExists = await _context.Set<Material>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == workOrderMaterial.MaterialId &&
                    x.IsActive,
                    cancellationToken);

            if (!materialExists)
            {
                throw new KeyNotFoundException(
                    "Aktif malzeme kaydı bulunamadı.");
            }

            // Ambara erişim ve organizasyon yetkisi
            // Application servisinde ayrıca doğrulanır.
            bool warehouseExists = await _context.Set<Warehouse>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == workOrderMaterial.WarehouseId &&
                    x.IsActive,
                    cancellationToken);

            if (!warehouseExists)
            {
                throw new KeyNotFoundException(
                    "Aktif ambar kaydı bulunamadı.");
            }

            int stockMovementId =
                workOrderMaterial.StockMovementId.Value;

            // Stok hareketinin başka bir iş emri
            // malzeme kaydında kullanılmasını engelle.
            bool isMovementLinked =
                await ExistsStockMovementLinkAsync(
                    stockMovementId,
                    cancellationToken: cancellationToken);

            if (isMovementLinked)
            {
                throw new InvalidOperationException(
                    "Bu stok hareketi başka bir malzeme " +
                    "kullanım kaydına bağlanmış.");
            }

            // Mevcut stok hareketini doğrula.
            var stockMovement = await _context
                .Set<StockMovement>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == stockMovementId,
                    cancellationToken);

            if (stockMovement == null)
            {
                throw new KeyNotFoundException(
                    "Stok hareketi bulunamadı.");
            }

            // Yalnızca stok çıkış hareketi
            // iş emri malzeme kullanımına bağlanabilir.
            if (stockMovement.MovementType !=
                StockMovementTypeEnum.Out)
            {
                throw new InvalidOperationException(
                    "İş emri malzeme kaydı yalnızca " +
                    "stok çıkış hareketine bağlanabilir.");
            }

            if (stockMovement.MaterialId !=
                workOrderMaterial.MaterialId)
            {
                throw new InvalidOperationException(
                    "Stok hareketindeki malzeme ile " +
                    "iş emrinde kullanılan malzeme farklı.");
            }

            if (stockMovement.WarehouseId !=
                workOrderMaterial.WarehouseId)
            {
                throw new InvalidOperationException(
                    "Stok hareketindeki ambar ile " +
                    "iş emri malzeme ambarı farklı.");
            }

            // Bir stok çıkış hareketinin bir
            // kullanım satırına karşılık geldiği
            // iş kuralını uyguluyoruz.
            if (stockMovement.Quantity !=
                workOrderMaterial.Quantity)
            {
                throw new InvalidOperationException(
                    "Stok çıkış miktarı ile iş emrinde " +
                    "kullanılan miktar eşleşmiyor.");
            }

            DateTime now = DateTime.UtcNow;

            workOrderMaterial.Description =
                string.IsNullOrWhiteSpace(workOrderMaterial.Description)
                    ? null
                    : workOrderMaterial.Description.Trim();

            workOrderMaterial.CreatedAt = now;
            workOrderMaterial.UpdatedAt = null;

            await _dbSet.AddAsync(
                workOrderMaterial,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            return workOrderMaterial;
        }

        /// <summary>
        /// Tarih aralığının UTC formatında ve
        /// kronolojik olarak geçerli olduğunu doğrular.
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

        /// <summary>
        /// Malzeme kullanım kayıtlarını iş emri,
        /// sorumlu organizasyon, takım, malzeme,
        /// ölçü birimi, kategori, ambar ve stok
        /// hareketi detaylarıyla getirir.
        /// </summary>
        private IQueryable<WorkOrderMaterial> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş emri ve sorumlu organizasyon
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.ResponsibleUnit)

                // İş emrinin takımı
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.Team)

                // İş emrinin iş alt türü
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.WorkSubType)

                // Kullanılan malzeme ve ölçü birimi
                .Include(x => x.Material)
                    .ThenInclude(x => x.MeasurementUnit)

                // Malzeme ve malzeme kategorisi
                .Include(x => x.Material)
                    .ThenInclude(x => x.MaterialCategory)

                // Malzemenin alındığı ambar
                .Include(x => x.Warehouse)

                // İlişkili stok çıkış hareketi
                .Include(x => x.StockMovement);
        }
    }
}
