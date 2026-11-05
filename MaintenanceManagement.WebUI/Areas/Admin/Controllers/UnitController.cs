using MaintenanceManagement.Application.DTOS.UnitDtos;
using MaintenanceManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Policy = "AdminOnly")]
    public class UnitController : Controller
    {
        #region Fields and Constructor

        private readonly IUnitService _unitService;

        private readonly IOrganizationTypeService
            _organizationTypeService;

        private readonly ILogger<UnitController> _logger;

        public UnitController(
            IUnitService unitService,
            IOrganizationTypeService organizationTypeService,
            ILogger<UnitController> logger)
        {
            _unitService = unitService;

            _organizationTypeService =
                organizationTypeService;

            _logger = logger;
        }

        #endregion

        #region Index

        /// <summary>
        /// Aktif ve pasif bütün organizasyon
        /// birimlerini listeler.
        ///
        /// ParentUnitId ilişkisi kullanılarak
        /// View üzerinde hiyerarşik gösterim
        /// oluşturulabilir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var units = await _unitService
                .GetAllAsync(cancellationToken);

            return View(units);
        }

        #endregion

        #region Details

        /// <summary>
        /// Birimin detay bilgilerini getirir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest();

            var unit = await _unitService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (unit == null)
                return NotFound();

            return View(unit);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni organizasyon birimi oluşturma formu.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(
            CancellationToken cancellationToken)
        {
            await LoadOrganizationTypesAsync(
                null,
                cancellationToken);

            await LoadParentUnitsAsync(
                null,
                cancellationToken);

            return View(new CreateUnitDto());
        }

        /// <summary>
        /// Yeni organizasyon birimini oluşturur.
        ///
        /// Birim kodu kullanıcıdan alınmaz.
        /// Kayıt sırasında otomatik atanır.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateUnitDto model,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. OTOMATİK KOD ALANI
            // =====================================

            // Code sunucuda üretileceği için
            // istemciden gelen değer kullanılmaz.

            model.Code = null;

            // Code alanı kullanıcı tarafından
            // doldurulmadığından ModelState
            // doğrulamasına dahil edilmez.

            ModelState.Remove(nameof(CreateUnitDto.Code));

            // =====================================
            // 2. ORGANİZASYON TÜRÜ KONTROLÜ
            // =====================================

            if (model.OrganizationTypeId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.OrganizationTypeId),
                    "Organizasyon türü seçiniz.");
            }

            // =====================================
            // 3. ÜST BİRİM KONTROLÜ
            // =====================================

            if (model.ParentUnitId.HasValue &&
                model.ParentUnitId.Value <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.ParentUnitId),
                    "Geçerli bir üst birim seçiniz.");
            }

            // =====================================
            // 4. MODEL DOĞRULAMASI
            // =====================================

            if (!ModelState.IsValid)
            {
                await LoadOrganizationTypesAsync(
                    model.OrganizationTypeId,
                    cancellationToken);

                await LoadParentUnitsAsync(
                    model.ParentUnitId,
                    cancellationToken);

                return View(model);
            }

            // =====================================
            // 5. BİRİM OLUŞTURMA
            // =====================================

            try
            {
                int unitId = await _unitService
                    .CreateAsync(
                        model,
                        cancellationToken);

                TempData["Success"] =
                    "Organizasyon birimi başarıyla oluşturuldu.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = unitId });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    ex.Message);
            }
            catch (ArgumentException ex)
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
                    "Organizasyon birimi oluşturulurken hata oluştu.");

                ModelState.AddModelError(
                    string.Empty,
                    "Organizasyon birimi oluşturulamadı.");
            }

            // =====================================
            // 6. FORMU YENİDEN YÜKLE
            // =====================================

            await LoadOrganizationTypesAsync(
                model.OrganizationTypeId,
                cancellationToken);

            await LoadParentUnitsAsync(
                model.ParentUnitId,
                cancellationToken);

            return View(model);
        }

        #endregion

        #region Edit

        /// <summary>
        /// Organizasyon biriminin
        /// güncelleme formunu açar.
        ///
        /// Mevcut üst birim seçili gelir.
        /// Birimin kendisi veya alt birimleri
        /// üst birim olarak seçilemez.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest();

            // =====================================
            // 1. BİRİMİ GETİR
            // =====================================

            var unit = await _unitService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (unit == null)
                return NotFound();

            // =====================================
            // 2. UPDATE DTO OLUŞTUR
            // =====================================

            var model = new UpdateUnitDto
            {
                Id = unit.Id,

                Name = unit.Name,

                Code = unit.Code,

                Description = unit.Description,

                OrganizationTypeId =
                    unit.OrganizationTypeId,

                ParentUnitId =
                    unit.ParentUnitId
            };

            // =====================================
            // 3. ORGANİZASYON TÜRLERİNİ YÜKLE
            // =====================================

            await LoadOrganizationTypesAsync(
                model.OrganizationTypeId,
                cancellationToken);

            // =====================================
            // 4. HİYERARŞİK ÜST BİRİMLERİ YÜKLE
            // =====================================

            await LoadParentUnitsAsync(
                model.ParentUnitId,
                cancellationToken,
                model.Id);

            return View(model);
        }

        /// <summary>
        /// Organizasyon birimini günceller.
        ///
        /// Üst birim değiştirilirse
        /// ParentUnitId güncellenir.
        ///
        /// Birim kodu değiştirilemez.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateUnitDto model,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. ID KONTROLÜ
            // =====================================

            if (id <= 0 || id != model.Id)
                return BadRequest();

            // =====================================
            // 2. ORGANİZASYON TÜRÜ KONTROLÜ
            // =====================================

            if (model.OrganizationTypeId <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.OrganizationTypeId),
                    "Organizasyon türü seçiniz.");
            }

            // =====================================
            // 3. ÜST BİRİM KONTROLÜ
            // =====================================

            if (model.ParentUnitId.HasValue)
            {
                if (model.ParentUnitId.Value <= 0)
                {
                    ModelState.AddModelError(
                        nameof(model.ParentUnitId),
                        "Geçerli bir üst birim seçiniz.");
                }

                if (model.ParentUnitId.Value == model.Id)
                {
                    ModelState.AddModelError(
                        nameof(model.ParentUnitId),
                        "Bir organizasyon birimi " +
                        "kendisini üst birim olarak seçemez.");
                }
            }

            // =====================================
            // 4. MODEL DOĞRULAMASI
            // =====================================

            if (!ModelState.IsValid)
            {
                await LoadOrganizationTypesAsync(
                    model.OrganizationTypeId,
                    cancellationToken);

                await LoadParentUnitsAsync(
                    model.ParentUnitId,
                    cancellationToken,
                    model.Id);

                return View(model);
            }

            // =====================================
            // 5. BİRİM GÜNCELLE
            // =====================================

            try
            {
                // Birim kodunu değiştirme işlemi
                // UnitService.UpdateAsync içerisinde
                // engellenmektedir.
                //
                // Üst birim hiyerarşisinin nihai
                // doğrulaması da serviste yapılır.

                bool updated = await _unitService
                    .UpdateAsync(
                        model,
                        cancellationToken);

                if (!updated)
                    return NotFound();

                TempData["Success"] =
                    "Organizasyon birimi başarıyla güncellendi.";

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
            catch (ArgumentException ex)
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
                    "Organizasyon birimi güncellenirken hata oluştu.");

                ModelState.AddModelError(
                    string.Empty,
                    "Organizasyon birimi güncellenemedi.");
            }

            // =====================================
            // 6. HATA DURUMUNDA FORMU YÜKLE
            // =====================================

            await LoadOrganizationTypesAsync(
                model.OrganizationTypeId,
                cancellationToken);

            await LoadParentUnitsAsync(
                model.ParentUnitId,
                cancellationToken,
                model.Id);

            return View(model);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Organizasyon biriminin aktiflik
        /// durumunu değiştirir.
        ///
        /// Birim kodu değiştirilmez.
        /// </summary>
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
                bool result = await _unitService
                    .ChangeStatusAsync(
                        id,
                        isActive,
                        cancellationToken);

                if (!result)
                {
                    TempData["Error"] =
                        "Organizasyon birimi bulunamadı.";
                }
                else
                {
                    TempData["Success"] = isActive
                        ? "Organizasyon birimi aktifleştirildi."
                        : "Organizasyon birimi pasifleştirildi.";
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
                    "Birim durumu değiştirilemedi. Id: {Id}",
                    id);

                TempData["Error"] =
                    "Birim durumu değiştirilirken hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Private Methods

        #region Load Organization Types

        /// <summary>
        /// Organizasyon türlerini Create ve Edit
        /// ekranları için SelectList olarak hazırlar.
        /// </summary>
        private async Task LoadOrganizationTypesAsync(
            int? selectedId,
            CancellationToken cancellationToken)
        {
            var organizationTypes =
                await _organizationTypeService
                    .GetAllAsync(cancellationToken);

            ViewBag.OrganizationTypes =
                new SelectList(
                    organizationTypes
                        .Where(x => x.IsActive ||
                                    x.Id == selectedId)
                        .OrderBy(x => x.Name),
                    "Id",
                    "Name",
                    selectedId);
        }

        #endregion

        #region Load Parent Units

        /// <summary>
        /// Bütün organizasyon birimlerini
        /// üst-alt birim hiyerarşisine göre
        /// sıralayarak SelectListItem listesi
        /// oluşturur.
        ///
        /// Pasif birimler görüntülenir fakat
        /// yeni üst birim olarak seçilemez.
        ///
        /// Güncellenen birim kendisini veya
        /// kendi alt birimlerini üst birim
        /// olarak seçemez.
        /// </summary>
        private async Task LoadParentUnitsAsync(
            int? selectedId,
            CancellationToken cancellationToken,
            int? currentUnitId = null)
        {
            // =====================================
            // 1. BÜTÜN BİRİMLERİ GETİR
            // =====================================

            var allUnits = await _unitService
                .GetAllAsync(cancellationToken);

            var units = allUnits.ToList();

            // =====================================
            // 2. SEÇİLEMEYECEK BİRİMLER
            // =====================================

            var excludedIds = new HashSet<int>();

            // Edit sırasında mevcut birim ve
            // bütün alt birimleri seçilemez.

            if (currentUnitId.HasValue)
            {
                var pending = new Stack<int>();

                pending.Push(currentUnitId.Value);

                while (pending.Count > 0)
                {
                    int currentId = pending.Pop();

                    if (!excludedIds.Add(currentId))
                        continue;

                    var children = units
                        .Where(x => x.ParentUnitId == currentId);

                    foreach (var child in children)
                    {
                        pending.Push(child.Id);
                    }
                }
            }

            // =====================================
            // 3. HİYERARŞİK LİSTE HAZIRLA
            // =====================================

            var items = new List<SelectListItem>();

            var visitedIds = new HashSet<int>();

            // =====================================
            // 4. RECURSIVE TREE METHOD
            // =====================================

            void AddChildren(
                int? parentId,
                int depth)
            {
                var children = units
                    .Where(x => x.ParentUnitId == parentId)
                    .OrderBy(x => x.Name)
                    .ToList();

                foreach (var child in children)
                {
                    // Döngü veya mükerrer
                    // gösterimi önle.
                    if (!visitedIds.Add(child.Id))
                        continue;

                    // =================================
                    // HİYERARŞİ GÖRÜNÜMÜ
                    // =================================

                    string prefix =
                        string.Concat(
                            Enumerable.Repeat(
                                "— ",
                                depth));

                    string text = prefix + child.Name;

                    // Pasif durumunu belirt.
                    if (!child.IsActive)
                    {
                        text += " (Pasif)";
                    }

                    // =================================
                    // SEÇİLEBİLİRLİK
                    // =================================

                    bool isExcluded =
                        excludedIds.Contains(child.Id);

                    bool isDisabled =
                        !child.IsActive || isExcluded;

                    items.Add(new SelectListItem
                    {
                        Value = child.Id.ToString(),

                        Text = text,

                        Selected =
                            child.Id == selectedId,

                        Disabled = isDisabled
                    });

                    // =================================
                    // ALT BİRİMLERİ EKLE
                    // =================================

                    AddChildren(
                        child.Id,
                        depth + 1);
                }
            }

            // =====================================
            // 5. KÖK BİRİMLERDEN BAŞLA
            // =====================================

            AddChildren(null, 0);

            // =====================================
            // 6. HİYERARŞİDE ERİŞİLEMEYEN KAYITLAR
            // =====================================

            // Veri tutarsızlığı veya hatalı
            // üst birim ilişkisi varsa listede
            // görünür fakat seçilemez.

            foreach (var unit in units
                .Where(x => !visitedIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ToList())
            {
                if (!visitedIds.Add(unit.Id))
                    continue;

                items.Add(new SelectListItem
                {
                    Value = unit.Id.ToString(),

                    Text = "⚠ " + unit.Name +
                           " (Hiyerarşi kontrol edilmeli)",

                    Selected =
                        unit.Id == selectedId,

                    Disabled = true
                });
            }

            // =====================================
            // 7. VIEWBAG
            // =====================================

            ViewBag.ParentUnits = items;
        }

        #endregion

        #endregion
    }
}
