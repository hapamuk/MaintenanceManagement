using MaintenanceManagement.Application.Interfaces;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class WorkOrderRepository :
        GenericRepository<WorkOrder>,
        IWorkOrderRepository
    {
        public WorkOrderRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün iş emirlerini ilişkili
        /// detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrini ID bilgisine göre
        /// bütün ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkOrder?>
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
        /// İş emri numarasına göre
        /// mevcut kaydı getirir.
        /// </summary>
        public async Task<WorkOrder?>
            GetByWorkOrderNumberAsync(
                string workOrderNumber,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workOrderNumber))
            {
                return null;
            }

            string normalizedNumber = workOrderNumber.Trim();

            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.WorkOrderNumber == normalizedNumber,
                    cancellationToken);
        }

        /// <summary>
        /// İş talebinden oluşturulmuş
        /// iş emrini getirir.
        /// </summary>
        public async Task<WorkOrder?>
            GetByWorkRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.WorkRequestId == workRequestId,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen planlı bakım kaydından
        /// oluşturulan iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyona ait
        /// bütün iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.ResponsibleUnitId == unitId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyona ait
        /// açık iş emirlerini getirir.
        ///
        /// New, InProgress ve PendingApproval
        /// durumları açık iş olarak değerlendirilir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetOpenWorkOrdersByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetOpenWorkOrdersQuery()
                .Where(x => x.ResponsibleUnitId == unitId)
                .Include(x => x.ResponsibleUnit)
                .Include(x => x.Team)
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType)
                .OrderByDescending(x => x.Priority)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen takıma atanmış
        /// bütün iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.TeamId == teamId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş alt türündeki
        /// bütün iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkSubTypeId == workSubTypeId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen demirbaşa ait
        /// bütün iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// oluşturulmuş iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
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
        /// Kullanıcının aktif takım üyeliği üzerinden
        /// halen görevli olduğu iş emirlerini getirir.
        ///
        /// İş emri personeli pasif veya serbest
        /// bırakılmışsa sorguya dahil edilmez.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetAssignedWorkOrdersByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    _context.Set<WorkOrderPersonnel>().Any(p =>
                        p.WorkOrderId == x.Id &&
                        p.IsActive &&
                        p.ReleasedAt == null &&
                        p.TeamMember.UserId == userId &&
                        p.TeamMember.IsActive &&
                        p.TeamMember.LeftAt == null &&
                        p.TeamMember.User.IsActive &&
                        p.TeamMember.Team.IsActive))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen durumdaki
        /// iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetByStatusAsync(
                WorkStatusEnum status,
                CancellationToken cancellationToken = default)
        {
            ValidateStatus(status);

            return await GetDetailsQuery()
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen öncelik seviyesindeki
        /// iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetByPriorityAsync(
                WorkPriorityEnum priority,
                CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(
                typeof(WorkPriorityEnum), priority))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(priority),
                    "Geçersiz iş emri önceliği.");
            }

            return await GetDetailsQuery()
                .Where(x => x.Priority == priority)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyonda yönetici onayı
        /// bekleyen iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetPendingApprovalByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.ResponsibleUnitId == unitId &&
                    x.Status == WorkStatusEnum.PendingApproval)
                .OrderBy(x => x.SubmittedForApprovalAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Planlanan bitiş tarihi geçmiş
        /// ve halen açık durumdaki iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetOverdueWorkOrdersAsync(
                CancellationToken cancellationToken = default)
        {
            DateTime now = DateTime.UtcNow;

            return await GetOpenWorkOrdersQuery()
                .Where(x =>
                    x.PlannedEndDate.HasValue &&
                    x.PlannedEndDate.Value < now)
                .Include(x => x.ResponsibleUnit)
                .Include(x => x.Team)
                .Include(x => x.WorkSubType)
                .OrderBy(x => x.PlannedEndDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Planlanan başlangıç tarihi belirtilen
        /// UTC aralığında olan iş emirlerini getirir.
        /// </summary>
        public async Task<List<WorkOrder>>
            GetByPlannedStartDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            if (startDateUtc > endDateUtc)
            {
                throw new ArgumentException(
                    "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }

            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedStartDate.HasValue &&
                    x.PlannedStartDate.Value >= startDateUtc &&
                    x.PlannedStartDate.Value <= endDateUtc)
                .OrderBy(x => x.PlannedStartDate)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emri numarasının başka bir
        /// kayıtta kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsWorkOrderNumberAsync(
            string workOrderNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(workOrderNumber))
            {
                return false;
            }

            string normalizedNumber = workOrderNumber.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderNumber == normalizedNumber &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş talebi için daha önce
        /// iş emri oluşturulup oluşturulmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasWorkOrderForRequestAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(
                    x => x.WorkRequestId == workRequestId,
                    cancellationToken);
        }

        /// <summary>
        /// Demirbaşa bağlı açık iş emri
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            return await GetOpenWorkOrdersQuery()
                .AnyAsync(
                    x => x.AssetId == assetId,
                    cancellationToken);
        }

        /// <summary>
        /// Takıma atanmış açık iş emri
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            return await GetOpenWorkOrdersQuery()
                .AnyAsync(
                    x => x.TeamId == teamId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrine atanmış personel kayıtlarını,
        /// takım üyeleri ve kullanıcı detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetPersonnelByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrderPersonnel>()
                .AsNoTracking()
                .Include(x => x.TeamMember)
                    .ThenInclude(x => x.User)
                .Include(x => x.TeamMember)
                    .ThenInclude(x => x.Team)
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.AssignedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde yapılan faaliyet ve
        /// işçilik kayıtlarını kronolojik getirir.
        /// </summary>
        public async Task<List<WorkOrderAction>>
            GetActionsByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrderAction>()
                .AsNoTracking()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.PerformedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinin bütün durum değişikliklerini
        /// eskiden yeniye doğru getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetStatusHistoryByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrderStatusHistory>()
                .AsNoTracking()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.ChangedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrine eklenmiş dosyaları getirir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrderFile>()
                .AsNoTracking()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde kullanılan malzemeleri getirir.
        /// </summary>
        public async Task<List<WorkOrderMaterial>>
            GetMaterialsByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrderMaterial>()
                .AsNoTracking()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrindeki bakım kontrol listesi
        /// sonuçlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetChecklistResultsByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await _context.Set<WorkOrderChecklistResult>()
                .AsNoTracking()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrini Yeni durumunda oluşturur
        /// ve başlangıç durum geçmişini kaydeder.
        ///
        /// İki SaveChangesAsync çağrısı gerektiğinden
        /// işlem bir veritabanı transaction'ı
        /// içerisinde tamamlanır.
        ///
        /// Dışarıdan başlatılmış bir transaction varsa
        /// yeni transaction açılmaz.
        /// </summary>
        public async Task<WorkOrder> CreateWithInitialHistoryAsync(
            WorkOrder workOrder,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(workOrder);

            if (workOrder.Id != 0)
            {
                throw new ArgumentException(
                    "Yeni iş emrinin ID değeri sıfır olmalıdır.",
                    nameof(workOrder));
            }

            if (string.IsNullOrWhiteSpace(workOrder.WorkOrderNumber))
            {
                throw new ArgumentException(
                    "İş emri numarası boş olamaz.",
                    nameof(workOrder));
            }

            if (workOrder.CreatedByUserId <= 0)
            {
                throw new ArgumentException(
                    "İş emrini oluşturan kullanıcı belirtilmelidir.",
                    nameof(workOrder));
            }

            if (workOrder.Status != WorkStatusEnum.New)
            {
                throw new InvalidOperationException(
                    "Yeni iş emri New durumunda oluşturulmalıdır.");
            }

            if (workOrder.PlannedStartDate.HasValue &&
                workOrder.PlannedEndDate.HasValue &&
                workOrder.PlannedEndDate.Value <
                    workOrder.PlannedStartDate.Value)
            {
                throw new InvalidOperationException(
                    "Planlanan bitiş tarihi başlangıçtan önce olamaz.");
            }

            workOrder.WorkOrderNumber =
                workOrder.WorkOrderNumber.Trim();

            bool numberExists = await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderNumber == workOrder.WorkOrderNumber,
                    cancellationToken);

            if (numberExists)
            {
                throw new InvalidOperationException(
                    "Bu iş emri numarası daha önce kullanılmış.");
            }

            if (workOrder.WorkRequestId.HasValue)
            {
                bool requestHasWorkOrder = await _dbSet
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.WorkRequestId == workOrder.WorkRequestId.Value,
                        cancellationToken);

                if (requestHasWorkOrder)
                {
                    throw new InvalidOperationException(
                        "Bu iş talebinden daha önce iş emri oluşturulmuş.");
                }
            }

            DateTime now = DateTime.UtcNow;

            workOrder.CreatedAt = now;
            workOrder.UpdatedAt = null;

            IDbContextTransaction? localTransaction = null;

            try
            {
                // Dış transaction bulunmuyorsa yeni bir tane aç.
                if (_context.Database.CurrentTransaction == null)
                {
                    localTransaction =
                        await _context.Database.BeginTransactionAsync(
                            cancellationToken);
                }

                // Önce iş emrini oluştur.
                await _dbSet.AddAsync(
                    workOrder,
                    cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                // Oluşan WorkOrder.Id ile
                // başlangıç durum geçmişini kaydet.
                var history = new WorkOrderStatusHistory
                {
                    WorkOrderId = workOrder.Id,

                    PreviousStatus = null,

                    NewStatus = WorkStatusEnum.New,

                    ChangedByUserId = workOrder.CreatedByUserId,

                    ChangedAt = now,

                    Description = "İş emri oluşturuldu.",

                    CreatedAt = now
                };

                await _context.Set<WorkOrderStatusHistory>()
                    .AddAsync(history, cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                // Yalnızca bu metotta açılan transaction
                // burada commit edilir.
                if (localTransaction != null)
                {
                    await localTransaction.CommitAsync(
                        cancellationToken);
                }

                return workOrder;
            }
            catch
            {
                if (localTransaction != null)
                {
                    await localTransaction.RollbackAsync(
                        CancellationToken.None);
                }

                throw;
            }
            finally
            {
                if (localTransaction != null)
                {
                    await localTransaction.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// İş emrinin durumunu değiştirir,
        /// ilgili tarih alanlarını günceller ve
        /// durum geçmişine yeni kayıt ekler.
        ///
        /// Değişiklikler tek SaveChangesAsync
        /// işlemiyle kaydedilir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            WorkStatusEnum newStatus,
            int changedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            ValidateStatus(newStatus);

            if (changedByUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(changedByUserId),
                    "Geçerli bir kullanıcı belirtilmelidir.");
            }

            var workOrder = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (workOrder == null)
            {
                return false;
            }

            if (workOrder.Status == newStatus)
            {
                return true;
            }

            WorkStatusEnum previousStatus = workOrder.Status;

            if (!IsValidStatusTransition(
                previousStatus,
                newStatus))
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;

            // İş emri başlatıldığında veya
            // düzeltme için yeniden açıldığında.
            if (newStatus == WorkStatusEnum.InProgress)
            {
                workOrder.StartedAt ??= now;

                // Onaydan geri döndüyse artık
                // aktif bir onay bekleme durumu yok.
                workOrder.SubmittedForApprovalAt = null;
            }

            // İş tamamlanarak yönetici onayına sunulduğunda.
            if (newStatus == WorkStatusEnum.PendingApproval)
            {
                workOrder.SubmittedForApprovalAt = now;
            }

            // Yönetici iş emrini onayladığında.
            if (newStatus == WorkStatusEnum.Completed)
            {
                workOrder.CompletedAt = now;

                // Bu kullanıcıya onay yetkisi verilmiş
                // olduğu servis katmanında doğrulanmalıdır.
                workOrder.ApprovedByUserId = changedByUserId;
            }

            // İş emri iptal edildiğinde.
            if (newStatus == WorkStatusEnum.Cancelled)
            {
                workOrder.CancelledAt = now;
            }

            workOrder.Status = newStatus;
            workOrder.UpdatedAt = now;

            var history = new WorkOrderStatusHistory
            {
                WorkOrderId = workOrder.Id,

                PreviousStatus = previousStatus,

                NewStatus = newStatus,

                ChangedByUserId = changedByUserId,

                ChangedAt = now,

                Description = string.IsNullOrWhiteSpace(description)
                    ? GetDefaultStatusDescription(newStatus)
                    : description.Trim(),

                CreatedAt = now
            };

            await _context.Set<WorkOrderStatusHistory>()
                .AddAsync(history, cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// İş emri durum geçişinin mevcut
        /// iş akışına uygunluğunu kontrol eder.
        /// </summary>
        private static bool IsValidStatusTransition(
            WorkStatusEnum currentStatus,
            WorkStatusEnum newStatus)
        {
            return currentStatus switch
            {
                WorkStatusEnum.New =>
                    newStatus == WorkStatusEnum.InProgress ||
                    newStatus == WorkStatusEnum.Cancelled,

                WorkStatusEnum.InProgress =>
                    newStatus == WorkStatusEnum.PendingApproval ||
                    newStatus == WorkStatusEnum.Cancelled,

                WorkStatusEnum.PendingApproval =>
                    newStatus == WorkStatusEnum.InProgress ||
                    newStatus == WorkStatusEnum.Completed ||
                    newStatus == WorkStatusEnum.Cancelled,

                WorkStatusEnum.Completed => false,

                WorkStatusEnum.Cancelled => false,

                _ => false
            };
        }

        /// <summary>
        /// Durum değişikliğinin varsayılan
        /// açıklamasını oluşturur.
        /// </summary>
        private static string GetDefaultStatusDescription(
            WorkStatusEnum status)
        {
            return status switch
            {
                WorkStatusEnum.New =>
                    "İş emri oluşturuldu.",

                WorkStatusEnum.InProgress =>
                    "İş emri devam ediyor durumuna alındı.",

                WorkStatusEnum.PendingApproval =>
                    "İş emri onaya gönderildi.",

                WorkStatusEnum.Completed =>
                    "İş emri onaylanarak tamamlandı.",

                WorkStatusEnum.Cancelled =>
                    "İş emri iptal edildi.",

                _ => "İş emri durumu değiştirildi."
            };
        }

        /// <summary>
        /// Verilen iş emri durumunun enum
        /// içerisinde tanımlı olduğunu doğrular.
        /// </summary>
        private static void ValidateStatus(
            WorkStatusEnum status)
        {
            if (!Enum.IsDefined(
                typeof(WorkStatusEnum), status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    "Geçersiz iş emri durumu.");
            }
        }

        /// <summary>
        /// Halen açık olan iş emirlerine
        /// ait ortak sorguyu oluşturur.
        /// </summary>
        private IQueryable<WorkOrder> GetOpenWorkOrdersQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.Status == WorkStatusEnum.New ||
                    x.Status == WorkStatusEnum.InProgress ||
                    x.Status == WorkStatusEnum.PendingApproval);
        }

        /// <summary>
        /// İş emrini ilgili demirbaş, talep, bakım,
        /// organizasyon, takım, iş türü ve
        /// kullanıcı detaylarıyla getiren ortak sorgudur.
        /// </summary>
        private IQueryable<WorkOrder> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // Demirbaş, model ve marka
                .Include(x => x.Asset)
                    .ThenInclude(x => x.AssetModel)
                        .ThenInclude(x => x.Brand)

                // İş emrinin oluşturulduğu talep
                .Include(x => x.WorkRequest)

                // Bağlı planlı bakım
                .Include(x => x.PlannedMaintenance)

                // Sorumlu organizasyon
                .Include(x => x.ResponsibleUnit)

                // İş emrini gerçekleştirecek takım
                .Include(x => x.Team)

                // İş alt türü ve iş türü
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType)

                // İş emrini oluşturan kullanıcı
                .Include(x => x.CreatedByUser)

                // İş emrini onaylayan kullanıcı
                .Include(x => x.ApprovedByUser);
        }
    }
}
