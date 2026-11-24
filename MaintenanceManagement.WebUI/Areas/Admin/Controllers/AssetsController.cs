using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetCatalog;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public sealed class AssetsController : Controller
    {
        private readonly IAssetCatalogService _catalog;
        public AssetsController(IAssetCatalogService catalog) => _catalog = catalog;

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] AssetListFilter filter, CancellationToken ct)
        {
            var vm = await _catalog.GetListAsync(filter, ct);
            return View(vm);
        }

        // Detay sayfasında tek kaynak mevcut AssetWizardController.Details metodudur.
        [HttpGet]
        public IActionResult Details(int id)
            => RedirectToAction("Details", "AssetWizard", new { area = "Admin", id });
    }
}
