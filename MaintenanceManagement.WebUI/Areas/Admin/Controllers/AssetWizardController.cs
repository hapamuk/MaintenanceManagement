using System;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using MaintenanceManagement.Application.DTOs.AssetWizard;
using MaintenanceManagement.Application.DTOs.AssetDefinitions;
using System.ComponentModel.DataAnnotations;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public sealed class AssetWizardController : Controller
    {
        private readonly IAssetWizardService _service;
        private readonly IAssetDefinitionService _definitions;
        public AssetWizardController(IAssetWizardService service, IAssetDefinitionService definitions)
        {
            _service = service;
            _definitions = definitions;
        }
        private int CurrentUserId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 0;

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct = default)
        {
            var vm = await _service.GetFormAsync(ct);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCode(string code, CancellationToken ct = default)
            => Json(new { exists = await _service.CodeExistsAsync((code ?? "").Trim(), ct) });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssetWizardInput input, CancellationToken ct = default)
        {
            if (CurrentUserId <= 0) return Challenge();
            // Demirbaş kodu kullanıcıdan alınmaz; servis, kayıttan sonra Asset.Id üzerinden üretir.
            input.AssetCode = string.Empty;
            if (input.AssetCategoryId > 0 && input.ResponsibleUnitId > 0 &&
                !await _definitions.CategoryAllowsUnitAsync(input.AssetCategoryId, input.ResponsibleUnitId, ct))
                ModelState.AddModelError("AssetCategoryId", "Kategori sorumlu organizasyon kapsamında değil.");
            if (input.AssetCategoryId > 0 && input.UsingUnitId > 0 &&
                !await _definitions.CategoryAllowsUnitAsync(input.AssetCategoryId, input.UsingUnitId, ct))
                ModelState.AddModelError("AssetCategoryId", "Kategori kullanan organizasyon kapsamında değil.");
            if (input.AssetCategoryId <= 0 || input.AssetKindId <= 0 || input.BrandId <= 0 || input.AssetModelId <= 0)
                ModelState.AddModelError("", "Kategori, demirbaş türü, marka ve model seçilmelidir.");
            if (input.ResponsibleUnitId <= 0)
                ModelState.AddModelError("", "Sorumlu organizasyon seçilmelidir.");
            if (!ModelState.IsValid)
            {
                var page = await _service.GetFormAsync(ct);
                page.Input = input;
                return View(page);
            }
            try
            {
                int id = await _service.CreateAsync(input, CurrentUserId, ct);
                TempData["SuccessMessage"] = "Demirbaş başarıyla kaydedildi.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ValidationException)
            {
                ModelState.AddModelError("", ex.Message);
                var page = await _service.GetFormAsync(ct);
                page.Input = input;
                return View(page);
            }
        }


        // GET: /Admin/AssetWizard/Edit/123 — altı adım ve eski değerler korunur.
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            if (id <= 0) return NotFound();
            var vm = await _service.GetEditFormAsync(id, ct);
            if (vm == null) return NotFound();
            SetupWizard("edit", id);
            return View("Create", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AssetWizardInput input, CancellationToken ct = default)
        {
            if (CurrentUserId <= 0) return Challenge();
            if (id <= 0) return NotFound();
            if (input.AssetCategoryId <= 0 || input.AssetKindId <= 0 || input.BrandId <= 0 || input.AssetModelId <= 0)
                ModelState.AddModelError("", "Kategori, tür, marka ve model seçilmelidir.");
            if (input.ResponsibleUnitId <= 0)
                ModelState.AddModelError("", "Sorumlu organizasyon seçilmelidir.");
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(id, input, CurrentUserId, ct);
                    TempData["SuccessMessage"] = "Demirbaş bilgileri güncellendi.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is ValidationException)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            var page = await _service.GetFormAsync(ct);
            page.Input = input;
            SetupWizard("edit", id);
            return View("Create", page);
        }

        // GET: /Admin/AssetWizard/Assign/123 — yalnızca 4. adım açılır.
        [HttpGet]
        public async Task<IActionResult> Assign(int id, CancellationToken ct = default)
        {
            if (id <= 0) return NotFound();
            var page = await _service.GetEditFormAsync(id, ct);
            if (page == null) return NotFound();
            page.Input.AssigneeUserId = null;
            SetupWizard("assign", id);
            return View("Create", page);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, AssetWizardInput input, CancellationToken ct = default)
        {
            if (CurrentUserId <= 0) return Challenge();
            if (id <= 0) return NotFound();
            // Bu action, diğer demirbaş alanlarını asla güncellemez.
            if (!input.AssigneeUserId.HasValue || input.AssigneeUserId.Value <= 0)
                ModelState.AddModelError("", "Zimmet verilecek personeli seçiniz.");
            if (input.AssigneeUserId.HasValue && input.AssigneeUserId.Value > 0)
            {
                try
                {
                    await _service.AssignExistingAsync(id, input.AssigneeUserId.Value, CurrentUserId, ct);
                    TempData["SuccessMessage"] = "Demirbaş zimmetlendi.";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (Exception ex) when (ex is InvalidOperationException || ex is ValidationException)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            var page = await _service.GetEditFormAsync(id, ct);
            if (page == null) return NotFound();
            page.Input.AssigneeUserId = input.AssigneeUserId;
            SetupWizard("assign", id);
            return View("Create", page);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, CancellationToken ct = default)
        {
            if (CurrentUserId <= 0) return Challenge();
            if (id <= 0) return NotFound();
            try
            {
                await _service.ReturnExistingAsync(id, CurrentUserId, ct);
                TempData["SuccessMessage"] = "Zimmet iadesi gerçekleştirildi; geçmiş kaydı korundu.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        private void SetupWizard(string mode, int id)
        {
            ViewData["WizardMode"] = mode;
            ViewData["AssetId"] = id;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var result = await _service.GetDetailAsync(id, ct);
            return result is null ? NotFound() : View(result);
        }
    }
}
