using AutoMapper;
using MaintenanceManagement.Application.DTOS.AssetDtos;
using MaintenanceManagement.Application.DTOS.AssetModelDtos;
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
    public class AssetModelService : IAssetModelService
    {
        private readonly IAssetModelRepository _assetModelRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public AssetModelService(
            IAssetModelRepository assetModelRepository,
            IBrandRepository brandRepository,
            IMapper mapper)
        {
            _assetModelRepository = assetModelRepository;
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün modelleri
        /// marka bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetModelDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var models = await _assetModelRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetModelDto>>(
                models);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif demirbaş modellerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetModelDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var models = await _assetModelRepository
                .GetActiveModelsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetModelDto>>(
                models);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Belirtilen modelin detaylarını ve
        /// bağlı demirbaş sayısını getirir.
        /// </summary>
        public async Task<GetByIdAssetModelDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var model = await _assetModelRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (model == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdAssetModelDto>(
                model);
        }

        #endregion

        #region GetByBrandId

        /// <summary>
        /// Markaya bağlı aktif ve pasif
        /// bütün modelleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetModelDto>>
            GetByBrandIdAsync(
                int brandId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(brandId, "Marka");

            var models = await _assetModelRepository
                .GetModelsByBrandIdAsync(
                    brandId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetModelDto>>(
                models);
        }

        #endregion

        #region GetActiveByBrandId

        /// <summary>
        /// Aktif markaya bağlı aktif modelleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetModelDto>>
            GetActiveByBrandIdAsync(
                int brandId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(brandId, "Marka");

            var brand = await _brandRepository
                .GetByIdAsync(
                    brandId,
                    cancellationToken);

            if (brand == null || !brand.IsActive)
            {
                return Array.Empty<ResultAssetModelDto>();
            }

            var models = await _assetModelRepository
                .GetActiveModelsByBrandIdAsync(
                    brandId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetModelDto>>(
                models);
        }

        #endregion

        #region GetByBrandAndName

        /// <summary>
        /// Marka ve model adına göre
        /// mevcut modeli getirir.
        /// </summary>
        public async Task<GetByIdAssetModelDto?>
            GetByBrandAndNameAsync(
                int brandId,
                string name,
                CancellationToken cancellationToken = default)
        {
            ValidateId(brandId, "Marka");

            string normalizedName =
                ValidateAndNormalizeName(name);

            var model = await _assetModelRepository
                .GetByBrandAndNameAsync(
                    brandId,
                    normalizedName,
                    cancellationToken);

            if (model == null)
            {
                return null;
            }

            // Demirbaş sayısının doğru doldurulması
            // için detay sorgusunu kullan.
            return await GetByIdAsync(
                model.Id,
                cancellationToken);
        }

        #endregion

        #region GetAssetsByModelId

        /// <summary>
        /// Modele bağlı demirbaşları DTO
        /// listesi halinde getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>>
            GetAssetsByModelIdAsync(
                int modelId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(modelId, "Demirbaş Modeli");

            var model = await _assetModelRepository
                .GetByIdAsync(
                    modelId,
                    cancellationToken);

            if (model == null)
            {
                return Array.Empty<ResultAssetDto>();
            }

            var assets = await _assetModelRepository
                .GetAssetsByModelIdAsync(
                    modelId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(
                assets);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni demirbaş modeli oluşturur.
        /// Aynı marka altında aynı model adına izin verilmez.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateAssetModelDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // DTO DataAnnotations doğrulaması.
            ValidateDto(dto);

            ValidateId(dto.BrandId, "Marka");

            string name =
                ValidateAndNormalizeName(dto.Name);

            // Seçilen marka mevcut ve aktif olmalıdır.
            await ValidateBrandAsync(
                dto.BrandId,
                cancellationToken);

            // Aynı marka içinde model adı benzersizliği.
            bool existsName = await _assetModelRepository
                .ExistsNameAsync(
                    dto.BrandId,
                    name,
                    null,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu markaya bağlı aynı isimde " +
                    "bir demirbaş modeli bulunmaktadır.");
            }

            // Create DTO -> AssetModel Entity.
            var model = _mapper.Map<AssetModel>(dto);

            model.BrandId = dto.BrandId;
            model.Name = name;

            // Yeni model varsayılan olarak aktiftir.
            model.IsActive = true;

            // CreatedAt ve SaveChanges işlemleri
            // mevcut GenericRepository tarafından yapılır.
            var createdModel = await _assetModelRepository
                .CreateAsync(
                    model,
                    cancellationToken);

            return createdModel.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut demirbaş modelinin adını veya
        /// uygun koşullarda bağlı markasını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateAssetModelDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Demirbaş Modeli");
            ValidateId(dto.BrandId, "Marka");

            string name =
                ValidateAndNormalizeName(dto.Name);

            // Güncellenecek modeli getir.
            var existingModel = await _assetModelRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingModel == null)
            {
                return false;
            }

            // Yeni marka mevcut ve aktif olmalıdır.
            await ValidateBrandAsync(
                dto.BrandId,
                cancellationToken);

            bool brandChanged =
                existingModel.BrandId != dto.BrandId;

            // Modele bağlı demirbaş varsa markayı
            // değiştirmek geçmiş kayıtları etkileyebilir.
            if (brandChanged)
            {
                bool hasAssets = await _assetModelRepository
                    .HasAssetsAsync(
                        existingModel.Id,
                        cancellationToken);

                if (hasAssets)
                {
                    throw new InvalidOperationException(
                        "Bu modele bağlı demirbaş kayıtları " +
                        "bulunduğu için modelin markası " +
                        "değiştirilemez.");
                }
            }

            // Güncellenecek modelin kendi ID değeri
            // benzersizlik sorgusundan hariç tutulur.
            bool existsName = await _assetModelRepository
                .ExistsNameAsync(
                    dto.BrandId,
                    name,
                    existingModel.Id,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Seçilen markada aynı isimle başka " +
                    "bir model bulunmaktadır.");
            }

            // Update DTO -> Mevcut Entity.
            // Id, IsActive, CreatedAt ve navigation
            // özellikleri AutoMapper tarafından korunur.
            _mapper.Map(dto, existingModel);

            existingModel.BrandId = dto.BrandId;
            existingModel.Name = name;

            return await _assetModelRepository
                .UpdateAsync(
                    existingModel,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Demirbaş modelini aktif veya pasif yapar.
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

            var existingModel = await _assetModelRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingModel == null)
            {
                return false;
            }

            // Zaten istenen durumdaysa işlem yapma.
            if (existingModel.IsActive == isActive)
            {
                return true;
            }

            // Model yeniden aktif yapılacaksa
            // bağlı markanın aktif olması gerekir.
            if (isActive)
            {
                await ValidateBrandAsync(
                    existingModel.BrandId,
                    cancellationToken);
            }

            // Model pasife alınırken mevcut demirbaşlar
            // korunur; yeni demirbaş seçimlerinde
            // bu model gösterilmemelidir.
            return await _assetModelRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Marka bazında model adı benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            int brandId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(brandId, "Marka");

            string normalizedName =
                ValidateAndNormalizeName(name);

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak Model");
            }

            return await _assetModelRepository
                .ExistsNameAsync(
                    brandId,
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasAssets

        /// <summary>
        /// Modele bağlı herhangi bir demirbaş
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssetsAsync(
            int modelId,
            CancellationToken cancellationToken = default)
        {
            if (modelId <= 0)
            {
                return false;
            }

            return await _assetModelRepository
                .HasAssetsAsync(
                    modelId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveAssets

        /// <summary>
        /// Modele bağlı aktif demirbaş bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssetsAsync(
            int modelId,
            CancellationToken cancellationToken = default)
        {
            if (modelId <= 0)
            {
                return false;
            }

            return await _assetModelRepository
                .HasActiveAssetsAsync(
                    modelId,
                    cancellationToken);
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
            var validationContext =
                new ValidationContext(dto);

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
        /// Model adını doğrular ve normalize eder.
        /// </summary>
        private static string ValidateAndNormalizeName(
            string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Demirbaş model adı zorunludur.");
            }

            string normalizedName = name.Trim();

            if (normalizedName.Length > 150)
            {
                throw new ValidationException(
                    "Demirbaş model adı en fazla " +
                    "150 karakter olabilir.");
            }

            return normalizedName;
        }

        /// <summary>
        /// Markanın mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateBrandAsync(
            int brandId,
            CancellationToken cancellationToken)
        {
            var brand = await _brandRepository
                .GetByIdAsync(
                    brandId,
                    cancellationToken);

            if (brand == null)
            {
                throw new InvalidOperationException(
                    "Seçilen marka bulunamadı.");
            }

            if (!brand.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif markaya demirbaş modeli atanamaz.");
            }
        }

        #endregion
    }
}
