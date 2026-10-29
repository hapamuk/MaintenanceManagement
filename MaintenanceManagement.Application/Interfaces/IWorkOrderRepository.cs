using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces
{
    public interface IWorkOrderRepository
        : IGenericRepository<WorkOrder>
    {
        /// <summary>
        /// Bütün iş emirlerini ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<WorkOrder>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş emrini ilişkili
        /// detaylarıyla getirir.
        /// </summary>
        Task<WorkOrder?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emri numarasına göre kayıt getirir.
        /// </summary>
        Task<WorkOrder?> GetByWorkOrderNumberAsync(
            string workOrderNumber,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinden oluşturulmuş iş emrini getirir.
        /// </summary>
        Task<WorkOrder?> GetByWorkRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlı bakım kaydından oluşturulan
        /// iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByPlannedMaintenanceIdAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByResponsibleUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyonun açık iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetOpenWorkOrdersByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takıma atanmış iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne bağlı iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByWorkSubTypeIdAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaşa ait iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcının oluşturduğu
        /// iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByCreatedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen personelin halen aktif görev aldığı
        /// iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetAssignedWorkOrdersByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen durumdaki iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByStatusAsync(
            WorkStatusEnum status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen öncelik seviyesindeki
        /// iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByPriorityAsync(
            WorkPriorityEnum priority,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonda onay bekleyen
        /// iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetPendingApprovalByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlanan bitiş tarihi geçmiş ancak
        /// tamamlanmamış iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetOverdueWorkOrdersAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlanan başlangıç tarihi belirtilen
        /// UTC aralığında olan iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetByPlannedStartDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emri numarasının daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsWorkOrderNumberAsync(
            string workOrderNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinden daha önce iş emri
        /// oluşturulup oluşturulmadığını kontrol eder.
        /// </summary>
        Task<bool> HasWorkOrderForRequestAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa ait açık iş emri
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takıma ait açık iş emri
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine atanmış personelleri getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetPersonnelByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin faaliyet ve işçilik kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetActionsByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin durum değişikliği geçmişini getirir.
        /// </summary>
        Task<List<WorkOrderStatusHistory>> GetStatusHistoryByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine eklenen dosyaları getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde kullanılan malzemeleri getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetMaterialsByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin bakım kontrol listesi
        /// sonuçlarını getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>>
            GetChecklistResultsByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni iş emrini başlangıç durum
        /// geçmişiyle birlikte oluşturur.
        /// </summary>
        Task<WorkOrder> CreateWithInitialHistoryAsync(
            WorkOrder workOrder,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin durumunu değiştirir ve
        /// durum geçmişine kayıt ekler.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            WorkStatusEnum newStatus,
            int changedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default);
    }
}
