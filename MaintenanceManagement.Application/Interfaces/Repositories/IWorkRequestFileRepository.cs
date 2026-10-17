using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkRequestFileRepository
       : IGenericRepository<WorkRequestFile>
    {
        /// <summary>
        /// Bütün dosya kayıtlarını iş talebi ve
        /// yükleyen kullanıcı detaylarıyla getirir.
        /// </summary>
        Task<List<WorkRequestFile>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen dosya kaydını ilişkili
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<WorkRequestFile?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dosya ID ve iş talebi ID değerlerinin
        /// eşleştiği dosya kaydını getirir.
        /// </summary>
        Task<WorkRequestFile?> GetByIdAndWorkRequestIdAsync(
            int id,
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş talebine ait bütün
        /// dosya kayıtlarını getirir.
        /// </summary>
        Task<List<WorkRequestFile>> GetFilesByWorkRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// yüklenen dosyaları getirir.
        /// </summary>
        Task<List<WorkRequestFile>> GetFilesByUploadedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine belirtilen kullanıcı tarafından
        /// yüklenen dosyaları getirir.
        /// </summary>
        Task<List<WorkRequestFile>> GetFilesByRequestAndUserAsync(
            int workRequestId,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dosyanın saklama yoluna göre
        /// veritabanı kaydını getirir.
        /// </summary>
        Task<WorkRequestFile?> GetByFilePathAsync(
            string filePath,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen MIME türüne ait
        /// dosya kayıtlarını getirir.
        /// </summary>
        Task<List<WorkRequestFile>> GetFilesByContentTypeAsync(
            string contentType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığında
        /// oluşturulmuş dosya kayıtlarını getirir.
        /// </summary>
        Task<List<WorkRequestFile>> GetFilesByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dosya saklama yolunun daha önce
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsFilePathAsync(
            string filePath,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı iş talebinde aynı dosya adına
        /// sahip kayıt olup olmadığını kontrol eder.
        ///
        /// Bu kontrol tek başına yeni dosya
        /// yüklemeyi engellemez.
        /// </summary>
        Task<bool> ExistsFileNameInRequestAsync(
            int workRequestId,
            string fileName,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine ait herhangi bir
        /// dosya bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasFilesAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine eklenen dosyaların sayısını getirir.
        /// </summary>
        Task<int> GetFileCountByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine eklenen dosyaların toplam
        /// boyutunu byte cinsinden getirir.
        /// </summary>
        Task<long> GetTotalFileSizeByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);
    }
}
