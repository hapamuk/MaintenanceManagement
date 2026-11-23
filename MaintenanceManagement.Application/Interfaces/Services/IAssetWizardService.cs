using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetWizard;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    // Veri erişimi mevcut Asset/AssetUser/AssetMovement/WorkOrder servislerine uyarlanır.
    public interface IAssetWizardService
    {
        Task<AssetWizardPageVm> GetFormAsync(CancellationToken ct = default);
        Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
        Task<int> CreateAsync(AssetWizardInput input, int actorUserId, CancellationToken ct = default);
        Task<AssetDetailPageVm?> GetDetailAsync(int assetId, CancellationToken ct = default);
    }
}
