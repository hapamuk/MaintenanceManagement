using MaintenanceManagement.Application.DTOS.WorkRequestActionDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkRequestActionService
    {
        #region Get Methods

        /// <summary>
        /// Bütün işlem geçmişi kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre işlem detayını getirir.
        /// </summary>
        Task<GetByIdWorkRequestActionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine ait bütün işlem geçmişini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByWorkRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Talep numarasına göre işlem geçmişini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByRequestNumberAsync(
                string requestNumber,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcının gerçekleştirdiği
        /// işlem kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyon birimine ait
        /// iş taleplerinin hareketlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İşlem türüne göre geçmiş kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByTypeAsync(
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli iş talebinin ilgili işlem türündeki
        /// geçmiş kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByRequestAndTypeAsync(
                int workRequestId,
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// UTC tarih aralığında gerçekleşen
        /// işlem geçmişini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestActionDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinin en son işlem kaydını getirir.
        /// </summary>
        Task<GetByIdWorkRequestActionDto?>
            GetLatestByRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinin belirli işlem türündeki
        /// en son kaydını getirir.
        /// </summary>
        Task<GetByIdWorkRequestActionDto?>
            GetLatestByTypeAsync(
                int workRequestId,
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// İş talebinin işlem geçmişi var mı?
        /// </summary>
        Task<bool> HasActionsAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinde belirli işlem türü
        /// bulunuyor mu kontrol eder.
        /// </summary>
        Task<bool> HasActionTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        /// <summary>
        /// İş talebinin toplam işlem geçmişi sayısı.
        /// </summary>
        Task<int> GetActionCountByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinin belirli türdeki
        /// işlem geçmişi sayısı.
        /// </summary>
        Task<int> GetActionCountByTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
