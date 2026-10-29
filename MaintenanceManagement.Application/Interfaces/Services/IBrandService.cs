using MaintenanceManagement.Application.DTOS.AssetModelDtos;
using MaintenanceManagement.Application.DTOS.BrandDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IBrandService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün markaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultBrandDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif markaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultBrandDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine ait marka detayını,
        /// toplam model ve aktif/pasif model sayılarını getirir.
        /// </summary>
        Task<GetByIdBrandDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markaya bağlı bütün
        /// demirbaş modellerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetModelDto>>
            GetModelsByBrandIdAsync(
                int brandId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markaya bağlı aktif
        /// demirbaş modellerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetModelDto>>
            GetActiveModelsByBrandIdAsync(
                int brandId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni marka oluşturur.
        /// Oluşturulan markanın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateBrandDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut markanın bilgilerini günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateBrandDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Markanın aktif veya pasif durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Marka adının başka bir kayıt tarafından
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markaya bağlı model olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasModelsAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markaya bağlı aktif model olup
        /// olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveModelsAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markaya bağlı demirbaş kaydı
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssetsAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
