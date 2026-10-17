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
    public class AssetMovementRepository :
       GenericRepository<AssetMovement>,
       IAssetMovementRepository
    {
        public AssetMovementRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün demirbaş hareketlerini ilişkili
        /// organizasyon, kullanıcı ve demirbaş
        /// detaylarıyla getirir.
        /// </summary>
        public async Task<List<AssetMovement>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Hareket kaydını ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<AssetMovement?>
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
        /// Demirbaşa ait bütün hareketleri
        /// en yeniden eskiye doğru getirir.
        /// </summary>
        public async Task<List<AssetMovement>>
            GetMovementsByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Demirbaşın en son hareketini getirir.
        /// </summary>
        public async Task<AssetMovement?>
            GetLatestMovementByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyonun kaynak veya hedef
        /// olarak yer aldığı hareketleri getirir.
        /// </summary>
        public async Task<List<AssetMovement>>
            GetMovementsByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.FromUnitId == unitId ||
                    x.ToUnitId == unitId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının önceki veya yeni zimmetli
        /// olarak yer aldığı hareketleri getirir.
        /// </summary>
        public async Task<List<AssetMovement>>
            GetMovementsByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.FromUserId == userId ||
                    x.ToUserId == userId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Hareket işlemini gerçekleştiren
        /// kullanıcıya ait kayıtları getirir.
        /// </summary>
        public async Task<List<AssetMovement>>
            GetMovementsByPerformedUserIdAsync(
                int performedByUserId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.PerformedByUserId == performedByUserId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen hareket türüne ait
        /// bütün kayıtları getirir.
        /// </summary>
        public async Task<List<AssetMovement>>
            GetMovementsByTypeAsync(
                AssetMovementTypeEnum movementType,
                CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(
                typeof(AssetMovementTypeEnum), movementType))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(movementType),
                    "Geçersiz demirbaş hareket türü.");
            }

            return await GetDetailsQuery()
                .Where(x => x.MovementType == movementType)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// UTC tarih aralığındaki hareketleri getirir.
        /// </summary>
        public async Task<List<AssetMovement>>
            GetMovementsByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            if (startDateUtc > endDateUtc)
            {
                throw new ArgumentException(
                    "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }

            return await GetDetailsQuery()
                .Where(x =>
                    x.MovementDate >= startDateUtc &&
                    x.MovementDate <= endDateUtc)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İki organizasyon arasında kaynak ve hedef
        /// yönüne göre gerçekleşen transferleri getirir.
        /// </summary>
        public async Task<List<AssetMovement>>
            GetTransfersBetweenUnitsAsync(
                int fromUnitId,
                int toUnitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.MovementType == AssetMovementTypeEnum.Transfer &&
                    x.FromUnitId == fromUnitId &&
                    x.ToUnitId == toUnitId)
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Demirbaşa ait herhangi bir hareket
        /// kaydı olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasMovementsAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(
                    x => x.AssetId == assetId,
                    cancellationToken);
        }

        /// <summary>
        /// Yeni demirbaş hareketi oluşturur.
        ///
        /// İsteğe bağlı olarak demirbaşın güncel
        /// organizasyonunu ve durumunu günceller.
        ///
        /// Tüm değişiklikler tek SaveChangesAsync
        /// çağrısı ile kaydedilir.
        /// </summary>
        public async Task<AssetMovement> RegisterMovementAsync(
            AssetMovement movement,
            int? newUnitId = null,
            AssetStatusEnum? newAssetStatus = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(movement);

            if (movement.AssetId <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir demirbaş seçilmelidir.",
                    nameof(movement));
            }

            if (movement.PerformedByUserId <= 0)
            {
                throw new ArgumentException(
                    "İşlemi gerçekleştiren kullanıcı belirtilmelidir.",
                    nameof(movement));
            }

            if (!Enum.IsDefined(
                typeof(AssetMovementTypeEnum),
                movement.MovementType))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(movement),
                    "Geçersiz hareket türü.");
            }

            if (newAssetStatus.HasValue &&
                !Enum.IsDefined(
                    typeof(AssetStatusEnum),
                    newAssetStatus.Value))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(newAssetStatus),
                    "Geçersiz demirbaş durumu.");
            }

            if (newUnitId.HasValue && newUnitId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(newUnitId),
                    "Geçerli bir organizasyon seçilmelidir.");
            }

            // Hareketin ilgili olduğu demirbaşı getir.
            var asset = await _context.Assets
                .FirstOrDefaultAsync(
                    x => x.Id == movement.AssetId,
                    cancellationToken);

            if (asset == null)
            {
                throw new KeyNotFoundException(
                    "İşlem yapılacak demirbaş bulunamadı.");
            }

            // Hareket tarihi belirtilmemişse
            // UTC tarihini kullan.
            if (movement.MovementDate == default)
            {
                movement.MovementDate = DateTime.UtcNow;
            }

            bool assetUpdated = false;

            // Demirbaşın güncel organizasyonunu değiştir.
            if (newUnitId.HasValue &&
                asset.UnitId != newUnitId.Value)
            {
                bool unitExists = await _context.Units
                    .AnyAsync(
                        x => x.Id == newUnitId.Value,
                        cancellationToken);

                if (!unitExists)
                {
                    throw new KeyNotFoundException(
                        "Hedef organizasyon bulunamadı.");
                }

                asset.UnitId = newUnitId.Value;
                assetUpdated = true;
            }

            // Demirbaşın güncel durumunu değiştir.
            if (newAssetStatus.HasValue &&
                asset.Status != newAssetStatus.Value)
            {
                asset.Status = newAssetStatus.Value;
                assetUpdated = true;
            }

            if (assetUpdated)
            {
                asset.UpdatedAt = DateTime.UtcNow;
            }

            movement.CreatedAt = DateTime.UtcNow;
            movement.UpdatedAt = null;

            await _dbSet.AddAsync(
                movement,
                cancellationToken);

            // Hareket ve demirbaş değişikliği
            // birlikte kaydedilir.
            await _context.SaveChangesAsync(
                cancellationToken);

            return movement;
        }

        /// <summary>
        /// Demirbaş, model, marka, kaynak/hedef
        /// organizasyon ve kullanıcı bilgilerini
        /// içeren ortak detay sorgusudur.
        /// </summary>
        private IQueryable<AssetMovement> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // Demirbaş ve güncel organizasyon
                .Include(x => x.Asset)
                    .ThenInclude(x => x.Unit)

                // Demirbaş modeli ve markası
                .Include(x => x.Asset)
                    .ThenInclude(x => x.AssetModel)
                        .ThenInclude(x => x.Brand)

                // Kaynak organizasyon
                .Include(x => x.FromUnit)

                // Hedef organizasyon
                .Include(x => x.ToUnit)

                // Önceki zimmetli
                .Include(x => x.FromUser)

                // Yeni zimmetli
                .Include(x => x.ToUser)

                // İşlemi gerçekleştiren kullanıcı
                .Include(x => x.PerformedByUser);
        }
    }
}
