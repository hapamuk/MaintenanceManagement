using MaintenanceManagement.Application.DTOS.WorkSubTypeDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkSubTypeService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün iş alt türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif iş alt türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre iş alt türü detayını getirir.
        /// </summary>
        Task<GetByIdWorkSubTypeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı bütün alt türleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı aktif alt türleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetActiveByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Unit Methods

        /// <summary>
        /// Birimde kullanılabilecek aktif alt türleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAvailableByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Birim ve iş türüne göre kullanılabilir
        /// aktif alt türleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAvailableByUnitAndWorkTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türünün organizasyon biriminde
        /// kullanılabilir olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> IsAvailableForUnitAsync(
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Authorization Methods

        /// <summary>
        /// Belirli rolün organizasyon biriminde
        /// yetkili olduğu iş alt türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAuthorizedByRoleAsync(
                int roleId,
                int unitId,
                string permissionCode = "WORK_ORDER_EXECUTE",
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının organizasyon biriminde
        /// yetkili olduğu iş alt türlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAuthorizedByUserAsync(
                int userId,
                int unitId,
                string permissionCode = "WORK_ORDER_EXECUTE",
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı ilgili iş alt türünde
        /// iş emri işlemi gerçekleştirebilir mi?
        /// </summary>
        Task<bool> CanUserExecuteAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni iş alt türü oluşturur ve ID döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkSubTypeDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut iş alt türünü günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkSubTypeDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// İş alt türünün aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı iş türünde aynı isimli
        /// alt tür bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            int workTypeId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Alt türe bağlı iş talebi var mı?
        /// </summary>
        Task<bool> HasWorkRequestsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Alt türe bağlı yeni iş talebi var mı?
        /// </summary>
        Task<bool> HasNewWorkRequestsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Alt türe bağlı açık iş emri var mı?
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Alt türe bağlı aktif bakım planı var mı?
        /// </summary>
        Task<bool> HasActiveMaintenancePlansAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Alt türe bağlı aktif rol yetkisi var mı?
        /// </summary>
        Task<bool> HasActiveRolePermissionsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
