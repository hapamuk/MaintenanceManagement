using MaintenanceManagement.Application.Interfaces;
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
    public class WorkOrderFileRepository :
         GenericRepository<WorkOrderFile>,
         IWorkOrderFileRepository
    {
        public WorkOrderFileRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün iş emri dosyalarını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Dosya kaydını ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkOrderFile?>
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
        /// Dosyanın belirtilen iş emrine
        /// ait olduğunu doğrulayarak getirir.
        /// </summary>
        public async Task<WorkOrderFile?>
            GetByIdAndWorkOrderIdAsync(
                int id,
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id &&
                         x.WorkOrderId == workOrderId,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş emrine eklenen
        /// bütün dosyaları getirir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// yüklenen dosyaları getirir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByUploadedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UploadedByUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirtilen iş emrine
        /// yüklediği dosyaları getirir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByWorkOrderAndUserAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.UploadedByUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerinin
        /// bütün dosya kayıtlarını getirir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrder.ResponsibleUnitId == unitId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Takıma atanmış iş emirlerinin
        /// dosya kayıtlarını getirir.
        ///
        /// İş emrinin mevcut TeamId değeri
        /// üzerinden filtreleme yapılır.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrder.TeamId == teamId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen durumdaki iş emirlerine
        /// eklenmiş dosyaları getirir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByWorkOrderStatusAsync(
                WorkStatusEnum status,
                CancellationToken cancellationToken = default)
        {
            ValidateWorkStatus(status);

            return await GetDetailsQuery()
                .Where(x => x.WorkOrder.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Dosyanın saklama yoluna göre
        /// mevcut veritabanı kaydını getirir.
        /// </summary>
        public async Task<WorkOrderFile?>
            GetByFilePathAsync(
                string filePath,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            string normalizedPath = filePath.Trim();

            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.FilePath == normalizedPath,
                    cancellationToken);
        }

        /// <summary>
        /// Belirtilen MIME türündeki
        /// dosya kayıtlarını getirir.
        ///
        /// Örnek:
        /// image/jpeg
        /// image/png
        /// application/pdf
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByContentTypeAsync(
                string contentType,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return new List<WorkOrderFile>();
            }

            string normalizedContentType = contentType.Trim();

            return await GetDetailsQuery()
                .Where(x =>
                    x.ContentType == normalizedContentType)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş emrindeki ilgili
        /// MIME türüne ait dosyaları getirir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByWorkOrderAndContentTypeAsync(
                int workOrderId,
                string contentType,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return new List<WorkOrderFile>();
            }

            string normalizedContentType = contentType.Trim();

            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.ContentType == normalizedContentType)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen UTC tarih aralığında
        /// oluşturulmuş dosya kayıtlarını getirir.
        ///
        /// Başlangıç ve bitiş değerleri dahildir.
        /// </summary>
        public async Task<List<WorkOrderFile>>
            GetFilesByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(startDateUtc, endDateUtc);

            return await GetDetailsQuery()
                .Where(x =>
                    x.CreatedAt >= startDateUtc &&
                    x.CreatedAt <= endDateUtc)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrine en son eklenen
        /// dosya kaydını getirir.
        /// </summary>
        public async Task<WorkOrderFile?>
            GetLatestFileByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Dosya saklama yolunun başka bir
        /// kayıtta kullanılıp kullanılmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> ExistsFilePathAsync(
            string filePath,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            string normalizedPath = filePath.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.FilePath == normalizedPath &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Aynı iş emrinde aynı dosya adına
        /// sahip kayıt olup olmadığını kontrol eder.
        ///
        /// Bu kontrol aynı dosya adının yeniden
        /// yüklenmesini otomatik olarak engellemez.
        /// </summary>
        public async Task<bool> ExistsFileNameInWorkOrderAsync(
            int workOrderId,
            string fileName,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            string normalizedName = fileName.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.FileName == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// İş emrine eklenmiş herhangi bir
        /// dosya olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasFilesAsync(
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
        /// İş emrine eklenen toplam
        /// dosya sayısını getirir.
        /// </summary>
        public async Task<int> GetFileCountByWorkOrderIdAsync(
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
        /// İş emrine eklenen dosyaların toplam
        /// boyutunu byte cinsinden getirir.
        ///
        /// Dosya bulunmuyorsa sıfır döndürür.
        /// </summary>
        public async Task<long>
            GetTotalFileSizeByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            long? totalSize = await _dbSet
                .AsNoTracking()
                .Where(x => x.WorkOrderId == workOrderId)
                .SumAsync(
                    x => (long?)x.FileSizeBytes,
                    cancellationToken);

            return totalSize ?? 0L;
        }

        /// <summary>
        /// Verilen iş emri durumunun
        /// WorkStatusEnum içerisinde tanımlı
        /// olup olmadığını kontrol eder.
        /// </summary>
        private static void ValidateWorkStatus(
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
        /// Tarih aralığının UTC formatında
        /// ve kronolojik olarak geçerli
        /// olduğunu doğrular.
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
        /// Dosya kaydını iş emri, sorumlu
        /// organizasyon, takım, iş alt türü,
        /// demirbaş ve yükleyen kullanıcı
        /// detaylarıyla getiren ortak sorgudur.
        /// </summary>
        private IQueryable<WorkOrderFile> GetDetailsQuery()
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

                // İş emrine bağlı demirbaş
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.Asset)

                // Dosyayı yükleyen kullanıcı
                .Include(x => x.UploadedByUser)
                    .ThenInclude(x => x.PrimaryUnit);
        }
    }
}
