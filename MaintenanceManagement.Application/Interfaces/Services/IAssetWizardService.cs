using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetWizard;
namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetWizardService
    {
        Task<AssetWizardPageVm> GetFormAsync(CancellationToken ct = default);
        Task<AssetWizardPageVm?> GetEditFormAsync(int id, CancellationToken ct = default);
        Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
        Task<int> CreateAsync(AssetWizardInput input, int actorUserId, CancellationToken ct = default);
        Task UpdateAsync(int id, AssetWizardInput input, int actorUserId, CancellationToken ct = default);
        Task AssignExistingAsync(int id, int assigneeUserId, int actorUserId, CancellationToken ct = default);
        Task ReturnExistingAsync(int id, int actorUserId, CancellationToken ct = default);
        Task<AssetDetailPageVm?> GetDetailAsync(int assetId, CancellationToken ct = default);
    }
}
