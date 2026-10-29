using MaintenanceManagement.Application.DTOS.SiteLocationDtos;
using MaintenanceManagement.Application.DTOS.WorkRequestDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface ISiteLocationService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün konumları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultSiteLocationDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif konumları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultSiteLocationDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre konum detayını getirir.
        /// </summary>
        Task<GetByIdSiteLocationDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konum adına göre detay bilgilerini getirir.
        /// </summary>
        Task<GetByIdSiteLocationDto?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bina bilgisine göre konumları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultSiteLocationDto>> GetByBuildingAsync(
            string building,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adres içerisinde metin araması yapar.
        /// </summary>
        Task<IReadOnlyList<ResultSiteLocationDto>> SearchByAddressAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Enlem ve boylam bilgileri dolu
        /// olan aktif konumları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultSiteLocationDto>>
            GetWithCoordinatesAsync(
                CancellationToken cancellationToken = default);

        #endregion

        #region WorkRequest Methods

        /// <summary>
        /// Konuma ait bütün iş taleplerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetWorkRequestsByLocationIdAsync(
                int locationId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Konuma ait Yeni (Draft) durumundaki
        /// iş taleplerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetNewWorkRequestsByLocationIdAsync(
                int locationId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni konum oluşturur.
        /// Oluşturulan kaydın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateSiteLocationDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut konumu günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateSiteLocationDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Konumun aktif/pasif durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı isimde başka konum var mı kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konuma bağlı iş talebi var mı kontrol eder.
        /// </summary>
        Task<bool> HasWorkRequestsAsync(
            int locationId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konuma bağlı Yeni (Draft) durumunda
        /// iş talebi var mı kontrol eder.
        /// </summary>
        Task<bool> HasNewWorkRequestsAsync(
            int locationId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
