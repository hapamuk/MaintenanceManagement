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
        /// Belirli organizasyona ait aktif rolleri getirir.
        /// </summary>
        Task<List<Role>> GetRolesByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı organizasyonda rol kodunun kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolün aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
