using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkOrderChecklistResultRepository
         : IGenericRepository<WorkOrderChecklistResult>
    {
        /// <summary>
        /// Bütün kontrol sonuçlarını ilişkili
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kontrol sonucu kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<WorkOrderChecklistResult?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait bütün kayıtlı
        /// kontrol sonuçlarını getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>> GetByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım kontrol maddesine ait farklı
        /// iş emirlerindeki sonuçları getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>>
            GetByMaintenanceChecklistIdAsync(
                int maintenanceChecklistId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emri ve kontrol maddesine ait
        /// tek sonuç kaydını getirir.
        /// </summary>
        Task<WorkOrderChecklistResult?>
            GetByWorkOrderAndChecklistIdAsync(
                int workOrderId,
                int maintenanceChecklistId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// gerçekleştirilen kontrolleri getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>> GetByCheckedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli iş emrinde
        /// gerçekleştirdiği kontrolleri getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>>
            GetByWorkOrderAndUserIdAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerinin
        /// kontrol sonuçlarını getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planına bağlı iş emirlerinin
        /// kontrol sonuçlarını getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki tamamlanmış
        /// kontrol sonuçlarını getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>> GetCompletedByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki başarılı
        /// kontrol sonuçlarını getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>> GetSuccessfulByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki başarısız
        /// kontrol sonuçlarını getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>> GetFailedByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait kayıtlı fakat henüz
        /// tamamlanmamış kontrolleri getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>>
            GetIncompleteByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığında
        /// gerçekleştirilen kontrolleri getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde henüz başarıyla tamamlanmamış
        /// aktif ve zorunlu kontrol maddelerini getirir.
        ///
        /// Sonuç kaydı olmayan maddeler de dahildir.
        /// </summary>
        Task<List<MaintenanceChecklist>>
            GetPendingRequiredChecklistsByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı iş emri ve kontrol maddesi için
        /// sonuç kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsResultAsync(
            int workOrderId,
            int maintenanceChecklistId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait herhangi bir
        /// kontrol sonucu bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasResultsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki başarısız
        /// zorunlu kontrol kaydı var mı?
        /// </summary>
        Task<bool> HasFailedRequiredResultsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki kayıtlı kontrol
        /// sonuçlarının toplam sayısını getirir.
        /// </summary>
        Task<int> GetResultCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki tamamlanan
        /// kontrol sayısını getirir.
        /// </summary>
        Task<int> GetCompletedCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki başarılı
        /// kontrol sayısını getirir.
        /// </summary>
        Task<int> GetSuccessfulCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin bakım planındaki bütün aktif
        /// zorunlu kontrollerin başarıyla tamamlanıp
        /// tamamlanmadığını doğrular.
        /// </summary>
        Task<bool> AreRequiredChecklistsSuccessfulAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde kontrol maddesine ait sonucu
        /// oluşturur veya mevcut sonucu günceller.
        ///
        /// İş emri InProgress durumunda olmalı
        /// ve kullanıcı aktif görevli olmalıdır.
        /// </summary>
        Task<WorkOrderChecklistResult> SaveResultAsync(
            int workOrderId,
            int maintenanceChecklistId,
            int checkedByUserId,
            bool isCompleted,
            bool? isSuccessful,
            string? notes = null,
            CancellationToken cancellationToken = default);
    }
}
