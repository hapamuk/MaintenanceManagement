using MaintenanceManagement.Application.DTOS.WarehouseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWarehouseRegistrationService
    {
        Task<int> CreateAsync(
            CreateWarehouseDto dto,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            UpdateWarehouseDto dto,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<WarehousePersonnelCandidateDto>>
            GetEligiblePersonnelAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<int>> GetAssignedPersonnelIdsAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);
    }
}
