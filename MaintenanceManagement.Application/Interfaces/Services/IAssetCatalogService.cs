using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetCatalog;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetCatalogService
    {
        Task<AssetListPageVm> GetListAsync(AssetListFilter filter, CancellationToken ct = default);
    }
}
