using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IPlannedMaintenanceRepository
        : IGenericRepository<PlannedMaintenance>
    {
        /// <summary>
        /// Bütün bakım planlarını demirbaş,
        /// organizasyon ve iş türü detaylarıyla getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen bakım planını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<PlannedMaintenance?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif bakım planlarını getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetActiveMaintenancesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa ait bütün bakım
        /// planlarını getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyona ait bütün
        /// bakım planlarını getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyona ait aktif
        /// bakım planlarını getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetActiveByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne bağlı
        /// bakım planlarını getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetByWorkSubTypeIdAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı bakım
        /// planlarını getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// oluşturulan bakım planlarını getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetByCreatedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen tarihe kadar bakım zamanı
        /// gelmiş aktif planları getirir.
        ///
        /// Tarih belirtilmezse geçerli UTC zamanı kullanılır.
        /// Gecikmiş bakım planları da dahildir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetDueMaintenancesAsync(
            DateTime? dueUntilUtc = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sonraki bakım tarihi geçmiş
        /// aktif bakım planlarını getirir.
        /// </summary>
        Task<List<PlannedMaintenance>> GetOverdueMaintenancesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen gün içerisinde bakım zamanı
        /// gelecek aktif planları getirir.
        ///
        /// Gecikmiş bakım planları dahil edilmez.
        /// </summary>
        Task<List<PlannedMaintenance>> GetUpcomingMaintenancesAsync(
            int days,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sonraki bakım tarihi belirtilen UTC
        /// aralığında olan planları getirir.
        /// </summary>
        Task<List<PlannedMaintenance>>
            GetByNextMaintenanceDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planından oluşturulan
        /// iş emirlerini getirir.
        /// </summary>
        Task<List<WorkOrder>> GetWorkOrdersByMaintenanceIdAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planına bağlı kontrol
        /// listesi maddelerini getirir.
        /// </summary>
        Task<List<MaintenanceChecklist>> GetChecklistsByMaintenanceIdAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planına bağlı en son tamamlanmış
        /// iş emrini getirir.
        /// </summary>
        Task<WorkOrder?> GetLatestCompletedWorkOrderAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı demirbaş ve iş alt türü için
        /// aktif bakım planı bulunup bulunmadığını
        /// kontrol eder.
        /// </summary>
        Task<bool> ExistsActivePlanAsync(
            int assetId,
            int workSubTypeId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planından herhangi bir
        /// iş emri oluşturulup oluşturulmadığını
        /// kontrol eder.
        /// </summary>
        Task<bool> HasWorkOrdersAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planına ait açık iş
        /// emri olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planında aktif kontrol
        /// maddesi bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveChecklistsAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonda bakım zamanı
        /// gelmiş aktif plan bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasDueMaintenanceByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım planının aktiflik
        /// durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bakım gerçekleştirildiğinde son bakım
        /// tarihini ve sonraki bakım tarihini günceller.
        ///
        /// Sonraki bakım:
        /// completedAtUtc + PeriodDays
        /// </summary>
        Task<bool> UpdateMaintenanceDatesAsync(
            int id,
            DateTime completedAtUtc,
            CancellationToken cancellationToken = default);
    }
}
