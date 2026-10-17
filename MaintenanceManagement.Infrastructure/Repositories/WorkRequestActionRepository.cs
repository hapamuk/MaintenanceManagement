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
    public class WorkRequestActionRepository :
     GenericRepository<WorkRequestAction>,
     IWorkRequestActionRepository
    {
        public WorkRequestActionRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün hareketleri iş talebi ve
        /// kullanıcı detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Hareket kaydını ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkRequestAction?>
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
        /// Hareketin belirtilen iş talebine
        /// ait olup olmadığını doğrulayarak getirir.
        /// </summary>
        public async Task<WorkRequestAction?>
            GetByIdAndWorkRequestIdAsync(
                int id,
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.WorkRequestId == workRequestId,
                    cancellationToken);
        }

        /// <summary>
        /// İş talebinin bütün hareket geçmişini
        /// eskiden yeniye doğru getirir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetActionsByWorkRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkRequestId == workRequestId)
                .OrderBy(x => x.PerformedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş talebi numarasına göre
        /// hareket geçmişini getirir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetActionsByRequestNumberAsync(
                string requestNumber,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(requestNumber))
            {
                return new List<WorkRequestAction>();
            }

            string normalizedNumber = requestNumber.Trim();

            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkRequest.RequestNumber == normalizedNumber)
                .OrderBy(x => x.PerformedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcının gerçekleştirdiği
        /// bütün iş talebi hareketlerini getirir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetActionsByUserIdAsync(
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
        /// Sorumlu organizasyona yönlendirilmiş
        /// iş taleplerinin bütün hareketlerini getirir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetActionsByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkRequest.ResponsibleUnitId == unitId)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen hareket türündeki
        /// bütün kayıtları getirir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetActionsByTypeAsync(
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default)
        {
            ValidateActionType(actionType);

            return await GetDetailsQuery()
                .Where(x => x.ActionType == actionType)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş talebindeki belirtilen türdeki
        /// bütün hareketleri getirir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetActionsByRequestAndTypeAsync(
                int workRequestId,
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default)
        {
            ValidateActionType(actionType);

            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkRequestId == workRequestId &&
                    x.ActionType == actionType)
                .OrderBy(x => x.PerformedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen UTC tarih aralığındaki
        /// hareket kayıtlarını getirir.
        ///
        /// Başlangıç ve bitiş tarihleri dahildir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetActionsByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
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

            return await GetDetailsQuery()
                .Where(x =>
                    x.PerformedAt >= startDateUtc &&
                    x.PerformedAt <= endDateUtc)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş talebinin en son hareketini getirir.
        /// </summary>
        public async Task<WorkRequestAction?>
            GetLatestActionByRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkRequestId == workRequestId)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş talebinin ilgili hareket
        /// türündeki en son kaydını getirir.
        /// </summary>
        public async Task<WorkRequestAction?>
            GetLatestActionByTypeAsync(
                int workRequestId,
                WorkRequestActionTypeEnum actionType,
                CancellationToken cancellationToken = default)
        {
            ValidateActionType(actionType);

            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkRequestId == workRequestId &&
                    x.ActionType == actionType)
                .OrderByDescending(x => x.PerformedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// İş talebine ait herhangi bir hareket
        /// kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActionsAsync(
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
        /// İş talebinde belirtilen türde bir
        /// hareket bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActionTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default)
        {
            ValidateActionType(actionType);

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkRequestId == workRequestId &&
                    x.ActionType == actionType,
                    cancellationToken);
        }

        /// <summary>
        /// İş talebinin toplam hareket
        /// kaydı sayısını getirir.
        /// </summary>
        public async Task<int> GetActionCountByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(
                    x => x.WorkRequestId == workRequestId,
                    cancellationToken);
        }

        /// <summary>
        /// İş talebinin belirtilen hareket
        /// türündeki kayıt sayısını getirir.
        /// </summary>
        public async Task<int> GetActionCountByTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default)
        {
            ValidateActionType(actionType);

            return await _dbSet
                .AsNoTracking()
                .CountAsync(x =>
                    x.WorkRequestId == workRequestId &&
                    x.ActionType == actionType,
                    cancellationToken);
        }

        /// <summary>
        /// İşlem türünün tanımlı bir enum
        /// değeri olup olmadığını kontrol eder.
        /// </summary>
        private static void ValidateActionType(
            WorkRequestActionTypeEnum actionType)
        {
            if (!Enum.IsDefined(
                typeof(WorkRequestActionTypeEnum),
                actionType))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(actionType),
                    "Geçersiz iş talebi hareket türü.");
            }
        }

        /// <summary>
        /// Hareket kayıtlarını iş talebi,
        /// iş türü, konum, işlemi yapan kullanıcı
        /// ve organizasyon bilgileriyle getirir.
        /// </summary>
        private IQueryable<WorkRequestAction> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş talebi, iş alt türü ve iş türü
                .Include(x => x.WorkRequest)
                    .ThenInclude(x => x.WorkSubType)
                        .ThenInclude(x => x.WorkType)

                // İş talebinin konumu
                .Include(x => x.WorkRequest)
                    .ThenInclude(x => x.SiteLocation)

                // İşlemi gerçekleştiren kullanıcı
                .Include(x => x.PerformedByUser)
                    .ThenInclude(x => x.PrimaryUnit);
        }
    }
}

