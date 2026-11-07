
using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class MaterialController : Controller
    {
        #region Fields

        private readonly IMaterialService _materialService;

        private readonly IMaterialCategoryService
            _materialCategoryService;

        private readonly IMeasurementUnitService
            _measurementUnitService;

        private readonly ILogger<MaterialController>
            _logger;

        #endregion

        #region Constructor

        public MaterialController(
            IMaterialService materialService,
            IMaterialCategoryService materialCategoryService,
            IMeasurementUnitService measurementUnitService,
            ILogger<MaterialController> logger)
        {
            _materialService = materialService;

            _materialCategoryService =
                materialCategoryService;

            _measurementUnitService =
                measurementUnitService;

            _logger = logger;
        }

        #endregion

        #region Index

        /// <summary>
        /// Bütün malzemeleri listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken = default)
        {
            var materials = await _materialService
                .GetAllAsync(cancellationToken);

            return View(materials);
        }

        #endregion

        #region Active

        /// <summary>
        /// Aktif malzemeleri listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Active(
            CancellationToken cancellationToken = default)
        {
            var materials = await _materialService
                .GetActiveAsync(cancellationToken);

            return View("Index", materials);
        }

        #endregion

        #region Inactive

        /// <summary>
        /// Pasif malzemeleri listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Inactive(
            CancellationToken cancellationToken = default)
        {
            var materials = await _materialService
                .GetInactiveAsync(cancellationToken);

            return View("Index", materials);
        }

        #endregion

        #region Search

        /// <summary>
        /// Malzeme kodu veya adına göre arama yapar.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(
            string? searchText,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return RedirectToAction(
                    onlyActive
                        ? nameof(Active)
                        : nameof(Index));
            }

            try
            {
                var materials = await _materialService
                    .SearchAsync(
                        searchText,
                        onlyActive,
                        cancellationToken);

                ViewBag.SearchText = searchText;
                ViewBag.OnlyActive = onlyActive;

                return View("Index", materials);
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;

                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region Details

        /// <summary>
        /// Malzeme detayını görüntüler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var material = await _materialService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        #endregion

        #region Create GET

        /// <summary>
        /// Yeni malzeme oluşturma sayfasını açar.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(
    CancellationToken cancellationToken = default)
        {
            await LoadDropdownsAsync(cancellationToken);

            string materialCode = await _materialService
                .GenerateUniqueMaterialCodeAsync(
                    cancellationToken);

            var model = new CreateMaterialDto
            {
                MaterialCode = materialCode,
                MinimumStockLevel = 0,
                UnitId = null
            };

            return View(model);
        }

        #endregion

        #region Create POST

        /// <summary>
        /// Yeni malzeme kaydı oluşturur.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    CreateMaterialDto model,
    CancellationToken cancellationToken = default)
        {
            // Doğrudan organizasyon ataması yapılmaz.
            model.UnitId = null;

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(cancellationToken);
                return View(model);
            }

            try
            {
                int materialId = await _materialService
                    .CreateAsync(
                        model,
                        cancellationToken);

                TempData["SuccessMessage"] =
                    "Malzeme başarıyla oluşturuldu.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = materialId });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (Exception ex) when
                (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "Malzeme oluşturulurken hata oluştu.");

                ModelState.AddModelError(
                    string.Empty,
                    "Malzeme kaydedilirken beklenmeyen bir hata oluştu.");
            }

            await LoadDropdownsAsync(cancellationToken);

            return View(model);
        }

        #endregion

        #region Edit GET

        /// <summary>
        /// Malzeme güncelleme sayfasını açar.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var material = await _materialService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (material == null)
            {
                return NotFound();
            }

            var model = new UpdateMaterialDto
            {
                Id = material.Id,
                MaterialCode = material.MaterialCode,
                Name = material.Name,
                MeasurementUnitId = material.MeasurementUnitId,
                MaterialCategoryId = material.MaterialCategoryId,
                UnitId = material.UnitId,
                MinimumStockLevel = material.MinimumStockLevel,
                Description = material.Description
            };

            await LoadDropdownsAsync(cancellationToken);

            ViewBag.CanChangeMeasurementUnit =
                await _materialService
                    .CanChangeMeasurementUnitAsync(
                        id,
                        cancellationToken);

            return View(model);
        }

        #endregion

        #region Edit POST

        /// <summary>
        /// Malzeme bilgilerini günceller.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateMaterialDto model,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0 || id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PrepareEditViewAsync(
                    model.Id,
                    cancellationToken);

                return View(model);
            }

            try
            {
                // =====================================
                // 1. MEVCUT MALZEME
                // =====================================

                var currentMaterial = await _materialService
                    .GetByIdAsync(
                        model.Id,
                        cancellationToken);

                if (currentMaterial == null)
                {
                    return NotFound();
                }

                // =====================================
                // MEVCUT ORGANİZASYON İLİŞKİSİNİ KORU
                // =====================================
                // Edit ekranında UnitId alanı bulunmadığı için
                // gönderilen değeri kullanma. Geçiş sürecinde
                // mevcut kayıttaki UnitId değeri korunur.
                model.UnitId = currentMaterial.UnitId;

                // =====================================
                // 2. MALZEME KODU KONTROLÜ
                // =====================================

                bool codeExists = await _materialService
                    .ExistsMaterialCodeAsync(
                        model.MaterialCode,
                        excludedId: model.Id,
                        cancellationToken: cancellationToken);

                if (codeExists)
                {
                    ModelState.AddModelError(
                        nameof(model.MaterialCode),
                        "Bu malzeme kodu başka bir kayıt tarafından kullanılmaktadır.");

                    await PrepareEditViewAsync(
                        model.Id,
                        cancellationToken);

                    return View(model);
                }

                // =====================================
                // 3. ÖLÇÜ BİRİMİ KONTROLÜ
                // =====================================

                if (currentMaterial.MeasurementUnitId !=
                    model.MeasurementUnitId)
                {
                    bool canChange = await _materialService
                        .CanChangeMeasurementUnitAsync(
                            model.Id,
                            cancellationToken);

                    if (!canChange)
                    {
                        ModelState.AddModelError(
                            nameof(model.MeasurementUnitId),
                            "Stok veya işlem geçmişi bulunan malzemenin ölçü birimi değiştirilemez.");

                        await PrepareEditViewAsync(
                            model.Id,
                            cancellationToken);

                        return View(model);
                    }
                }

                // =====================================
                // 4. MALZEMEYİ GÜNCELLE
                // =====================================

                bool updated = await _materialService
                    .UpdateAsync(
                        model,
                        cancellationToken);

                if (!updated)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Malzeme güncellenemedi.");

                    await PrepareEditViewAsync(
                        model.Id,
                        cancellationToken);

                    return View(model);
                }

                TempData["SuccessMessage"] =
                    "Malzeme başarıyla güncellendi.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = model.Id });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (Exception ex) when
                (ex is not OperationCanceledException)
            {
                _logger.LogError(
                    ex,
                    "Malzeme güncellenirken hata oluştu. Id: {Id}",
                    model.Id);

                ModelState.AddModelError(
                    string.Empty,
                    "Malzeme güncellenirken beklenmeyen bir hata oluştu.");
            }

            await PrepareEditViewAsync(
                model.Id,
                cancellationToken);

            return View(model);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Malzemeyi aktif veya pasif hale getirir.
        /// </summary>
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
                bool changed = await _materialService
                    .ChangeStatusAsync(
                        id,
                        isActive,
                        cancellationToken);

                if (!changed)
                {
                    TempData["ErrorMessage"] =
                        "Malzeme bulunamadı veya durumu değiştirilemedi.";
                }
                else
                {
                    TempData["SuccessMessage"] = isActive
                        ? "Malzeme aktif hale getirildi."
                        : "Malzeme pasif hale getirildi.";
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
                    "Malzeme durumu değiştirilemedi. Id: {Id}",
                    id);

                TempData["ErrorMessage"] =
                    "Malzeme durumu değiştirilirken hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region UpdateMinimumStockLevel

        /// <summary>
        /// Minimum stok seviyesini değiştirir.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMinimumStockLevel(
            int id,
            decimal minimumStockLevel,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                bool updated = await _materialService
                    .UpdateMinimumStockLevelAsync(
                        id,
                        minimumStockLevel,
                        cancellationToken);

                TempData[updated
                    ? "SuccessMessage"
                    : "ErrorMessage"] = updated
                    ? "Minimum stok seviyesi güncellendi."
                    : "Minimum stok seviyesi güncellenemedi.";
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
                    "Minimum stok seviyesi güncellenemedi. Id: {Id}",
                    id);

                TempData["ErrorMessage"] =
                    "Minimum stok seviyesi güncellenirken hata oluştu.";
            }

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        #endregion

        #region WarehouseStocks

        /// <summary>
        /// Malzemeye ait ambar stoklarını görüntüler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> WarehouseStocks(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var material = await _materialService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (material == null)
            {
                return NotFound();
            }

            var stocks = await _materialService
                .GetWarehouseStocksByMaterialIdAsync(
                    id,
                    cancellationToken);

            ViewBag.MaterialId = id;
            ViewBag.MaterialName = material.Name;
            ViewBag.MaterialCode = material.MaterialCode;

            return View(stocks);
        }

        #endregion

        #region ByCategory

        /// <summary>
        /// Belirli kategoriye ait malzemeleri getirir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ByCategory(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                return BadRequest();
            }

            var materials = await _materialService
                .GetByCategoryIdAsync(
                    categoryId,
                    cancellationToken);

            ViewBag.CategoryId = categoryId;

            return View("Index", materials);
        }

        #endregion

        #region ByMeasurementUnit

        /// <summary>
        /// Ölçü birimine göre malzemeleri listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ByMeasurementUnit(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            if (measurementUnitId <= 0)
            {
                return BadRequest();
            }

            var materials = await _materialService
                .GetByMeasurementUnitIdAsync(
                    measurementUnitId,
                    cancellationToken);

            ViewBag.MeasurementUnitId = measurementUnitId;

            return View("Index", materials);
        }

        #endregion

        #region WithoutCategory

        /// <summary>
        /// Kategorisi bulunmayan malzemeleri listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> WithoutCategory(
            CancellationToken cancellationToken = default)
        {
            var materials = await _materialService
                .GetWithoutCategoryAsync(cancellationToken);

            return View("Index", materials);
        }

        #endregion

        #region LoadDropdowns

        /// <summary>
        /// Create ve Edit ekranlarındaki
        /// ölçü birimi ve kategori seçeneklerini yükler.
        /// </summary>
        private async Task LoadDropdownsAsync(
            CancellationToken cancellationToken = default)
        {
            var measurementUnits = await _measurementUnitService
                .GetActiveAsync(cancellationToken);

            var categories = await _materialCategoryService
                .GetActiveAsync(cancellationToken);

            ViewBag.MeasurementUnits = measurementUnits
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            ViewBag.MaterialCategories = categories
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }

        #endregion

        #region PrepareEditView

        /// <summary>
        /// Hatalı Edit POST sonrasında
        /// dropdown ve ölçü birimi yetkilerini
        /// yeniden hazırlar.
        /// </summary>
        private async Task PrepareEditViewAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            await LoadDropdownsAsync(cancellationToken);

            ViewBag.CanChangeMeasurementUnit =
                await _materialService
                    .CanChangeMeasurementUnitAsync(
                        materialId,
                        cancellationToken);
        }

        #endregion
    }
}
