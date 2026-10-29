using MaintenanceManagement.Application.DTOS.WorkOrderActionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkOrderActionService
    {
        #region Get Methods

        /// <summary>
        /// Bütün iş emri faaliyetlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre faaliyet detayını getirir.
        /// </summary>
        Task<GetByIdWorkOrderActionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait faaliyetleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının gerçekleştirdiği faaliyetleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByPerformedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli iş emrindeki
        /// faaliyetlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByWorkOrderAndUserAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu birimin faaliyetlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Takımın iş emirlerine ait faaliyetleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığındaki
        /// faaliyetleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli iş emrindeki faaliyetleri
        /// UTC tarih aralığına göre getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByWorkOrderAndDateRangeAsync(
                int workOrderId,
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin son faaliyet kaydını getirir.
        /// </summary>
        Task<GetByIdWorkOrderActionDto?>
            GetLatestActionByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Devam eden iş emrine yeni faaliyet ekler.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkOrderActionDto dto,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının kendi faaliyet kaydını günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkOrderActionDto dto,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// İş emrinde faaliyet bulunuyor mu?
        /// </summary>
        Task<bool> HasActionsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirtilen iş emrinde
        /// faaliyet kaydı bulunuyor mu?
        ///
        /// Parametre sırası Repository ile uyumludur.
        /// </summary>
        Task<bool> HasActionsByUserIdAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        /// <summary>
        /// İş emrindeki faaliyet sayısını getirir.
        /// </summary>
        Task<int> GetActionCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Labor Methods

        /// <summary>
        /// İş emrinin toplam çalışma süresi (dakika).
        /// </summary>
        Task<long> GetTotalLaborMinutesByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının toplam çalışma süresi (dakika).
        /// </summary>
        Task<long> GetTotalLaborMinutesByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli iş emrindeki
        /// toplam çalışma süresi (dakika).
        /// </summary>
        Task<long> GetTotalLaborMinutesByWorkOrderAndUserAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takımın toplam çalışma süresi (dakika).
        /// </summary>
        Task<long> GetTotalLaborMinutesByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
