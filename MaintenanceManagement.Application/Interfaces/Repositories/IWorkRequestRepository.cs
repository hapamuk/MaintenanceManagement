using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkRequestRepository
        : IGenericRepository<WorkRequest>
    {
        /// <summary>
        /// Bütün iş taleplerini ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<WorkRequest>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş talebini ilişkili
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<WorkRequest?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talep numarasına göre iş talebini getirir.
        /// </summary>
        Task<WorkRequest?> GetByRequestNumberAsync(
            string requestNumber,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talebi oluşturan kullanıcının
        /// bütün iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByRequestedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talebi oluşturan organizasyona ait
        /// iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByRequesterUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyona yönlendirilmiş
        /// bütün iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByResponsibleUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyonun Yeni (Draft)
        /// durumundaki bütün taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetNewRequestsByResponsibleUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyona gönderilmiş ve
        /// Yeni (Draft) durumunda olan talepleri getirir.
        /// SubmittedAt alanı dolu olmalıdır.
        /// </summary>
        Task<List<WorkRequest>>
            GetSubmittedNewRequestsByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İlgili yöneticiye bağlı talepleri getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByManagerUserIdAsync(
            int managerUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne bağlı talepleri getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByWorkSubTypeIdAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa bağlı iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konuma bağlı iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetBySiteLocationIdAsync(
            int siteLocationId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen durumdaki iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByStatusAsync(
            WorkRequestStatusEnum status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen öncelik seviyesindeki
        /// iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByPriorityAsync(
            WorkPriorityEnum priority,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen tarih aralığında oluşturulmuş
        /// iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talep numarasının başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsRequestNumberAsync(
            string requestNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talebin bir iş emrine dönüştürülüp
        /// dönüştürülmediğini ilişkili kayıt üzerinden kontrol eder.
        /// </summary>
        Task<bool> HasWorkOrderAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talebe bağlı iş emrini getirir.
        /// </summary>
        Task<WorkOrder?> GetWorkOrderByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talebe eklenmiş dosyaları getirir.
        /// </summary>
        Task<List<WorkRequestFile>> GetFilesByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talebe ait işlem geçmişini getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetActionsByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni durumundaki talebi gönderir.
        /// SubmittedAt alanını doldurur ve
        /// gönderim hareketini kayıt altına alır.
        /// </summary>
        Task<bool> SubmitAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talebin durumunu günceller ve
        /// ilgili işlem hareketini kaydeder.
        ///
        /// Desteklenen geçişler:
        /// Draft -> Rejected
        /// Draft -> ConvertedToWorkOrder
        /// Draft -> Cancelled
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            WorkRequestStatusEnum status,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default);
    }
}
