using MaintenanceManagement.Application.DTOS.WorkOrderChecklistResultDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkOrderChecklistResultService
    {
        #region Get Methods

        /// <summary>
        /// Bütün iş emri kontrol sonuçlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre kontrol sonucu getirir.
        /// </summary>
        Task<GetByIdWorkOrderChecklistResultDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait kontrol sonuçlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol maddesinin bütün iş emirlerindeki
        /// sonuçlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByMaintenanceChecklistIdAsync(
                int maintenanceChecklistId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emri ve kontrol maddesine ait sonucu getirir.
        /// </summary>
        Task<GetByIdWorkOrderChecklistResultDto?>
            GetByWorkOrderAndChecklistIdAsync(
                int workOrderId,
                int maintenanceChecklistId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının kontrol ettiği kayıtları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByCheckedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli iş emrindeki
        /// kontrol sonuçlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByWorkOrderAndUserIdAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu birime ait kontrol sonuçlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Planlı bakıma ait kontrol sonuçlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki tamamlanmış sonuçları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetCompletedByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki başarılı sonuçları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetSuccessfulByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki başarısız sonuçları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetFailedByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki tamamlanmamış kayıtları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetIncompleteByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kontrol tarihi verilen UTC tarih aralığında
        /// bulunan sonuçları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// İş emrinin kontrol maddesi sonucunu kaydeder.
        ///
        /// Repository mevcut kayıt varsa onu günceller,
        /// yoksa yeni sonuç kaydı oluşturur.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkOrderChecklistResultDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut kontrol sonucu kaydını günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkOrderChecklistResultDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// İş emri ve kontrol maddesine ait
        /// kayıt bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasResultAsync(
            int workOrderId,
            int maintenanceChecklistId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki zorunlu kontrollerin
        /// tamamlanmamış olanları var mı?
        /// </summary>
        Task<bool> HasUncompletedRequiredChecklistsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde başarısız zorunlu
        /// kontroller bulunuyor mu?
        /// </summary>
        Task<bool> HasFailedRequiredChecklistsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin zorunlu kontrol sonuçları
        /// onaya gönderilmeye uygun mu?
        /// </summary>
        Task<bool> AreRequiredChecklistsSuccessfulAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        Task<int> GetResultCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        Task<int> GetCompletedCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        Task<int> GetSuccessfulCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        Task<int> GetFailedCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
