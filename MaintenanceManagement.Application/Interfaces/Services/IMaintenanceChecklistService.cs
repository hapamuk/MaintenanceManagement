using MaintenanceManagement.Application.DTOS.MaintenanceChecklistDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IMaintenanceChecklistService
    {
        #region Get Methods

        /// <summary>
        /// Bütün kontrol maddelerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre kontrol detayını getirir.
        /// </summary>
        Task<GetByIdMaintenanceChecklistDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlı bakıma ait bütün kontrol
        /// maddelerini sıra numarasına göre getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlı bakımın aktif kontrol maddeleri.
        /// </summary>
        Task<IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetActiveByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlı bakımın aktif ve zorunlu
        /// kontrol maddelerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetActiveRequiredByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa bağlı kontrol maddelerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimine bağlı
        /// kontrol maddelerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne bağlı kontrol
        /// maddelerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlı bakım ve sıra numarasına
        /// göre kontrol maddesi getirir.
        /// </summary>
        Task<GetByIdMaintenanceChecklistDto?>
            GetBySequenceNumberAsync(
                int plannedMaintenanceId,
                int sequenceNumber,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki tamamlanmamış zorunlu
        /// kontrol maddelerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetUncompletedRequiredByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Planlı bakıma yeni kontrol maddesi ekler.
        /// </summary>
        Task<int> CreateAsync(
            CreateMaintenanceChecklistDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesinin başlık, açıklama,
        /// sıra ve zorunluluk bilgisini günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateMaintenanceChecklistDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesini aktif veya pasif yapar.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesinin sıra numarasını değiştirir.
        /// </summary>
        Task<bool> UpdateSequenceNumberAsync(
            int id,
            int sequenceNumber,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı bakım planında başlık kullanılıyor mu?
        /// </summary>
        Task<bool> ExistsTitleAsync(
            int plannedMaintenanceId,
            string title,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı bakım planında sıra numarası
        /// kullanılıyor mu?
        /// </summary>
        Task<bool> ExistsSequenceNumberAsync(
            int plannedMaintenanceId,
            int sequenceNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesinin iş emri
        /// sonuç kaydı bulunuyor mu?
        /// </summary>
        Task<bool> HasResultsAsync(
            int checklistId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlı bakımın aktif kontrol
        /// maddesi bulunuyor mu?
        /// </summary>
        Task<bool> HasActiveChecklistsAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde tamamlanmamış zorunlu
        /// kontrol maddeleri var mı?
        /// </summary>
        Task<bool> HasUncompletedRequiredChecklistsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde başarısız zorunlu
        /// kontrol maddeleri var mı?
        /// </summary>
        Task<bool> HasFailedRequiredChecklistsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        /// <summary>
        /// Planlı bakımın aktif kontrol sayısı.
        /// </summary>
        Task<int> GetActiveChecklistCountAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesinin iş emirlerindeki
        /// toplam sonuç kaydı sayısı.
        /// </summary>
        Task<int> GetResultCountByChecklistIdAsync(
            int checklistId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
