using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces
{
    public interface IWorkOrderFileRepository
        : IGenericRepository<WorkOrderFile>
    {
        /// <summary>
        /// Bütün iş emri dosyalarını iş emri
        /// ve kullanıcı detaylarıyla getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen dosyayı ilişkili
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<WorkOrderFile?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dosya ID ve iş emri ID değerlerini
        /// birlikte kontrol ederek dosyayı getirir.
        /// </summary>
        Task<WorkOrderFile?> GetByIdAndWorkOrderIdAsync(
            int id,
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş emrine eklenen
        /// bütün dosyaları getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// yüklenen dosyaları getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByUploadedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş emrine ilgili kullanıcı
        /// tarafından eklenen dosyaları getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByWorkOrderAndUserAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyona ait
        /// iş emirlerinin dosyalarını getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByResponsibleUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takıma atanmış
        /// iş emirlerinin dosyalarını getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen durumda olan iş emirlerine
        /// eklenmiş dosyaları getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByWorkOrderStatusAsync(
            WorkStatusEnum status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dosyanın depolama yoluna göre
        /// mevcut kaydı getirir.
        /// </summary>
        Task<WorkOrderFile?> GetByFilePathAsync(
            string filePath,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen MIME türündeki
        /// dosyaları getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByContentTypeAsync(
            string contentType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki belirtilen MIME türüne
        /// sahip dosyaları getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByWorkOrderAndContentTypeAsync(
            int workOrderId,
            string contentType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığında
        /// eklenmiş dosyaları getirir.
        /// </summary>
        Task<List<WorkOrderFile>> GetFilesByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine en son eklenen
        /// dosya kaydını getirir.
        /// </summary>
        Task<WorkOrderFile?> GetLatestFileByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Depolama yolunun başka bir kayıt
        /// tarafından kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsFilePathAsync(
            string filePath,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı iş emrine aynı isimle dosya
        /// eklenmiş olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsFileNameInWorkOrderAsync(
            int workOrderId,
            string fileName,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine eklenmiş herhangi bir
        /// dosya bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasFilesAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş emrindeki
        /// toplam dosya sayısını getirir.
        /// </summary>
        Task<int> GetFileCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki dosyaların toplam
        /// boyutunu byte cinsinden getirir.
        /// </summary>
        Task<long> GetTotalFileSizeByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);
    }
}
