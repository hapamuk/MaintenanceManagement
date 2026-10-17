using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class WorkRequestFileRepository :
       GenericRepository<WorkRequestFile>,
       IWorkRequestFileRepository
    {
        public WorkRequestFileRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün dosya kayıtlarını iş talebi
        /// ve yükleyen kullanıcı detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkRequestFile>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen dosya kaydını ilişkili
        /// detaylarıyla birlikte getirir.
        /// </summary>
        public async Task<WorkRequestFile?>
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
        /// Dosya ID ve iş talebi ID değerlerinin
        /// aynı kayıtta bulunmasını doğrulayarak
        /// ilgili dosyayı getirir.
        /// </summary>
        public async Task<WorkRequestFile?>
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
        /// Belirtilen iş talebine eklenen
        /// bütün dosyaları getirir.
        ///
        /// İş talebinin mevcut durumundan bağımsızdır.
        /// </summary>
        public async Task<List<WorkRequestFile>>
            GetFilesByWorkRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkRequestId == workRequestId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcının yüklediği
        /// bütün iş talebi dosyalarını getirir.
        /// </summary>
        public async Task<List<WorkRequestFile>>
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
        /// İş talebine belirtilen kullanıcı
        /// tarafından eklenen dosyaları getirir.
        /// </summary>
        public async Task<List<WorkRequestFile>>
            GetFilesByRequestAndUserAsync(
                int workRequestId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkRequestId == workRequestId &&
                    x.UploadedByUserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Dosya saklama yoluna göre
        /// mevcut veritabanı kaydını getirir.
        /// </summary>
        public async Task<WorkRequestFile?>
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
        /// MIME türüne göre dosyaları getirir.
        ///
        /// Örnek:
        /// image/jpeg
        /// image/png
        /// application/pdf
        /// </summary>
        public async Task<List<WorkRequestFile>>
            GetFilesByContentTypeAsync(
                string contentType,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return new List<WorkRequestFile>();
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
        /// Belirtilen UTC tarih aralığında oluşturulmuş
        /// dosya kayıtlarını getirir.
        ///
        /// Başlangıç ve bitiş tarihleri dahildir.
        /// </summary>
        public async Task<List<WorkRequestFile>>
            GetFilesByDateRangeAsync(
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
                    x.CreatedAt >= startDateUtc &&
                    x.CreatedAt <= endDateUtc)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Dosya saklama yolunun daha önce
        /// kullanılıp kullanılmadığını kontrol eder.
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
        /// Aynı iş talebinde aynı dosya adına
        /// sahip başka kayıt olup olmadığını kontrol eder.
        ///
        /// Bu metot aynı dosya adının ikinci kez
        /// yüklenmesini otomatik olarak engellemez.
        /// </summary>
        public async Task<bool> ExistsFileNameInRequestAsync(
            int workRequestId,
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
                    x.WorkRequestId == workRequestId &&
                    x.FileName == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// İş talebine eklenmiş herhangi
        /// bir dosya olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasFilesAsync(
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
        /// Belirtilen iş talebine eklenen
        /// dosyaların sayısını getirir.
        /// </summary>
        public async Task<int> GetFileCountByRequestIdAsync(
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
        /// İş talebine eklenen dosyaların toplam
        /// boyutunu byte cinsinden getirir.
        ///
        /// Dosya bulunmuyorsa sıfır döndürür.
        /// </summary>
        public async Task<long> GetTotalFileSizeByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            long? totalSize = await _dbSet
                .AsNoTracking()
                .Where(x => x.WorkRequestId == workRequestId)
                .SumAsync(
                    x => (long?)x.FileSizeBytes,
                    cancellationToken);

            return totalSize ?? 0L;
        }

        /// <summary>
        /// Dosya kaydını iş talebi, iş alt türü,
        /// iş türü, dosyayı yükleyen kullanıcı
        /// ve kullanıcının organizasyon bilgileriyle
        /// birlikte getiren ortak sorgudur.
        /// </summary>
        private IQueryable<WorkRequestFile> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş talebi ve iş sınıflandırması
                .Include(x => x.WorkRequest)
                    .ThenInclude(x => x.WorkSubType)
                        .ThenInclude(x => x.WorkType)

                // Dosyayı yükleyen kullanıcı
                .Include(x => x.UploadedByUser)
                    .ThenInclude(x => x.PrimaryUnit);
        }
    }
}
