using MaintenanceManagement.Application.DTOS.WorkSubTypeDtos;
using MaintenanceManagement.Application.DTOS.WorkTypeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkTypeService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün iş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkTypeDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif iş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkTypeDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre iş türü detaylarını getirir.
        /// </summary>
        Task<GetByIdWorkTypeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimine tanımlı
        /// aktif ve pasif iş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkTypeDto>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimine tanımlı
        /// aktif iş türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkTypeDto>> GetActiveByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        #endregion

        #region WorkSubType Methods

        /// <summary>
        /// İş türüne bağlı bütün alt türleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetWorkSubTypesAsync(
                int workTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı aktif alt türleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetActiveWorkSubTypesAsync(
                int workTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon biriminde seçilen iş türüne
        /// ait kullanılabilir alt türleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAvailableWorkSubTypesByUnitAndTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni iş türü oluşturur.
        /// Oluşturulan kaydın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkTypeDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türünün adını ve açıklamasını günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkTypeDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// İş türünün aktif/pasif durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı isimde başka iş türü var mı kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı alt tür bulunuyor mu?
        /// </summary>
        Task<bool> HasWorkSubTypesAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı aktif alt tür bulunuyor mu?
        /// </summary>
        Task<bool> HasActiveWorkSubTypesAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türünün birim ataması var mı?
        /// </summary>
        Task<bool> HasUnitAssignmentsAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı iş talebi var mı?
        /// </summary>
        Task<bool> HasWorkRequestsAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı açık iş emri var mı?
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı aktif bakım planı var mı?
        /// </summary>
        Task<bool> HasActiveMaintenancePlansAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
