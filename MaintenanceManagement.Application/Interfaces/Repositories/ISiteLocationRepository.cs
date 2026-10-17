using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface ISiteLocationRepository
        : IGenericRepository<SiteLocation>
    {
        /// <summary>
        /// Aktif konumları alfabetik sırayla getirir.
        /// </summary>
        Task<List<SiteLocation>> GetActiveLocationsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bütün konumları ilişkili iş talepleriyle getirir.
        /// </summary>
        Task<List<SiteLocation>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen konumu ilişkili iş talebi
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<SiteLocation?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konum adına göre kayıt getirir.
        /// </summary>
        Task<SiteLocation?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen binaya ait konum kayıtlarını getirir.
        /// </summary>
        Task<List<SiteLocation>> GetLocationsByBuildingAsync(
            string building,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adres bilgisine göre konumları arar.
        /// </summary>
        Task<List<SiteLocation>> SearchByAddressAsync(
            string address,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Enlem ve boylam bilgileri tanımlanmış
        /// aktif konumları getirir.
        /// </summary>
        Task<List<SiteLocation>> GetLocationsWithCoordinatesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konuma bağlı bütün iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetWorkRequestsByLocationIdAsync(
            int locationId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konuma bağlı Yeni (Draft) durumundaki
        /// iş taleplerini getirir.
        /// </summary>
        Task<List<WorkRequest>> GetNewWorkRequestsByLocationIdAsync(
            int locationId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konum adının başka bir kayıt tarafından
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konuma bağlı herhangi bir iş talebi
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasWorkRequestsAsync(
            int locationId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konuma bağlı Yeni (Draft) durumunda
        /// iş talebi bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasNewWorkRequestsAsync(
            int locationId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Konumun aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
