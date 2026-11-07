
using MaintenanceManagement.Application.DTOS.MeasurementUnitDtos;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize] // Kimlik doğrulama yapılandırıldığında etkinleştirin.
    public class MeasurementUnitController : Controller
    {
        #region Fields

        private readonly IMeasurementUnitService
            _measurementUnitService;

        private readonly ILogger<MeasurementUnitController>
            _logger;

        #endregion

        #region Constructor

        public MeasurementUnitController(
            IMeasurementUnitService measurementUnitService,
            ILogger<MeasurementUnitController> logger)
        {
            _measurementUnitService = measurementUnitService;
            _logger = logger;
        }

        #endregion

        #region Index

        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken = default)
        {
            var units = await _measurementUnitService
                .GetAllAsync(cancellationToken);

            return View(units);
        }

        #endregion

        #region Active

        [HttpGet]
        public async Task<IActionResult> Active(
            CancellationToken cancellationToken = default)
        {
            var units = await _measurementUnitService
                .GetActiveAsync(cancellationToken);

            ViewBag.CurrentFilter = "Active";

            return View("Index", units);
        }

        #endregion

        #region Inactive

        [HttpGet]
        public async Task<IActionResult> Inactive(
            CancellationToken cancellationToken = default)
        {
            var units = await _measurementUnitService
                .GetInactiveAsync(cancellationToken);

            ViewBag.CurrentFilter = "Inactive";

            return View("Index", units);
        }

        #endregion

        #region Search

        [HttpGet]
        public async Task<IActionResult> Search(
            string? searchText,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return RedirectToAction(
                    onlyActive ? nameof(Active) : nameof(Index));
            }

            try
            {
                var units = await _measurementUnitService
                    .SearchAsync(
                        searchText,
                        onlyActive,
                        cancellationToken);

                ViewBag.SearchText = searchText;
                ViewBag.OnlyActive = onlyActive;
                ViewBag.CurrentFilter = "Search";

                return View("Index", units);
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var unit = await _measurementUnitService
                .GetByIdAsync(id, cancellationToken);

            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        #endregion

        #region Create GET

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateMeasurementUnitDto());
        }

        #endregion

        #region Create POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateMeasurementUnitDto model,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.Name = model.Name?.Trim() ?? string.Empty;
                model.Symbol = model.Symbol?.Trim() ?? string.Empty;

                bool existsName = await _measurementUnitService
                    .ExistsNameAsync(
                        model.Name,
                        cancellationToken: cancellationToken);

                if (existsName)
                {
                    ModelState.AddModelError(
                        nameof(model.Name),
                        "Bu ölçü birimi adı zaten kullanılmaktadır.");

                    return View(model);
                }

                bool existsSymbol = await _measurementUnitService
                    .ExistsSymbolAsync(
                        model.Symbol,
                        cancellationToken: cancellationToken);

                if (existsSymbol)
                {
                    ModelState.AddModelError(
                        nameof(model.Symbol),
                        "Bu ölçü birimi kısaltması zaten kullanılmaktadır.");

                    return View(model);
                }

                int id = await _measurementUnitService
                    .CreateAsync(model, cancellationToken);

                TempData["SuccessMessage"] =
                    "Ölçü birimi başarıyla oluşturuldu.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex) when
                (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "Ölçü birimi oluşturulurken hata oluştu.");

                ModelState.AddModelError(
                    string.Empty,
                    "Ölçü birimi kaydedilirken beklenmeyen bir hata oluştu.");
            }

            return View(model);
        }

        #endregion

        #region Edit GET

        [HttpGet]
        public async Task<IActionResult> Edit(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var unit = await _measurementUnitService
                .GetByIdAsync(id, cancellationToken);

            if (unit == null)
            {
                return NotFound();
            }

            var model = new UpdateMeasurementUnitDto
            {
                Id = unit.Id,
                Name = unit.Name,
                Symbol = unit.Symbol
            };

            return View(model);
        }

        #endregion

        #region Edit POST

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateMeasurementUnitDto model,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0 || id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                model.Name = model.Name?.Trim() ?? string.Empty;
                model.Symbol = model.Symbol?.Trim() ?? string.Empty;

                var currentUnit = await _measurementUnitService
                    .GetByIdAsync(id, cancellationToken);

                if (currentUnit == null)
                {
                    return NotFound();
                }

                bool existsName = await _measurementUnitService
                    .ExistsNameAsync(
                        model.Name,
                        excludedId: id,
                        cancellationToken: cancellationToken);

                if (existsName)
                {
                    ModelState.AddModelError(
                        nameof(model.Name),
                        "Bu ad başka bir ölçü birimi tarafından kullanılmaktadır.");

                    return View(model);
                }

                bool existsSymbol = await _measurementUnitService
                    .ExistsSymbolAsync(
                        model.Symbol,
                        excludedId: id,
                        cancellationToken: cancellationToken);

                if (existsSymbol)
                {
                    ModelState.AddModelError(
                        nameof(model.Symbol),
                        "Bu kısaltma başka bir ölçü birimi tarafından kullanılmaktadır.");

                    return View(model);
                }

                bool updated = await _measurementUnitService
                    .UpdateAsync(model, cancellationToken);

                if (!updated)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Ölçü birimi güncellenemedi.");

                    return View(model);
                }

                TempData["SuccessMessage"] =
                    "Ölçü birimi başarıyla güncellendi.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.Id });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex) when
                (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "Ölçü birimi güncellenirken hata oluştu. Id: {Id}",
                    model.Id);

                ModelState.AddModelError(
                    string.Empty,
                    "Güncelleme sırasında beklenmeyen bir hata oluştu.");
            }

            return View(model);
        }

        #endregion

        #region ChangeStatus

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                bool changed = await _measurementUnitService
                    .ChangeStatusAsync(
                        id,
                        isActive,
                        cancellationToken);

                if (!changed)
                {
                    TempData["ErrorMessage"] =
                        "Ölçü birimi bulunamadı veya durumu değiştirilemedi.";
                }
                else
                {
                    TempData["SuccessMessage"] = isActive
                        ? "Ölçü birimi aktif hale getirildi."
                        : "Ölçü birimi pasif hale getirildi.";
                }
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex) when
                (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "Ölçü biriminin durumu değiştirilemedi. Id: {Id}",
                    id);

                TempData["ErrorMessage"] =
                    "Durum değiştirilirken beklenmeyen bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Materials

        [HttpGet]
        public async Task<IActionResult> Materials(
            int id,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var unit = await _measurementUnitService
                .GetByIdAsync(id, cancellationToken);

            if (unit == null)
            {
                return NotFound();
            }

            var materials = onlyActive
                ? await _measurementUnitService
                    .GetActiveMaterialsByMeasurementUnitIdAsync(
                        id, cancellationToken)
                : await _measurementUnitService
                    .GetMaterialsByMeasurementUnitIdAsync(
                        id, cancellationToken);

            ViewBag.MeasurementUnitId = id;
            ViewBag.MeasurementUnitName = unit.Name;
            ViewBag.MeasurementUnitSymbol = unit.Symbol;
            ViewBag.OnlyActive = onlyActive;

            return View(materials);
        }

        #endregion

        #region WithMaterials

        [HttpGet]
        public async Task<IActionResult> WithMaterials(
            CancellationToken cancellationToken = default)
        {
            var units = await _measurementUnitService
                .GetUnitsWithMaterialsAsync(cancellationToken);

            ViewBag.CurrentFilter = "WithMaterials";

            return View("Index", units);
        }

        #endregion

        #region WithoutMaterials

        [HttpGet]
        public async Task<IActionResult> WithoutMaterials(
            CancellationToken cancellationToken = default)
        {
            var units = await _measurementUnitService
                .GetUnitsWithoutMaterialsAsync(cancellationToken);

            ViewBag.CurrentFilter = "WithoutMaterials";

            return View("Index", units);
        }

        #endregion
    }
}
