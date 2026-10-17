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
    public class WorkRequestRepository :
         GenericRepository<WorkRequest>,
         IWorkRequestRepository
    {
        public WorkRequestRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün iş taleplerini ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş talebini ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkRequest?>
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
        /// Talep numarasına göre iş talebini getirir.
        /// </summary>
        public async Task<WorkRequest?>
            GetByRequestNumberAsync(
                string requestNumber,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(requestNumber))
            {
                return null;
            }

            string normalizedNumber = requestNumber.Trim();

            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.RequestNumber == normalizedNumber,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcının oluşturduğu bütün
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByRequestedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.RequestedByUserId == userId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Talebi oluşturan organizasyona ait
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByRequesterUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.RequesterUnitId == unitId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen sorumlu organizasyona
        /// yönlendirilmiş talepleri getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.ResponsibleUnitId == unitId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyonun Yeni (Draft)
        /// durumundaki bütün taleplerini getirir.
        ///
        /// Henüz gönderilmemiş kayıtlar da dahildir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetNewRequestsByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.ResponsibleUnitId == unitId &&
                    x.Status == WorkRequestStatusEnum.Draft)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyona gönderilmiş,
        /// Yeni (Draft) durumundaki talepleri getirir.
        ///
        /// SubmittedAt alanı dolu olmalıdır.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetSubmittedNewRequestsByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.ResponsibleUnitId == unitId &&
                    x.Status == WorkRequestStatusEnum.Draft &&
                    x.SubmittedAt.HasValue)
                .OrderBy(x => x.SubmittedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen yöneticiye bağlı
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByManagerUserIdAsync(
                int managerUserId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.ManagerUserId == managerUserId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş alt türüne bağlı
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkSubTypeId == workSubTypeId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen demirbaşa bağlı
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.AssetId == assetId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen konuma ait
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetBySiteLocationIdAsync(
                int siteLocationId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.SiteLocationId == siteLocationId)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen durumdaki iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByStatusAsync(
                WorkRequestStatusEnum status,
                CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(
                typeof(WorkRequestStatusEnum), status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    "Geçersiz iş talebi durumu.");
            }

            return await GetDetailsQuery()
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen öncelik seviyesindeki
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByPriorityAsync(
                WorkPriorityEnum priority,
                CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(
                typeof(WorkPriorityEnum), priority))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(priority),
                    "Geçersiz iş talebi önceliği.");
            }

            return await GetDetailsQuery()
                .Where(x => x.Priority == priority)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen tarih aralığında oluşturulmuş
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<List<WorkRequest>>
            GetByDateRangeAsync(
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
                    x.RequestDate >= startDateUtc &&
                    x.RequestDate <= endDateUtc)
                .OrderByDescending(x => x.RequestDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Talep numarasının daha önce başka bir
        /// kayıtta kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsRequestNumberAsync(
            string requestNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(requestNumber))
            {
                return false;
            }

            string normalizedNumber = requestNumber.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.RequestNumber == normalizedNumber &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// İş talebinden oluşturulmuş
        /// bir iş emri bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasWorkOrderAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrders
                .AsNoTracking()
                .AnyAsync(
                    x => x.WorkRequestId == workRequestId,
                    cancellationToken);
        }

        /// <summary>
        /// İş talebine bağlı iş emrini getirir.
        /// </summary>
        public async Task<WorkOrder?>
            GetWorkOrderByRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrders
                .AsNoTracking()
                .Include(x => x.Team)
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType)
                .FirstOrDefaultAsync(
                    x => x.WorkRequestId == workRequestId,
                    cancellationToken);
        }

        /// <summary>
        /// İş talebine eklenen dosyaları getirir.
        /// </summary>
        public async Task<List<WorkRequestFile>>
            GetFilesByRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequestFiles
                .AsNoTracking()
                .Where(x => x.WorkRequestId == workRequestId)
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş talebinin işlem geçmişini getirir.
        /// </summary>
        public async Task<List<WorkRequestAction>>
            GetActionsByRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequestActions
                .AsNoTracking()
                .Where(x => x.WorkRequestId == workRequestId)
                .OrderBy(x => x.PerformedAt)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Yeni durumundaki iş talebini gönderir.
        ///
        /// Submitted isimli ayrı bir status olmadığı için
        /// talebin durumu Draft olarak kalır.
        ///
        /// SubmittedAt doldurulur ve gönderim hareketi eklenir.
        /// </summary>
        public async Task<bool> SubmitAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            if (performedByUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(performedByUserId),
                    "Geçerli bir kullanıcı belirtilmelidir.");
            }

            var request = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (request == null)
            {
                return false;
            }

            // Yalnızca Yeni durumundaki talepler gönderilebilir.
            if (request.Status != WorkRequestStatusEnum.Draft)
            {
                return false;
            }

            // Daha önce gönderilmiş talep yeniden gönderilemez.
            if (request.SubmittedAt.HasValue)
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;

            request.SubmittedAt = now;
            request.UpdatedAt = now;

            var action = new WorkRequestAction
            {
                WorkRequestId = request.Id,

                PerformedByUserId = performedByUserId,

                ActionType = WorkRequestActionTypeEnum.Submitted,

                Description = "İş talebi gönderildi.",

                PerformedAt = now,

                CreatedAt = now
            };

            await _context.WorkRequestActions
                .AddAsync(action, cancellationToken);

            // SubmittedAt ve hareket kaydı birlikte kaydedilir.
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        /// <summary>
        /// İş talebinin durumunu değiştirir.
        ///
        /// Desteklenen geçişler:
        /// Draft -> Rejected
        /// Draft -> ConvertedToWorkOrder
        /// Draft -> Cancelled
        ///
        /// Durum değişikliği ve hareket kaydı
        /// birlikte veritabanına kaydedilir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            WorkRequestStatusEnum status,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            if (!Enum.IsDefined(
                typeof(WorkRequestStatusEnum), status))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(status),
                    "Geçersiz iş talebi durumu.");
            }

            if (performedByUserId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(performedByUserId),
                    "Geçerli bir kullanıcı belirtilmelidir.");
            }

            var request = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (request == null)
            {
                return false;
            }

            // Durum zaten aynıysa ikinci işlem oluşturulmaz.
            if (request.Status == status)
            {
                return true;
            }

            // Sonuçlandırılmış talepler bu metotla
            // yeniden Yeni durumuna döndürülemez.
            if (request.Status != WorkRequestStatusEnum.Draft)
            {
                return false;
            }

            // Draft, bu metotla hedef durum olarak kullanılamaz.
            if (status == WorkRequestStatusEnum.Draft)
            {
                return false;
            }

            // Gönderilmemiş talep reddedilemez
            // veya iş emrine dönüştürülemez.
            if (!request.SubmittedAt.HasValue &&
                (status == WorkRequestStatusEnum.Rejected ||
                 status == WorkRequestStatusEnum.ConvertedToWorkOrder))
            {
                return false;
            }

            // İş emrine dönüşüm durumunda ilişkili
            // iş emrinin gerçekten bulunması gerekir.
            if (status == WorkRequestStatusEnum.ConvertedToWorkOrder)
            {
                bool hasWorkOrder = await _context.WorkOrders
                    .AsNoTracking()
                    .AnyAsync(
                        x => x.WorkRequestId == request.Id,
                        cancellationToken);

                if (!hasWorkOrder)
                {
                    return false;
                }
            }

            WorkRequestActionTypeEnum actionType = status switch
            {
                WorkRequestStatusEnum.Rejected =>
                    WorkRequestActionTypeEnum.Rejected,

                WorkRequestStatusEnum.ConvertedToWorkOrder =>
                    WorkRequestActionTypeEnum.Converted,

                WorkRequestStatusEnum.Cancelled =>
                    WorkRequestActionTypeEnum.Cancelled,

                _ => throw new InvalidOperationException(
                    "Desteklenmeyen iş talebi durum geçişi.")
            };

            DateTime now = DateTime.UtcNow;

            request.Status = status;
            request.UpdatedAt = now;

            string actionDescription = string.IsNullOrWhiteSpace(description)
                ? status switch
                {
                    WorkRequestStatusEnum.Rejected =>
                        "İş talebi reddedildi.",

                    WorkRequestStatusEnum.ConvertedToWorkOrder =>
                        "İş talebi iş emrine dönüştürüldü.",

                    WorkRequestStatusEnum.Cancelled =>
                        "İş talebi iptal edildi.",

                    _ => "İş talebi durumu güncellendi."
                }
                : description.Trim();

            var action = new WorkRequestAction
            {
                WorkRequestId = request.Id,

                PerformedByUserId = performedByUserId,

                ActionType = actionType,

                Description = actionDescription,

                PerformedAt = now,

                CreatedAt = now
            };

            await _context.WorkRequestActions
                .AddAsync(action, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        /// <summary>
        /// İş talebinin ilişkili iş türünü, demirbaşı,
        /// markayı, modeli ve konumunu getiren ortak sorgudur.
        ///
        /// Kullanıcı ve organizasyon ID değerleri
        /// WorkRequest üzerinde korunur.
        /// </summary>
        private IQueryable<WorkRequest> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş alt türü ve bağlı iş türü
                .Include(x => x.WorkSubType)
                    .ThenInclude(x => x.WorkType)

                // İlgili demirbaş, model ve marka
                .Include(x => x.Asset)
                    .ThenInclude(x => x.AssetModel)
                        .ThenInclude(x => x.Brand)

                // İş talebinin fiziksel konumu
                .Include(x => x.SiteLocation);
        }
    }
}
