using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetDefinitions;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetDefinitionService
    {
        Task<DefinitionPageVm> GetPageAsync(string type, CancellationToken ct = default);
        Task<IReadOnlyList<int>> GetCategoryUnitIdsAsync(int categoryId, CancellationToken ct = default);
        Task SaveCategoryAsync(AssetCategoryDefinitionInput input, CancellationToken ct = default);
        Task SaveBrandAsync(BrandDefinitionInput input, CancellationToken ct = default);
        Task SaveModelAsync(AssetModelDefinitionInput input, CancellationToken ct = default);
        Task<IReadOnlyList<DefinitionOption>> GetModelsForBrandAsync(int brandId, CancellationToken ct = default);
        Task<bool> CategoryAllowsUnitAsync(int categoryId, int unitId, CancellationToken ct = default);
        Task<string> IssueAssetCodeAsync(CancellationToken ct = default);
    }
}
