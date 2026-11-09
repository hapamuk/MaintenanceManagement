
using MaintenanceManagement.Application.DTOS.WarehouseDtos;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class WarehouseController : Controller
    {
        #region Fields

        private readonly IWarehouseService
            _warehouseService;

        private readonly IWarehouseRegistrationService
            _warehouseRegistrationService;

        private readonly IUnitService
            _unitService;

        private readonly ILogger<WarehouseController>
            _logger;

        #endregion

        #region Constructor

        public WarehouseController(
            IWarehouseService warehouseService,
            IWarehouseRegistrationService warehouseRegistrationService,
            IUnitService unitService,
            ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;

            _warehouseRegistrationService =
                warehouseRegistrationService;

            _unitService = unitService;

            _logger = logger;
        }

        #endregion

        #region Index

        /// <summary>
        /// Bütün ambarları listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken = default)
        {
            var warehouses = await _warehouseService
                .GetAllAsync(cancellationToken);

            return View(warehouses);
        }

        #endregion

        #region Active

        [HttpGet]
        public async Task<IActionResult> Active(
            CancellationToken cancellationToken = default)
        {
            var warehouses = await _warehouseService
                .GetActiveAsync(cancellationToken);

            ViewBag.CurrentFilter = "Active";

            return View("Index", warehouses);
        }

        #endregion

        #region Inactive

        [HttpGet]
        public async Task<IActionResult> Inactive(
            CancellationToken cancellationToken = default)
        {
            var warehouses = await _warehouseService
                .GetInactiveAsync(cancellationToken);

            ViewBag.CurrentFilter = "Inactive";

            return View("Index", warehouses);
        }

        #endregion

        #region Search

        /// <summary>
        /// Ambar koduna veya adına göre arama yapar.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(
            string? searchText,
            int? unitId = null,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var warehouses = await _warehouseService
                    .SearchAsync(
                        searchText,
                        unitId,
                        onlyActive,
                        cancellationToken);

                ViewBag.SearchText = searchText;
                ViewBag.UnitId = unitId;
                ViewBag.OnlyActive = onlyActive;
                ViewBag.CurrentFilter = "Search";

                return View("Index", warehouses);
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

            var warehouse = await _warehouseService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (warehouse == null)
            {
                return NotFound();
            }

            var assignedIds = await _warehouseRegistrationService
                .GetAssignedPersonnelIdsAsync(id, cancellationToken);
            ViewBag.AssignedPersonnelIds = assignedIds;

            if (warehouse.UnitId.HasValue &&
                await IsSelectableWarehouseUnitAsync(warehouse.UnitId, cancellationToken))
            {
                var candidates = await _warehouseRegistrationService
                    .GetEligiblePersonnelAsync(warehouse.UnitId.Value, cancellationToken);
                var unitRows = await _unitService.GetActiveAsync(cancellationToken);
                var unitNames = unitRows.ToDictionary(x => x.Id, x => x.Name);
                ViewBag.AssignedPersonnelNames = candidates
                    .Where(x => assignedIds.Contains(x.UserId))
                    .ToDictionary(x => x.UserId, x => x.FullName + " — " +
                        (unitNames.TryGetValue(x.UnitId, out var unitName)
                            ? unitName : "Birim adı bulunamadı"));
            }

            return View(warehouse);
        }

        #endregion

        #region Create GET

        /// <summary>
        /// Yeni ambar oluşturma ekranını açar.
        /// Ambar kodu otomatik üretilir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(
            CancellationToken cancellationToken = default)
        {
            await LoadOrganizationsAsync(cancellationToken);

            string warehouseCode =
                await GenerateUniqueWarehouseCodeAsync(
                    cancellationToken);

            var model = new CreateWarehouseDto
            {
                Code = warehouseCode,
                IsActive = false,

                SelectedPersonnelIds = new List<int>()
            };

            return View(model);
        }

        #endregion

        #region Create POST

        /// <summary>
        /// Yeni ambar ve görevli personelleri oluşturur.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateWarehouseDto model,
            CancellationToken cancellationToken = default)
        {
            model.SelectedPersonnelIds ??= new List<int>();

            // Ambarın bağlı olduğu tek bir organizasyon seçilir.
            if (!model.UnitId.HasValue || model.UnitId.Value <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.UnitId),
                    "Ambar için bir organizasyon seçiniz.");
            }

            if (model.UnitId.HasValue && model.UnitId.Value > 0 &&
                !await IsSelectableWarehouseUnitAsync(
                    model.UnitId, cancellationToken))
            {
                ModelState.AddModelError(
                    nameof(model.UnitId),
                    "Ambar yalnızca Genel Müdürlük veya Daire Başkanlığına tanımlanabilir.");
            }

            if (!ModelState.IsValid)
            {
                await PrepareCreateViewAsync(
                    model,
                    cancellationToken);

                return View(model);
            }

            try
            {
                // =====================================
                // 1. OTOMATİK KOD DOĞRULAMASI
                // =====================================

                bool validCode = IsValidWarehouseCode(model.Code);

                bool codeExists = false;

                if (validCode)
                {
                    codeExists = await _warehouseService
                        .ExistsCodeAsync(
                            model.Code,
                            cancellationToken: cancellationToken);
                }

                if (!validCode || codeExists)
                {
                    model.Code =
                        await GenerateUniqueWarehouseCodeAsync(
                            cancellationToken);
                }

                // =====================================
                // 2. SEÇİLEN PERSONELLER
                // =====================================

                model.SelectedPersonnelIds =
                    model.SelectedPersonnelIds
                        .Where(x => x > 0)
                        .Distinct()
                        .ToList();

                // =====================================
                // 3. AMBAR + PERSONEL KAYDI
                // =====================================

                int warehouseId =
                    await _warehouseRegistrationService
                        .CreateAsync(
                            model,
                            cancellationToken);

                // =====================================
                // 4. BAŞARI MESAJI
                // =====================================

                TempData["SuccessMessage"] =
                    "Ambar ve görevli personeller başarıyla kaydedildi.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = warehouseId });
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
                    "Ambar oluşturulurken hata oluştu.");

                ModelState.AddModelError(
                    string.Empty,
                    "Ambar kaydedilirken beklenmeyen bir hata oluştu.");
            }

            await PrepareCreateViewAsync(
                model,
                cancellationToken);

            return View(model);
        }

        #endregion

        #region Edit GET

        /// <summary>
        /// Ambar güncelleme ekranını açar.
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

            var warehouse = await _warehouseService
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (warehouse == null)
            {
                return NotFound();
            }

            var personnelIds =
                await _warehouseRegistrationService
                    .GetAssignedPersonnelIdsAsync(
                        id,
                        cancellationToken);

            var model = new UpdateWarehouseDto
            {
                Id = warehouse.Id,
                Code = warehouse.Code,
                Name = warehouse.Name,
                UnitId = warehouse.UnitId,
                Location = warehouse.Location,
                Description = warehouse.Description,

                SelectedPersonnelIds = personnelIds.ToList()
            };

            await PrepareEditViewAsync(
                model,
                cancellationToken);

            return View(model);
        }

        #endregion

        #region Edit POST

        /// <summary>
        /// Ambar bilgilerini ve görevli personelleri
        /// günceller. Ambar kodu değiştirilemez.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            UpdateWarehouseDto model,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0 || id != model.Id)
            {
                return BadRequest();
            }

            model.SelectedPersonnelIds ??= new List<int>();

            try
            {
                // =====================================
                // 1. MEVCUT AMBARI GETİR
                // =====================================

                var currentWarehouse = await _warehouseService
                    .GetByIdAsync(
                        id,
                        cancellationToken);

                if (currentWarehouse == null)
                {
                    return NotFound();
                }

                // =====================================
                // 2. AMBAR KODUNU KORU
                // =====================================

                // Kullanıcıdan gelen Code değeri kullanılmaz.
                model.Code = currentWarehouse.Code;

                // Kod, ModelState içerisinde önceden
                // oluşmuşsa mevcut değerle tekrar doğrula.
                ModelState.Remove(nameof(model.Code));

                if (!TryValidateModel(
                    model,
                    prefix: string.Empty))
                {
                    await PrepareEditViewAsync(
                        model,
                        cancellationToken);

                    return View(model);
                }

                // =====================================
                // 3. ORGANİZASYON DOĞRULAMASI
                // =====================================

                if (!model.UnitId.HasValue ||
                    model.UnitId.Value <= 0)
                {
                    ModelState.AddModelError(
                        nameof(model.UnitId),
                        "Ambar için bir organizasyon seçiniz.");
                }

                if (model.UnitId.HasValue && model.UnitId.Value > 0 &&
                    !await IsSelectableWarehouseUnitAsync(
                        model.UnitId, cancellationToken))
                {
                    ModelState.AddModelError(
                        nameof(model.UnitId),
                        "Ambar yalnızca Genel Müdürlük veya Daire Başkanlığına tanımlanabilir.");
                }

                if (!ModelState.IsValid)
                {
                    await PrepareEditViewAsync(
                        model,
                        cancellationToken);

                    return View(model);
                }

                // =====================================
                // 4. PERSONEL LİSTESİNİ NORMALİZE ET
                // =====================================

                model.SelectedPersonnelIds =
                    model.SelectedPersonnelIds
                        .Where(x => x > 0)
                        .Distinct()
                        .ToList();

                // =====================================
                // 5. AMBAR VE PERSONELİ GÜNCELLE
                // =====================================

                bool updated =
                    await _warehouseRegistrationService
                        .UpdateAsync(
                            model,
                            cancellationToken);

                if (!updated)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Ambar güncellenemedi.");

                    await PrepareEditViewAsync(
                        model,
                        cancellationToken);

                    return View(model);
                }

                TempData["SuccessMessage"] =
                    "Ambar ve görevli personeller güncellendi.";

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
                    "Ambar güncellenirken hata oluştu. Id: {Id}",
                    model.Id);

                ModelState.AddModelError(
                    string.Empty,
                    "Ambar güncellenirken beklenmeyen bir hata oluştu.");
            }

            await PrepareEditViewAsync(
                model,
                cancellationToken);

            return View(model);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Ambarı aktif veya pasif hale getirir.
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
                bool changed = await _warehouseService
                    .ChangeStatusAsync(
                        id,
                        isActive,
                        cancellationToken);

                TempData[changed
                    ? "SuccessMessage"
                    : "ErrorMessage"] = changed
                    ? (isActive
                        ? "Ambar aktif hale getirildi."
                        : "Ambar pasif hale getirildi.")
                    : "Ambarın durumu değiştirilemedi.";
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
                    "Ambar durumu değiştirilirken hata oluştu. Id: {Id}",
                    id);

                TempData["ErrorMessage"] =
                    "Ambar durumu değiştirilirken hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Stocks

        /// <summary>
        /// Ambardaki bütün stok kayıtlarını görüntüler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Stocks(
            int id,
            bool onlyPositive = false,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var warehouse = await _warehouseService
                .GetByIdAsync(id, cancellationToken);

            if (warehouse == null)
            {
                return NotFound();
            }

            var stocks = onlyPositive
                ? await _warehouseService
                    .GetPositiveStocksByWarehouseIdAsync(
                        id,
                        cancellationToken)
                : await _warehouseService
                    .GetStocksByWarehouseIdAsync(
                        id,
                        cancellationToken);

            ViewBag.WarehouseId = id;
            ViewBag.WarehouseName = warehouse.Name;
            ViewBag.OnlyPositive = onlyPositive;

            return View(stocks);
        }

        #endregion

        #region Materials

        /// <summary>
        /// Pozitif stoğu olan malzemeleri listeler.
        /// Yönetim ekranıdır; iş emri yetki kontrolü
        /// servis katmanında ayrıca uygulanmalıdır.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Materials(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var warehouse = await _warehouseService
                .GetByIdAsync(id, cancellationToken);

            if (warehouse == null)
            {
                return NotFound();
            }

            var materials = await _warehouseService
                .GetMaterialsInStockAsync(
                    id,
                    cancellationToken);

            ViewBag.WarehouseId = id;
            ViewBag.WarehouseName = warehouse.Name;

            return View(materials);
        }

        #endregion

        #region Movements

        /// <summary>
        /// Ambara ait stok hareketlerini listeler.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Movements(
            int id,
            int take = 100,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            if (take < 1 || take > 1000)
            {
                return BadRequest();
            }

            var warehouse = await _warehouseService
                .GetByIdAsync(id, cancellationToken);

            if (warehouse == null)
            {
                return NotFound();
            }

            var movements = await _warehouseService
                .GetStockMovementsByWarehouseIdAsync(
                    id,
                    take,
                    cancellationToken);

            ViewBag.WarehouseId = id;
            ViewBag.WarehouseName = warehouse.Name;

            return View(movements);
        }

        #endregion

        #region GetPersonnelByUnit

        /// <summary>
        /// Yalnızca Genel Müdürlük veya Daire Başkanlığı seçilirse
        /// ilgili birim ve aktif alt birimlerinin personel adaylarını getirir.
        /// Alt birim kapsamı WarehouseRegistrationService tarafından sağlanır.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPersonnelByUnit(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            if (!await IsSelectableWarehouseUnitAsync(
                    unitId, cancellationToken))
            {
                return BadRequest(new
                {
                    message = "Personel seçimi için Genel Müdürlük veya Daire Başkanlığı seçiniz."
                });
            }

            try
            {
                var personnel = await _warehouseRegistrationService
                    .GetEligiblePersonnelAsync(
                        unitId,
                        cancellationToken);

                // Personelin bağlı olduğu alt birimin adını da gönder.
                // Böylece Razor kartlarında "Birim #2" gibi ID gösterilmez.
                var units = await _unitService.GetActiveAsync(cancellationToken);
                var unitNames = units.ToDictionary(x => x.Id, x => x.Name);

                return Json(personnel.Select(x => new
                {
                    id = x.UserId,
                    name = x.FullName,
                    unitId = x.UnitId,
                    unitName = unitNames.TryGetValue(x.UnitId, out var unitName)
                        ? unitName
                        : "Birim adı bulunamadı"
                }));
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion

        #region GenerateUniqueWarehouseCode

        /// <summary>
        /// Sistemde kullanılmayan 6 haneli
        /// rastgele ambar kodu önerisi oluşturur.
        /// </summary>
        private async Task<string>
            GenerateUniqueWarehouseCodeAsync(
                CancellationToken cancellationToken = default)
        {
            const int maxAttempts = 100;

            for (int attempt = 0;
                 attempt < maxAttempts;
                 attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int randomNumber =
                    RandomNumberGenerator.GetInt32(
                        100000,
                        1000000);

                string code = randomNumber.ToString(
                    System.Globalization.CultureInfo.InvariantCulture);

                bool exists = await _warehouseService
                    .ExistsCodeAsync(
                        code,
                        cancellationToken: cancellationToken);

                if (!exists)
                {
                    return code;
                }
            }

            throw new InvalidOperationException(
                "Benzersiz ambar kodu oluşturulamadı.");
        }

        #endregion

        #region IsValidWarehouseCode

        private static bool IsValidWarehouseCode(string? code)
        {
            return code != null &&
                   code.Length == 6 &&
                   code[0] >= '1' &&
                   code[0] <= '9' &&
                   code.All(x => x >= '0' && x <= '9');
        }

        #endregion

        #region IsWarehouseOrganizationName

        /// <summary>
        /// Şimdilik mevcut isimlendirme standardına göre ambar açılabilecek
        /// birimleri belirler. Kalıcı çözüm OrganizationType üzerinden olmalıdır.
        /// </summary>
        private static bool IsWarehouseOrganizationName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string normalizedName = name.Trim();

            return normalizedName.Equals(
                       "Genel Müdürlük",
                       StringComparison.OrdinalIgnoreCase)
                   || normalizedName.EndsWith(
                       "Dairesi Başkanlığı",
                       StringComparison.OrdinalIgnoreCase)
                   || normalizedName.EndsWith(
                       "Daire Başkanlığı",
                       StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region IsSelectableWarehouseUnit

        /// <summary>
        /// Ambar tanıtımında seçilebilecek aktif birimi kontrol eder.
        /// Kullanıcı yalnızca istemci tarafındaki filtreye güvenemez.
        /// </summary>
        private async Task<bool> IsSelectableWarehouseUnitAsync(
            int? unitId,
            CancellationToken cancellationToken = default)
        {
            if (!unitId.HasValue || unitId.Value <= 0)
            {
                return false;
            }

            var units = await _unitService
                .GetActiveAsync(cancellationToken);

            return units.Any(x =>
                x.Id == unitId.Value &&
                IsWarehouseOrganizationName(x.Name));
        }

        #endregion

        #region LoadOrganizations

        /// <summary>
        /// Ambar sahibi olabilecek aktif Genel Müdürlük ve Daire Başkanlıkları.
        /// ParentUnitId, yalnızca seçilebilir üst ataya bağlanarak korunur.
        /// Müdürlük/şeflik ambar sahibi olarak listelenmez.
        /// </summary>
        private async Task LoadOrganizationsAsync(
            CancellationToken cancellationToken = default)
        {
            var allUnits = (await _unitService
                .GetActiveAsync(cancellationToken)).ToList();

            var selectable = allUnits
                .Where(x => IsWarehouseOrganizationName(x.Name))
                .ToList();

            var selectableIds = selectable
                .Select(x => x.Id)
                .ToHashSet();

            var byId = allUnits.ToDictionary(x => x.Id);

            int? FindSelectableParent(int id)
            {
                var visited = new HashSet<int> { id };
                var parentId = byId[id].ParentUnitId;

                while (parentId.HasValue &&
                       visited.Add(parentId.Value) &&
                       byId.TryGetValue(parentId.Value, out var parent))
                {
                    if (selectableIds.Contains(parent.Id))
                        return parent.Id;

                    parentId = parent.ParentUnitId;
                }

                return null;
            }

            ViewBag.OrganizationNodes = selectable
                .Select(x => new OrganizationTreeNodeVm
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentUnitId = FindSelectableParent(x.Id)
                })
                .OrderBy(x => x.ParentUnitId.HasValue ? 1 : 0)
                .ThenBy(x => x.Name)
                .ToList();
        }

        #endregion

        #region PrepareCreateView

        private async Task PrepareCreateViewAsync(
            CreateWarehouseDto model,
            CancellationToken cancellationToken = default)
        {
            await LoadOrganizationsAsync(cancellationToken);

            if (await IsSelectableWarehouseUnitAsync(
                    model.UnitId, cancellationToken))
            {
                ViewBag.EligiblePersonnel =
                    await _warehouseRegistrationService
                        .GetEligiblePersonnelAsync(
                            model.UnitId!.Value,
                            cancellationToken);
            }
            else
            {
                ViewBag.EligiblePersonnel =
                    Array.Empty<WarehousePersonnelCandidateDto>();
            }
        }

        #endregion

        #region PrepareEditView

        private async Task PrepareEditViewAsync(
            UpdateWarehouseDto model,
            CancellationToken cancellationToken = default)
        {
            await LoadOrganizationsAsync(cancellationToken);

            if (await IsSelectableWarehouseUnitAsync(
                    model.UnitId, cancellationToken))
            {
                ViewBag.EligiblePersonnel =
                    await _warehouseRegistrationService
                        .GetEligiblePersonnelAsync(
                            model.UnitId!.Value,
                            cancellationToken);
            }
            else
            {
                ViewBag.EligiblePersonnel =
                    Array.Empty<WarehousePersonnelCandidateDto>();
            }
        }

        #endregion
    }
}
