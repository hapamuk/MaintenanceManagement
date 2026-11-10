using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.WarehouseOperations;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWarehouseOperationsService
    {
        Task<string> StockInAsync(StockOperationInput input,int userId,CancellationToken ct=default);
        Task<string> StockOutAsync(StockOperationInput input,int userId,CancellationToken ct=default);
        Task<string> ConsumeForWorkOrderAsync(WorkOrderMaterialUsageInput input,int userId,CancellationToken ct=default);
        Task<string> TransferCartAsync(WarehouseTransferCartInput input, int userId, CancellationToken ct = default);
        Task<IReadOnlyList<TransferMaterialOption>> GetTransferableMaterialsAsync(int sourceWarehouseId, int targetWarehouseId, int userId, CancellationToken ct = default);
        Task<string> TransferAsync(WarehouseTransferInput input,int userId,CancellationToken ct=default);
        Task<IReadOnlyList<OptionRow>> GetWarehousesAsync(int userId,CancellationToken ct=default);
        Task<IReadOnlyList<OptionRow>> GetAuthorizedWarehousesAsync(int userId,CancellationToken ct=default);
        Task<IReadOnlyList<OptionRow>> GetMaterialsForWarehouseAsync(int warehouseId,int userId,bool onlyStocked=false,CancellationToken ct=default);
        Task<IReadOnlyList<OptionRow>> GetTransferTargetsAsync(int sourceWarehouseId,int materialId,int userId,CancellationToken ct=default);
        Task<IReadOnlyList<OptionRow>> GetMaterialsAsync(CancellationToken ct=default);
        Task<IReadOnlyList<MovementRow>> GetMovementsAsync(int userId,int? warehouseId,string? search,CancellationToken ct=default);
        Task<IReadOnlyList<StockRow>> GetStocksAsync(int userId,int? warehouseId,CancellationToken ct=default);
        Task<IReadOnlyList<WorkOrderUseRow>> GetWorkOrderUsesAsync(int userId,string? search,CancellationToken ct=default);
    }
}
