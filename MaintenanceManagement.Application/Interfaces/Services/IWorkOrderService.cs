using MaintenanceManagement.Application.DTOS.WorkOrdersDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkOrderService
    {
        #region Get Methods

        /// <summary>
        /// Bütün iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre iş emri detayını getirir.
        /// </summary>
        Task<GetByIdWorkOrderDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emri numarasına göre detay getirir.
        /// </summary>
        Task<GetByIdWorkOrderDto?> GetByWorkOrderNumberAsync(
            string workOrderNumber,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinden oluşturulan iş emrini getirir.
        /// </summary>
        Task<GetByIdWorkOrderDto?> GetByWorkRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlı bakım kaydından oluşturulan
        /// iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu birime ait iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Takıma ait iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne ait iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa bağlı iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Duruma göre iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>> GetByStatusAsync(
            WorkStatusEnum status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Önceliğe göre iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>> GetByPriorityAsync(
            WorkPriorityEnum priority,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrini oluşturan kullanıcıya göre getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByCreatedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Personele atanmış iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>>
            GetAssignedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu birimin açık iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>>
            GetOpenByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Birimin onay bekleyen iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>>
            GetPendingApprovalByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlanan başlangıç tarihi aralığına
        /// göre iş emirlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByPlannedStartDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Doğrudan veya planlı bakım kaynaklı
        /// yeni iş emri oluşturur.
        ///
        /// İş talebinden dönüşüm, ayrıca atomik
        /// işlem gerektirdiğinden bu metotla yapılmaz.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkOrderDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni durumundaki iş emrinin
        /// düzenlenebilir alanlarını günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkOrderDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Workflow Methods

        /// <summary>
        /// İş emrinin durumunu izin verilen
        /// durum geçişlerine göre değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            WorkStatusEnum newStatus,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Personel ataması yapılan iş emrini başlatır.
        /// </summary>
        Task<bool> StartWorkAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrini onaya gönderir.
        /// </summary>
        Task<bool> SubmitForApprovalAsync(
            int id,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrini onaylayarak tamamlar.
        /// </summary>
        Task<bool> ApproveAsync(
            int id,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Onay bekleyen iş emrini yeniden
        /// devam eden iş durumuna döndürür.
        /// </summary>
        Task<bool> ReturnToInProgressAsync(
            int id,
            int performedByUserId,
            string reason,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrini iptal eder.
        /// </summary>
        Task<bool> CancelAsync(
            int id,
            int performedByUserId,
            string reason,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// İş emri numarası kullanılıyor mu?
        /// </summary>
        Task<bool> ExistsWorkOrderNumberAsync(
            string workOrderNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine bağlı iş emri var mı?
        /// </summary>
        Task<bool> HasWorkOrderForRequestAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takımın açık iş emri var mı?
        /// </summary>
        Task<bool> HasOpenWorkOrdersByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
