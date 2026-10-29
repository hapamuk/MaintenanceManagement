using MaintenanceManagement.Application.DTOS.WorkOrderStatusHistoryDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkOrderStatusHistoryService
    {
        #region Get Methods

        /// <summary>
        /// Bütün durum geçmişlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Durum geçmişi ID değerine göre
        /// kayıt detayını getirir.
        /// </summary>
        Task<GetByIdWorkOrderStatusHistoryDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait durum geçmişini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Durum değişikliğini gerçekleştiren
        /// kullanıcıya göre kayıtları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByChangedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyon biriminin
        /// iş emirlerine ait durum geçmişini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Takıma ait iş emirlerinin
        /// durum geçmişini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Önceki duruma göre kayıtları getirir.
        ///
        /// null: İlk oluşturulma geçmişleri.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByPreviousStatusAsync(
                WorkStatusEnum? previousStatus,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni duruma göre kayıtları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByNewStatusAsync(
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli bir durum geçişine ait
        /// geçmiş kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByStatusTransitionAsync(
                WorkStatusEnum? previousStatus,
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli iş emrinin, belirtilen
        /// yeni duruma geçtiği kayıtları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByWorkOrderAndNewStatusAsync(
                int workOrderId,
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// UTC tarih aralığındaki
        /// durum değişikliklerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli iş emrinin UTC tarih
        /// aralığındaki geçmişini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderStatusHistoryDto>>
            GetByWorkOrderAndDateRangeAsync(
                int workOrderId,
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin ilk durum geçmişini getirir.
        /// </summary>
        Task<GetByIdWorkOrderStatusHistoryDto?>
            GetFirstHistoryByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin en son durum geçmişini getirir.
        /// </summary>
        Task<GetByIdWorkOrderStatusHistoryDto?>
            GetLatestHistoryByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// İş emrinin geçmiş kaydı var mı?
        /// </summary>
        Task<bool> HasHistoryAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emri herhangi bir zamanda
        /// belirtilen duruma geçmiş mi?
        /// </summary>
        Task<bool> HasReachedStatusAsync(
            int workOrderId,
            WorkStatusEnum status,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        /// <summary>
        /// İş emrindeki durum değişikliği
        /// geçmişi kayıt sayısını getirir.
        /// </summary>
        Task<int> GetHistoryCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
