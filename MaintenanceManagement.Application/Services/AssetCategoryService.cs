using AutoMapper;
using MaintenanceManagement.Application.DTOS.AssetCategoryDtos;
using MaintenanceManagement.Application.DTOS.AssetDtos;
using MaintenanceManagement.Application.DTOS.AssetKindDtos;
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
    public class AssetCategoryService : IAssetCategoryService
    {
        private readonly IAssetCategoryRepository _assetCategoryRepository;
        private readonly IMapper _mapper;

        public AssetCategoryService(
            IAssetCategoryRepository assetCategoryRepository,
            IMapper mapper)
        {
            _assetCategoryRepository = assetCategoryRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün demirbaş kategorilerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetCategoryDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var categories = await _assetCategoryRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetCategoryDto>>(
                categories);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Yalnızca aktif demirbaş kategorilerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetCategoryDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var categories = await _assetCategoryRepository
                .GetActiveCategoriesAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetCategoryDto>>(
                categories);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Kategori detaylarını ve bağlı tür sayılarını getirir.
        /// </summary>
        public async Task<GetByIdAssetCategoryDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var category = await _assetCategoryRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (category == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdAssetCategoryDto>(
                category);
        }

        #endregion

        #region GetKindsByCategoryId

        /// <summary>
        /// Kategoriye bağlı aktif ve pasif bütün
        /// demirbaş türlerini DTO listesi halinde getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetKindDto>>
            GetKindsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(categoryId);

            // Kategorinin mevcut olduğunu kontrol et.
            var category = await _assetCategoryRepository
                .GetByIdAsync(
                    categoryId,
                    cancellationToken);

            if (category == null)
            {
                return Array.Empty<ResultAssetKindDto>();
            }

            var kinds = await _assetCategoryRepository
                .GetKindsByCategoryIdAsync(
                    categoryId,
                    cancellationToken);

            // AssetKind -> ResultAssetKindDto
            var result = _mapper.Map<List<ResultAssetKindDto>>(
                kinds);

            // AssetKind.AssetCategory navigation alanı
            // sorguda yüklenmemiş olsa bile kategori
            // adının doğru doldurulmasını sağla.
            foreach (var kind in result)
            {
                kind.AssetCategoryName = category.Name;
            }

            return result;
        }

        #endregion

        #region GetActiveKindsByCategoryId

        /// <summary>
        /// Aktif kategoriye bağlı aktif
        /// demirbaş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetKindDto>>
            GetActiveKindsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(categoryId);

            var category = await _assetCategoryRepository
                .GetByIdAsync(
                    categoryId,
                    cancellationToken);

            // Pasif kategorinin türleri yeni
            // demirbaş girişlerinde gösterilmez.
            if (category == null || !category.IsActive)
            {
                return Array.Empty<ResultAssetKindDto>();
            }

            var kinds = await _assetCategoryRepository
                .GetActiveKindsByCategoryIdAsync(
                    categoryId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultAssetKindDto>>(
                kinds);

            foreach (var kind in result)
            {
                kind.AssetCategoryName = category.Name;
            }

            return result;
        }

        #endregion

        #region GetAssetsByCategoryId

        /// <summary>
        /// Kategoriye bağlı demirbaş kayıtlarını getirir.
        /// İlişki AssetCategory -> AssetKind -> Asset
        /// üzerinden kurulur.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>>
            GetAssetsByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(categoryId);

            var category = await _assetCategoryRepository
                .GetByIdAsync(
                    categoryId,
                    cancellationToken);

            if (category == null)
            {
                return Array.Empty<ResultAssetDto>();
            }

            var assets = await _assetCategoryRepository
                .GetAssetsByCategoryIdAsync(
                    categoryId,
                    cancellationToken);

            // Asset -> ResultAssetDto
            var result = _mapper.Map<List<ResultAssetDto>>(
                assets);

            // Kategori adı mevcut kayıttan alınır.
            foreach (var asset in result)
            {
                asset.AssetCategoryName = category.Name;
            }

            return result;
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni demirbaş kategorisi oluşturur.
        /// Kategori adı sistem genelinde benzersizdir.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateAssetCategoryDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // DTO üzerindeki DataAnnotations kuralları.
            ValidateDto(dto);

            // Kategori adını doğrula ve temizle.
            string name = ValidateAndNormalizeName(dto.Name);

            // Açıklama bilgisini temizle.
            string? description =
                ValidateAndNormalizeDescription(dto.Description);

            // Aynı isimde kategori bulunamaz.
            bool existsName = await _assetCategoryRepository
                .ExistsNameAsync(
                    name,
                    null,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu kategori adıyla daha önce " +
                    "demirbaş kategorisi oluşturulmuştur.");
            }

            // DTO -> Entity dönüşümü.
            var category = _mapper.Map<AssetCategory>(dto);

            category.Name = name;
            category.Description = description;

            // Yeni kategori varsayılan olarak aktiftir.
            category.IsActive = true;

            // GenericRepository üzerinden kayıt oluşturulur.
            // CreatedAt ve SaveChanges işlemleri mevcut
            // Repository düzeni tarafından yönetilir.
            var createdCategory = await _assetCategoryRepository
                .CreateAsync(
                    category,
                    cancellationToken);

            return createdCategory.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut demirbaş kategorisinin
        /// adını ve açıklamasını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateAssetCategoryDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // DTO doğrulaması.
            ValidateDto(dto);

            ValidateId(dto.Id);

            string name = ValidateAndNormalizeName(dto.Name);

            string? description =
                ValidateAndNormalizeDescription(dto.Description);

            // Güncellenecek kategoriyi getir.
            var existingCategory = await _assetCategoryRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingCategory == null)
            {
                return false;
            }

            // Benzersizlik kontrolünde kendi ID
            // değerini sorgudan hariç tut.
            bool existsName = await _assetCategoryRepository
                .ExistsNameAsync(
                    name,
                    existingCategory.Id,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu kategori adı başka bir kayıt " +
                    "tarafından kullanılmaktadır.");
            }

            // DTO -> Mevcut Entity dönüşümü.
            // Id, IsActive, CreatedAt ve Kinds korunur.
            _mapper.Map(dto, existingCategory);

            existingCategory.Name = name;
            existingCategory.Description = description;

            // UpdatedAt ve SaveChanges işlemleri
            // GenericRepository tarafından gerçekleştirilir.
            return await _assetCategoryRepository
                .UpdateAsync(
                    existingCategory,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Demirbaş kategorisinin aktiflik durumunu değiştirir.
        /// Aktif türü bulunan kategori pasife alınamaz.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            var existingCategory = await _assetCategoryRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingCategory == null)
            {
                return false;
            }

            // Durum zaten aynıysa yeniden kaydetme.
            if (existingCategory.IsActive == isActive)
            {
                return true;
            }

            // Kategori pasife alınacaksa
            // bağlı aktif türleri kontrol et.
            if (!isActive)
            {
                bool hasActiveKinds = await _assetCategoryRepository
                    .HasActiveKindsAsync(
                        id,
                        cancellationToken);

                if (hasActiveKinds)
                {
                    throw new InvalidOperationException(
                        "Bu kategoriye bağlı aktif demirbaş " +
                        "türleri bulunmaktadır. Kategori pasife " +
                        "alınmadan önce ilgili türler " +
                        "pasife alınmalıdır.");
                }
            }

            // Mevcut demirbaş kayıtları korunur.
            return await _assetCategoryRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Kategori adı benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedName =
                ValidateAndNormalizeName(name);

            if (excludedId.HasValue)
            {
                ValidateId(excludedId.Value);
            }

            return await _assetCategoryRepository
                .ExistsNameAsync(
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasKinds

        /// <summary>
        /// Kategoriye bağlı herhangi bir
        /// demirbaş türü bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasKindsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                return false;
            }

            return await _assetCategoryRepository
                .HasKindsAsync(
                    categoryId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveKinds

        /// <summary>
        /// Kategoriye bağlı aktif demirbaş
        /// türü bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveKindsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                return false;
            }

            return await _assetCategoryRepository
                .HasActiveKindsAsync(
                    categoryId,
                    cancellationToken);
        }

        #endregion

        #region HasAssets

        /// <summary>
        /// Kategoriye bağlı herhangi bir demirbaş
        /// kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssetsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                return false;
            }

            return await _assetCategoryRepository
                .HasAssetsAsync(
                    categoryId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveAssets

        /// <summary>
        /// Kategoriye bağlı aktif demirbaş
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssetsAsync(
            int categoryId,
            CancellationToken cancellationToken = default)
        {
            if (categoryId <= 0)
            {
                return false;
            }

            return await _assetCategoryRepository
                .HasActiveAssetsAsync(
                    categoryId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations
        /// doğrulamalarını çalıştırır.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            var validationContext = new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                validationContext,
                validateAllProperties: true);
        }

        /// <summary>
        /// Kategori adını kontrol eder ve normalize eder.
        /// </summary>
        private static string ValidateAndNormalizeName(
            string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Demirbaş kategori adı zorunludur.");
            }

            string normalizedName = name.Trim();

            if (normalizedName.Length > 150)
            {
                throw new ValidationException(
                    "Demirbaş kategori adı en fazla " +
                    "150 karakter olabilir.");
            }

            return normalizedName;
        }

        /// <summary>
        /// Açıklama alanını temizler.
        /// Boş değerleri null olarak kaydeder.
        /// </summary>
        private static string? ValidateAndNormalizeDescription(
            string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            string normalizedDescription = description.Trim();

            if (normalizedDescription.Length > 500)
            {
                throw new ValidationException(
                    "Kategori açıklaması en fazla " +
                    "500 karakter olabilir.");
            }

            return normalizedDescription;
        }

        /// <summary>
        /// Kategori ID değerinin geçerli olduğunu kontrol eder.
        /// </summary>
        private static void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    "Geçerli bir demirbaş kategorisi seçiniz.");
            }
        }

        #endregion
    }
}
