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
    public class StockMovementRepository :
       GenericRepository<StockMovement>,
       IStockMovementRepository
    {
        public StockMovementRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün stok hareketlerini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Stok hareketini ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<StockMovement?>
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
        /// Ambara ait son stok hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetByWarehouseIdAsync(
                int warehouseId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetDetailsQuery()
                .Where(x => x.WarehouseId == warehouseId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Malzemeye ait son stok hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetByMaterialIdAsync(
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetDetailsQuery()
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
            GetByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetDetailsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcının gerçekleştirdiği
        /// stok hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetByPerformedByUserIdAsync(
                int userId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetDetailsQuery()
                .Where(x => x.PerformedByUserId == userId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Stok hareket türüne göre kayıtları getirir.
        ///
        /// Desteklenen türler:
        /// In = 1
        /// Out = 2
        /// </summary>
        public async Task<List<StockMovement>>
            GetByMovementTypeAsync(
                StockMovementTypeEnum movementType,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateMovementType(movementType);
            ValidateTake(take);

            return await GetDetailsQuery()
                .Where(x => x.MovementType == movementType)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirli ambardaki In veya Out
        /// hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetByWarehouseAndTypeAsync(
                int warehouseId,
                StockMovementTypeEnum movementType,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateMovementType(movementType);
            ValidateTake(take);

            return await GetDetailsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MovementType == movementType)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen UTC tarih aralığındaki
        /// stok hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            return await GetDetailsQuery()
                .Where(x =>
                    x.MovementDate >= startDateUtc &&
                    x.MovementDate <= endDateUtc)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Referans numarasına bağlı
        /// stok hareketlerini getirir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetByReferenceNumberAsync(
                string referenceNumber,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(referenceNumber))
            {
                return new List<StockMovement>();
            }

            string normalizedReference =
                referenceNumber.Trim();

            return await GetDetailsQuery()
                .Where(x =>
                    x.ReferenceNumber == normalizedReference)
                .OrderBy(x => x.MovementDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aynı transfer grubuna bağlı
        /// stok çıkış ve giriş hareketlerini getirir.
        ///
        /// Transfer çıkışı Out,
        /// transfer girişi In olarak kaydedilir.
        /// </summary>
        public async Task<List<StockMovement>>
            GetByTransferGroupIdAsync(
                Guid transferGroupId,
                CancellationToken cancellationToken = default)
        {
            if (transferGroupId == Guid.Empty)
            {
                return new List<StockMovement>();
            }

            return await GetDetailsQuery()
                .Where(x =>
                    x.TransferGroupId == transferGroupId)
                .OrderBy(x =>
                    x.MovementType == StockMovementTypeEnum.Out
                        ? 0
                        : 1)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirli ambardaki transfer hareketlerini
        /// getirir.
        ///
        /// TransferGroupId dolu olan
        /// In ve Out kayıtları filtrelenir.
        ///
        /// Normal stok girişleri ve çıkışları
        /// bu listeye dahil edilmez.
        /// </summary>
        public async Task<List<StockMovement>>
            GetTransfersByWarehouseIdAsync(
                int warehouseId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateTake(take);

            return await GetDetailsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.TransferGroupId.HasValue)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrindeki malzeme kullanımına
        /// bağlanan stok çıkışlarını getirir.
        ///
        /// İş emri ilişkisi WorkOrderMaterial
        /// üzerinden kurulmaktadır.
        /// </summary>
        public async Task<List<StockMovement>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(movement =>
                    movement.MovementType ==
                        StockMovementTypeEnum.Out &&

                    _context.Set<WorkOrderMaterial>()
                        .Any(usage =>
                            usage.WorkOrderId == workOrderId &&
                            usage.StockMovementId == movement.Id))
                .OrderBy(x => x.MovementDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Ambar ve malzemeye ait
        /// en son stok hareketini getirir.
        /// </summary>
        public async Task<StockMovement?>
            GetLatestMovementAsync(
                int warehouseId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Ambar ve malzemeye ait stok
        /// hareketi bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasMovementsAsync(
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
        /// Ambardaki toplam stok
        /// hareketi sayısını getirir.
        /// </summary>
        public async Task<int>
            GetMovementCountByWarehouseIdAsync(
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
        /// Ambardaki malzemenin toplam
        /// giriş miktarını hesaplar.
        ///
        /// MovementType = In olan
        /// bütün kayıtlar dahildir.
        ///
        /// Transfer girişleri de dahildir.
        /// </summary>
        public async Task<decimal> GetTotalIncomingQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            decimal? total = await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId &&
                    x.MovementType == StockMovementTypeEnum.In)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return total ?? 0m;
        }

        /// <summary>
        /// Ambardaki malzemenin toplam
        /// çıkış miktarını hesaplar.
        ///
        /// MovementType = Out olan
        /// bütün kayıtlar dahildir.
        ///
        /// Transfer çıkışları da dahildir.
        /// </summary>
        public async Task<decimal> GetTotalOutgoingQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            decimal? total = await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId &&
                    x.MovementType == StockMovementTypeEnum.Out)
                .SumAsync(
                    x => (decimal?)x.Quantity,
                    cancellationToken);

            return total ?? 0m;
        }

        /// <summary>
        /// Stok hareket geçmişine göre
        /// net miktar değişimini hesaplar.
        ///
        /// Net miktar = Toplam giriş - Toplam çıkış.
        /// </summary>
        public async Task<decimal> GetNetQuantityChangeAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            decimal incoming =
                await GetTotalIncomingQuantityAsync(
                    warehouseId,
                    materialId,
                    cancellationToken);

            decimal outgoing =
                await GetTotalOutgoingQuantityAsync(
                    warehouseId,
                    materialId,
                    cancellationToken);

            return incoming - outgoing;
        }

        /// <summary>
        /// Normal stok giriş veya çıkış hareketini
        /// WarehouseStock ile birlikte kaydeder.
        ///
        /// In stok miktarını artırır.
        /// Out stok miktarını azaltır.
        ///
        /// TransferGroupId normal hareketler için
        /// null olarak kaydedilir.
        ///
        /// Stok ve hareket tek SaveChangesAsync
        /// çağrısıyla veritabanına kaydedilir.
        /// </summary>
        public async Task<StockMovement> RecordMovementAsync(
            int warehouseId,
            int materialId,
            int performedByUserId,
            StockMovementTypeEnum movementType,
            decimal quantity,
            DateTime movementDateUtc,
            string? referenceNumber = null,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            ValidateMovementType(movementType);

            ValidateWriteArguments(
                warehouseId,
                materialId,
                performedByUserId,
                quantity,
                movementDateUtc);

            await ValidateReferencesAsync(
                warehouseId,
                materialId,
                performedByUserId,
                cancellationToken);

            DateTime now = DateTime.UtcNow;

            bool isIncrease =
                movementType == StockMovementTypeEnum.In;

            // Tracking açık bırakılır.
            // WarehouseStock.RowVersion üzerinden
            // eşzamanlı güncelleme kontrol edilir.
            var stock = await _context.Set<WarehouseStock>()
                .FirstOrDefaultAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId,
                    cancellationToken);

            if (stock == null)
            {
                // Stok çıkışında mevcut stok kartı
                // bulunması zorunludur.
                if (!isIncrease)
                {
                    throw new InvalidOperationException(
                        "Stok kaydı bulunmayan ambardan " +
                        "malzeme çıkışı yapılamaz.");
                }

                // İlk stok girişi ise
                // sıfır bakiyeli stok kartı oluşturulur.
                stock = new WarehouseStock
                {
                    WarehouseId = warehouseId,

                    MaterialId = materialId,

                    Quantity = 0m,

                    LastMovementDate = null,

                    CreatedAt = now,

                    UpdatedAt = null
                };

                _context.Set<WarehouseStock>().Add(stock);
            }

            // Stok miktarını güncelle.
            ApplyStockChange(
                stock,
                quantity,
                isIncrease,
                movementDateUtc,
                now);

            // Stok hareketini oluştur.
            var movement = new StockMovement
            {
                WarehouseId = warehouseId,

                MaterialId = materialId,

                PerformedByUserId = performedByUserId,

                MovementType = movementType,

                Quantity = quantity,

                MovementDate = movementDateUtc,

                ReferenceNumber =
                    NormalizeOptionalText(referenceNumber),

                // Normal giriş veya çıkış.
                TransferGroupId = null,

                Description =
                    NormalizeOptionalText(description),

                CreatedAt = now,

                UpdatedAt = null
            };

            _context.Set<StockMovement>().Add(movement);

            // Stok kartı ve hareket kaydı aynı
            // SaveChangesAsync ile kaydedilir.
            await _context.SaveChangesAsync(
                cancellationToken);

            return movement;
        }

        /// <summary>
        /// Kaynak ambardan hedef ambara
        /// stok transferi gerçekleştirir.
        ///
        /// Kaynak ambarda Out kaydı,
        /// hedef ambarda In kaydı oluşturulur.
        ///
        /// İki kayıt ortak TransferGroupId
        /// üzerinden ilişkilendirilir.
        ///
        /// İki stok bakiyesi ve iki hareket
        /// aynı SaveChangesAsync ile kaydedilir.
        /// </summary>
        public async Task<
            (StockMovement OutMovement, StockMovement InMovement)>
            TransferStockAsync(
                int fromWarehouseId,
                int toWarehouseId,
                int materialId,
                int performedByUserId,
                decimal quantity,
                DateTime movementDateUtc,
                string? referenceNumber = null,
                string? description = null,
                CancellationToken cancellationToken = default)
        {
            ValidateWriteArguments(
                fromWarehouseId,
                materialId,
                performedByUserId,
                quantity,
                movementDateUtc);

            if (toWarehouseId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(toWarehouseId),
                    "Geçerli bir hedef ambar belirtilmelidir.");
            }

            if (fromWarehouseId == toWarehouseId)
            {
                throw new InvalidOperationException(
                    "Kaynak ve hedef ambar aynı olamaz.");
            }

            // Kaynak ambar, malzeme ve
            // kullanıcı bilgilerini kontrol et.
            await ValidateReferencesAsync(
                fromWarehouseId,
                materialId,
                performedByUserId,
                cancellationToken);

            // Hedef ambarın aktif olduğunu doğrula.
            bool targetWarehouseExists =
                await _context.Set<Warehouse>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == toWarehouseId &&
                        x.IsActive,
                        cancellationToken);

            if (!targetWarehouseExists)
            {
                throw new InvalidOperationException(
                    "Aktif hedef ambar bulunamadı.");
            }

            DateTime now = DateTime.UtcNow;

            // Kaynak stok kartını getir.
            var sourceStock =
                await _context.Set<WarehouseStock>()
                    .FirstOrDefaultAsync(x =>
                        x.WarehouseId == fromWarehouseId &&
                        x.MaterialId == materialId,
                        cancellationToken);

            if (sourceStock == null)
            {
                throw new InvalidOperationException(
                    "Kaynak ambarda malzeme stok kaydı bulunamadı.");
            }

            // Hedef stok kartını getir.
            var targetStock =
                await _context.Set<WarehouseStock>()
                    .FirstOrDefaultAsync(x =>
                        x.WarehouseId == toWarehouseId &&
                        x.MaterialId == materialId,
                        cancellationToken);

            if (targetStock == null)
            {
                // Hedefte stok kartı bulunmuyorsa
                // yeni stok kartı oluştur.
                targetStock = new WarehouseStock
                {
                    WarehouseId = toWarehouseId,

                    MaterialId = materialId,

                    Quantity = 0m,

                    LastMovementDate = null,

                    CreatedAt = now,

                    UpdatedAt = null
                };

                _context.Set<WarehouseStock>().Add(targetStock);
            }

            // Kaynak ambardan stok düş.
            ApplyStockChange(
                sourceStock,
                quantity,
                increase: false,
                movementDateUtc: movementDateUtc,
                nowUtc: now);

            // Hedef ambara stok ekle.
            ApplyStockChange(
                targetStock,
                quantity,
                increase: true,
                movementDateUtc: movementDateUtc,
                nowUtc: now);

            // İki hareketi birbirine bağlayan
            // ortak transfer kimliği.
            Guid transferGroupId = Guid.NewGuid();

            string? normalizedReference =
                NormalizeOptionalText(referenceNumber);

            string? normalizedDescription =
                NormalizeOptionalText(description);

            // Kaynak ambar stok çıkışı.
            var outMovement = new StockMovement
            {
                WarehouseId = fromWarehouseId,

                MaterialId = materialId,

                PerformedByUserId = performedByUserId,

                // TransferOut yerine Out.
                MovementType = StockMovementTypeEnum.Out,

                Quantity = quantity,

                MovementDate = movementDateUtc,

                ReferenceNumber = normalizedReference,

                TransferGroupId = transferGroupId,

                Description = normalizedDescription,

                CreatedAt = now,

                UpdatedAt = null
            };

            // Hedef ambar stok girişi.
            var inMovement = new StockMovement
            {
                WarehouseId = toWarehouseId,

                MaterialId = materialId,

                PerformedByUserId = performedByUserId,

                // TransferIn yerine In.
                MovementType = StockMovementTypeEnum.In,

                Quantity = quantity,

                MovementDate = movementDateUtc,

                ReferenceNumber = normalizedReference,

                TransferGroupId = transferGroupId,

                Description = normalizedDescription,

                CreatedAt = now,

                UpdatedAt = null
            };

            _context.Set<StockMovement>()
                .Add(outMovement);

            _context.Set<StockMovement>()
                .Add(inMovement);

            // İki bakiye ve iki hareket
            // tek SaveChangesAsync ile kaydedilir.
            await _context.SaveChangesAsync(
                cancellationToken);

            return (outMovement, inMovement);
        }

        /// <summary>
        /// Stok artırma veya azaltma işlemlerinin
        /// ortak hesaplama metodudur.
        ///
        /// SaveChangesAsync çağrısı yapmaz.
        /// </summary>
        private static void ApplyStockChange(
            WarehouseStock stock,
            decimal quantity,
            bool increase,
            DateTime movementDateUtc,
            DateTime nowUtc)
        {
            if (stock.Quantity < 0m)
            {
                throw new InvalidOperationException(
                    "Mevcut stok bakiyesi negatiftir.");
            }

            if (increase)
            {
                stock.Quantity =
                    checked(stock.Quantity + quantity);
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

            // Hareket tarihi geçmişteyse stok kartının
            // son hareket tarihi geriye alınmaz.
            if (!stock.LastMovementDate.HasValue ||
                movementDateUtc > stock.LastMovementDate.Value)
            {
                stock.LastMovementDate = movementDateUtc;
            }

            stock.UpdatedAt = nowUtc;
        }

        /// <summary>
        /// Ambar, malzeme ve kullanıcı
        /// kayıtlarının aktif olduğunu doğrular.
        ///
        /// Kullanıcının ambar ve organizasyon
        /// yetkisi servis katmanında kontrol edilmelidir.
        /// </summary>
        private async Task ValidateReferencesAsync(
            int warehouseId,
            int materialId,
            int performedByUserId,
            CancellationToken cancellationToken)
        {
            bool warehouseExists =
                await _context.Set<Warehouse>()
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

            bool materialExists =
                await _context.Set<Material>()
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

            bool userExists =
                await _context.Set<User>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == performedByUserId &&
                        x.IsActive,
                        cancellationToken);

            if (!userExists)
            {
                throw new InvalidOperationException(
                    "Aktif kullanıcı bulunamadı.");
            }
        }

        /// <summary>
        /// Stok hareketi oluşturulurken kullanılan
        /// temel parametrelerin geçerliliğini doğrular.
        /// </summary>
        private static void ValidateWriteArguments(
            int warehouseId,
            int materialId,
            int performedByUserId,
            decimal quantity,
            DateTime movementDateUtc)
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

            if (performedByUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(performedByUserId),
                    "Geçerli bir kullanıcı belirtilmelidir.");
            }

            if (quantity <= 0m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Stok hareket miktarı sıfırdan büyük olmalıdır.");
            }

            if (movementDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Stok hareketi tarihi UTC formatında olmalıdır.",
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
        /// Hareket türünün yalnızca In
        /// veya Out olmasını doğrular.
        /// </summary>
        private static void ValidateMovementType(
            StockMovementTypeEnum movementType)
        {
            if (movementType != StockMovementTypeEnum.In &&
                movementType != StockMovementTypeEnum.Out)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(movementType),
                    "Stok hareket türü In veya Out olmalıdır.");
            }
        }

        /// <summary>
        /// Listeleme için istenen
        /// kayıt sayısını doğrular.
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
        /// Tarih aralığının UTC formatında
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
                    "Başlangıç ve bitiş tarihleri UTC olmalıdır.");
            }
        }

        /// <summary>
        /// Opsiyonel metin alanlarının
        /// boşluklarını düzenler.
        /// </summary>
        private static string? NormalizeOptionalText(
            string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        /// <summary>
        /// Stok hareketlerini ambar,
        /// organizasyon, malzeme, ölçü birimi,
        /// kategori ve kullanıcı bilgileriyle getirir.
        /// </summary>
        private IQueryable<StockMovement> GetDetailsQuery()
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
                    .ThenInclude(x => x.MaterialCategory)

                // İşlemi gerçekleştiren kullanıcı
                .Include(x => x.PerformedByUser);
        }
    }
}

