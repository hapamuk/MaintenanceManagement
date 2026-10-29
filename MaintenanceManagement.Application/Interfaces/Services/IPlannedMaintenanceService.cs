using MaintenanceManagement.Application.DTOS.PlannedMaintenancesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IPlannedMaintenanceService
    {
        #region Get Methods

        /// <summary>
        /// Bütün planlı bakımları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre bakım detayını getirir.
        /// </summary>
        Task<GetByIdPlannedMaintenanceDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif bakım planlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa ait bakım planlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon biriminin bakım planlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne ait bakım planlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının oluşturduğu bakım planlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByCreatedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Maintenance Date Methods

        /// <summary>
        /// Belirtilen gün itibarıyla bakım zamanı
        /// gelmiş aktif planları getirir.
        ///
        /// Gecikmiş bakımları da kapsar.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetDueMaintenancesAsync(
                DateTime referenceDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım tarihi geçmiş aktif planları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetOverdueMaintenancesAsync(
                DateTime referenceDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Önümüzdeki belirtilen gün sayısı içinde
        /// yapılması gereken aktif bakımları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetUpcomingMaintenancesAsync(
                int days,
                DateTime referenceDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Bir sonraki bakım tarihine göre
        /// tarih aralığında bulunan planları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByNextMaintenanceDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni bakım planı oluşturur.
        /// Oluşturan kullanıcı oturumdan belirlenir.
        /// </summary>
        Task<int> CreateAsync(
            CreatePlannedMaintenanceDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut bakım planının düzenlenebilir
        /// bilgilerini günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdatePlannedMaintenanceDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planını aktif veya pasif yapar.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        /// <summary>
        /// Belirtilen tarihe kadar bakım zamanı
        /// gelmiş aktif planların sayısını getirir.
        /// </summary>
        Task<int> GetDueMaintenanceCountAsync(
            DateTime referenceDateUtc,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
