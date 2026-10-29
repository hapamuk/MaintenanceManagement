using MaintenanceManagement.Application.DTOS.UnitWorkTypeDtos;
using MaintenanceManagement.Application.DTOS.WorkSubTypeDtos;
using MaintenanceManagement.Application.DTOS.WorkTypeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IUnitWorkTypeService
    {
        #region Get Methods

        /// <summary>
        /// Bütün birim-iş türü atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitWorkTypeDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre atama detayını getirir.
        /// </summary>
        Task<GetByIdUnitWorkTypeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birime ait bütün iş türü atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Birime ait aktif iş türü atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetActiveByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne ait bütün birim atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne ait aktif birim atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetActiveByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Birim ve iş türüne ait atama detayını getirir.
        /// </summary>
        Task<GetByIdUnitWorkTypeDto?>
            GetByUnitAndWorkTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default);

        #endregion

        #region WorkType / WorkSubType Methods

        /// <summary>
        /// Birimde kullanılabilen aktif iş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkTypeDto>>
            GetActiveWorkTypesByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Birimde kullanılabilen aktif iş alt türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAvailableWorkSubTypesByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni birim-iş türü ataması oluşturur.
        /// Oluşturulan atamanın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateUnitWorkTypeDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut birim-iş türü atamasını günceller.
        /// Kayıt yoksa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateUnitWorkTypeDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Atamayı aktif veya pasif yapar.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı birim ve iş türü ataması
        /// daha önce oluşturulmuş mu kontrol eder.
        /// </summary>
        Task<bool> ExistsAssignmentAsync(
            int unitId,
            int workTypeId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş türü organizasyon
        /// biriminde kullanılabilir mi kontrol eder.
        /// </summary>
        Task<bool> IsWorkTypeAvailableForUnitAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birim ve iş türünde bekleyen
        /// iş talebi bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasPendingWorkRequestsAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birim ve iş türüne bağlı açık
        /// iş emri bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
