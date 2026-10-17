using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IPermissionRepository
        : IGenericRepository<Permission>
    {
        /// <summary>
        /// Aktif yetkileri alfabetik sırayla getirir.
        /// </summary>
        Task<List<Permission>> GetActivePermissionsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Tüm yetkileri ilişkili rol ve iş alt türü
        /// yetkilendirme kayıtlarıyla birlikte getirir.
        /// </summary>
        Task<List<Permission>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen yetkiyi ilişkili rol ve iş alt türü
        /// bilgileriyle birlikte getirir.
        /// </summary>
        Task<Permission?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen modüle ait aktif yetkileri getirir.
        /// </summary>
        Task<List<Permission>> GetPermissionsByModuleAsync(
            string module,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı modülde aynı isimde yetki bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            string module,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yetki kodunun daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yetkinin herhangi bir rol veya iş alt türü
        /// yetkilendirmesinde kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssignmentsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yetkinin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
