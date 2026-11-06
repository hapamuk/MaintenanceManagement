using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class PlannedMaintenanceRepository :
        GenericRepository<PlannedMaintenance>,
        IPlannedMaintenanceRepository
    {
        public PlannedMaintenanceRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün bakım planlarını ilişkili
        /// detaylarıyla getirir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen bakım planını demirbaş,
        /// organizasyon ve iş türü bilgileriyle getirir.
        /// </summary>
        public async Task<PlannedMaintenance?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Aktif bakım planlarını sonraki
        /// bakım tarihlerine göre getirir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetActiveMaintenancesAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetActivePlansQuery()
                .Include(x => x.Asset)
                .Include(x => x.Unit)
                .Include(x => x.WorkSubType)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen demirbaşın geçmiş ve mevcut
        /// bütün bakım planlarını getirir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.NextMaintenanceDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona ait aktif ve pasif
        /// bütün bakım planlarını getirir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UnitId == unitId)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona ait aktif
        /// bakım planlarını getirir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetActiveByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.UnitId == unitId &&
                    x.IsActive)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş alt türüne bağlı bütün
        /// bakım planlarını getirir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkSubTypeId == workSubTypeId)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş türüne bağlı bakım planlarını getirir.
        /// İş alt türü ilişkisi üzerinden sorgulanır.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkSubType.WorkTypeId == workTypeId)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının oluşturduğu bakım
        /// planlarını getirir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetByCreatedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.CreatedByUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen tarihe kadar bakım zamanı
        /// gelmiş aktif planları getirir.
        ///
        /// Gecikmiş planlar da dahildir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetDueMaintenancesAsync(
                DateTime? dueUntilUtc = null,
                CancellationToken cancellationToken = default)
        {
            DateTime dueDate = dueUntilUtc ?? DateTime.UtcNow;

            ValidateUtcDate(dueDate);

            return await GetDetailsQuery()
                .Where(x =>
                    x.IsActive &&
                    x.NextMaintenanceDate <= dueDate)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bakım tarihi geçmiş aktif
        /// bakım planlarını getirir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetOverdueMaintenancesAsync(
                CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await GetDetailsQuery()
                .Where(x =>
                    x.IsActive &&
                    x.NextMaintenanceDate < now)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Önümüzdeki belirtilen gün içerisinde
        /// zamanı gelecek aktif bakımları getirir.
        ///
        /// Geçmiş tarihli bakımlar dahil edilmez.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetUpcomingMaintenancesAsync(
                int days,
                CancellationToken cancellationToken = default)
        {
            if (days < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(days),
                    "Gün sayısı negatif olamaz.");
            }

            DateTime now = DateTime.UtcNow;

            DateTime endDate = now.AddDays(days);

            return await GetDetailsQuery()
                .Where(x =>
                    x.IsActive &&
                    x.NextMaintenanceDate >= now &&
                    x.NextMaintenanceDate <= endDate)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sonraki bakım tarihi belirtilen
        /// UTC aralığında olan bakım planlarını getirir.
        ///
        /// Aktif ve pasif planlar dahildir.
        /// </summary>
        public async Task<List<PlannedMaintenance>>
            GetByNextMaintenanceDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(startDateUtc, endDateUtc);

            return await GetDetailsQuery()
                .Where(x =>
                    x.NextMaintenanceDate >= startDateUtc &&
                    x.NextMaintenanceDate <= endDateUtc)
                .OrderBy(x => x.NextMaintenanceDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bakım planından oluşturulmuş
        /// bütün iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetWorkOrdersByMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrder>()
                .AsNoTracking()
                .Include(x => x.Team)
                .Include(x => x.ResponsibleUnit)
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType)
                .Where(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bakım planına tanımlanmış
        /// kontrol listesi maddelerini getirir.
        ///
        /// Pasif maddeler de dahildir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetChecklistsByMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<MaintenanceChecklist>()
                .AsNoTracking()
                .Where(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId)
                .OrderBy(x => x.SequenceNumber)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bakım planına bağlı en son
        /// tamamlanan iş emrini getirir.
        /// </summary>
        public async Task<WorkOrder?>
            GetLatestCompletedWorkOrderAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrder>()
                .AsNoTracking()
                .Where(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.Status == WorkStatusEnum.Completed)
                .OrderByDescending(x => x.CompletedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Aynı demirbaş ve iş alt türü için
        /// aktif bakım planı bulunup bulunmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> ExistsActivePlanAsync(
            int assetId,
            int workSubTypeId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.AssetId == assetId &&
                    x.WorkSubTypeId == workSubTypeId &&
                    x.IsActive &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Bakım planından herhangi bir
        /// iş emri oluşturulup oluşturulmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasWorkOrdersAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrder>()
                .AsNoTracking()
                .AnyAsync(
                    x => x.PlannedMaintenanceId == plannedMaintenanceId,
                    cancellationToken);
        }

        /// <summary>
        /// Bakım planına bağlı açık iş
        /// emri bulunup bulunmadığını kontrol eder.
        ///
        /// New, InProgress ve PendingApproval
        /// durumları açık kabul edilir.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrder>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    (
                        x.Status == WorkStatusEnum.New ||
                        x.Status == WorkStatusEnum.InProgress ||
                        x.Status == WorkStatusEnum.PendingApproval
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// Bakım planına bağlı aktif
        /// kontrol maddesi olup olmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveChecklistsAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<MaintenanceChecklist>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyonda bakım zamanı gelmiş
        /// aktif bakım planı bulunup bulunmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasDueMaintenanceByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UnitId == unitId &&
                    x.IsActive &&
                    x.NextMaintenanceDate <= now,
                    cancellationToken);
        }

        /// <summary>
        /// Bakım planının aktiflik durumunu
        /// değiştirir ve veritabanına kaydeder.
        ///
        /// Fiziksel silme yapılmaz.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var maintenance = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (maintenance == null)
            {
                return false;
            }

            if (maintenance.IsActive == isActive)
            {
                return true;
            }

            maintenance.IsActive = isActive;
            maintenance.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Gerçekleştirilen bakıma ait tarihi
        /// son bakım tarihi olarak kaydeder.
        ///
        /// Bir sonraki bakım tarihi:
        /// completedAtUtc + PeriodDays
        ///
        /// İş emri durumunu değiştirmez.
        /// </summary>
        public async Task<bool> UpdateMaintenanceDatesAsync(
            int id,
            DateTime completedAtUtc,
            CancellationToken cancellationToken = default)
        {
            ValidateUtcDate(completedAtUtc);

            DateTime now = DateTime.UtcNow;

            if (completedAtUtc > now)
            {
                throw new ArgumentException(
                    "Bakım tamamlanma tarihi gelecekte olamaz.",
                    nameof(completedAtUtc));
            }

            var maintenance = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (maintenance == null)
            {
                return false;
            }

            if (!maintenance.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif bakım planının tarihleri güncellenemez.");
            }

            if (maintenance.PeriodDays <= 0)
            {
                throw new InvalidOperationException(
                    "Bakım periyodu sıfırdan büyük olmalıdır.");
            }

            if (maintenance.LastMaintenanceDate.HasValue)
            {
                // Aynı tamamlanma tarihi tekrar uygulanmışsa
                // işlemi başarılı kabul et.
                if (maintenance.LastMaintenanceDate.Value ==
                    completedAtUtc)
                {
                    return true;
                }

                // Geçmiş bakım kaydı yeni tarihin
                // üzerine yazılmamalıdır.
                if (completedAtUtc <
                    maintenance.LastMaintenanceDate.Value)
                {
                    throw new InvalidOperationException(
                        "Yeni tamamlanma tarihi son bakım " +
                        "tarihinden önce olamaz.");
                }
            }

            DateTime nextMaintenanceDate =
                completedAtUtc.AddDays(maintenance.PeriodDays);

            maintenance.LastMaintenanceDate = completedAtUtc;
            maintenance.NextMaintenanceDate = nextMaintenanceDate;

            maintenance.UpdatedAt = now;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Yalnızca aktif bakım planlarını
        /// sorgulayan ortak metottur.
        /// </summary>
        private IQueryable<PlannedMaintenance> GetActivePlansQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive);
        }

        /// <summary>
        /// Bakım planlarını demirbaş, marka,
        /// model, kategori, organizasyon ve
        /// iş türü bilgileriyle getirir.
        /// </summary>
        private IQueryable<PlannedMaintenance> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // Demirbaş ve güncel organizasyonu
                .Include(x => x.Asset)
                    .ThenInclude(x => x.Unit)

                // Demirbaş modeli ve markası
                .Include(x => x.Asset)
                    .ThenInclude(x => x.AssetModel)
                        .ThenInclude(x => x.Brand)

                // Demirbaş türü ve kategorisi
                .Include(x => x.Asset)
                    .ThenInclude(x => x.AssetKind)
                        .ThenInclude(x => x.AssetCategory)

                // Bakımdan sorumlu organizasyon
                // OrganizationTypeEnum, Unit üzerinde bir enum alanıdır.
                .Include(x => x.Unit)

                // İş alt türü ve bağlı iş türü
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType);
        }

        /// <summary>
        /// Tek bir tarihin UTC olduğunu kontrol eder.
        /// </summary>
        private static void ValidateUtcDate(DateTime date)
        {
            if (date.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Tarih UTC formatında olmalıdır.");
            }
        }

        /// <summary>
        /// Tarih aralığının UTC ve kronolojik
        /// olarak geçerli olduğunu kontrol eder.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            ValidateUtcDate(startDateUtc);
            ValidateUtcDate(endDateUtc);

            if (startDateUtc > endDateUtc)
            {
                throw new ArgumentException(
                    "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }
        }
    }
}
