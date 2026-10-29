using MaintenanceManagement.Application.DTOS.TeamDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface ITeamService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün ekipleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif ekipleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine ait ekip detayını getirir.
        /// </summary>
        Task<GetByIdTeamDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimine bağlı bütün ekipleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamDto>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimine bağlı aktif ekipleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamDto>> GetActiveByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni ekip oluşturur ve oluşan kaydın
        /// ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateTeamDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut ekibin bilgilerini günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateTeamDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Ekibin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı organizasyon biriminde ekip adının
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            int unitId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı organizasyon biriminde ekip kodunun
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ekipte aktif üyeler bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveMembersAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ekibe atanmış açık iş emirleri bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
