using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkOrderStatusHistoryRepository
         : IGenericRepository<WorkOrderStatusHistory>
    {
        /// <summary>
        /// Bütün durum geçmişi kayıtlarını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen durum geçmişi kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<WorkOrderStatusHistory?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin bütün durum değişikliklerini
        /// kronolojik sırayla getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının gerçekleştirdiği
        /// durum değişikliklerini getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByChangedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen sorumlu organizasyonun
        /// iş emirlerine ait durum geçmişini getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByResponsibleUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takıma bağlı iş emirlerinin
        /// durum geçmişini getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Önceki durum değerine göre
        /// durum geçmişini getirir.
        ///
        /// null gönderilirse iş emrinin
        /// ilk oluşturulma kayıtları getirilir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByPreviousStatusAsync(
            WorkStatusEnum? previousStatus,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni durum değerine göre
        /// geçmiş kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByNewStatusAsync(
            WorkStatusEnum newStatus,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen önceki durumdan yeni duruma
        /// gerçekleşmiş geçişleri getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByStatusTransitionAsync(
            WorkStatusEnum? previousStatus,
            WorkStatusEnum newStatus,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin belirtilen yeni duruma
        /// geçtiği geçmiş kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByWorkOrderAndNewStatusAsync(
            int workOrderId,
            WorkStatusEnum newStatus,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığındaki
        /// durum değişikliklerini getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin belirtilen UTC tarih aralığındaki
        /// durum değişikliklerini getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>>
            GetByWorkOrderAndDateRangeAsync(
                int workOrderId,
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin ilk durum geçmişi
        /// kaydını getirir.
        /// </summary>
        Task<WorkOrderStatusHistory?> GetFirstHistoryByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin en son durum
        /// değişikliği kaydını getirir.
        /// </summary>
        Task<WorkOrderStatusHistory?> GetLatestHistoryByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde herhangi bir durum geçmişi
        /// kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasHistoryAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin geçmişte belirtilen
        /// duruma ulaşıp ulaşmadığını kontrol eder.
        /// </summary>
        Task<bool> HasReachedStatusAsync(
            int workOrderId,
            WorkStatusEnum status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin durum geçmişi
        /// kayıtlarının sayısını getirir.
        /// </summary>
        Task<int> GetHistoryCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);
    }
}
