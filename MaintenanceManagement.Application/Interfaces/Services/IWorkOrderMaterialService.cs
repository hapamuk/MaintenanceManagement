using MaintenanceManagement.Application.DTOS.WorkOrderMaterialDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkOrderMaterialService
    {
        #region Get Methods

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        Task<GetByIdWorkOrderMaterialDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByWorkOrderAndMaterialIdAsync(
                int workOrderId,
                int materialId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByWorkOrderAndWarehouseIdAsync(
                int workOrderId,
                int warehouseId,
                CancellationToken cancellationToken = default);

        Task<GetByIdWorkOrderMaterialDto?>
            GetByStockMovementIdAsync(
                int stockMovementId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Gerçekleşmiş bir stok çıkış hareketini
        /// iş emrindeki malzeme kullanımına bağlar.
        ///
        /// stockMovementId güvenilir stok çıkış
        /// işleminden elde edilmelidir.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkOrderMaterialDto dto,
            int currentUserId,
            int stockMovementId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin yalnızca birim maliyet
        /// ve açıklama bilgilerini günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkOrderMaterialDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        Task<bool> HasMaterialsByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsStockMovementLinkAsync(
            int stockMovementId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        Task<int> GetMaterialRecordCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Quantity Methods

        Task<decimal> GetTotalQuantityByWorkOrderAndMaterialIdAsync(
            int workOrderId,
            int materialId,
            CancellationToken cancellationToken = default);

        Task<decimal> GetTotalQuantityByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Cost Methods

        Task<decimal> GetTotalCostByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        Task<decimal> GetTotalCostByWorkOrderAndMaterialIdAsync(
            int workOrderId,
            int materialId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
