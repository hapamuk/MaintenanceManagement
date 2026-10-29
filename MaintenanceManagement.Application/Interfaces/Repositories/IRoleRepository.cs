using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        /// <summary>
        /// Bütün rolleri bağlı birim bilgileriyle getirir.
        /// Aktif ve pasif roller dahildir.
        /// </summary>
        Task<IReadOnlyList<Role>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen rolü birim, kullanıcı ataması
        /// ve yetki ilişkileriyle getirir.
        /// </summary>
        Task<Role?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen birimin aktif rollerini getirir.
        /// </summary>
        Task<IReadOnlyList<Role>> GetByUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı organizasyon biriminde rol kodunun
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolün aktiflik durumunu günceller.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
