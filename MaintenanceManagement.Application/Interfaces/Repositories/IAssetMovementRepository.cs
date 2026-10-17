using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IAssetMovementRepository
     : IGenericRepository<AssetMovement>
    {
        /// <summary>
        /// Bütün hareketleri ilişkili demirbaş,
        /// organizasyon ve kullanıcı detaylarıyla getirir.
        /// </summary>
        Task<List<AssetMovement>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen hareketi bütün ilişkili
        /// detaylarıyla getirir.
        /// </summary>
        Task<AssetMovement?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşın bütün hareket geçmişini getirir.
        /// </summary>
        Task<List<AssetMovement>> GetMovementsByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşın en son hareket kaydını getirir.
        /// </summary>
        Task<AssetMovement?> GetLatestMovementByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bir organizasyonun kaynak veya hedef olduğu
        /// bütün demirbaş hareketlerini getirir.
        /// </summary>
        Task<List<AssetMovement>> GetMovementsByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının kaynak veya hedef olduğu
        /// bütün demirbaş hareketlerini getirir.
        /// </summary>
        Task<List<AssetMovement>> GetMovementsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// gerçekleştirilen hareketleri getirir.
        /// </summary>
        Task<List<AssetMovement>> GetMovementsByPerformedUserIdAsync(
            int performedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen hareket türüne ait kayıtları getirir.
        /// </summary>
        Task<List<AssetMovement>> GetMovementsByTypeAsync(
            AssetMovementTypeEnum movementType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen tarih aralığındaki
        /// hareket kayıtlarını getirir.
        /// Başlangıç ve bitiş tarihleri dahildir.
        /// </summary>
        Task<List<AssetMovement>> GetMovementsByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kaynak ve hedef organizasyon arasında
        /// gerçekleşen transferleri getirir.
        /// </summary>
        Task<List<AssetMovement>> GetTransfersBetweenUnitsAsync(
            int fromUnitId,
            int toUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaş için herhangi
        /// bir hareket kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasMovementsAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş hareketini kaydeder.
        ///
        /// Gerekiyorsa demirbaşın güncel organizasyonunu
        /// ve durumunu aynı SaveChanges işleminde günceller.
        /// </summary>
        Task<AssetMovement> RegisterMovementAsync(
            AssetMovement movement,
            int? newUnitId = null,
            AssetStatusEnum? newAssetStatus = null,
            CancellationToken cancellationToken = default);
    }
}
