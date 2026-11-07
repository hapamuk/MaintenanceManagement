using AutoMapper;
using MaintenanceManagement.Application.DTOS.MaterialCategoryDtos;
using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class MaterialCategoryService
       : IMaterialCategoryService
    {
        #region Constants

        private const int MaxNameLength = 150;

        private const int MaxDescriptionLength = 500;

        #endregion

        #region Fields

        private readonly IMaterialCategoryRepository
            _materialCategoryRepository;

        private readonly IMaterialCategoryUnitRepository _materialCategoryUnitRepository;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public MaterialCategoryService(
            IMaterialCategoryRepository materialCategoryRepository,
            IMapper mapper,
            IMaterialCategoryUnitRepository materialCategoryUnitRepository)
        {
            _materialCategoryRepository =
                materialCategoryRepository;

            _mapper = mapper;
            _materialCategoryUnitRepository = materialCategoryUnitRepository;
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün malzeme kategorilerini
        /// ilişkili malzemeleriyle birlikte getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KATEGORİLERİ GETİR
            // =====================================

            var categories =
                await _materialCategoryRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            // =====================================
            // 2. SIRALAMA VE DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(
                categories
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID değerine göre kategori detayını getirir.
        /// </summary>
        public async Task<GetByIdMaterialCategoryDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID KONTROLÜ
            // =====================================

            if (id <= 0)
            {
                return null;
            }

            // =====================================
            // 2. KATEGORİ DETAYINI GETİR
            // =====================================

            var category =
                await _materialCategoryRepository
                    .GetByIdWithDetailsAsync(
                        id,
                        cancellationToken);

            if (category == null)
            {
                return null;
            }

            // =====================================
            // 3. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<GetByIdMaterialCategoryDto>(
                category);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif malzeme kategorilerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. AKTİF KATEGORİLERİ GETİR
            // =====================================

            var categories =
                await _materialCategoryRepository
                    .GetActiveCategoriesAsync(
                        cancellationToken);

            // =====================================
            // 2. DETAYLI DTO DÖNÜŞÜMÜ
            // =====================================

            return await MapResultsWithDetailsAsync(
                categories,
                cancellationToken);
        }

        #endregion

        #region GetInactive

        /// <summary>
        /// Pasif malzeme kategorilerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetInactiveAsync(
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. PASİF KATEGORİLERİ GETİR
            // =====================================

            var categories =
                await _materialCategoryRepository
                    .GetInactiveCategoriesAsync(
                        cancellationToken);

            // =====================================
            // 2. DETAYLI DTO DÖNÜŞÜMÜ
            // =====================================

            return await MapResultsWithDetailsAsync(
                categories,
                cancellationToken);
        }

        #endregion

        #region GetByName

        /// <summary>
        /// Kategori adına göre kayıt detayını getirir.
        /// </summary>
        public async Task<GetByIdMaterialCategoryDto?>
            GetByNameAsync(
                string name,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KATEGORİ ADI DOĞRULAMASI
            // =====================================

            string normalizedName = ValidateName(name);

            // =====================================
            // 2. REPOSITORY SORGUSU
            // =====================================

            var category =
                await _materialCategoryRepository
                    .GetByNameAsync(
                        normalizedName,
                        cancellationToken);

            if (category == null)
            {
                return null;
            }

            // =====================================
            // 3. DETAYLI DTO
            // =====================================

            return await GetByIdAsync(
                category.Id,
                cancellationToken);
        }

        #endregion

        #region Search

        /// <summary>
        /// Kategori adına göre arama gerçekleştirir.
        /// İstenirse yalnızca aktif kayıtları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialCategoryDto>>
            SearchAsync(
                string searchText,
                bool onlyActive = false,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ARAMA METNİNİ DOĞRULA
            // =====================================

            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new ValidationException(
                    "Kategori arama metni boş olamaz.");
            }

            string normalizedSearchText =
                searchText.Trim();

            if (normalizedSearchText.Length > MaxNameLength)
            {
                throw new ValidationException(
                    "Arama metni en fazla 150 " +
                    "karakter olabilir.");
            }

            // =====================================
            // 2. REPOSITORY ARAMASI
            // =====================================

            var categories =
                await _materialCategoryRepository
                    .SearchCategoriesAsync(
                        normalizedSearchText,
                        onlyActive,
                        cancellationToken);

            // =====================================
            // 3. DTO DÖNÜŞÜMÜ
            // =====================================

            return await MapResultsWithDetailsAsync(
                categories,
                cancellationToken);
        }

        #endregion

        #region GetByMaterialId

        /// <summary>
        /// Malzemenin bağlı olduğu kategoriyi getirir.
        /// </summary>
        public async Task<GetByIdMaterialCategoryDto?>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. MALZEME ID KONTROLÜ
            // =====================================

            ValidateId(
                materialId,
                "Malzeme");

            // =====================================
            // 2. KATEGORİYİ GETİR
            // =====================================

            var category =
                await _materialCategoryRepository
                    .GetByMaterialIdAsync(
                        materialId,
                        cancellationToken);

            if (category == null)
            {
                return null;
            }

            // =====================================
            // 3. DETAYLI DTO
            // =====================================

            return await GetByIdAsync(
                category.Id,
                cancellationToken);
        }

        #endregion

        #region GetCategoriesWithMaterials

        /// <summary>
        /// En az bir malzeme ilişkisi bulunan
        /// kategorileri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetCategoriesWithMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KATEGORİLERİ GETİR
            // =====================================

            var categories =
                await _materialCategoryRepository
                    .GetCategoriesWithMaterialsAsync(
                        cancellationToken);

            // =====================================
            // 2. DETAYLI DTO DÖNÜŞÜMÜ
            // =====================================

            return await MapResultsWithDetailsAsync(
                categories,
                cancellationToken);
        }

        #endregion

        #region GetCategoriesWithoutMaterials

        /// <summary>
        /// Hiçbir malzeme ilişkisi bulunmayan
        /// kategorileri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialCategoryDto>>
            GetCategoriesWithoutMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KATEGORİLERİ GETİR
            // =====================================

            var categories =
                await _materialCategoryRepository
                    .GetCategoriesWithoutMaterialsAsync(
                        cancellationToken);

            // =====================================
            // 2. DETAYLI DTO DÖNÜŞÜMÜ
            // =====================================

            return await MapResultsWithDetailsAsync(
                categories,
                cancellationToken);
        }

        #endregion

        #region GetMaterialsByCategoryId

        /// <summary>
        /// Kategoriye ait aktif ve pasif
        /// bütün malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetMaterialsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KATEGORİ ID KONTROLÜ
            // =====================================

            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            // =====================================
            // 2. MALZEMELERİ GETİR
            // =====================================

            var materials =
                await _materialCategoryRepository
                    .GetMaterialsByCategoryIdAsync(
                        categoryId,
                        cancellationToken);

            // =====================================
            // 3. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<List<ResultMaterialDto>>(
                materials);
        }

        #endregion

        #region GetActiveMaterialsByCategoryId

        /// <summary>
        /// Kategoriye ait yalnızca aktif
        /// malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetActiveMaterialsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KATEGORİ ID KONTROLÜ
            // =====================================

            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            // =====================================
            // 2. AKTİF MALZEMELERİ GETİR
            // =====================================

            var materials =
                await _materialCategoryRepository
                    .GetActiveMaterialsByCategoryIdAsync(
                        categoryId,
                        cancellationToken);

            // =====================================
            // 3. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<List<ResultMaterialDto>>(
                materials);
        }

        #endregion

        #region GetMaterialsByCategoryAndUnitId

        /// <summary>
        /// Kategori ve organizasyon birimine
        /// göre malzeme kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetMaterialsByCategoryAndUnitIdAsync(
                int categoryId,
                int unitId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KATEGORİ ID KONTROLÜ
            // =====================================

            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            // =====================================
            // 2. BİRİM ID KONTROLÜ
            // =====================================

            ValidateId(
                unitId,
                "Organizasyon Birimi");

            // =====================================
            // 3. MALZEMELERİ GETİR
            // =====================================

            var materials =
                await _materialCategoryRepository
                    .GetMaterialsByCategoryAndUnitIdAsync(
                        categoryId,
                        unitId,
                        cancellationToken);

            // =====================================
            // 4. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<List<ResultMaterialDto>>(
                materials);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni malzeme kategorisi oluşturur.
        ///
        /// Seçilen organizasyonlar aynı transaction
        /// içerisinde kaydedilir.
        ///
        /// Organizasyon seçilmezse kategori
        /// bütün organizasyonlara açık olur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateMaterialCategoryDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);

            // =====================================
            // 2. ORGANİZASYON SEÇİMLERİ
            // =====================================

            var selectedIds = (
                dto.SelectedUnitIds ?? new List<int>())
                .Distinct()
                .ToList();

            if (selectedIds.Any(x => x <= 0))
            {
                throw new ValidationException(
                    "Geçersiz organizasyon seçimi.");
            }

            // =====================================
            // 3. KATEGORİ VE İLİŞKİLERİ KAYDET
            // =====================================

            return await _materialCategoryUnitRepository
                .ExecuteInTransactionAsync(
                    async () =>
                    {
                        // DTO -> ENTITY
                        var category =
                            _mapper.Map<MaterialCategory>(dto);

                        // Başlangıç durumu
                        category.IsActive = true;

                        // Mevcut kategori repository kaydı
                        var createdCategory =
                            await _materialCategoryRepository
                                .CreateAsync(
                                    category,
                                    cancellationToken);

                        // Organizasyon ilişkilerini kaydet
                        await _materialCategoryUnitRepository
                            .ReplaceCategoryUnitsAsync(
                                createdCategory.Id,
                                selectedIds,
                                cancellationToken);

                        return createdCategory.Id;
                    },
                    cancellationToken);
        }

        #endregion


        #region Update

        /// <summary>
        /// Malzeme kategorisini ve organizasyon
        /// seçimlerini birlikte günceller.
        ///
        /// Boş organizasyon listesi, kategorinin
        /// bütün organizasyonlara açılması anlamına gelir.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateMaterialCategoryDto dto,
    CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);

            if (dto.Id <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir malzeme kategorisi seçiniz.");
            }

            // =====================================
            // 2. ORGANİZASYON SEÇİMLERİ
            // =====================================

            var selectedIds = (
                dto.SelectedUnitIds ?? new List<int>())
                .Distinct()
                .ToList();

            if (selectedIds.Any(x => x <= 0))
            {
                throw new ValidationException(
                    "Geçersiz organizasyon seçimi.");
            }

            // =====================================
            // 3. TRANSACTION
            // =====================================

            return await _materialCategoryUnitRepository
                .ExecuteInTransactionAsync(
                    async () =>
                    {
                        // =================================
                        // 4. MEVCUT KATEGORİ
                        // =================================

                        var category =
                            await _materialCategoryRepository
                                .GetByIdAsync(
                                    dto.Id,
                                    cancellationToken);

                        if (category == null)
                        {
                            return false;
                        }

                        // =================================
                        // 5. SİSTEM ALANLARINI KORU
                        // =================================

                        int originalId = category.Id;

                        bool originalIsActive =
                            category.IsActive;

                        DateTime originalCreatedAt =
                            category.CreatedAt;

                        // =================================
                        // 6. DTO -> ENTITY
                        // =================================

                        _mapper.Map(dto, category);

                        category.Id = originalId;

                        category.IsActive = originalIsActive;

                        category.CreatedAt =
                            originalCreatedAt;

                        category.UpdatedAt =
                            DateTime.UtcNow;

                        // =================================
                        // 7. KATEGORİYİ GÜNCELLE
                        // =================================

                        bool updated =
                            await _materialCategoryRepository
                                .UpdateAsync(
                                    category,
                                    cancellationToken);

                        if (!updated)
                        {
                            return false;
                        }

                        // =================================
                        // 8. ORGANİZASYONLARI GÜNCELLE
                        // =================================

                        await _materialCategoryUnitRepository
                            .ReplaceCategoryUnitsAsync(
                                category.Id,
                                selectedIds,
                                cancellationToken);

                        // =================================
                        // 9. SONUÇ
                        // =================================

                        return true;
                    },
                    cancellationToken);
        }

        #endregion


        #region ChangeStatus

        /// <summary>
        /// Malzeme kategorisini aktif veya pasif yapar.
        ///
        /// Aktif malzemelerin kullandığı kategori
        /// pasif hale getirilemez.
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
                "Malzeme Kategorisi");

            // =====================================
            // 2. MEVCUT KATEGORİYİ GETİR
            // =====================================

            var materialCategory =
                await _materialCategoryRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (materialCategory == null)
            {
                return false;
            }

            // =====================================
            // 3. DURUM DEĞİŞMEDİYSE
            // =====================================

            if (materialCategory.IsActive == isActive)
            {
                return true;
            }

            // =====================================
            // 4. PASİFLEŞTİRME KONTROLÜ
            // =====================================

            if (!isActive)
            {
                bool hasActiveMaterials =
                    await _materialCategoryRepository
                        .HasActiveMaterialsAsync(
                            id,
                            cancellationToken);

                if (hasActiveMaterials)
                {
                    throw new InvalidOperationException(
                        "Bu kategori aktif malzemeler " +
                        "tarafından kullanılmaktadır. " +
                        "Aktif malzemeler bu kategoriye " +
                        "bağlıyken pasifleştirilemez.");
                }
            }

            // =====================================
            // 5. REPOSITORY DURUM DEĞİŞİKLİĞİ
            // =====================================

            return await _materialCategoryRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Kategori adının mevcut olup olmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. AD DOĞRULAMASI
            // =====================================

            string normalizedName =
                ValidateName(name);

            // =====================================
            // 2. HARİÇ TUTULACAK ID
            // =====================================

            ValidateExcludedId(excludedId);

            // =====================================
            // 3. BENZERSİZLİK KONTROLÜ
            // =====================================

            return await _materialCategoryRepository
                .ExistsNameAsync(
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasMaterials

        /// <summary>
        /// Kategoriye bağlı malzeme bulunuyor mu?
        /// </summary>
        public async Task<bool> HasMaterialsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            return await _materialCategoryRepository
                .HasMaterialsAsync(
                    categoryId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveMaterials

        /// <summary>
        /// Kategoriye bağlı aktif malzeme bulunuyor mu?
        /// </summary>
        public async Task<bool> HasActiveMaterialsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            return await _materialCategoryRepository
                .HasActiveMaterialsAsync(
                    categoryId,
                    cancellationToken);
        }

        #endregion

        #region GetMaterialCount

        /// <summary>
        /// Kategoriye bağlı toplam malzeme
        /// sayısını getirir.
        /// </summary>
        public async Task<int> GetMaterialCountAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            return await _materialCategoryRepository
                .GetMaterialCountAsync(
                    categoryId,
                    cancellationToken);
        }

        #endregion

        #region GetActiveMaterialCount

        /// <summary>
        /// Kategoriye bağlı aktif malzeme
        /// sayısını getirir.
        /// </summary>
        public async Task<int> GetActiveMaterialCountAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Malzeme Kategorisi");

            return await _materialCategoryRepository
                .GetActiveMaterialCountAsync(
                    categoryId,
                    cancellationToken);
        }

        #endregion

        #region Private Mapping Methods

        /// <summary>
        /// Entity koleksiyonunu Result DTO
        /// koleksiyonuna dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultMaterialCategoryDto>
            MapResults(
                IEnumerable<MaterialCategory> categories)
        {
            return _mapper.Map<List<ResultMaterialCategoryDto>>(
                categories.ToList());
        }

        /// <summary>
        /// Seçilen kategori kayıtlarını Materials
        /// ilişkisi yüklenmiş kayıtlarla eşleştirir.
        ///
        /// Böylece MaterialCount değeri
        /// doğru hesaplanabilir.
        /// </summary>
        private async Task<IReadOnlyList<ResultMaterialCategoryDto>>
            MapResultsWithDetailsAsync(
                IEnumerable<MaterialCategory> categories,
                CancellationToken cancellationToken)
        {
            // =====================================
            // 1. KATEGORİ ID'LERİNİ AL
            // =====================================

            var selectedIds = categories
                .Select(x => x.Id)
                .ToHashSet();

            // =====================================
            // 2. BOŞ LİSTE KONTROLÜ
            // =====================================

            if (selectedIds.Count == 0)
            {
                return new List<ResultMaterialCategoryDto>();
            }

            // =====================================
            // 3. DETAYLI KAYITLARI GETİR
            // =====================================

            var detailedCategories =
                await _materialCategoryRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            // =====================================
            // 4. SEÇİLEN KAYITLARI FİLTRELE
            // =====================================

            var filteredCategories =
                detailedCategories
                    .Where(x => selectedIds.Contains(x.Id))
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
                    .ToList();

            // =====================================
            // 5. DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(filteredCategories);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations
        /// doğrulamalarını gerçekleştirir.
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
        /// ID değerinin pozitif olduğunu doğrular.
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
        /// Hariç tutulacak kayıt ID'sini doğrular.
        /// </summary>
        private static void ValidateExcludedId(
            int? excludedId)
        {
            if (excludedId.HasValue &&
                excludedId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(excludedId),
                    "Hariç tutulacak kategori ID geçersiz.");
            }
        }

        /// <summary>
        /// Kategori adını doğrular ve normalize eder.
        /// </summary>
        private static string ValidateName(
            string? name)
        {
            // =====================================
            // 1. ZORUNLU ALAN
            // =====================================

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Malzeme kategorisi adı zorunludur.");
            }

            // =====================================
            // 2. BOŞLUKLARI TEMİZLE
            // =====================================

            string normalized =
                name.Trim();

            // =====================================
            // 3. UZUNLUK KONTROLÜ
            // =====================================

            if (normalized.Length > MaxNameLength)
            {
                throw new ValidationException(
                    "Malzeme kategorisi adı en fazla " +
                    "150 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// Kategori açıklamasını normalize eder
        /// ve uzunluğunu doğrular.
        /// </summary>
        private static string? NormalizeDescription(
            string? description)
        {
            // =====================================
            // 1. BOŞ AÇIKLAMA
            // =====================================

            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            // =====================================
            // 2. BOŞLUKLARI TEMİZLE
            // =====================================

            string normalized =
                description.Trim();

            // =====================================
            // 3. UZUNLUK KONTROLÜ
            // =====================================

            if (normalized.Length > MaxDescriptionLength)
            {
                throw new ValidationException(
                    "Kategori açıklaması en fazla " +
                    "500 karakter olabilir.");
            }

            return normalized;
        }



        #endregion

        public async Task<IReadOnlyList<int>>
    GetSelectedUnitIdsAsync(
        int materialCategoryId,
        CancellationToken cancellationToken = default)
        {
            if (materialCategoryId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(materialCategoryId),
                    "Geçerli bir kategori seçiniz.");
            }

            return await _materialCategoryUnitRepository
                .GetUnitIdsByCategoryIdAsync(
                    materialCategoryId,
                    cancellationToken);
        }
    }
}
