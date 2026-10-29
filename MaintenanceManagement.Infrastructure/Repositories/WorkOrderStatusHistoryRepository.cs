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
    public class WorkOrderStatusHistoryRepository :
       GenericRepository<WorkOrderStatusHistory>,
       IWorkOrderStatusHistoryRepository
    {
        public WorkOrderStatusHistoryRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün iş emri durum geçmişi kayıtlarını
        /// ilişkili detaylarıyla birlikte getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen durum geçmişi kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkOrderStatusHistory?>
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
        /// İş emrinin bütün durum değişikliklerini
        /// eskiden yeniye doğru getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.ChangedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının gerçekleştirdiği
        /// bütün durum değişikliklerini getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByChangedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.ChangedByUserId == userId)
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerinin
        /// durum değişikliği geçmişini getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrder.ResponsibleUnitId == unitId)
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen takıma ait iş emirlerinin
        /// durum değişikliği geçmişini getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrder.TeamId == teamId)
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Önceki durum değerine göre geçmiş
        /// kayıtlarını getirir.
        ///
        /// previousStatus null olduğunda ilk
        /// oluşturulma kayıtlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByPreviousStatusAsync(
                WorkStatusEnum? previousStatus,
                CancellationToken cancellationToken = default)
        {
            if (previousStatus.HasValue)
            {
                ValidateStatus(previousStatus.Value);
            }

            return await GetDetailsQuery()
                .Where(x => x.PreviousStatus == previousStatus)
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emirlerinin belirtilen yeni
        /// duruma geçtiği kayıtları getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByNewStatusAsync(
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default)
        {
            ValidateStatus(newStatus);

            return await GetDetailsQuery()
                .Where(x => x.NewStatus == newStatus)
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen önceki durumdan yeni duruma
        /// gerçekleşen geçişleri getirir.
        ///
        /// Örnek:
        /// InProgress -> PendingApproval
        /// PendingApproval -> Completed
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByStatusTransitionAsync(
                WorkStatusEnum? previousStatus,
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default)
        {
            if (previousStatus.HasValue)
            {
                ValidateStatus(previousStatus.Value);
            }

            ValidateStatus(newStatus);

            return await GetDetailsQuery()
                .Where(x =>
                    x.PreviousStatus == previousStatus &&
                    x.NewStatus == newStatus)
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş emrinin ilgili yeni
        /// duruma geçtiği kayıtları getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByWorkOrderAndNewStatusAsync(
                int workOrderId,
                WorkStatusEnum newStatus,
                CancellationToken cancellationToken = default)
        {
            ValidateStatus(newStatus);

            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.NewStatus == newStatus)
                .OrderBy(x => x.ChangedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen UTC tarih aralığında gerçekleşmiş
        /// durum değişikliklerini getirir.
        ///
        /// Başlangıç ve bitiş değerleri dahildir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(startDateUtc, endDateUtc);

            return await GetDetailsQuery()
                .Where(x =>
                    x.ChangedAt >= startDateUtc &&
                    x.ChangedAt <= endDateUtc)
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinin belirtilen UTC tarih
        /// aralığındaki değişikliklerini getirir.
        /// </summary>
        public async Task<List<WorkOrderStatusHistory>>
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
                    x.ChangedAt >= startDateUtc &&
                    x.ChangedAt <= endDateUtc)
                .OrderBy(x => x.ChangedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinin ilk durum geçmişi
        /// kaydını getirir.
        /// </summary>
        public async Task<WorkOrderStatusHistory?>
            GetFirstHistoryByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.ChangedAt)
                .ThenBy(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinin en son durum
        /// değişikliği kaydını getirir.
        /// </summary>
        public async Task<WorkOrderStatusHistory?>
            GetLatestHistoryByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderByDescending(x => x.ChangedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrine ait herhangi bir durum
        /// geçmişi kaydı olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasHistoryAsync(
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
        /// İş emrinin geçmişte belirtilen
        /// duruma ulaşıp ulaşmadığını kontrol eder.
        ///
        /// Mevcut durumu değil, geçmişte
        /// kaydedilmiş durumları sorgular.
        /// </summary>
        public async Task<bool> HasReachedStatusAsync(
            int workOrderId,
            WorkStatusEnum status,
            CancellationToken cancellationToken = default)
        {
            ValidateStatus(status);

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.NewStatus == status,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinin toplam durum geçmişi
        /// kayıt sayısını getirir.
        /// </summary>
        public async Task<int> GetHistoryCountByWorkOrderIdAsync(
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
        /// Durum değerinin WorkStatusEnum
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
        /// Tarihlerin UTC formatında olduğunu
        /// ve başlangıcın bitişten sonra
        /// olmadığını doğrular.
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
        /// İş emri durum geçmişini iş emri,
        /// organizasyon, iş türü ve değişikliği
        /// yapan kullanıcı detaylarıyla getirir.
        /// </summary>
        private IQueryable<WorkOrderStatusHistory> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş emri ve sorumlu organizasyon
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.ResponsibleUnit)

                // İş emri, iş alt türü ve iş türü
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.WorkSubType)
                        .ThenInclude(x => x.WorkType)

                // Durumu değiştiren kullanıcı
                .Include(x => x.ChangedByUser)
                    .ThenInclude(x => x.PrimaryUnit);
        }
    }
}
