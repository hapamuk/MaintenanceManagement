using MaintenanceManagement.Application.DTOS.RoleDtos;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly IUnitService _unitService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(
            IRoleService roleService,
            IUnitService unitService,
            ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _unitService = unitService;
            _logger = logger;
        }

        // =====================================
        // ROL LİSTESİ
        // =====================================

        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var roles = await _roleService
                .GetAllAsync(cancellationToken);

            return View(roles);
        }

        // =====================================
        // ROL DETAYI
        // =====================================

        [HttpGet]
        public async Task<IActionResult> Details(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest();

            var role = await _roleService
                .GetByIdAsync(id, cancellationToken);

            if (role == null)
                return NotFound();

            return View(role);
        }

        // =====================================
        // YENİ ROL - GET
        // =====================================

        [HttpGet]
        public async Task<IActionResult> Create(
            CancellationToken cancellationToken)
        {
            await LoadUnitOptionsAsync(
                null, cancellationToken);

            return View(new CreateRoleDto());
        }

        // =====================================
        // YENİ ROL - POST
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateRoleDto model,
            CancellationToken cancellationToken)
        {
            if (model.UnitId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.UnitId),
                    "Lütfen birim seçiniz.");
            }

            if (!ModelState.IsValid)
            {
                await LoadUnitOptionsAsync(
                    model.UnitId, cancellationToken);

                return View(model);
            }

            try
            {
                int roleId = await _roleService
                    .CreateAsync(model, cancellationToken);

                TempData["Success"] =
                    "Rol başarıyla oluşturuldu.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = roleId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Rol oluşturulamadı.");

                ModelState.AddModelError(
                    string.Empty,
                    "Rol oluşturulurken hata oluştu.");
            }

            await LoadUnitOptionsAsync(
                model.UnitId, cancellationToken);

            return View(model);
        }

        // =====================================
        // ROL GÜNCELLE - GET
        // =====================================

        [HttpGet]
        public async Task<IActionResult> Edit(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest();

            var role = await _roleService
                .GetByIdAsync(id, cancellationToken);

            if (role == null)
                return NotFound();

            var model = new UpdateRoleDto
            {
                Id = role.Id,
                UnitId = role.UnitId,
                Name = role.Name,
                Code = role.Code,
                Description = role.Description
            };

            await LoadUnitOptionsAsync(
                model.UnitId, cancellationToken);

            return View(model);
        }

        // =====================================
        // ROL GÜNCELLE - POST
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateRoleDto model,
            CancellationToken cancellationToken)
        {
            if (id <= 0 || id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadUnitOptionsAsync(
                    model.UnitId, cancellationToken);

                return View(model);
            }

            try
            {
                bool updated = await _roleService
                    .UpdateAsync(model, cancellationToken);

                if (!updated)
                    return NotFound();

                TempData["Success"] =
                    "Rol başarıyla güncellendi.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.Id });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Rol güncellenemedi. Id: {RoleId}",
                    id);

                ModelState.AddModelError(
                    string.Empty,
                    "Güncelleme sırasında hata oluştu.");
            }

            await LoadUnitOptionsAsync(
                model.UnitId, cancellationToken);

            return View(model);
        }

        // =====================================
        // AKTİF / PASİF
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(
            int id,
            bool isActive,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                bool changed = await _roleService
                    .ChangeStatusAsync(
                        id, isActive, cancellationToken);

                if (changed)
                {
                    TempData["Success"] = isActive
                        ? "Rol aktifleştirildi."
                        : "Rol pasifleştirildi.";
                }
                else
                {
                    TempData["Error"] =
                        "Rol bulunamadı veya durum değiştirilemedi.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Rol durumu değiştirilemedi. Id: {RoleId}",
                    id);

                TempData["Error"] =
                    "İşlem sırasında hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================
        // BİRİM SEÇİM LİSTESİ
        // =====================================

        private async Task LoadUnitOptionsAsync(
            int? selectedUnitId,
            CancellationToken cancellationToken)
        {
            var units = await _unitService
                .GetAllAsync(cancellationToken);

            ViewBag.UnitOptions = units
                .Where(x =>
                    x.IsActive ||
                    x.Id == selectedUnitId)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Name} ({x.Code})",
                    Selected = x.Id == selectedUnitId
                })
                .ToList();
        }
    }
}
    

