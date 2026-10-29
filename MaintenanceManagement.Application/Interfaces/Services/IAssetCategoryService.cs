using MaintenanceManagement.Application.DTOS.AssetCategoryDtos;
using MaintenanceManagement.Application.DTOS.AssetDtos;
using MaintenanceManagement.Application.DTOS.AssetKindDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetCategoryService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün demirbaş kategorilerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetCategoryDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif demirbaş kategorilerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetCategoryDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine ait kategori detayını,
        /// toplam, aktif ve pasif tür sayılarını getirir.
        /// </summary>
        Task<GetByIdAssetCategoryDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif ve pasif
        /// bütün demirbaş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetKindDto>>
            GetKindsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif kategoriye bağlı aktif
        /// demirbaş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetKindDto>>
            GetActiveKindsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı demirbaş kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>>
            GetAssetsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni demirbaş kategorisi oluşturur.
        /// Oluşturulan kaydın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateAssetCategoryDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut demirbaş kategorisini günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateAssetCategoryDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Demirbaş kategorisinin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Kategori adının başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı demirbaş türü
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasKindsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif demirbaş türü
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveKindsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı demirbaş kayıtlarının
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssetsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif demirbaş kayıtlarının
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssetsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
