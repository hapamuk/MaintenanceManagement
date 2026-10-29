using MaintenanceManagement.Application.DTOS.AssetMovementDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetMovementService
    {
        #region Get Methods

        /// <summary>
        /// Bütün demirbaş hareketlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetMovementDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre hareket detayını getirir.
        /// </summary>
        Task<GetByIdAssetMovementDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa ait bütün hareketleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetMovementDto>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşın son hareketini getirir.
        /// </summary>
        Task<GetByIdAssetMovementDto?> GetLatestByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen birimle ilişkili hareketleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetMovementDto>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcıyla ilişkili hareketleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetMovementDto>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcının gerçekleştirdiği
        /// hareketleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetMovementDto>>
            GetByPerformedUserIdAsync(
                int performedByUserId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Hareket türüne göre filtreleme yapar.
        /// </summary>
        Task<IReadOnlyList<ResultAssetMovementDto>> GetByTypeAsync(
            AssetMovementTypeEnum movementType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// UTC tarih aralığındaki hareketleri getirir.
        /// Başlangıç ve bitiş anları dahildir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetMovementDto>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kaynak ve hedef birim arasındaki
        /// devir hareketlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetMovementDto>>
            GetTransfersBetweenUnitsAsync(
                int fromUnitId,
                int toUnitId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni demirbaş hareketi kaydeder.
        /// İşlemi yapan kullanıcı ID değeri
        /// güvenilir oturum bilgisinden alınmalıdır.
        ///
        /// Mevcut Repository ile Registration,
        /// Transfer ve Scrap hareketleri desteklenir.
        /// </summary>
        Task<int> CreateAsync(
            CreateAssetMovementDto dto,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut hareketin yalnızca
        /// açıklamasını günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateAssetMovementDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Demirbaşa ait hareket kaydı
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasMovementsAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
