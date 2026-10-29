using AutoMapper;
using MaintenanceManagement.Application.DTOS.AssetModelDtos;
using MaintenanceManagement.Application.DTOS.BrandDtos;
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
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;

        public BrandService(
            IBrandRepository brandRepository,
            IMapper mapper)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün markaları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultBrandDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var brands = await _brandRepository
                .GetAllWithModelsAsync(cancellationToken);

            return _mapper.Map<List<ResultBrandDto>>(brands);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Yalnızca aktif markaları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultBrandDto>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var brands = await _brandRepository
                .GetActiveBrandsAsync(cancellationToken);

            return _mapper.Map<List<ResultBrandDto>>(brands);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Marka detaylarını ve bağlı model sayılarını getirir.
        /// </summary>
        public async Task<GetByIdBrandDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var brand = await _brandRepository
                .GetByIdWithModelsAsync(
                    id,
                    cancellationToken);

            if (brand == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdBrandDto>(brand);
        }

        #endregion

        #region GetModelsByBrandId

        /// <summary>
        /// Markaya bağlı aktif ve pasif bütün modelleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetModelDto>>
            GetModelsByBrandIdAsync(
                int brandId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(brandId);

            var brand = await _brandRepository
                .GetByIdAsync(
                    brandId,
                    cancellationToken);

            if (brand == null)
            {
                return Array.Empty<ResultAssetModelDto>();
            }

            var models = await _brandRepository
                .GetModelsByBrandIdAsync(
                    brandId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultAssetModelDto>>(
                models);

            // Model sorgusunda Brand navigation özelliği
            // yüklenmese de marka adını doğru doldur.
            foreach (var model in result)
            {
                model.BrandName = brand.Name;
            }

            return result;
        }

        #endregion

        #region GetActiveModelsByBrandId

        /// <summary>
        /// Markaya bağlı aktif modelleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetModelDto>>
            GetActiveModelsByBrandIdAsync(
                int brandId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(brandId);

            var brand = await _brandRepository
                .GetByIdAsync(
                    brandId,
                    cancellationToken);

            if (brand == null || !brand.IsActive)
            {
                return Array.Empty<ResultAssetModelDto>();
            }

            var models = await _brandRepository
                .GetActiveModelsByBrandIdAsync(
                    brandId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultAssetModelDto>>(
                models);

            foreach (var model in result)
            {
                model.BrandName = brand.Name;
            }

            return result;
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni marka oluşturur.
        /// Marka adı sistem genelinde benzersiz olmalıdır.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateBrandDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            string name = ValidateAndNormalizeName(dto.Name);

            // Aynı isimde marka bulunamaz.
            bool existsName = await _brandRepository
                .ExistsNameAsync(
                    name,
                    null,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu marka adıyla daha önce " +
                    "kayıt oluşturulmuştur.");
            }

            // DTO -> Entity
            var brand = _mapper.Map<Brand>(dto);

            brand.Name = name;

            // Yeni marka varsayılan olarak aktiftir.
            brand.IsActive = true;

            // CreatedAt ve SaveChanges işlemleri
            // GenericRepository tarafından gerçekleştirilir.
            var createdBrand = await _brandRepository
                .CreateAsync(
                    brand,
                    cancellationToken);

            return createdBrand.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut markanın adını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateBrandDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id);

            string name = ValidateAndNormalizeName(dto.Name);

            var existingBrand = await _brandRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingBrand == null)
            {
                return false;
            }

            // Aynı isimde başka marka olup olmadığını
            // kontrol ederken kendi ID değerini hariç tut.
            bool existsName = await _brandRepository
                .ExistsNameAsync(
                    name,
                    existingBrand.Id,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu marka adı başka bir kayıt " +
                    "tarafından kullanılmaktadır.");
            }

            // DTO -> Mevcut Entity
            _mapper.Map(dto, existingBrand);

            existingBrand.Name = name;

            // Id, IsActive, CreatedAt ve Models korunur.
            // UpdatedAt ve SaveChanges işlemleri
            // GenericRepository tarafından gerçekleştirilir.
            return await _brandRepository
                .UpdateAsync(
                    existingBrand,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Markanın aktif/pasif durumunu değiştirir.
        /// Aktif modeli bulunan markayı pasife almaz.
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

            var existingBrand = await _brandRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingBrand == null)
            {
                return false;
            }

            // Zaten istenilen durumdaysa
            // tekrar veritabanı işlemi yapma.
            if (existingBrand.IsActive == isActive)
            {
                return true;
            }

            // Marka pasife alınacaksa aktif model kontrolü.
            if (!isActive)
            {
                bool hasActiveModels = await _brandRepository
                    .HasActiveModelsAsync(
                        id,
                        cancellationToken);

                if (hasActiveModels)
                {
                    throw new InvalidOperationException(
                        "Markaya bağlı aktif modeller " +
                        "bulunmaktadır. Marka pasife alınmadan " +
                        "önce ilgili modeller pasife alınmalıdır.");
                }
            }

            return await _brandRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Marka adı benzersizliğini kontrol eder.
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

            return await _brandRepository
                .ExistsNameAsync(
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasModels

        /// <summary>
        /// Markaya bağlı model bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasModelsAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            if (brandId <= 0)
            {
                return false;
            }

            return await _brandRepository
                .HasModelsAsync(
                    brandId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveModels

        /// <summary>
        /// Markaya bağlı aktif model bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveModelsAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            if (brandId <= 0)
            {
                return false;
            }

            return await _brandRepository
                .HasActiveModelsAsync(
                    brandId,
                    cancellationToken);
        }

        #endregion

        #region HasAssets

        /// <summary>
        /// Markaya bağlı modeller üzerinden
        /// demirbaş kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssetsAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            if (brandId <= 0)
            {
                return false;
            }

            return await _brandRepository
                .HasAssetsAsync(
                    brandId,
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
        /// Marka adını doğrular ve normalize eder.
        /// </summary>
        private static string ValidateAndNormalizeName(
            string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Marka adı zorunludur.");
            }

            string normalizedName = name.Trim();

            if (normalizedName.Length > 150)
            {
                throw new ValidationException(
                    "Marka adı en fazla 150 karakter olabilir.");
            }

            return normalizedName;
        }

        /// <summary>
        /// Marka ID değerinin geçerli olduğunu doğrular.
        /// </summary>
        private static void ValidateId(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    "Geçerli bir marka seçiniz.");
            }
        }

        #endregion
    }
}
