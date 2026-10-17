using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IUnitRepository : IGenericRepository<Unit>
    {
        /// <summary>
        /// Aktif organizasyon ve birimleri getirir.
        /// </summary>
        Task<List<Unit>> GetActiveUnitsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon ve birimleri organizasyon türü
        /// ve üst birim bilgileriyle birlikte getirir.
        /// </summary>
        Task<List<Unit>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon veya birimi
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<Unit?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon türüne bağlı
        /// aktif birimleri getirir.
        /// </summary>
        Task<List<Unit>> GetUnitsByOrganizationTypeIdAsync(
            int organizationTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen üst birime bağlı
        /// aktif alt birimleri getirir.
        /// </summary>
        Task<List<Unit>> GetChildUnitsAsync(
            int parentUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı üst birim altında belirtilen
        /// birim adının bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? parentUnitId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birim kodunun daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen birime bağlı alt birim
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasChildUnitsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birime bağlı aktif takım bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveTeamsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Birimin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
