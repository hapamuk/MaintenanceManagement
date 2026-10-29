using MaintenanceManagement.Application.DTOS.AssetDtos;
using MaintenanceManagement.Application.DTOS.AssetModelDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetModelService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün modelleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetModelDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif modelleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetModelDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre model detaylarını getirir.
        /// </summary>
        Task<GetByIdAssetModelDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markaya bağlı bütün modelleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetModelDto>> GetByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markaya bağlı aktif modelleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetModelDto>>
            GetActiveByBrandIdAsync(
                int brandId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Marka ID ve model adına göre kayıt getirir.
        /// </summary>
        Task<GetByIdAssetModelDto?> GetByBrandAndNameAsync(
            int brandId,
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Modele bağlı demirbaşları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetAssetsByModelIdAsync(
            int modelId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni model oluşturur ve oluşan
        /// kaydın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateAssetModelDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut model bilgilerini günceller.
        /// Kayıt yoksa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateAssetModelDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Modelin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı marka altında model adının
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            int brandId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Modele bağlı demirbaş kaydı
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssetsAsync(
            int modelId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Modele bağlı aktif demirbaş
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssetsAsync(
            int modelId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
