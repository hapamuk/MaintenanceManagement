using MaintenanceManagement.Application.DTOS.MaterialCategoryDtos;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class MaterialCategoryController : Controller
    {
        #region Fields

        private readonly IMaterialCategoryService
            _materialCategoryService;

        private readonly IUnitService
            _unitService;

        private readonly ILogger<MaterialCategoryController>
            _logger;

        #endregion

        #region Constructor

        public MaterialCategoryController(
            IMaterialCategoryService materialCategoryService,
            IUnitService unitService,
            ILogger<MaterialCategoryController> logger)
        {
            _materialCategoryService =
                materialCategoryService;

            _unitService = unitService;

            _logger = logger;
        }

        #endregion

        #region Index

        /// <summary>
        /// Bütün malzeme kategorilerini listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var categories = await _materialCategoryService
                .GetAllAsync(cancellationToken);

            return View(categories);
        }

        #endregion

        #region Active

        /// <summary>
        /// Aktif malzeme kategorilerini listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Active(
            CancellationToken cancellationToken)
        {
            var categories = await _materialCategoryService
                .GetActiveAsync(cancellationToken);

            return View("Index", categories);
        }

        #endregion

        #region Inactive

        /// <summary>
        /// Pasif malzeme kategorilerini listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Inactive(
            CancellationToken cancellationToken)
        {
            var categories = await _materialCategoryService
                .GetInactiveAsync(cancellationToken);

            return View("Index", categories);
        }

        #endregion

        #region Search

        /// <summary>
        /// Malzeme kategorilerini adına göre arar.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(
            string? searchText,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var categories =
                    await _materialCategoryService
                        .SearchAsync(
                            searchText,
                            onlyActive,
                            cancellationToken);

                ViewBag.SearchText = searchText;
                ViewBag.OnlyActive = onlyActive;

                return View("Index", categories);
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
        /// Malzeme kategorisinin detaylarını getirir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var category = await _materialCategoryService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (category == null)
            {
                return NotFound();
            }

            // Kategoriye bağlı organizasyonları getir.
            ViewBag.SelectedUnitIds =
                await _materialCategoryService
                    .GetSelectedUnitIdsAsync(
                        id,
                        cancellationToken);

            // Organizasyon adlarını da detayda
            // görüntüleyebilmek için doldur.
            await LoadOrganizationsAsync(
                cancellationToken);

            return View(category);
        }

        #endregion

        #region Create GET

        /// <summary>
        /// Yeni malzeme kategorisi formunu açar.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(
            CancellationToken cancellationToken)
        {
            await LoadOrganizationsAsync(
                cancellationToken);

            var model = new CreateMaterialCategoryDto
            {
                SelectedUnitIds = new()
            };

            return View(model);
        }

        #endregion

        #region Create POST

        /// <summary>
        /// Yeni malzeme kategorisini ve
        /// organizasyon seçimlerini kaydeder.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateMaterialCategoryDto model,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. MODEL DOĞRULAMASI
            // =====================================

            if (!ModelState.IsValid)
            {
                await LoadOrganizationsAsync(
                    cancellationToken);

                return View(model);
            }

            try
            {
                // =================================
                // 2. KATEGORİ ADI KONTROLÜ
                // =================================

                bool exists = await _materialCategoryService
                    .ExistsNameAsync(
                        model.Name,
                        cancellationToken: cancellationToken);

                if (exists)
                {
                    ModelState.AddModelError(
                        nameof(model.Name),
                        "Bu isimde bir malzeme kategorisi zaten bulunmaktadır.");

                    await LoadOrganizationsAsync(
                        cancellationToken);

                    return View(model);
                }

                // =================================
                // 3. KATEGORİYİ OLUŞTUR
                // =================================

                int categoryId =
                    await _materialCategoryService
                        .CreateAsync(
                            model,
                            cancellationToken);

                // SelectedUnitIds listesi
                // MaterialCategoryService tarafından
                // MaterialCategoryUnit tablosuna kaydedilir.

                // =================================
                // 4. BAŞARI MESAJI
                // =================================

                TempData["SuccessMessage"] =
                    "Malzeme kategorisi başarıyla oluşturuldu.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = categoryId });
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
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Malzeme kategorisi oluşturulurken hata oluştu.");

                ModelState.AddModelError(
                    string.Empty,
                    "Kategori kaydedilirken beklenmeyen bir hata oluştu.");
            }

            // =====================================
            // 5. HATA DURUMUNDA FORMU YÜKLE
            // =====================================

            await LoadOrganizationsAsync(
                cancellationToken);

            return View(model);
        }

        #endregion

        #region Edit GET

        /// <summary>
        /// Mevcut kategorinin güncelleme formunu açar.
        /// Kayıtlı organizasyonlar seçili gelir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(
            int id,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            if (id <= 0)
            {
                return BadRequest();
            }

            // =====================================
            // 2. KATEGORİYİ GETİR
            // =====================================

            var category =
                await _materialCategoryService
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (category == null)
            {
                return NotFound();
            }

            // =====================================
            // 3. UPDATE DTO OLUŞTUR
            // =====================================

            var model = new UpdateMaterialCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            // =====================================
            // 4. KAYITLI ORGANİZASYONLAR
            // =====================================

            model.SelectedUnitIds =
                (await _materialCategoryService
                    .GetSelectedUnitIdsAsync(
                        id,
                        cancellationToken))
                .ToList();

            // =====================================
            // 5. DROPDOWN
            // =====================================

            await LoadOrganizationsAsync(
                cancellationToken);

            return View(model);
        }

        #endregion

        #region Edit POST

        /// <summary>
        /// Kategorinin bilgilerini ve organizasyon
        /// seçimlerini birlikte günceller.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateMaterialCategoryDto model,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. ROUTE / MODEL ID KONTROLÜ
            // =====================================

            if (id <= 0 || id != model.Id)
            {
                return BadRequest();
            }

            // =====================================
            // 2. MODEL DOĞRULAMASI
            // =====================================

            if (!ModelState.IsValid)
            {
                await LoadOrganizationsAsync(
                    cancellationToken);

                return View(model);
            }

            try
            {
                // =================================
                // 3. KATEGORİ MEVCUT MU?
                // =================================

                var currentCategory =
                    await _materialCategoryService
                        .GetByIdAsync(
                            model.Id,
                            cancellationToken);

                if (currentCategory == null)
                {
                    return NotFound();
                }

                // =================================
                // 4. İSİM BENZERSİZLİĞİ
                // =================================

                bool exists = await _materialCategoryService
                    .ExistsNameAsync(
                        model.Name,
                        excludedId: model.Id,
                        cancellationToken: cancellationToken);

                if (exists)
                {
                    ModelState.AddModelError(
                        nameof(model.Name),
                        "Bu isimde başka bir malzeme kategorisi bulunmaktadır.");

                    await LoadOrganizationsAsync(
                        cancellationToken);

                    return View(model);
                }

                // =================================
                // 5. GÜNCELLE
                // =================================

                bool updated =
                    await _materialCategoryService
                        .UpdateAsync(
                            model,
                            cancellationToken);

                if (!updated)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Malzeme kategorisi güncellenemedi.");

                    await LoadOrganizationsAsync(
                        cancellationToken);

                    return View(model);
                }

                // =================================
                // 6. BAŞARI MESAJI
                // =================================

                TempData["SuccessMessage"] =
                    "Malzeme kategorisi başarıyla güncellendi.";

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
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Malzeme kategorisi güncellenirken hata oluştu. Id: {Id}",
                    model.Id);

                ModelState.AddModelError(
                    string.Empty,
                    "Kategori güncellenirken beklenmeyen bir hata oluştu.");
            }

            // =====================================
            // 7. HATA DURUMUNDA FORMU YÜKLE
            // =====================================

            await LoadOrganizationsAsync(
                cancellationToken);

            return View(model);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Kategoriyi aktif veya pasif hale getirir.
        /// Aktif malzemeler mevcutsa pasifleştirme
        /// servis tarafından engellenir.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(
            int id,
            bool isActive,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                bool changed =
                    await _materialCategoryService
                        .ChangeStatusAsync(
                            id,
                            isActive,
                            cancellationToken);

                if (!changed)
                {
                    TempData["ErrorMessage"] =
                        "Malzeme kategorisi bulunamadı veya durumu değiştirilemedi.";
                }
                else
                {
                    TempData["SuccessMessage"] =
                        isActive
                            ? "Malzeme kategorisi aktif hale getirildi."
                            : "Malzeme kategorisi pasif hale getirildi.";
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Malzeme kategorisi durumu değiştirilemedi. Id: {Id}",
                    id);

                TempData["ErrorMessage"] =
                    "Kategori durumu değiştirilirken hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Materials

        /// <summary>
        /// Belirtilen kategoriye bağlı malzemeleri listeler.
        /// </summary>
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

            var category =
                await _materialCategoryService
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (category == null)
            {
                return NotFound();
            }

            var materials = onlyActive
                ? await _materialCategoryService
                    .GetActiveMaterialsByCategoryIdAsync(
                        id,
                        cancellationToken)
                : await _materialCategoryService
                    .GetMaterialsByCategoryIdAsync(
                        id,
                        cancellationToken);

            ViewBag.CategoryId = id;
            ViewBag.CategoryName = category.Name;
            ViewBag.OnlyActive = onlyActive;

            return View(materials);
        }

        #endregion

       

        /// <summary>
        /// Malzeme kategorisi tanıtımı ve
        /// güncelleme ekranları için aktif
        /// organizasyonları listeler.
        /// </summary>
        #region LoadOrganizations

        private async Task LoadOrganizationsAsync(
            CancellationToken cancellationToken = default)
        {
            var units = await _unitService
                .GetActiveAsync(cancellationToken);

            ViewBag.OrganizationNodes = units
                .Select(x => new OrganizationTreeNodeVm
                {
                    Id = x.Id,
                    ParentUnitId = x.ParentUnitId,
                    Name = x.Name
                })
                .ToList();
        }

        #endregion
    }
}