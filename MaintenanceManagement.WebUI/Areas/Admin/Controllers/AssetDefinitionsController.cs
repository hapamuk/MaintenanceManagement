using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetDefinitions;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize] // Kurumsal rol politikanız varsa Authorize(Policy=...) olarak sınırlandırın.
    public sealed class AssetDefinitionsController : Controller
    {
        private readonly IAssetDefinitionService _service;
        public AssetDefinitionsController(IAssetDefinitionService service) => _service = service;

        [HttpGet] public async Task<IActionResult> Categories(CancellationToken ct = default)
            => View("Definition", await _service.GetPageAsync("category", ct));
        [HttpGet] public async Task<IActionResult> Brands(CancellationToken ct = default)
            => View("Definition", await _service.GetPageAsync("brand", ct));
        [HttpGet] public async Task<IActionResult> Models(CancellationToken ct = default)
            => View("Definition", await _service.GetPageAsync("model", ct));

        [HttpGet]
        public async Task<IActionResult> CategoryUnits(int id, CancellationToken ct = default)
        {
            if (id <= 0) return BadRequest(new { message = "Geçerli kategori seçiniz." });
            try
            {
                return Json(await _service.GetCategoryUnitIdsAsync(id, ct));
            }
            catch (ValidationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCategory([Bind(Prefix="Category")] AssetCategoryDefinitionInput input, CancellationToken ct = default)
        {
            if (ModelState.IsValid)
                try { await _service.SaveCategoryAsync(input, ct); TempData["SuccessMessage"] = "Kategori kaydedildi."; return RedirectToAction(nameof(Categories)); }
                catch (ValidationException e) { ModelState.AddModelError("", e.Message); }
            var page = await _service.GetPageAsync("category", ct); page.Category = input;
            return View("Definition", page);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBrand([Bind(Prefix="Brand")] BrandDefinitionInput input, CancellationToken ct = default)
        {
            if (ModelState.IsValid)
                try { await _service.SaveBrandAsync(input, ct); TempData["SuccessMessage"] = "Marka kaydedildi."; return RedirectToAction(nameof(Brands)); }
                catch (ValidationException e) { ModelState.AddModelError("", e.Message); }
            var page = await _service.GetPageAsync("brand", ct); page.Brand = input;
            return View("Definition", page);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveModel([Bind(Prefix="Model")] AssetModelDefinitionInput input, CancellationToken ct = default)
        {
            if (ModelState.IsValid)
                try { await _service.SaveModelAsync(input, ct); TempData["SuccessMessage"] = "Model kaydedildi."; return RedirectToAction(nameof(Models)); }
                catch (ValidationException e) { ModelState.AddModelError("", e.Message); }
            var page = await _service.GetPageAsync("model", ct); page.Model = input;
            return View("Definition", page);
        }

        [HttpGet]
        public async Task<IActionResult> ModelsByBrand(int brandId, CancellationToken ct = default)
            => Json(await _service.GetModelsForBrandAsync(brandId, ct));
    }
}
