using MaintenanceManagement.Application.DTOS.WorkRequestFileDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkRequestFileService
    {
        #region Get Methods

        /// <summary>
        /// Bütün iş talebi dosya kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestFileDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre dosya detayını getirir.
        /// </summary>
        Task<GetByIdWorkRequestFileDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dosyanın belirtilen iş talebine
        /// ait olduğunu doğrulayarak detayını getirir.
        /// </summary>
        Task<GetByIdWorkRequestFileDto?> GetByIdAndWorkRequestIdAsync(
            int id,
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine ait bütün dosyaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetByWorkRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcının yüklediği
        /// bütün dosyaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetByUploadedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli bir iş talebinde kullanıcının
        /// yüklediği dosyaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestFileDto>>
            GetByRequestAndUserAsync(
                int workRequestId,
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İçerik türüne göre dosyaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestFileDto>> GetByContentTypeAsync(
            string contentType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığında
        /// yüklenmiş dosyaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWorkRequestFileDto>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Depolama yoluna göre dosya detayını getirir.
        /// </summary>
        Task<GetByIdWorkRequestFileDto?> GetByFilePathAsync(
            string filePath,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Önceden güvenli biçimde yüklenmiş dosyanın
        /// metadata kaydını oluşturur.
        ///
        /// currentUserId doğrulanmış oturumdan,
        /// FilePath ise güvenilir depolama katmanından
        /// alınmalıdır.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkRequestFileDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dosyanın görünen adını günceller.
        /// Fiziksel dosyayı yeniden adlandırmaz.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkRequestFileDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Depolama yolu başka dosya kaydında
        /// kullanılıyor mu kontrol eder.
        /// </summary>
        Task<bool> ExistsFilePathAsync(
            string filePath,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı iş talebi içerisinde aynı
        /// dosya adı bulunuyor mu kontrol eder.
        /// </summary>
        Task<bool> ExistsFileNameInRequestAsync(
            int workRequestId,
            string fileName,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinde herhangi bir dosya var mı?
        /// </summary>
        Task<bool> HasFilesAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine bağlı toplam dosya sayısı.
        /// </summary>
        Task<int> GetFileCountByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine bağlı dosyaların
        /// toplam boyutu (bayt).
        /// </summary>
        Task<long> GetTotalFileSizeByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
