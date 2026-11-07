
using MaintenanceManagement.Application.DTOS.WarehouseDtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWarehouseRegistrationRepository
    {
        /// <summary>
        /// Ambar ve personel atamalarını
        /// tek transaction içinde oluşturur.
        /// </summary>
        Task<int> CreateWithPersonnelAsync(
            CreateWarehouseDto dto,
            IReadOnlyCollection<int> personnelIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve personel atamalarını
        /// tek transaction içinde günceller.
        /// </summary>
        Task<bool> UpdateWithPersonnelAsync(
            UpdateWarehouseDto dto,
            IReadOnlyCollection<int> personnelIds,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyondaki uygun personelleri getirir.
        /// </summary>
        Task<IReadOnlyList<WarehousePersonnelCandidateDto>>
            GetEligiblePersonnelByUnitAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambara atanmış personellerin ID listesini getirir.
        /// </summary>
        Task<IReadOnlyList<int>> GetAssignedPersonnelIdsAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);
    }
}
