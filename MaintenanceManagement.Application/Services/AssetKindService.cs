using AutoMapper;
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
    public class AssetKindService : IAssetKindService
    {
        private readonly IAssetKindRepository _assetKindRepository;
        private readonly IAssetCategoryRepository _assetCategoryRepository;
        private readonly IMapper _mapper;

        public AssetKindService(
            IAssetKindRepository assetKindRepository,
            IAssetCategoryRepository assetCategoryRepository,
            IMapper mapper)
        {
            _assetKindRepository = assetKindRepository;
            _assetCategoryRepository = assetCategoryRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün demirbaş türlerini
        /// kategori bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetKindDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var kinds = await _assetKindRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetKindDto>>(
                kinds);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif demirbaş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetKindDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var kinds = await _assetKindRepository
                .GetActiveKindsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetKindDto>>(
                kinds);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Demirbaş türünü kategori ve demirbaş
        /// detaylarıyla birlikte getirir.
        /// </summary>
        public async Task<GetByIdAssetKindDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var kind = await _assetKindRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (kind == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdAssetKindDto>(
                kind);
        }

        #endregion

        #region GetByCategoryId

        /// <summary>
        /// Belirtilen kategoriye bağlı bütün
        /// demirbaş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetKindDto>>
            GetByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Demirbaş Kategorisi");

            var category = await _assetCategoryRepository
                .GetByIdAsync(
                    categoryId,
                    cancellationToken);

            if (category == null)
            {
                return Array.Empty<ResultAssetKindDto>();
            }

            var kinds = await _assetKindRepository
                .GetKindsByCategoryIdAsync(
                    categoryId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultAssetKindDto>>(
                kinds);

            // Kategori navigation bilgisi yüklenmemiş olsa
            // bile kategori adını doğru şekilde doldur.
            foreach (var kind in result)
            {
                kind.AssetCategoryName = category.Name;
            }

            return result;
        }

        #endregion

        #region GetActiveByCategoryId

        /// <summary>
        /// Aktif kategoriye bağlı aktif
        /// demirbaş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetKindDto>>
            GetActiveByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Demirbaş Kategorisi");

            var category = await _assetCategoryRepository
                .GetByIdAsync(
                    categoryId,
                    cancellationToken);

            // Pasif kategorideki türler yeni demirbaş
            // oluşturma ekranlarında gösterilmez.
            if (category == null || !category.IsActive)
            {
                return Array.Empty<ResultAssetKindDto>();
            }

            var kinds = await _assetKindRepository
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

        #region GetAssetsByKindId

        /// <summary>
        /// Belirtilen demirbaş türüne bağlı
        /// bütün demirbaşları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>>
            GetAssetsByKindIdAsync(
                int kindId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                kindId,
                "Demirbaş Türü");

            var kind = await _assetKindRepository
                .GetByIdAsync(
                    kindId,
                    cancellationToken);

            if (kind == null)
            {
                return Array.Empty<ResultAssetDto>();
            }

            var assets = await _assetKindRepository
                .GetAssetsByKindIdAsync(
                    kindId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(
                assets);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni demirbaş türü oluşturur.
        /// Aynı kategoride aynı isimli ikinci tür oluşturulamaz.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateAssetKindDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // DTO üzerindeki DataAnnotations kontrolleri.
            ValidateDto(dto);

            ValidateId(
                dto.AssetCategoryId,
                "Demirbaş Kategorisi");

            // Tür adını kontrol et ve normalize et.
            string name = ValidateAndNormalizeName(
                dto.Name);

            // Kategori mevcut ve aktif olmalıdır.
            await ValidateCategoryAsync(
                dto.AssetCategoryId,
                cancellationToken);

            // Aynı kategori altında aynı tür adı bulunamaz.
            bool existsName = await _assetKindRepository
                .ExistsNameAsync(
                    dto.AssetCategoryId,
                    name,
                    null,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu kategoriye bağlı aynı isimde " +
                    "bir demirbaş türü bulunmaktadır.");
            }

            // DTO -> Entity
            var kind = _mapper.Map<AssetKind>(dto);

            kind.AssetCategoryId = dto.AssetCategoryId;
            kind.Name = name;

            // Yeni tür varsayılan olarak aktiftir.
            kind.IsActive = true;

            // CreatedAt ve SaveChanges işlemleri
            // mevcut GenericRepository tarafından yönetilir.
            var createdKind = await _assetKindRepository
                .CreateAsync(
                    kind,
                    cancellationToken);

            return createdKind.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut demirbaş türünün adını veya
        /// uygun koşullarda bağlı kategorisini günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateAssetKindDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(
                dto.Id,
                "Demirbaş Türü");

            ValidateId(
                dto.AssetCategoryId,
                "Demirbaş Kategorisi");

            string name = ValidateAndNormalizeName(
                dto.Name);

            // Güncellenecek türü getir.
            var existingKind = await _assetKindRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingKind == null)
            {
                return false;
            }

            // Seçilen kategori aktif ve mevcut olmalıdır.
            await ValidateCategoryAsync(
                dto.AssetCategoryId,
                cancellationToken);

            bool categoryChanged =
                existingKind.AssetCategoryId != dto.AssetCategoryId;

            // Kategoriyi değiştirmek, ilişkili demirbaşların
            // kategorisini dolaylı olarak değiştirecektir.
            if (categoryChanged)
            {
                bool hasAssets = await _assetKindRepository
                    .HasAssetsAsync(
                        existingKind.Id,
                        cancellationToken);

                if (hasAssets)
                {
                    throw new InvalidOperationException(
                        "Bu demirbaş türüne bağlı kayıtlar " +
                        "bulunduğu için kategorisi değiştirilemez.");
                }
            }

            // Aynı kategoride aynı adla başka bir tür
            // bulunup bulunmadığını kontrol et.
            bool existsName = await _assetKindRepository
                .ExistsNameAsync(
                    dto.AssetCategoryId,
                    name,
                    existingKind.Id,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Seçilen kategoride aynı isimle " +
                    "başka bir demirbaş türü bulunmaktadır.");
            }

            // DTO -> Mevcut Entity
            _mapper.Map(dto, existingKind);

            existingKind.AssetCategoryId = dto.AssetCategoryId;
            existingKind.Name = name;

            // Id, IsActive, CreatedAt ve navigation
            // özellikleri AutoMapper ile korunur.
            return await _assetKindRepository
                .UpdateAsync(
                    existingKind,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Demirbaş türünü aktif veya pasif yapar.
        /// Mevcut demirbaş kayıtlarını değiştirmez.
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

            var existingKind = await _assetKindRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingKind == null)
            {
                return false;
            }

            // Durum zaten istenildiği gibiyse
            // tekrar güncelleme yapma.
            if (existingKind.IsActive == isActive)
            {
                return true;
            }

            // Pasif tür tekrar aktif hale getiriliyorsa
            // kategorinin de aktif olması gerekir.
            if (isActive)
            {
                await ValidateCategoryAsync(
                    existingKind.AssetCategoryId,
                    cancellationToken);
            }

            // Türün pasife alınması mevcut
            // demirbaş kayıtlarını pasife almaz.
            return await _assetKindRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Kategori bazında tür adı benzersizliğini
        /// kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            int categoryId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                categoryId,
                "Demirbaş Kategorisi");

            string normalizedName =
                ValidateAndNormalizeName(name);

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak Tür");
            }

            return await _assetKindRepository
                .ExistsNameAsync(
                    categoryId,
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasAssets

        /// <summary>
        /// Türle ilişkili herhangi bir
        /// demirbaş kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssetsAsync(
            int kindId,
            CancellationToken cancellationToken = default)
        {
            if (kindId <= 0)
            {
                return false;
            }

            return await _assetKindRepository
                .HasAssetsAsync(
                    kindId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveAssets

        /// <summary>
        /// Türle ilişkili aktif demirbaş
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssetsAsync(
            int kindId,
            CancellationToken cancellationToken = default)
        {
            if (kindId <= 0)
            {
                return false;
            }

            return await _assetKindRepository
                .HasActiveAssetsAsync(
                    kindId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations kurallarını
        /// servis katmanında çalıştırır.
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
        /// ID değerinin sıfırdan büyük olduğunu doğrular.
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
        /// Tür adının zorunlu olmasını, uzunluğunu
        /// ve baştaki/sondaki boşluklarını kontrol eder.
        /// </summary>
        private static string ValidateAndNormalizeName(
            string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Demirbaş türü adı zorunludur.");
            }

            string normalizedName = name.Trim();

            if (normalizedName.Length > 150)
            {
                throw new ValidationException(
                    "Demirbaş türü adı en fazla " +
                    "150 karakter olabilir.");
            }

            return normalizedName;
        }

        /// <summary>
        /// Demirbaş kategorisinin mevcut ve aktif
        /// olduğunu doğrular.
        /// </summary>
        private async Task ValidateCategoryAsync(
            int categoryId,
            CancellationToken cancellationToken)
        {
            var category = await _assetCategoryRepository
                .GetByIdAsync(
                    categoryId,
                    cancellationToken);

            if (category == null)
            {
                throw new InvalidOperationException(
                    "Seçilen demirbaş kategorisi bulunamadı.");
            }

            if (!category.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif kategoriye demirbaş türü atanamaz.");
            }
        }

        #endregion
    }
}
