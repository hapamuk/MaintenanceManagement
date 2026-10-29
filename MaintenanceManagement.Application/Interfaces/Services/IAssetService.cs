using MaintenanceManagement.Application.DTOS.AssetDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetService
    {
        #region Get Methods

        /// <summary>
        /// Bütün demirbaş kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif durumdaki demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş ID değerine göre detaylarını getirir.
        /// </summary>
        Task<GetByIdAssetDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş koduna göre detaylarını getirir.
        /// </summary>
        Task<GetByIdAssetDto?> GetByAssetCodeAsync(
            string assetCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimine ait demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markaya ait demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen modele ait demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetByModelIdAsync(
            int modelId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoriye ait demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen türe ait demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetByKindIdAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşları durumlarına göre filtreler.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetByStatusAsync(
            AssetStatusEnum status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcıyla ilişkili demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Warranty Methods

        /// <summary>
        /// Belirtilen gün sayısı içerisinde
        /// garantisi sona erecek demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>>
            GetExpiringWarrantyAsync(
                int days,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Garanti süresi sona ermiş
        /// demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>>
            GetExpiredWarrantyAsync(
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni demirbaş oluşturur.
        /// Oluşturulan kaydın ID değerini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateAssetDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut demirbaş bilgilerini günceller.
        /// Kayıt bulunamadığında false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateAssetDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Demirbaşın durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            AssetStatusEnum status,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        Task<bool> ExistsAssetCodeAsync(
            string assetCode,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsSerialNumberAsync(
            string serialNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsInventoryNumberAsync(
            string inventoryNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        Task<bool> HasOpenWorkOrdersAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveAssignmentsAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveMaintenancePlansAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
