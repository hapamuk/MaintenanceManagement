using MaintenanceManagement.Application.DTOS.WarehouseDtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWarehouseRegistrationRepository
    {
        Task<int> CreateWithPersonnelAsync(CreateWarehouseDto dto,
            IReadOnlyCollection<int> personnelIds,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateWithPersonnelAsync(UpdateWarehouseDto dto,
            IReadOnlyCollection<int> personnelIds,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WarehousePersonnelCandidateDto>>
            GetEligiblePersonnelByUnitAsync(int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Seçilen üst birim ve bütün aktif alt birimlerindeki
        /// uygun personelleri listeler.
        /// </summary>
        Task<IReadOnlyList<WarehousePersonnelCandidateDto>>
            GetEligiblePersonnelByUnitIdsAsync(
                IReadOnlyCollection<int> unitIds,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<int>> GetAssignedPersonnelIdsAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);
    }
}
