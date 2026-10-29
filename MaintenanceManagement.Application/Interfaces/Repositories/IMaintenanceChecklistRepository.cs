using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IMaintenanceChecklistRepository
        : IGenericRepository<MaintenanceChecklist>
    {
        /// <summary>
        /// Bütün kontrol maddelerini bakım planı
        /// ve ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<MaintenanceChecklist>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kontrol maddesini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<MaintenanceChecklist?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planına ait aktif ve pasif
        /// bütün kontrol maddelerini getirir.
        /// </summary>
        Task<List<MaintenanceChecklist>> GetByPlannedMaintenanceIdAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planına ait aktif
        /// kontrol maddelerini getirir.
        /// </summary>
        Task<List<MaintenanceChecklist>>
            GetActiveByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planındaki aktif ve zorunlu
        /// kontrol maddelerini getirir.
        /// </summary>
        Task<List<MaintenanceChecklist>>
            GetActiveRequiredByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa ait bütün bakım
        /// kontrol maddelerini getirir.
        /// </summary>
        Task<List<MaintenanceChecklist>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyona ait bakım planlarının
        /// kontrol maddelerini getirir.
        /// </summary>
        Task<List<MaintenanceChecklist>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne bağlı bakım
        /// kontrol maddelerini getirir.
        /// </summary>
        Task<List<MaintenanceChecklist>> GetByWorkSubTypeIdAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planındaki belirli sıra
        /// numarasına ait kontrol maddesini getirir.
        /// </summary>
        Task<MaintenanceChecklist?> GetBySequenceNumberAsync(
            int plannedMaintenanceId,
            int sequenceNumber,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı bakım planında aynı başlıkla
        /// kontrol maddesi bulunup bulunmadığını
        /// kontrol eder.
        /// </summary>
        Task<bool> ExistsTitleAsync(
            int plannedMaintenanceId,
            string title,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planında sıra numarasının
        /// daha önce kullanılıp kullanılmadığını
        /// kontrol eder.
        /// </summary>
        Task<bool> ExistsSequenceNumberAsync(
            int plannedMaintenanceId,
            int sequenceNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesine ait bütün iş emri
        /// kontrol sonuçlarını getirir.
        /// </summary>
        Task<List<WorkOrderChecklistResult>> GetResultsByChecklistIdAsync(
            int checklistId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesinin herhangi bir iş
        /// emrinde kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> HasResultsAsync(
            int checklistId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planında aktif kontrol
        /// maddesi olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveChecklistsAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planındaki aktif kontrol
        /// maddelerinin sayısını getirir.
        /// </summary>
        Task<int> GetActiveChecklistCountAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde henüz tamamlanmamış zorunlu
        /// kontrol maddelerini getirir.
        ///
        /// Hiç sonuç kaydı oluşturulmayan maddeler
        /// de tamamlanmamış kabul edilir.
        /// </summary>
        Task<List<MaintenanceChecklist>>
            GetUncompletedRequiredChecklistsByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde tamamlanmamış zorunlu
        /// kontrol bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasUncompletedRequiredChecklistsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde başarısız olarak işaretlenmiş
        /// aktif ve zorunlu kontrol maddesi
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasFailedRequiredChecklistsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesinin aktiflik
        /// durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesinin sırasını günceller.
        /// Aynı bakım planında sıra çakışması
        /// bulunmamalıdır.
        /// </summary>
        Task<bool> UpdateSequenceNumberAsync(
            int id,
            int sequenceNumber,
            CancellationToken cancellationToken = default);
    }
}
