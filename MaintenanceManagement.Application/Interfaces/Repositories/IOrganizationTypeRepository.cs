using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IOrganizationTypeRepository
          : IGenericRepository<OrganizationType>
    {
        /// <summary>
        /// Aktif organizasyon türlerini getirir.
        /// </summary>
        Task<List<OrganizationType>> GetActiveOrganizationTypesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon türlerini bağlı birimleriyle birlikte getirir.
        /// </summary>
        Task<List<OrganizationType>> GetAllWithUnitsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon türünü bağlı birimleriyle getirir.
        /// </summary>
        Task<OrganizationType?> GetByIdWithUnitsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon türü adının daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// Güncellemede excludedId kullanılarak mevcut kayıt hariç tutulur.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon türü kodunun daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon türüne bağlı birim bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasUnitsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon türünün aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
