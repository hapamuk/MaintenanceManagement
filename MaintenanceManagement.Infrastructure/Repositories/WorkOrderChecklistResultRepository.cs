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
    public class WorkOrderChecklistResultRepository :
         GenericRepository<WorkOrderChecklistResult>,
         IWorkOrderChecklistResultRepository
    {
        public WorkOrderChecklistResultRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün kontrol sonuçlarını ilişkili
        /// detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.CheckedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kontrol sonucunu ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkOrderChecklistResult?>
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
        /// İş emrine ait bütün kayıtlı
        /// kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.MaintenanceChecklist.SequenceNumber)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen bakım kontrol maddesinin
        /// farklı iş emirlerindeki sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetByMaintenanceChecklistIdAsync(
                int maintenanceChecklistId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.MaintenanceChecklistId ==
                        maintenanceChecklistId)
                .OrderByDescending(x => x.CheckedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emri ve bakım kontrol maddesine
        /// ait mevcut sonuç kaydını getirir.
        /// </summary>
        public async Task<WorkOrderChecklistResult?>
            GetByWorkOrderAndChecklistIdAsync(
                int workOrderId,
                int maintenanceChecklistId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.MaintenanceChecklistId ==
                        maintenanceChecklistId,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcı tarafından gerçekleştirilen
        /// kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetByCheckedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.CheckedByUserId == userId)
                .OrderByDescending(x => x.CheckedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirli iş emrinde
        /// gerçekleştirdiği kontrolleri getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetByWorkOrderAndUserIdAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.CheckedByUserId == userId)
                .OrderBy(x => x.CheckedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerinin
        /// bakım kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrder.ResponsibleUnitId == unitId)
                .OrderByDescending(x => x.CheckedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen bakım planından oluşturulan
        /// iş emirlerinin kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrder.PlannedMaintenanceId ==
                        plannedMaintenanceId &&
                    x.MaintenanceChecklist.PlannedMaintenanceId ==
                        plannedMaintenanceId)
                .OrderByDescending(x => x.CheckedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrindeki tamamlanmış
        /// kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetCompletedByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.IsCompleted)
                .OrderBy(x => x.MaintenanceChecklist.SequenceNumber)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrindeki başarılı olarak
        /// tamamlanmış kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetSuccessfulByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.IsCompleted &&
                    x.IsSuccessful == true)
                .OrderBy(x => x.MaintenanceChecklist.SequenceNumber)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrindeki başarısız olarak
        /// tamamlanmış kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetFailedByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.IsCompleted &&
                    x.IsSuccessful == false)
                .OrderBy(x => x.MaintenanceChecklist.SequenceNumber)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde kayıtlı ancak henüz
        /// tamamlanmamış kontrol sonuçlarını getirir.
        ///
        /// Hiç sonuç kaydı bulunmayan maddeler
        /// bu metotla getirilmez.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetIncompleteByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    !x.IsCompleted)
                .OrderBy(x => x.MaintenanceChecklist.SequenceNumber)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen UTC tarih aralığında
        /// tamamlanan kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(startDateUtc, endDateUtc);

            return await GetDetailsQuery()
                .Where(x =>
                    x.CheckedAt.HasValue &&
                    x.CheckedAt.Value >= startDateUtc &&
                    x.CheckedAt.Value <= endDateUtc)
                .OrderByDescending(x => x.CheckedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde henüz başarıyla tamamlanmamış
        /// aktif ve zorunlu kontrol maddelerini getirir.
        ///
        /// Kontrolün başarıyla tamamlanmış sayılması için:
        /// IsCompleted = true
        /// IsSuccessful = true
        ///
        /// Bu şartları karşılayan sonuç kaydı
        /// bulunmayan maddeler listelenir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetPendingRequiredChecklistsByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            int? plannedMaintenanceId =
                await GetPlannedMaintenanceIdAsync(
                    workOrderId,
                    cancellationToken);

            if (!plannedMaintenanceId.HasValue)
            {
                return new List<MaintenanceChecklist>();
            }

            return await _context.Set<MaintenanceChecklist>()
                .AsNoTracking()
                .Where(checklist =>
                    checklist.PlannedMaintenanceId ==
                        plannedMaintenanceId.Value &&

                    checklist.IsActive &&

                    checklist.IsRequired &&

                    !_dbSet.Any(result =>
                        result.WorkOrderId == workOrderId &&

                        result.MaintenanceChecklistId ==
                            checklist.Id &&

                        result.IsCompleted &&

                        result.IsSuccessful == true))
                .OrderBy(x => x.SequenceNumber)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emri ve kontrol maddesi için
        /// mevcut sonuç kaydı olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsResultAsync(
            int workOrderId,
            int maintenanceChecklistId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.MaintenanceChecklistId ==
                        maintenanceChecklistId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinde herhangi bir kontrol
        /// sonucu kaydı olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasResultsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(
                    x => x.WorkOrderId == workOrderId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinin bakım planında başarısız
        /// tamamlanmış aktif ve zorunlu kontrol
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasFailedRequiredResultsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(result =>
                    result.WorkOrderId == workOrderId &&

                    result.IsCompleted &&

                    result.IsSuccessful == false &&

                    result.MaintenanceChecklist.IsActive &&

                    result.MaintenanceChecklist.IsRequired &&

                    result.MaintenanceChecklist.PlannedMaintenanceId ==
                        result.WorkOrder.PlannedMaintenanceId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinde kayıtlı kontrol
        /// sonuçlarının sayısını getirir.
        /// </summary>
        public async Task<int> GetResultCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(
                    x => x.WorkOrderId == workOrderId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrindeki tamamlanan
        /// kontrol sonuçlarının sayısını getirir.
        /// </summary>
        public async Task<int> GetCompletedCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.IsCompleted,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrindeki başarılı
        /// kontrol sonuçlarının sayısını getirir.
        /// </summary>
        public async Task<int> GetSuccessfulCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.IsCompleted &&
                    x.IsSuccessful == true,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrindeki bütün aktif zorunlu
        /// kontrol maddelerinin başarıyla
        /// tamamlanıp tamamlanmadığını kontrol eder.
        ///
        /// İş emrinde bakım planı bulunmuyorsa
        /// veya hiç aktif kontrol maddesi
        /// tanımlanmamışsa false döner.
        /// </summary>
        public async Task<bool>
            AreRequiredChecklistsSuccessfulAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            var workOrder = await _context.Set<WorkOrder>()
                .AsNoTracking()
                .Where(x => x.Id == workOrderId)
                .Select(x => new
                {
                    x.PlannedMaintenanceId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (workOrder == null ||
                !workOrder.PlannedMaintenanceId.HasValue)
            {
                return false;
            }

            int plannedMaintenanceId =
                workOrder.PlannedMaintenanceId.Value;

            // Bakım planı aktif olmalıdır.
            bool isPlanActive = await _context
                .Set<PlannedMaintenance>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == plannedMaintenanceId &&
                    x.IsActive,
                    cancellationToken);

            if (!isPlanActive)
            {
                return false;
            }

            // Bakım planında en az bir aktif
            // kontrol maddesi bulunmalıdır.
            bool hasActiveChecklist =
                await _context.Set<MaintenanceChecklist>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.PlannedMaintenanceId ==
                            plannedMaintenanceId &&
                        x.IsActive,
                        cancellationToken);

            if (!hasActiveChecklist)
            {
                return false;
            }

            // Başarıyla tamamlanmamış zorunlu
            // herhangi bir kontrol var mı?
            bool hasPendingRequired =
                await _context.Set<MaintenanceChecklist>()
                    .AsNoTracking()
                    .AnyAsync(checklist =>
                        checklist.PlannedMaintenanceId ==
                            plannedMaintenanceId &&

                        checklist.IsActive &&

                        checklist.IsRequired &&

                        !_dbSet.Any(result =>
                            result.WorkOrderId == workOrderId &&

                            result.MaintenanceChecklistId ==
                                checklist.Id &&

                            result.IsCompleted &&

                            result.IsSuccessful == true),
                        cancellationToken);

            return !hasPendingRequired;
        }

        /// <summary>
        /// Devam eden iş emrinde bakım kontrol
        /// sonucunu oluşturur veya günceller.
        ///
        /// İş emrinin bakım planı ile kontrol
        /// maddesinin bakım planı aynı olmalıdır.
        ///
        /// Kontrolü yapan kullanıcı iş emrinde
        /// aktif olarak görevli bulunmalıdır.
        /// </summary>
        public async Task<WorkOrderChecklistResult> SaveResultAsync(
            int workOrderId,
            int maintenanceChecklistId,
            int checkedByUserId,
            bool isCompleted,
            bool? isSuccessful,
            string? notes = null,
            CancellationToken cancellationToken = default)
        {
            if (workOrderId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(workOrderId),
                    "Geçerli bir iş emri belirtilmelidir.");
            }

            if (maintenanceChecklistId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maintenanceChecklistId),
                    "Geçerli bir kontrol maddesi belirtilmelidir.");
            }

            if (checkedByUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(checkedByUserId),
                    "Geçerli bir kullanıcı belirtilmelidir.");
            }

            // Tamamlanmamış kontrolün
            // başarı sonucu bulunmamalıdır.
            if (!isCompleted && isSuccessful.HasValue)
            {
                throw new ArgumentException(
                    "Tamamlanmamış kontrol için " +
                    "başarı sonucu belirlenemez.",
                    nameof(isSuccessful));
            }

            // Tamamlanan kontrolün başarı
            // sonucu mutlaka belirtilmelidir.
            if (isCompleted && !isSuccessful.HasValue)
            {
                throw new ArgumentException(
                    "Tamamlanan kontrolün başarı " +
                    "sonucu belirtilmelidir.",
                    nameof(isSuccessful));
            }

            // Başarısız sonuçlar gerekçeli olmalıdır.
            if (isCompleted &&
                isSuccessful == false &&
                string.IsNullOrWhiteSpace(notes))
            {
                throw new ArgumentException(
                    "Başarısız kontrol için açıklama zorunludur.",
                    nameof(notes));
            }

            DateTime now = DateTime.UtcNow;

            // İş emri ve ilgili ilişki bilgilerini getir.
            var workOrder = await _context.Set<WorkOrder>()
                .AsNoTracking()
                .Where(x => x.Id == workOrderId)
                .Select(x => new
                {
                    x.Id,
                    x.Status,
                    x.PlannedMaintenanceId,
                    x.WorkSubTypeId,
                    x.ResponsibleUnitId,
                    x.TeamId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (workOrder == null)
            {
                throw new KeyNotFoundException(
                    "İş emri bulunamadı.");
            }

            // Kontrol sonuçları yalnızca
            // devam eden iş emirlerine kaydedilir.
            if (workOrder.Status != WorkStatusEnum.InProgress)
            {
                throw new InvalidOperationException(
                    "Yalnızca devam eden iş emirlerine " +
                    "bakım kontrol sonucu kaydedilebilir.");
            }

            if (!workOrder.PlannedMaintenanceId.HasValue)
            {
                throw new InvalidOperationException(
                    "İş emrine bağlı bir bakım planı bulunmuyor.");
            }

            // Kontrol maddesinin bu iş emrinin
            // bakım planına ait olduğunu doğrula.
            bool isValidChecklist = await _context
                .Set<MaintenanceChecklist>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == maintenanceChecklistId &&

                    x.PlannedMaintenanceId ==
                        workOrder.PlannedMaintenanceId.Value &&

                    x.IsActive &&

                    x.PlannedMaintenance.IsActive &&

                    x.PlannedMaintenance.UnitId ==
                        workOrder.ResponsibleUnitId &&

                    x.PlannedMaintenance.WorkSubTypeId ==
                        workOrder.WorkSubTypeId,
                    cancellationToken);

            if (!isValidChecklist)
            {
                throw new InvalidOperationException(
                    "Kontrol maddesi bu iş emrinin aktif " +
                    "bakım planına ait değildir veya " +
                    "organizasyon ve iş türü eşleşmiyor.");
            }

            // Kontrolü gerçekleştiren personelin
            // iş emrinde aktif görevli olması gerekir.
            bool isAssigned = await _context
                .Set<WorkOrderPersonnel>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&

                    x.IsActive &&

                    x.ReleasedAt == null &&

                    x.AssignedAt <= now &&

                    x.TeamMember.UserId == checkedByUserId &&

                    x.TeamMember.IsActive &&

                    x.TeamMember.JoinedAt <= now &&

                    (!x.TeamMember.LeftAt.HasValue ||
                     x.TeamMember.LeftAt.Value > now) &&

                    x.TeamMember.TeamId == workOrder.TeamId &&

                    x.TeamMember.Team.IsActive &&

                    x.TeamMember.Team.UnitId ==
                        workOrder.ResponsibleUnitId &&

                    x.TeamMember.User.IsActive,
                    cancellationToken);

            if (!isAssigned)
            {
                throw new InvalidOperationException(
                    "Kontrolü gerçekleştiren kullanıcı bu " +
                    "iş emrinde aktif görevli değildir.");
            }

            // Aynı iş emri ve kontrol maddesi
            // için yeni kayıt yerine mevcut
            // kayıt güncellenir.
            var result = await _dbSet
                .FirstOrDefaultAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.MaintenanceChecklistId ==
                        maintenanceChecklistId,
                    cancellationToken);

            string? normalizedNotes =
                string.IsNullOrWhiteSpace(notes)
                    ? null
                    : notes.Trim();

            if (result == null)
            {
                result = new WorkOrderChecklistResult
                {
                    WorkOrderId = workOrderId,

                    MaintenanceChecklistId =
                        maintenanceChecklistId,

                    IsCompleted = isCompleted,

                    IsSuccessful = isCompleted
                        ? isSuccessful
                        : null,

                    CheckedAt = isCompleted ? now : null,

                    CheckedByUserId = isCompleted
                        ? checkedByUserId
                        : null,

                    Notes = normalizedNotes,

                    CreatedAt = now,

                    UpdatedAt = null
                };

                await _dbSet.AddAsync(
                    result,
                    cancellationToken);
            }
            else
            {
                result.IsCompleted = isCompleted;

                result.IsSuccessful = isCompleted
                    ? isSuccessful
                    : null;

                result.CheckedAt = isCompleted
                    ? now
                    : null;

                result.CheckedByUserId = isCompleted
                    ? checkedByUserId
                    : null;

                result.Notes = normalizedNotes;

                result.UpdatedAt = now;
            }

            await _context.SaveChangesAsync(
                cancellationToken);

            return result;
        }

        /// <summary>
        /// İş emrine bağlı bakım planı ID
        /// bilgisini getirir.
        /// </summary>
        private async Task<int?> GetPlannedMaintenanceIdAsync(
            int workOrderId,
            CancellationToken cancellationToken)
        {
            return await _context.Set<WorkOrder>()
                .AsNoTracking()
                .Where(x => x.Id == workOrderId)
                .Select(x => x.PlannedMaintenanceId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Tarih aralığının UTC formatında
        /// ve kronolojik olarak uygun olduğunu doğrular.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Başlangıç ve bitiş tarihleri UTC olmalıdır.");
            }

            if (startDateUtc > endDateUtc)
            {
                throw new ArgumentException(
                    "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }
        }

        /// <summary>
        /// Kontrol sonuçlarını iş emri,
        /// bakım planı, demirbaş, organizasyon
        /// ve kontrol maddesiyle birlikte getirir.
        /// </summary>
        private IQueryable<WorkOrderChecklistResult> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş emri ve sorumlu organizasyon
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.ResponsibleUnit)

                // İş emrinin takımı
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.Team)

                // İş alt türü ve iş türü
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.WorkSubType)
                        .ThenInclude(x => x.WorkType)

                // Kontrol maddesi ve bakım planı
                .Include(x => x.MaintenanceChecklist)
                    .ThenInclude(x => x.PlannedMaintenance)

                // Bakım planındaki demirbaş
                .Include(x => x.MaintenanceChecklist)
                    .ThenInclude(x => x.PlannedMaintenance)
                        .ThenInclude(x => x.Asset);
        }
    }
}
