using MaintenanceManagement.Application.DTOS.OrganizationTypeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IOrganizationTypeService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün organizasyon
        /// türlerini listeler.
        /// </summary>
        Task<IReadOnlyList<ResultOrganizationTypeDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif organizasyon
        /// türlerini listeler.
        /// </summary>
        Task<IReadOnlyList<ResultOrganizationTypeDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine ait organizasyon
        /// türünün detaylarını getirir.
        /// </summary>
        Task<GetByIdOrganizationTypeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create Method

        /// <summary>
        /// Yeni organizasyon türü oluşturur.
        /// Oluşturulan kaydın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateOrganizationTypeDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Update Method

        /// <summary>
        /// Mevcut organizasyon türünü günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateOrganizationTypeDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region ChangeStatus Method

        /// <summary>
        /// Organizasyon türünün aktif veya
        /// pasif durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Organizasyon türü adının başka
        /// bir kayıtta kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon türü kodunun başka
        /// bir kayıtta kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon türüne bağlı
        /// birim bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasUnitsAsync(
            int id,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
