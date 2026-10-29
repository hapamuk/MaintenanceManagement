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
    public class WorkOrderActionRepository :
       GenericRepository<WorkOrderAction>,
       IWorkOrderActionRepository
    {
        public WorkOrderActionRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün iş emri faaliyetlerini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen faaliyet kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkOrderAction?>
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
        /// İş emrine ait bütün faaliyetleri
        /// eskiden yeniye doğru getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.PerformedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// gerçekleştirilen faaliyetleri getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetByPerformedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.PerformedByUserId == userId)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirtilen iş emrinde
        /// gerçekleştirdiği faaliyetleri getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetByWorkOrderAndUserAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.PerformedByUserId == userId)
                .OrderBy(x => x.PerformedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerinin
        /// faaliyet kayıtlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrder.ResponsibleUnitId == unitId)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen takıma atanmış iş emirlerinin
        /// faaliyet kayıtlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrder.TeamId == teamId)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen UTC tarih aralığında
        /// gerçekleştirilen faaliyetleri getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(startDateUtc, endDateUtc);

            return await GetDetailsQuery()
                .Where(x =>
                    x.PerformedAt >= startDateUtc &&
                    x.PerformedAt <= endDateUtc)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinin belirtilen UTC tarih
        /// aralığındaki faaliyetlerini getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetByWorkOrderAndDateRangeAsync(
                int workOrderId,
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(startDateUtc, endDateUtc);

            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.PerformedAt >= startDateUtc &&
                    x.PerformedAt <= endDateUtc)
                .OrderBy(x => x.PerformedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrine ait en son faaliyet
        /// kaydını getirir.
        /// </summary>
        public async Task<WorkOrderAction?>
            GetLatestActionByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde herhangi bir faaliyet
        /// kaydı olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActionsAsync(
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
        /// Kullanıcının belirtilen iş emrinde
        /// faaliyet kaydı olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActionsByUserIdAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.PerformedByUserId == userId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrine ait toplam faaliyet
        /// kaydı sayısını getirir.
        /// </summary>
        public async Task<int> GetActionCountByWorkOrderIdAsync(
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
        /// İş emrinde gerçekleştirilen faaliyetlerin
        /// toplam işçilik süresini dakika olarak getirir.
        /// </summary>
        public async Task<long>
            GetTotalLaborMinutesByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            long? totalMinutes = await _dbSet
                .AsNoTracking()
                .Where(x => x.WorkOrderId == workOrderId)
                .SumAsync(
                    x => (long?)x.LaborMinutes,
                    cancellationToken);

            return totalMinutes ?? 0L;
        }

        /// <summary>
        /// Kullanıcının bütün iş emirlerinde
        /// kaydettiği toplam işçilik süresini getirir.
        /// </summary>
        public async Task<long>
            GetTotalLaborMinutesByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            long? totalMinutes = await _dbSet
                .AsNoTracking()
                .Where(x => x.PerformedByUserId == userId)
                .SumAsync(
                    x => (long?)x.LaborMinutes,
                    cancellationToken);

            return totalMinutes ?? 0L;
        }

        /// <summary>
        /// Kullanıcının belirtilen iş emrindeki
        /// toplam işçilik süresini getirir.
        /// </summary>
        public async Task<long>
            GetTotalLaborMinutesByWorkOrderAndUserAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            long? totalMinutes = await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.PerformedByUserId == userId)
                .SumAsync(
                    x => (long?)x.LaborMinutes,
                    cancellationToken);

            return totalMinutes ?? 0L;
        }

        /// <summary>
        /// Takıma atanmış iş emirlerinde kaydedilen
        /// toplam işçilik süresini getirir.
        ///
        /// Hesaplama iş emrinin TeamId
        /// bilgisine göre yapılır.
        /// </summary>
        public async Task<long> GetTotalLaborMinutesByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            long? totalMinutes = await _dbSet
                .AsNoTracking()
                .Where(x => x.WorkOrder.TeamId == teamId)
                .SumAsync(
                    x => (long?)x.LaborMinutes,
                    cancellationToken);

            return totalMinutes ?? 0L;
        }

        /// <summary>
        /// Devam eden iş emrine faaliyet kaydı ekler.
        ///
        /// Faaliyeti yapan kullanıcının ilgili iş
        /// emrinde aktif görevlendirmesi bulunmalıdır.
        ///
        /// Faaliyet tarihini ve işçilik süresini
        /// doğrulayarak veritabanına kaydeder.
        /// </summary>
        public async Task<WorkOrderAction> RegisterActionAsync(
            WorkOrderAction action,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (action.Id != 0)
            {
                throw new ArgumentException(
                    "Yeni faaliyet kaydının ID değeri sıfır olmalıdır.",
                    nameof(action));
            }

            if (action.WorkOrderId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(action),
                    "Geçerli bir iş emri belirtilmelidir.");
            }

            if (action.PerformedByUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(action),
                    "Faaliyeti yapan kullanıcı belirtilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(action.Description))
            {
                throw new ArgumentException(
                    "Faaliyet açıklaması boş olamaz.",
                    nameof(action));
            }

            if (action.LaborMinutes < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(action.LaborMinutes),
                    "İşçilik süresi negatif olamaz.");
            }

            DateTime now = DateTime.UtcNow;

            if (action.PerformedAt == default)
            {
                action.PerformedAt = now;
            }

            if (action.PerformedAt.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Faaliyet tarihi UTC formatında olmalıdır.",
                    nameof(action.PerformedAt));
            }

            if (action.PerformedAt > now)
            {
                throw new InvalidOperationException(
                    "Faaliyet tarihi gelecekte olamaz.");
            }

            // İş emrini kontrol et.
            var workOrder = await _context.Set<WorkOrder>()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == action.WorkOrderId,
                    cancellationToken);

            if (workOrder == null)
            {
                throw new KeyNotFoundException(
                    "İş emri bulunamadı.");
            }

            // Faaliyet yalnızca devam eden
            // iş emrine kaydedilebilir.
            if (workOrder.Status != WorkStatusEnum.InProgress)
            {
                throw new InvalidOperationException(
                    "Yalnızca devam eden iş emirlerine " +
                    "faaliyet kaydı eklenebilir.");
            }

            // İş emrinin başlangıcından önce
            // faaliyet tarihi girilmesini engelle.
            if (workOrder.StartedAt.HasValue &&
                action.PerformedAt < workOrder.StartedAt.Value)
            {
                throw new InvalidOperationException(
                    "Faaliyet tarihi iş emrinin " +
                    "başlangıç tarihinden önce olamaz.");
            }

            // Faaliyeti gerçekleştiren kullanıcının
            // iş emrinde halen aktif görevli
            // olup olmadığını kontrol et.
            bool isAssigned = await _context
                .Set<WorkOrderPersonnel>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderId == action.WorkOrderId &&

                    x.IsActive &&

                    x.ReleasedAt == null &&

                    x.AssignedAt <= action.PerformedAt &&

                    x.TeamMember.UserId ==
                        action.PerformedByUserId &&

                    x.TeamMember.IsActive &&

                    x.TeamMember.JoinedAt <=
                        action.PerformedAt &&

                    (!x.TeamMember.LeftAt.HasValue ||
                     x.TeamMember.LeftAt.Value > now) &&

                    x.TeamMember.TeamId ==
                        workOrder.TeamId &&

                    x.TeamMember.Team.IsActive &&

                    x.TeamMember.Team.UnitId ==
                        workOrder.ResponsibleUnitId &&

                    x.TeamMember.User.IsActive,
                    cancellationToken);

            if (!isAssigned)
            {
                throw new InvalidOperationException(
                    "Kullanıcının bu iş emrinde aktif " +
                    "personel görevlendirmesi bulunmamaktadır.");
            }

            // Faaliyet metinlerini düzenle.
            action.Description = action.Description.Trim();

            action.Notes = string.IsNullOrWhiteSpace(action.Notes)
                ? null
                : action.Notes.Trim();

            action.CreatedAt = now;
            action.UpdatedAt = null;

            await _dbSet.AddAsync(
                action,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            return action;
        }

        /// <summary>
        /// Başlangıç ve bitiş tarihlerini doğrular.
        /// Tarihler UTC formatında olmalıdır.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Tarih değerleri UTC formatında olmalıdır.");
            }

            if (startDateUtc > endDateUtc)
            {
                throw new ArgumentException(
                    "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }
        }

        /// <summary>
        /// Faaliyet kayıtlarını iş emri, organizasyon,
        /// takım, iş türü, iş alt türü, demirbaş
        /// ve kullanıcı bilgileriyle getirir.
        /// </summary>
        private IQueryable<WorkOrderAction> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş emri ve sorumlu organizasyon
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.ResponsibleUnit)

                // İş emrine atanmış takım
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.Team)

                // İş alt türü ve iş türü
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.WorkSubType)
                        .ThenInclude(x => x.WorkType)

                // İş emrine bağlı demirbaş
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.Asset)

                // Faaliyeti gerçekleştiren kullanıcı
                .Include(x => x.PerformedByUser)
                    .ThenInclude(x => x.PrimaryUnit);
        }
    }
}
