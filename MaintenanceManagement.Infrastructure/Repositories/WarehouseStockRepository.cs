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
    public class WarehouseStockRepository :
        GenericRepository<WarehouseStock>,
        IWarehouseStockRepository
    {
        public WarehouseStockRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün ambar stok kayıtlarını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Warehouse.Name)
                .ThenBy(x => x.Material.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen ambar stok kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WarehouseStock?>
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
        /// Ambar ve malzeme bilgisine göre
        /// stok kaydını getirir.
        /// </summary>
        public async Task<WarehouseStock?>
            GetByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId,
                    cancellationToken);
        }

        /// <summary>
        /// Ambardaki bütün stok kayıtlarını
        /// malzeme adına göre sıralı getirir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WarehouseId == warehouseId)
                .OrderBy(x => x.Material.Name)
                .ThenBy(x => x.MaterialId)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemenin bütün ambarlardaki
        /// stok kayıtlarını getirir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.MaterialId == materialId)
                .OrderBy(x => x.Warehouse.Name)
                .ThenBy(x => x.WarehouseId)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambarda pozitif miktara sahip
        /// stok kayıtlarını getirir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetPositiveStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.Quantity > 0m)
                .OrderBy(x => x.Material.Name)
                .ThenBy(x => x.MaterialId)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemenin pozitif stok miktarı
        /// bulunan ambar kayıtlarını getirir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetPositiveStocksByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.MaterialId == materialId &&
                    x.Quantity > 0m)
                .OrderBy(x => x.Warehouse.Name)
                .ThenBy(x => x.WarehouseId)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemenin aktif ambarlardaki
        /// stok kayıtlarını getirir.
        ///
        /// Sıfır miktarlı kayıtlar da dahildir.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetActiveWarehouseStocksByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.MaterialId == materialId &&
                    x.Warehouse.IsActive)
                .OrderBy(x => x.Warehouse.Name)
                .ThenBy(x => x.WarehouseId)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyonun kendi aktif ambarları ve
        /// ortak aktif ambarlarındaki stokları getirir.
        ///
        /// Yalnızca aktif malzemeler listelenir.
        /// Malzemenin organizasyonu da dikkate alınır.
        /// </summary>
        public async Task<List<WarehouseStock>>
            GetAvailableStocksByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.Warehouse.IsActive &&

                    (x.Warehouse.UnitId == unitId ||
                     x.Warehouse.UnitId == null) &&

                    x.Material.IsActive &&

                    (x.Material.UnitId == unitId ||
                     x.Material.UnitId == null))
                .OrderBy(x => x.Warehouse.Name)
                .ThenBy(x => x.Material.Name)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen ambar ve malzemenin
        /// mevcut stok miktarını getirir.
        ///
        /// Stok kaydı yoksa sıfır döndürür.
        /// </summary>
        public async Task<decimal> GetStockQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            decimal? quantity = await _dbSet
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
        /// Malzemenin aktif ve pasif bütün
        /// ambarlardaki stok miktarını hesaplar.
        /// </summary>
        public async Task<decimal>
            GetTotalStockQuantityByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            decimal? quantity = await _dbSet
                .AsNoTracking()
                .Where(x => x.MaterialId == materialId)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return quantity ?? 0m;
        }

        /// <summary>
        /// Malzemenin yalnızca aktif ambarlardaki
        /// kullanılabilir stok miktarını hesaplar.
        ///
        /// Malzeme aktif değilse kullanılabilir
        /// miktar sıfır olarak değerlendirilir.
        /// </summary>
        public async Task<decimal>
            GetAvailableStockQuantityByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            decimal? quantity = await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.MaterialId == materialId &&
                    x.Material.IsActive &&
                    x.Warehouse.IsActive)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return quantity ?? 0m;
        }

        /// <summary>
        /// Ambardaki bütün stok satırlarının
        /// sayısını getirir.
        ///
        /// Sıfır miktarlı satırlar da dahildir.
        /// </summary>
        public async Task<int>
            GetStockRecordCountByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(
                    x => x.WarehouseId == warehouseId,
                    cancellationToken);
        }

        /// <summary>
        /// Ambarda pozitif stok bakiyesi bulunan
        /// farklı malzemelerin sayısını getirir.
        /// </summary>
        public async Task<int>
            GetPositiveMaterialCountByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.Quantity > 0m)
                .Select(x => x.MaterialId)
                .Distinct()
                .CountAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanılabilir stok miktarı minimum
        /// seviyeye eşit veya altında olan
        /// aktif malzemeleri getirir.
        ///
        /// Hesaplama malzeme bazında yapılır,
        /// bütün aktif ambarlar birlikte değerlendirilir.
        /// </summary>
        public async Task<List<Material>>
            GetCriticalStockMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .Include(x => x.MeasurementUnit)
                .Include(x => x.MaterialCategory)
                .Where(material =>
                    material.IsActive &&

                    (
                        _dbSet
                            .Where(stock =>
                                stock.MaterialId == material.Id &&
                                stock.Warehouse.IsActive)
                            .Sum(stock =>
                                (decimal?)stock.Quantity) ?? 0m

                    ) <= material.MinimumStockLevel)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aktif ambarlarda kullanılabilir stok
        /// miktarı sıfır veya altında olan
        /// aktif malzemeleri getirir.
        /// </summary>
        public async Task<List<Material>>
            GetOutOfStockMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<Material>()
                .AsNoTracking()
                .Include(x => x.MeasurementUnit)
                .Include(x => x.MaterialCategory)
                .Where(material =>
                    material.IsActive &&

                    (
                        _dbSet
                            .Where(stock =>
                                stock.MaterialId == material.Id &&
                                stock.Warehouse.IsActive)
                            .Sum(stock =>
                                (decimal?)stock.Quantity) ?? 0m

                    ) <= 0m)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aynı ambarda aynı malzemenin
        /// stok kaydı bulunup bulunmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> ExistsStockRecordAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen ambar ve malzeme için
        /// pozitif stok miktarı bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasPositiveStockAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId &&
                    x.Quantity > 0m,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen miktarda stok çıkışını
        /// karşılayabilecek stok bulunup
        /// bulunmadığını kontrol eder.
        ///
        /// Ambar ve malzeme aktif olmalıdır.
        /// </summary>
        public async Task<bool> HasSufficientStockAsync(
            int warehouseId,
            int materialId,
            decimal requiredQuantity,
            CancellationToken cancellationToken = default)
        {
            if (requiredQuantity <= 0m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(requiredQuantity),
                    "İstenen miktar sıfırdan büyük olmalıdır.");
            }

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId &&

                    x.Warehouse.IsActive &&
                    x.Material.IsActive &&

                    x.Quantity >= requiredQuantity,
                    cancellationToken);
        }

        /// <summary>
        /// Ambarda herhangi bir malzemenin
        /// pozitif stok bakiyesi olup olmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool>
            HasAnyPositiveStockByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.Quantity > 0m,
                    cancellationToken);
        }

        /// <summary>
        /// Malzemenin bütün ambarlardaki
        /// son stok hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetMovementsByMaterialIdAsync(
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetMovementsQuery()
                .Where(x => x.MaterialId == materialId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirli ambar ve malzemenin
        /// son stok hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetMovementsByWarehouseAndMaterialIdAsync(
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
        /// Ambar ve malzemenin en son gerçekleşen
        /// stok hareketi kaydını getirir.
        /// </summary>
        public async Task<StockMovement?> GetLatestMovementAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            return await GetMovementsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Aktif ambar ve aktif malzeme için
        /// başlangıç miktarı sıfır olan
        /// yeni stok kartı oluşturur.
        ///
        /// Aynı ambar ve malzeme için ikinci
        /// stok kartı oluşturulamaz.
        /// </summary>
        public async Task<WarehouseStock> CreateStockRecordAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            if (warehouseId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(warehouseId),
                    "Geçerli bir ambar belirtilmelidir.");
            }

            if (materialId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(materialId),
                    "Geçerli bir malzeme belirtilmelidir.");
            }

            bool warehouseExists = await _context.Set<Warehouse>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == warehouseId &&
                    x.IsActive,
                    cancellationToken);

            if (!warehouseExists)
            {
                throw new InvalidOperationException(
                    "Aktif ambar bulunamadı.");
            }

            bool materialExists = await _context.Set<Material>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == materialId &&
                    x.IsActive,
                    cancellationToken);

            if (!materialExists)
            {
                throw new InvalidOperationException(
                    "Aktif malzeme bulunamadı.");
            }

            bool stockExists = await ExistsStockRecordAsync(
                warehouseId,
                materialId,
                cancellationToken);

            if (stockExists)
            {
                throw new InvalidOperationException(
                    "Bu malzemenin ilgili ambarda " +
                    "stok kaydı zaten bulunuyor.");
            }

            DateTime now = DateTime.UtcNow;

            var warehouseStock = new WarehouseStock
            {
                WarehouseId = warehouseId,

                MaterialId = materialId,

                Quantity = 0m,

                LastMovementDate = null,

                CreatedAt = now,

                UpdatedAt = null

                // RowVersion SQL Server tarafından
                // otomatik üretilecektir.
            };

            await _dbSet.AddAsync(
                warehouseStock,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            return warehouseStock;
        }

        /// <summary>
        /// Mevcut stok miktarını belirtilen
        /// miktar kadar artırır.
        ///
        /// Stok giriş veya transfer giriş
        /// hareketiyle aynı transaction içinde
        /// kullanılmalıdır.
        /// </summary>
        public async Task<WarehouseStock> IncreaseStockAsync(
            int warehouseId,
            int materialId,
            decimal quantity,
            DateTime movementDateUtc,
            CancellationToken cancellationToken = default)
        {
            return await ChangeQuantityAsync(
                warehouseId,
                materialId,
                quantity,
                movementDateUtc,
                increase: true,
                cancellationToken);
        }

        /// <summary>
        /// Mevcut stok miktarını belirtilen
        /// miktar kadar azaltır.
        ///
        /// Stok çıkışı veya transfer çıkışı
        /// hareketiyle aynı transaction içinde
        /// kullanılmalıdır.
        ///
        /// Yetersiz stok durumunda hata oluşturur.
        /// </summary>
        public async Task<WarehouseStock> DecreaseStockAsync(
            int warehouseId,
            int materialId,
            decimal quantity,
            DateTime movementDateUtc,
            CancellationToken cancellationToken = default)
        {
            return await ChangeQuantityAsync(
                warehouseId,
                materialId,
                quantity,
                movementDateUtc,
                increase: false,
                cancellationToken);
        }

        /// <summary>
        /// Stok artırma ve azaltma işlemlerinin
        /// ortak doğrulama ve kaydetme metodudur.
        ///
        /// EF Core tarafından takip edilen Entity
        /// güncellenir ve RowVersion concurrency
        /// kontrolüyle kaydedilir.
        /// </summary>
        private async Task<WarehouseStock> ChangeQuantityAsync(
            int warehouseId,
            int materialId,
            decimal quantity,
            DateTime movementDateUtc,
            bool increase,
            CancellationToken cancellationToken)
        {
            if (warehouseId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(warehouseId),
                    "Geçerli bir ambar belirtilmelidir.");
            }

            if (materialId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(materialId),
                    "Geçerli bir malzeme belirtilmelidir.");
            }

            if (quantity <= 0m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Stok işlem miktarı sıfırdan büyük olmalıdır.");
            }

            ValidateMovementDate(movementDateUtc);

            // Tracking açık:
            // EF Core RowVersion kontrolünü
            // SaveChangesAsync sırasında uygulayacak.
            var stock = await _dbSet
                .Include(x => x.Warehouse)
                .Include(x => x.Material)
                .FirstOrDefaultAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId,
                    cancellationToken);

            if (stock == null)
            {
                throw new KeyNotFoundException(
                    "Ambar stok kaydı bulunamadı. " +
                    "Önce stok kartı oluşturulmalıdır.");
            }

            if (!stock.Warehouse.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif ambarda stok işlemi yapılamaz.");
            }

            if (!stock.Material.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif malzemeye stok işlemi yapılamaz.");
            }

            if (stock.Quantity < 0m)
            {
                throw new InvalidOperationException(
                    "Mevcut stok bakiyesi negatiftir. " +
                    "Önce stok mutabakatı yapılmalıdır.");
            }

            if (increase)
            {
                stock.Quantity = checked(stock.Quantity + quantity);
            }
            else
            {
                if (stock.Quantity < quantity)
                {
                    throw new InvalidOperationException(
                        "Yetersiz stok bakiyesi. " +
                        "İstenen miktarda çıkış yapılamaz.");
                }

                stock.Quantity -= quantity;
            }

            // Son stok hareketi tarihi geriye alınmaz.
            if (!stock.LastMovementDate.HasValue ||
                movementDateUtc > stock.LastMovementDate.Value)
            {
                stock.LastMovementDate = movementDateUtc;
            }

            stock.UpdatedAt = DateTime.UtcNow;

            // RowVersion concurrency token olarak
            // tanımlıysa EF Core çakışan güncellemelerde
            // DbUpdateConcurrencyException fırlatır.
            await _context.SaveChangesAsync(
                cancellationToken);

            return stock;
        }

        /// <summary>
        /// Ambar stok kayıtlarını ambar,
        /// organizasyon, malzeme, ölçü birimi
        /// ve kategori bilgileriyle getirir.
        /// </summary>
        private IQueryable<WarehouseStock> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // Ambar ve bağlı organizasyon
                .Include(x => x.Warehouse)
                    .ThenInclude(x => x.Unit)

                // Malzeme ve ölçü birimi
                .Include(x => x.Material)
                    .ThenInclude(x => x.MeasurementUnit)

                // Malzeme kategorisi
                .Include(x => x.Material)
                    .ThenInclude(x => x.MaterialCategory);
        }

        /// <summary>
        /// Stok hareketlerini ilgili ambar,
        /// malzeme ve ölçü birimi bilgileriyle getirir.
        /// </summary>
        private IQueryable<StockMovement> GetMovementsQuery()
        {
            return _context.Set<StockMovement>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Material)
                    .ThenInclude(x => x.MeasurementUnit);
        }

        /// <summary>
        /// Stok hareketi tarihinin UTC
        /// ve gelecekte olmayan bir değer
        /// olduğunu doğrular.
        /// </summary>
        private static void ValidateMovementDate(
            DateTime movementDateUtc)
        {
            if (movementDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Stok hareketi tarihi UTC olmalıdır.",
                    nameof(movementDateUtc));
            }

            if (movementDateUtc > DateTime.UtcNow)
            {
                throw new ArgumentException(
                    "Stok hareketi tarihi gelecekte olamaz.",
                    nameof(movementDateUtc));
            }
        }

        /// <summary>
        /// Stok hareketi listelemesinde
        /// istenen kayıt sayısını doğrular.
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
    }
}

