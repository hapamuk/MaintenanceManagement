using MaintenanceManagement.Application.DTOS.UnitDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IUnitService
    {
        #region Get Methods

        /// <summary>
        /// Sistemdeki bütün organizasyon birimlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif organizasyon birimlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine ait birimin
        /// detay bilgilerini getirir.
        /// </summary>
        Task<GetByIdUnitDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon türüne bağlı birimleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitDto>> GetByOrganizationTypeAsync(OrganizationTypeEnum organizationType,CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen birime bağlı doğrudan
        /// alt organizasyon birimlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitDto>> GetChildUnitsAsync(
            int parentUnitId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni organizasyon birimi oluşturur.
        /// Oluşturulan kaydın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateUnitDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimini günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateUnitDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon biriminin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı üst birim altında aynı adla
        /// kayıt bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? parentUnitId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birim kodunun başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birime bağlı alt birimler bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasChildUnitsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birime bağlı aktif ekip bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveTeamsAsync(
            int id,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
