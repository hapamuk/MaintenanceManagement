using MaintenanceManagement.Application.DTOS.AssetDtos;
using MaintenanceManagement.Application.DTOS.AssetKindDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetKindService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün demirbaş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetKindDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif demirbaş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetKindDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre demirbaş türünün
        /// detaylarını getirir.
        /// </summary>
        Task<GetByIdAssetKindDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoriye bağlı aktif ve pasif
        /// bütün demirbaş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetKindDto>> GetByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen aktif kategoriye bağlı
        /// aktif demirbaş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetKindDto>>
            GetActiveByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaş türüne bağlı
        /// bütün demirbaş kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetDto>> GetAssetsByKindIdAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni demirbaş türü oluşturur.
        /// Oluşturulan kaydın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateAssetKindDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut demirbaş türünü günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateAssetKindDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Demirbaş türünün aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı kategori içerisinde belirtilen
        /// tür adının bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            int categoryId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş türüne bağlı herhangi bir
        /// demirbaş bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssetsAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş türüne bağlı aktif
        /// demirbaş bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssetsAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
