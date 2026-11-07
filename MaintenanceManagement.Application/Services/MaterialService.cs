using AutoMapper;
using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.DTOS.WarehouseStockDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class MaterialService : IMaterialService
    {
        #region Fields

        private readonly IMaterialRepository
            _materialRepository;

        private readonly IMeasurementUnitRepository
            _measurementUnitRepository;

        private readonly IMaterialCategoryRepository
            _materialCategoryRepository;

        private readonly IUnitRepository
            _unitRepository;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public MaterialService(
            IMaterialRepository materialRepository,
            IMeasurementUnitRepository measurementUnitRepository,
            IMaterialCategoryRepository materialCategoryRepository,
            IUnitRepository unitRepository,
            IMapper mapper)
        {
            _materialRepository =
                materialRepository;

            _measurementUnitRepository =
                measurementUnitRepository;

            _materialCategoryRepository =
                materialCategoryRepository;

            _unitRepository =
                unitRepository;

            _mapper = mapper;
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün malzemeleri ilişkileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.MaterialCode));
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID değerine göre malzeme detayını getirir.
        /// </summary>
        public async Task<GetByIdMaterialDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var material = await _materialRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (material == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdMaterialDto>(
                material);
        }

        #endregion

        #region GetByMaterialCode

        /// <summary>
        /// Malzeme koduna göre malzeme detayını getirir.
        /// </summary>
        public async Task<GetByIdMaterialDto?>
            GetByMaterialCodeAsync(
                string materialCode,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. MALZEME KODUNU DOĞRULA
            // =====================================

            string normalizedCode =
                ValidateRequiredText(
                    materialCode,
                    "Malzeme Kodu");

            // =====================================
            // 2. REPOSITORY SORGUSU
            // =====================================

            var material = await _materialRepository
                .GetByMaterialCodeAsync(
                    normalizedCode,
                    cancellationToken);

            if (material == null)
            {
                return null;
            }

            // =====================================
            // 3. DETAYLI DTO DÖNÜŞÜMÜ
            // =====================================

            return await GetByIdAsync(
                material.Id,
                cancellationToken);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var activeMaterials = await _materialRepository
                .GetActiveMaterialsAsync(
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                activeMaterials,
                cancellationToken);
        }

        #endregion

        #region GetInactive

        /// <summary>
        /// Pasif malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetInactiveAsync(
                CancellationToken cancellationToken = default)
        {
            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .Where(x => !x.IsActive)
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.MaterialCode));
        }

        #endregion

        #region GetByCategoryId

        /// <summary>
        /// Belirli kategoriye ait malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .Where(x =>
                        x.MaterialCategoryId == categoryId)
                    .OrderBy(x => x.Name));
        }

        #endregion

        #region GetByMeasurementUnitId

        /// <summary>
        /// Ölçü birimine göre malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetByMeasurementUnitIdAsync(
                int measurementUnitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .Where(x =>
                        x.MeasurementUnitId == measurementUnitId)
                    .OrderBy(x => x.Name));
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Organizasyon birimine göre
        /// malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Organizasyon Birimi");

            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .Where(x => x.UnitId == unitId)
                    .OrderBy(x => x.Name));
        }

        #endregion

        #region GetByCategoryAndUnitId

        /// <summary>
        /// Kategori ve organizasyon birimine
        /// göre malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetByCategoryAndUnitIdAsync(
                int categoryId,
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            ValidateId(
                unitId,
                "Organizasyon Birimi");

            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .Where(x =>
                        x.MaterialCategoryId == categoryId &&
                        x.UnitId == unitId)
                    .OrderBy(x => x.Name));
        }

        #endregion

        #region GetByMeasurementUnitAndCategoryId

        /// <summary>
        /// Ölçü birimi ve kategoriye göre
        /// malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetByMeasurementUnitAndCategoryIdAsync(
                int measurementUnitId,
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .Where(x =>
                        x.MeasurementUnitId == measurementUnitId &&
                        x.MaterialCategoryId == categoryId)
                    .OrderBy(x => x.Name));
        }

        #endregion

        #region GetWithoutCategory

        /// <summary>
        /// Kategori atanmamış malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetWithoutCategoryAsync(
                CancellationToken cancellationToken = default)
        {
            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .Where(x =>
                        !x.MaterialCategoryId.HasValue)
                    .OrderBy(x => x.Name));
        }

        #endregion

        #region GetWithoutUnit

        /// <summary>
        /// Organizasyon birimi atanmamış
        /// malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetWithoutUnitAsync(
                CancellationToken cancellationToken = default)
        {
            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                materials
                    .Where(x =>
                        !x.UnitId.HasValue)
                    .OrderBy(x => x.Name));
        }

        #endregion

        #region Search

        /// <summary>
        /// Malzeme kodu veya adına göre arama yapar.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            SearchAsync(
                string searchText,
                bool onlyActive = false,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ARAMA METNİ
            // =====================================

            string normalizedSearchText =
                ValidateRequiredText(
                    searchText,
                    "Arama Metni");

            // =====================================
            // 2. MALZEMELERİ GETİR
            // =====================================

            var materials = await GetAllEntitiesAsync(
                cancellationToken);

            // =====================================
            // 3. ARAMA VE AKTİFLİK FİLTRESİ
            // =====================================

            var filteredMaterials = materials
                .Where(x =>
                    (!onlyActive || x.IsActive) &&
                    (
                        (x.MaterialCode != null &&
                         x.MaterialCode.Contains(
                             normalizedSearchText,
                             StringComparison.CurrentCultureIgnoreCase))
                        ||
                        (x.Name != null &&
                         x.Name.Contains(
                             normalizedSearchText,
                             StringComparison.CurrentCultureIgnoreCase))
                    ))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToList();

            // =====================================
            // 4. DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(filteredMaterials);
        }

        #endregion

        #region GetWarehouseStocksByMaterialId

        /// <summary>
        /// Malzemenin ambar stok kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetWarehouseStocksByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. MALZEME ID
            // =====================================

            ValidateId(
                materialId,
                "Malzeme");

            // =====================================
            // 2. AMBAR STOKLARINI GETİR
            // =====================================

            var warehouseStocks =
                await _materialRepository
                    .GetWarehouseStocksByMaterialIdAsync(
                        materialId,
                        cancellationToken);

            // =====================================
            // 3. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<List<ResultWarehouseStockDto>>(
                warehouseStocks);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni malzeme oluşturur.
        ///
        /// Malzeme kodu benzersiz olmalıdır.
        /// Ölçü birimi zorunludur.
        /// Kategori ve organizasyon birimi opsiyoneldir.
        /// </summary>
        public async Task<int> CreateAsync(
    CreateMaterialDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. MALZEME KODUNU KONTROL ET
            // =====================================

            string requestedCode =
                dto.MaterialCode?.Trim() ?? string.Empty;

            bool validFormat =
                requestedCode.Length == 6 &&
                requestedCode.All(c => c >= '0' && c <= '9') &&
                requestedCode[0] != '0';

            bool codeExists = false;

            if (validFormat)
            {
                codeExists = await _materialRepository
                    .ExistsMaterialCodeAsync(
                        requestedCode,
                        null,
                        cancellationToken);
            }

            string materialCode;

            if (!validFormat || codeExists)
            {
                materialCode = await GenerateUniqueMaterialCodeAsync(
                    cancellationToken);
            }
            else
            {
                materialCode = requestedCode;
            }

            // DTO doğrulamasından önce oluşturulan kodu ata.
            dto.MaterialCode = materialCode;

            // =====================================
            // 2. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            // =====================================
            // 3. MALZEME ADI
            // =====================================

            string name = ValidateRequiredText(
                dto.Name,
                "Malzeme Adı");

            // =====================================
            // 4. AÇIKLAMA
            // =====================================

            string? description =
                NormalizeOptionalText(dto.Description);

            // =====================================
            // 5. MİNİMUM STOK
            // =====================================

            ValidateMinimumStockLevel(
                dto.MinimumStockLevel);

            // =====================================
            // 6. İLİŞKİLERİ DOĞRULA
            // =====================================

            await ValidateRelationsAsync(
                dto.MeasurementUnitId,
                dto.MaterialCategoryId,
                null,
                cancellationToken);

            // =====================================
            // 7. DTO -> ENTITY
            // =====================================

            var material = _mapper.Map<Material>(dto);

            // =====================================
            // 8. MALZEME BİLGİLERİ
            // =====================================

            material.MaterialCode = materialCode;

            material.Name = name;

            material.Description = description;

            material.MeasurementUnitId =
                dto.MeasurementUnitId;

            material.MaterialCategoryId =
                dto.MaterialCategoryId;

            // Organizasyon doğrudan atanmaz.
            material.UnitId = null;

            material.MinimumStockLevel =
                dto.MinimumStockLevel;

            material.IsActive = true;

            // =====================================
            // 9. KAYDET
            // =====================================

            var createdMaterial = await _materialRepository
                .CreateAsync(
                    material,
                    cancellationToken);

            // =====================================
            // 10. KAYIT ID
            // =====================================

            return createdMaterial.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Malzeme bilgilerini günceller.
        ///
        /// Ölçü birimi değişikliği,
        /// mevcut stok ve işlem geçmişine göre
        /// Repository tarafından kontrol edilir.
        /// </summary>
        public async Task<bool> UpdateAsync(
    UpdateMaterialDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(dto.Id, "Malzeme");

            // =====================================
            // 2. MEVCUT MALZEMEYİ GETİR
            // =====================================

            var material = await _materialRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (material == null)
            {
                return false;
            }

            // =====================================
            // 3. KORUNACAK ALANLAR
            // =====================================

            int originalId = material.Id;

            string originalMaterialCode =
                material.MaterialCode;

            bool originalIsActive =
                material.IsActive;

            DateTime originalCreatedAt =
                material.CreatedAt;

            // Mevcut organizasyon ilişkisi korunur.
            int? originalUnitId = material.UnitId;

            // =====================================
            // 4. DTO DOĞRULAMASI
            // =====================================

            // Kod kullanıcıdan alınmaz.
            dto.MaterialCode = originalMaterialCode;

            // Organizasyon ekranda seçilmez.
            dto.UnitId = originalUnitId;

            ValidateDto(dto);

            // =====================================
            // 5. METİNLERİ NORMALİZE ET
            // =====================================

            string name = ValidateRequiredText(
                dto.Name,
                "Malzeme Adı");

            string? description =
                NormalizeOptionalText(dto.Description);

            ValidateMinimumStockLevel(
                dto.MinimumStockLevel);

            // =====================================
            // 6. ÖLÇÜ BİRİMİ DEĞİŞİKLİĞİ
            // =====================================

            bool measurementUnitChanged =
                material.MeasurementUnitId != dto.MeasurementUnitId;

            if (measurementUnitChanged)
            {
                bool canChange = await _materialRepository
                    .CanChangeMeasurementUnitAsync(
                        material.Id,
                        cancellationToken);

                if (!canChange)
                {
                    throw new InvalidOperationException(
                        "Malzemeye ait stok veya işlem geçmişi " +
                        "bulunduğundan ölçü birimi değiştirilemez.");
                }
            }

            // =====================================
            // 7. İLİŞKİLERİ DOĞRULA
            // =====================================

            await ValidateRelationsAsync(
                dto.MeasurementUnitId,
                dto.MaterialCategoryId,
                originalUnitId,
                cancellationToken);

            // =====================================
            // 8. DTO -> MEVCUT ENTITY
            // =====================================

            _mapper.Map(dto, material);

            // =====================================
            // 9. GÜNCELLENEBİLİR ALANLAR
            // =====================================

            material.Name = name;

            material.Description = description;

            material.MeasurementUnitId =
                dto.MeasurementUnitId;

            material.MaterialCategoryId =
                dto.MaterialCategoryId;

            material.MinimumStockLevel =
                dto.MinimumStockLevel;

            // =====================================
            // 10. KORUNACAK SİSTEM ALANLARI
            // =====================================

            material.Id = originalId;

            // Kod güncellemede asla değişmez.
            material.MaterialCode = originalMaterialCode;

            material.IsActive = originalIsActive;

            material.CreatedAt = originalCreatedAt;

            material.UnitId = originalUnitId;

            material.UpdatedAt = DateTime.UtcNow;

            // =====================================
            // 11. REPOSITORY GÜNCELLEMESİ
            // =====================================

            return await _materialRepository
                .UpdateAsync(
                    material,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Malzemeyi aktif veya pasif yapar.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                id,
                "Malzeme");

            // =====================================
            // 2. MEVCUT MALZEMEYİ GETİR
            // =====================================

            var material = await _materialRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (material == null)
            {
                return false;
            }

            // =====================================
            // 3. DURUM DEĞİŞMEMİŞSE
            // =====================================

            if (material.IsActive == isActive)
            {
                return true;
            }

            // =====================================
            // 4. AKTİFLEŞTİRME DOĞRULAMASI
            // =====================================

            if (isActive)
            {
                await ValidateRelationsAsync(
                    material.MeasurementUnitId,
                    material.MaterialCategoryId,
                    material.UnitId,
                    cancellationToken);
            }

            // =====================================
            // 5. REPOSITORY DURUM DEĞİŞİKLİĞİ
            // =====================================

            return await _materialRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region UpdateMinimumStockLevel

        /// <summary>
        /// Malzemenin minimum stok seviyesini
        /// özel Repository metodu üzerinden günceller.
        /// </summary>
        public async Task<bool> UpdateMinimumStockLevelAsync(
            int id,
            decimal minimumStockLevel,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                id,
                "Malzeme");

            // =====================================
            // 2. STOK SEVİYESİ DOĞRULAMASI
            // =====================================

            ValidateMinimumStockLevel(
                minimumStockLevel);

            // =====================================
            // 3. MEVCUT MALZEMEYİ GETİR
            // =====================================

            var material = await _materialRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (material == null)
            {
                return false;
            }

            // =====================================
            // 4. DEĞER DEĞİŞMEMİŞSE
            // =====================================

            if (material.MinimumStockLevel ==
                minimumStockLevel)
            {
                return true;
            }

            // =====================================
            // 5. REPOSITORY GÜNCELLEMESİ
            // =====================================

            return await _materialRepository
                .UpdateMinimumStockLevelAsync(
                    id,
                    minimumStockLevel,
                    cancellationToken);
        }

        #endregion

        #region ExistsMaterialCode

        /// <summary>
        /// Malzeme kodunun kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsMaterialCodeAsync(
            string materialCode,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KOD DOĞRULAMASI
            // =====================================

            string normalizedCode =
                ValidateRequiredText(
                    materialCode,
                    "Malzeme Kodu");

            // =====================================
            // 2. HARİÇ TUTULACAK ID
            // =====================================

            ValidateExcludedId(
                excludedId);

            // =====================================
            // 3. REPOSITORY KONTROLÜ
            // =====================================

            return await _materialRepository
                .ExistsMaterialCodeAsync(
                    normalizedCode,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region CanChangeMeasurementUnit

        /// <summary>
        /// Malzemenin ölçü biriminin
        /// değiştirilebilir olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> CanChangeMeasurementUnitAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. MALZEME ID
            // =====================================

            ValidateId(
                materialId,
                "Malzeme");

            // =====================================
            // 2. MEVCUT MALZEMEYİ GETİR
            // =====================================

            var material = await _materialRepository
                .GetByIdAsync(
                    materialId,
                    cancellationToken);

            if (material == null)
            {
                return false;
            }

            // =====================================
            // 3. REPOSITORY KONTROLÜ
            // =====================================

            return await _materialRepository
                .CanChangeMeasurementUnitAsync(
                    materialId,
                    cancellationToken);
        }

        #endregion

        #region Private Mapping Methods

        /// <summary>
        /// İlişkili verileri yüklenmiş
        /// malzemeleri Result DTO listesine dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultMaterialDto> MapResults(
            IEnumerable<Material> materials)
        {
            return _mapper.Map<List<ResultMaterialDto>>(
                materials.ToList());
        }

        /// <summary>
        /// Bütün malzemeleri ilişkileriyle getirir.
        /// </summary>
        private async Task<List<Material>> GetAllEntitiesAsync(
            CancellationToken cancellationToken)
        {
            var materials = await _materialRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return materials.ToList();
        }

        /// <summary>
        /// Repository'den alınan seçili kayıtların
        /// ilişkisel alanlarının doldurulmasını sağlar.
        /// </summary>
        private async Task<IReadOnlyList<ResultMaterialDto>>
            MapResultsWithDetailsAsync(
                IEnumerable<Material> materials,
                CancellationToken cancellationToken)
        {
            // =====================================
            // 1. SEÇİLİ MALZEME ID'LERİ
            // =====================================

            var selectedIds = materials
                .Select(x => x.Id)
                .ToHashSet();

            if (selectedIds.Count == 0)
            {
                return new List<ResultMaterialDto>();
            }

            // =====================================
            // 2. DETAYLI KAYITLAR
            // =====================================

            var detailedMaterials = await GetAllEntitiesAsync(
                cancellationToken);

            // =====================================
            // 3. SEÇİLİ KAYITLARI FİLTRELE
            // =====================================

            var filteredMaterials = detailedMaterials
                .Where(x => selectedIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.MaterialCode)
                .ToList();

            // =====================================
            // 4. DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(filteredMaterials);
        }

        #endregion

        #region Private Relation Validation Methods

        /// <summary>
        /// Ölçü birimi, malzeme kategorisi ve
        /// organizasyon birimi ilişkilerini doğrular.
        ///
        /// Kategori ve organizasyon birimi opsiyoneldir.
        /// </summary>
        private async Task ValidateRelationsAsync(
            int measurementUnitId,
            int? materialCategoryId,
            int? unitId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. ÖLÇÜ BİRİMİ ID
            // =====================================

            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            // =====================================
            // 2. ÖLÇÜ BİRİMİNİ GETİR
            // =====================================

            var measurementUnit =
                await _measurementUnitRepository
                    .GetByIdAsync(
                        measurementUnitId,
                        cancellationToken);

            if (measurementUnit == null ||
                !measurementUnit.IsActive)
            {
                throw new InvalidOperationException(
                    "Ölçü birimi bulunamadı " +
                    "veya pasif durumdadır.");
            }

            // =====================================
            // 3. MALZEME KATEGORİSİ
            // =====================================

            if (materialCategoryId.HasValue)
            {
                ValidateId(
                    materialCategoryId.Value,
                    "Malzeme Kategorisi");

                var materialCategory =
                    await _materialCategoryRepository
                        .GetByIdAsync(
                            materialCategoryId.Value,
                            cancellationToken);

                if (materialCategory == null ||
                    !materialCategory.IsActive)
                {
                    throw new InvalidOperationException(
                        "Malzeme kategorisi bulunamadı " +
                        "veya pasif durumdadır.");
                }
            }

            // =====================================
            // 4. ORGANİZASYON BİRİMİ
            // =====================================

            if (unitId.HasValue)
            {
                ValidateId(
                    unitId.Value,
                    "Organizasyon Birimi");

                var unit = await _unitRepository
                    .GetByIdAsync(
                        unitId.Value,
                        cancellationToken);

                if (unit == null || !unit.IsActive)
                {
                    throw new InvalidOperationException(
                        "Organizasyon birimi bulunamadı " +
                        "veya pasif durumdadır.");
                }
            }
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO DataAnnotations doğrulaması.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);
        }

        /// <summary>
        /// ID değerinin pozitif olmasını sağlar.
        /// </summary>
        private static void ValidateId(
            int id,
            string fieldName)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    $"Geçerli bir {fieldName} seçiniz.");
            }
        }

        /// <summary>
        /// Opsiyonel olarak hariç tutulan
        /// ID değerini doğrular.
        /// </summary>
        private static void ValidateExcludedId(
            int? excludedId)
        {
            if (excludedId.HasValue &&
                excludedId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(excludedId),
                    "Hariç tutulacak malzeme ID geçersiz.");
            }
        }

        /// <summary>
        /// Zorunlu metin alanlarını normalize eder.
        /// </summary>
        private static string ValidateRequiredText(
            string? value,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(
                    $"{fieldName} zorunludur.");
            }

            string normalized = value.Trim();

            if (normalized.Any(char.IsControl))
            {
                throw new ValidationException(
                    $"{fieldName} geçersiz karakter içeriyor.");
            }

            return normalized;
        }

        /// <summary>
        /// Opsiyonel açıklama alanını normalize eder.
        /// </summary>
        private static string? NormalizeOptionalText(
            string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }

        /// <summary>
        /// Minimum stok seviyesi negatif olamaz.
        /// </summary>
        private static void ValidateMinimumStockLevel(
            decimal minimumStockLevel)
        {
            if (minimumStockLevel < 0)
            {
                throw new ValidationException(
                    "Minimum stok seviyesi " +
                    "negatif olamaz.");
            }
        }

        #endregion

        #region GenerateUniqueMaterialCode

        /// <summary>
        /// Sistemde kullanılmayan rastgele 6 haneli
        /// malzeme kodu oluşturur.
        /// </summary>
        public async Task<string> GenerateUniqueMaterialCodeAsync(
            CancellationToken cancellationToken = default)
        {
            const int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // 100000 dahil, 1000000 hariç.
                // Böylece her zaman 6 haneli sayı üretilir.
                int randomNumber = RandomNumberGenerator.GetInt32(
                    100000,
                    1000000);

                string materialCode = randomNumber.ToString(
                    System.Globalization.CultureInfo.InvariantCulture);

                bool exists = await _materialRepository
                    .ExistsMaterialCodeAsync(
                        materialCode,
                        null,
                        cancellationToken);

                if (!exists)
                {
                    return materialCode;
                }
            }

            throw new InvalidOperationException(
                "Benzersiz malzeme kodu oluşturulamadı. " +
                "Lütfen işlemi tekrar deneyiniz.");
        }

        #endregion
    }
}
