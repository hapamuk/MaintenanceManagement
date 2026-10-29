using AutoMapper;
using MaintenanceManagement.Application.DTOS.AssetDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IAssetModelRepository _assetModelRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IAssetKindRepository _assetKindRepository;
        private readonly IAssetCategoryRepository _assetCategoryRepository;
        private readonly IMapper _mapper;

        public AssetService(
            IAssetRepository assetRepository,
            IUnitRepository unitRepository,
            IAssetModelRepository assetModelRepository,
            IBrandRepository brandRepository,
            IAssetKindRepository assetKindRepository,
            IAssetCategoryRepository assetCategoryRepository,
            IMapper mapper)
        {
            _assetRepository = assetRepository;
            _unitRepository = unitRepository;
            _assetModelRepository = assetModelRepository;
            _brandRepository = brandRepository;
            _assetKindRepository = assetKindRepository;
            _assetCategoryRepository = assetCategoryRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Bütün demirbaşları ilişkili
        /// bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var assets = await _assetRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif durumdaki demirbaşları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var assets = await _assetRepository
                .GetActiveAssetsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Demirbaş detayını, ilişkili kullanıcıları,
        /// hareketleri ve iş emri sayılarını getirir.
        /// </summary>
        public async Task<GetByIdAssetDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var asset = await _assetRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (asset == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdAssetDto>(asset);
        }

        #endregion

        #region GetByAssetCode

        /// <summary>
        /// Demirbaş koduna göre kayıt getirir.
        /// </summary>
        public async Task<GetByIdAssetDto?> GetByAssetCodeAsync(
            string assetCode,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(assetCode))
            {
                return null;
            }

            var asset = await _assetRepository
                .GetByAssetCodeAsync(
                    assetCode.Trim(),
                    cancellationToken);

            if (asset == null)
            {
                return null;
            }

            // Detay bilgilerini doğru doldurmak için
            // ilişkileri yükleyen metodu kullan.
            return await GetByIdAsync(
                asset.Id,
                cancellationToken);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Organizasyon birimine ait demirbaşları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var assets = await _assetRepository
                .GetAssetsByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetByBrandId

        /// <summary>
        /// Markaya ait demirbaşları getirir.
        /// Marka ilişkisi AssetModel üzerinden kurulur.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>> GetByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(brandId, "Marka");

            var assets = await _assetRepository
                .GetAssetsByBrandIdAsync(
                    brandId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetByModelId

        /// <summary>
        /// Seçilen modele ait demirbaşları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>> GetByModelIdAsync(
            int modelId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(modelId, "Demirbaş Modeli");

            var assets = await _assetRepository
                .GetAssetsByModelIdAsync(
                    modelId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetByCategoryId

        /// <summary>
        /// Belirtilen kategoriye ait demirbaşları getirir.
        /// Kategori ilişkisi AssetKind üzerinden kurulur.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>>
            GetByCategoryIdAsync(
                int categoryId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(categoryId, "Demirbaş Kategorisi");

            var assets = await _assetRepository
                .GetAssetsByCategoryIdAsync(
                    categoryId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetByKindId

        /// <summary>
        /// Belirtilen türe ait demirbaşları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>> GetByKindIdAsync(
            int kindId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(kindId, "Demirbaş Türü");

            var assets = await _assetRepository
                .GetAssetsByKindIdAsync(
                    kindId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetByStatus

        /// <summary>
        /// Demirbaşları durum değerine göre getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>> GetByStatusAsync(
            AssetStatusEnum status,
            CancellationToken cancellationToken = default)
        {
            ValidateStatus(status);

            var assets = await _assetRepository
                .GetAssetsByStatusAsync(
                    status,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetByUserId

        /// <summary>
        /// Kullanıcıyla ilişkili demirbaşları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var assets = await _assetRepository
                .GetAssetsByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetExpiringWarranty

        /// <summary>
        /// Belirtilen gün içinde garantisi bitecek
        /// demirbaşları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>>
            GetExpiringWarrantyAsync(
                int days,
                CancellationToken cancellationToken = default)
        {
            if (days < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(days),
                    "Gün sayısı negatif olamaz.");
            }

            var assets = await _assetRepository
                .GetAssetsWithExpiringWarrantyAsync(
                    days,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region GetExpiredWarranty

        /// <summary>
        /// Garanti süresi sona ermiş demirbaşları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetDto>>
            GetExpiredWarrantyAsync(
                CancellationToken cancellationToken = default)
        {
            var assets = await _assetRepository
                .GetExpiredWarrantyAssetsAsync(
                    cancellationToken);

            return _mapper.Map<List<ResultAssetDto>>(assets);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni demirbaş oluşturur.
        /// Kod, seri numarası, envanter numarası ve
        /// ilişkili kayıtların doğrulamasını yapar.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateAssetDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // 1. DTO doğrulaması
            ValidateDto(dto);

            // 2. Zorunlu alanları düzenle
            string assetCode = ValidateAndNormalizeRequired(
                dto.AssetCode,
                100,
                "Demirbaş kodu");

            string name = ValidateAndNormalizeRequired(
                dto.Name,
                200,
                "Demirbaş adı");

            // 3. İsteğe bağlı alanları düzenle
            string? serialNumber = NormalizeOptional(
                dto.SerialNumber,
                150,
                "Seri numarası");

            string? inventoryNumber = NormalizeOptional(
                dto.InventoryNumber,
                100,
                "Envanter numarası");

            string? description = NormalizeOptional(
                dto.Description,
                1000,
                "Açıklama");

            // 4. Tarih doğrulaması
            ValidateDates(
                dto.PurchaseDate,
                dto.WarrantyEndDate);

            // 5. Birim, model, marka,
            // tür ve kategori doğrulaması
            await ValidateRelationsAsync(
                dto.UnitId,
                dto.AssetModelId,
                dto.AssetKindId,
                cancellationToken);

            // 6. Benzersizlik kontrolü
            await ValidateUniquenessAsync(
                assetCode,
                serialNumber,
                inventoryNumber,
                null,
                cancellationToken);

            // 7. DTO -> Entity
            var asset = _mapper.Map<Asset>(dto);

            asset.AssetCode = assetCode;
            asset.Name = name;
            asset.SerialNumber = serialNumber;
            asset.InventoryNumber = inventoryNumber;
            asset.Description = description;

            asset.UnitId = dto.UnitId;
            asset.AssetModelId = dto.AssetModelId;
            asset.AssetKindId = dto.AssetKindId;

            asset.PurchaseDate = dto.PurchaseDate;
            asset.WarrantyEndDate = dto.WarrantyEndDate;

            // 8. Yeni demirbaşın başlangıç durumu
            asset.Status = AssetStatusEnum.Active;

            // 9. Kayıt işlemi
            var createdAsset = await _assetRepository
                .CreateAsync(
                    asset,
                    cancellationToken);

            return createdAsset.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut demirbaş bilgilerini günceller.
        /// Durum değişikliği ve birimler arası
        /// demirbaş hareketi bu metotta yapılmaz.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateAssetDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Demirbaş");

            string assetCode = ValidateAndNormalizeRequired(
                dto.AssetCode,
                100,
                "Demirbaş kodu");

            string name = ValidateAndNormalizeRequired(
                dto.Name,
                200,
                "Demirbaş adı");

            string? serialNumber = NormalizeOptional(
                dto.SerialNumber,
                150,
                "Seri numarası");

            string? inventoryNumber = NormalizeOptional(
                dto.InventoryNumber,
                100,
                "Envanter numarası");

            string? description = NormalizeOptional(
                dto.Description,
                1000,
                "Açıklama");

            ValidateDates(
                dto.PurchaseDate,
                dto.WarrantyEndDate);

            // Güncellenecek demirbaşı getir.
            var existingAsset = await _assetRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingAsset == null)
            {
                return false;
            }

            // Hurdaya ayrılmış demirbaşın kayıt
            // bilgileri normal güncellemeyle değiştirilemez.
            if (existingAsset.Status == AssetStatusEnum.Scrapped)
            {
                throw new InvalidOperationException(
                    "Hurdaya ayrılmış demirbaş normal " +
                    "güncelleme işlemine kapalıdır.");
            }

            // Organizasyon birimi değişikliği,
            // AssetMovement üzerinden yürütülmelidir.
            if (existingAsset.UnitId != dto.UnitId)
            {
                throw new InvalidOperationException(
                    "Demirbaşın organizasyon birimi normal " +
                    "güncelleme işlemiyle değiştirilemez. " +
                    "Birim değişikliği için demirbaş " +
                    "hareket işlemi kullanılmalıdır.");
            }

            // İlişkileri doğrula.
            await ValidateRelationsAsync(
                dto.UnitId,
                dto.AssetModelId,
                dto.AssetKindId,
                cancellationToken);

            // Model veya tür değişikliği kontrolü.
            bool classificationChanged =
                existingAsset.AssetModelId != dto.AssetModelId ||
                existingAsset.AssetKindId != dto.AssetKindId;

            if (classificationChanged)
            {
                // Önceki işlemlere bağlı geçmişi
                // yanlışlıkla değiştirmemek için
                // ilişkili kayıtları kontrol et.
                var assetWithDetails = await _assetRepository
                    .GetByIdWithDetailsAsync(
                        existingAsset.Id,
                        cancellationToken);

                if (assetWithDetails == null)
                {
                    return false;
                }

                bool hasHistory =
                    assetWithDetails.AssetUsers.Any() ||
                    assetWithDetails.AssetMovements.Any() ||
                    assetWithDetails.WorkRequests.Any() ||
                    assetWithDetails.WorkOrders.Any() ||
                    assetWithDetails.PlannedMaintenances.Any();

                if (hasHistory)
                {
                    throw new InvalidOperationException(
                        "Demirbaşa ait zimmet, hareket, " +
                        "iş talebi, iş emri veya bakım geçmişi " +
                        "bulunduğu için model ve tür bilgileri " +
                        "normal güncelleme ile değiştirilemez.");
                }
            }

            // Benzersizlik kontrolünde kendi ID
            // değerini hariç tut.
            await ValidateUniquenessAsync(
                assetCode,
                serialNumber,
                inventoryNumber,
                existingAsset.Id,
                cancellationToken);

            // DTO -> Mevcut Entity
            _mapper.Map(dto, existingAsset);

            existingAsset.AssetCode = assetCode;
            existingAsset.Name = name;
            existingAsset.SerialNumber = serialNumber;
            existingAsset.InventoryNumber = inventoryNumber;
            existingAsset.Description = description;

            existingAsset.AssetModelId = dto.AssetModelId;
            existingAsset.AssetKindId = dto.AssetKindId;

            existingAsset.PurchaseDate = dto.PurchaseDate;
            existingAsset.WarrantyEndDate = dto.WarrantyEndDate;

            // UnitId ve Status korunur.
            return await _assetRepository
                .UpdateAsync(
                    existingAsset,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Demirbaşın durumunu değiştirir.
        /// Hurdaya ayırma öncesinde ilişkili
        /// açık işlemler kontrol edilir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            AssetStatusEnum status,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            ValidateStatus(status);

            var asset = await _assetRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (asset == null)
            {
                return false;
            }

            // Zaten istenen durumdaysa işlem yapma.
            if (asset.Status == status)
            {
                return true;
            }

            // Hurdaya ayrılmış demirbaş,
            // normal durum değişikliğiyle geri açılamaz.
            if (asset.Status == AssetStatusEnum.Scrapped)
            {
                throw new InvalidOperationException(
                    "Hurdaya ayrılmış demirbaşın durumu " +
                    "normal işlemle değiştirilemez.");
            }

            // Kullanım dışı demirbaş yeniden aktifleştiriliyorsa
            // ilişkili kayıtların aktifliği kontrol edilir.
            if (status == AssetStatusEnum.Active)
            {
                await ValidateRelationsAsync(
                    asset.UnitId,
                    asset.AssetModelId,
                    asset.AssetKindId,
                    cancellationToken);
            }

            // Hurdaya ayırma işlemi
            if (status == AssetStatusEnum.Scrapped)
            {
                bool hasOpenWorkOrders = await _assetRepository
                    .HasOpenWorkOrdersAsync(
                        id,
                        cancellationToken);

                if (hasOpenWorkOrders)
                {
                    throw new InvalidOperationException(
                        "Demirbaşa ait açık iş emirleri " +
                        "bulunduğu için hurdaya ayrılamaz.");
                }

                bool hasActiveAssignments = await _assetRepository
                    .HasActiveAssignmentsAsync(
                        id,
                        cancellationToken);

                if (hasActiveAssignments)
                {
                    throw new InvalidOperationException(
                        "Demirbaşın aktif zimmet kaydı " +
                        "bulunmaktadır. Öncelikle zimmet " +
                        "işlemi sonlandırılmalıdır.");
                }

                bool hasActiveMaintenancePlans =
                    await _assetRepository
                        .HasActiveMaintenancePlansAsync(
                            id,
                            cancellationToken);

                if (hasActiveMaintenancePlans)
                {
                    throw new InvalidOperationException(
                        "Demirbaşa bağlı aktif bakım " +
                        "planları bulunmaktadır. Öncelikle " +
                        "bakım planları sonlandırılmalıdır.");
                }
            }

            // Durum değişikliğini Repository gerçekleştirir.
            return await _assetRepository
                .ChangeStatusAsync(
                    id,
                    status,
                    cancellationToken);
        }

        #endregion

        #region ExistsAssetCode

        /// <summary>
        /// Demirbaş kodu benzersizlik kontrolü.
        /// </summary>
        public async Task<bool> ExistsAssetCodeAsync(
            string assetCode,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedCode = ValidateAndNormalizeRequired(
                assetCode,
                100,
                "Demirbaş kodu");

            ValidateExcludedId(excludedId);

            return await _assetRepository
                .ExistsAssetCodeAsync(
                    normalizedCode,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsSerialNumber

        /// <summary>
        /// Seri numarası benzersizlik kontrolü.
        /// </summary>
        public async Task<bool> ExistsSerialNumberAsync(
            string serialNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string? normalizedSerial = NormalizeOptional(
                serialNumber,
                150,
                "Seri numarası");

            if (normalizedSerial == null)
            {
                return false;
            }

            ValidateExcludedId(excludedId);

            return await _assetRepository
                .ExistsSerialNumberAsync(
                    normalizedSerial,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsInventoryNumber

        /// <summary>
        /// Envanter numarası benzersizlik kontrolü.
        /// </summary>
        public async Task<bool> ExistsInventoryNumberAsync(
            string inventoryNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string? normalizedInventory = NormalizeOptional(
                inventoryNumber,
                100,
                "Envanter numarası");

            if (normalizedInventory == null)
            {
                return false;
            }

            ValidateExcludedId(excludedId);

            return await _assetRepository
                .ExistsInventoryNumberAsync(
                    normalizedInventory,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasOpenWorkOrders

        /// <summary>
        /// Demirbaşa ait açık iş emri var mı kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            return await _assetRepository
                .HasOpenWorkOrdersAsync(
                    assetId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveAssignments

        /// <summary>
        /// Demirbaşın aktif zimmet kaydı var mı kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssignmentsAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            return await _assetRepository
                .HasActiveAssignmentsAsync(
                    assetId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveMaintenancePlans

        /// <summary>
        /// Demirbaşa ait aktif bakım planı var mı kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveMaintenancePlansAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            return await _assetRepository
                .HasActiveMaintenancePlansAsync(
                    assetId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO DataAnnotations doğrulamalarını gerçekleştirir.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            var context = new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                context,
                validateAllProperties: true);
        }

        /// <summary>
        /// Pozitif ID kontrolü.
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
        /// Hariç tutulacak kayıt ID kontrolü.
        /// </summary>
        private static void ValidateExcludedId(int? excludedId)
        {
            if (excludedId.HasValue && excludedId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(excludedId),
                    "Hariç tutulacak ID geçerli olmalıdır.");
            }
        }

        /// <summary>
        /// Zorunlu metinleri doğrular ve temizler.
        /// </summary>
        private static string ValidateAndNormalizeRequired(
            string? value,
            int maxLength,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(
                    $"{fieldName} zorunludur.");
            }

            string normalized = value.Trim();

            if (normalized.Length > maxLength)
            {
                throw new ValidationException(
                    $"{fieldName} en fazla {maxLength} " +
                    "karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// İsteğe bağlı metinleri temizler.
        /// </summary>
        private static string? NormalizeOptional(
            string? value,
            int maxLength,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            string normalized = value.Trim();

            if (normalized.Length > maxLength)
            {
                throw new ValidationException(
                    $"{fieldName} en fazla {maxLength} " +
                    "karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// Satın alma ve garanti bitiş tarihlerini doğrular.
        /// </summary>
        private static void ValidateDates(
            DateTime? purchaseDate,
            DateTime? warrantyEndDate)
        {
            if (purchaseDate.HasValue &&
                warrantyEndDate.HasValue &&
                warrantyEndDate.Value.Date <
                    purchaseDate.Value.Date)
            {
                throw new ValidationException(
                    "Garanti bitiş tarihi satın alma " +
                    "tarihinden önce olamaz.");
            }
        }

        /// <summary>
        /// Enum değerinin geçerli olduğunu doğrular.
        /// </summary>
        private static void ValidateStatus(
            AssetStatusEnum status)
        {
            if (!Enum.IsDefined(typeof(AssetStatusEnum), status))
            {
                throw new ValidationException(
                    "Geçersiz demirbaş durumu.");
            }
        }

        /// <summary>
        /// Demirbaş kodu, seri numarası ve
        /// envanter numarası benzersizliğini doğrular.
        /// </summary>
        private async Task ValidateUniquenessAsync(
            string assetCode,
            string? serialNumber,
            string? inventoryNumber,
            int? excludedId,
            CancellationToken cancellationToken)
        {
            bool codeExists = await _assetRepository
                .ExistsAssetCodeAsync(
                    assetCode,
                    excludedId,
                    cancellationToken);

            if (codeExists)
            {
                throw new InvalidOperationException(
                    "Bu demirbaş kodu daha önce kullanılmıştır.");
            }

            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                bool serialExists = await _assetRepository
                    .ExistsSerialNumberAsync(
                        serialNumber,
                        excludedId,
                        cancellationToken);

                if (serialExists)
                {
                    throw new InvalidOperationException(
                        "Bu seri numarası başka bir " +
                        "demirbaşa aittir.");
                }
            }

            if (!string.IsNullOrWhiteSpace(inventoryNumber))
            {
                bool inventoryExists = await _assetRepository
                    .ExistsInventoryNumberAsync(
                        inventoryNumber,
                        excludedId,
                        cancellationToken);

                if (inventoryExists)
                {
                    throw new InvalidOperationException(
                        "Bu envanter numarası başka bir " +
                        "demirbaşa aittir.");
                }
            }
        }

        /// <summary>
        /// Organizasyon birimi, model, marka,
        /// tür ve kategorinin mevcut ve aktif
        /// olduğunu doğrular.
        /// </summary>
        private async Task ValidateRelationsAsync(
            int unitId,
            int assetModelId,
            int assetKindId,
            CancellationToken cancellationToken)
        {
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(assetModelId, "Demirbaş Modeli");
            ValidateId(assetKindId, "Demirbaş Türü");

            // ==========================================
            // UNIT
            // ==========================================

            var unit = await _unitRepository
                .GetByIdAsync(unitId, cancellationToken);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    "Seçilen organizasyon birimi bulunamadı.");
            }

            if (!unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif organizasyon birimine " +
                    "demirbaş kaydedilemez.");
            }

            // ==========================================
            // ASSET MODEL
            // ==========================================

            var model = await _assetModelRepository
                .GetByIdAsync(assetModelId, cancellationToken);

            if (model == null)
            {
                throw new InvalidOperationException(
                    "Seçilen demirbaş modeli bulunamadı.");
            }

            if (!model.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif demirbaş modeli kullanılamaz.");
            }

            // ==========================================
            // BRAND
            // ==========================================

            var brand = await _brandRepository
                .GetByIdAsync(model.BrandId, cancellationToken);

            if (brand == null)
            {
                throw new InvalidOperationException(
                    "Modelin bağlı olduğu marka bulunamadı.");
            }

            if (!brand.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif markaya ait model kullanılamaz.");
            }

            // ==========================================
            // ASSET KIND
            // ==========================================

            var kind = await _assetKindRepository
                .GetByIdAsync(assetKindId, cancellationToken);

            if (kind == null)
            {
                throw new InvalidOperationException(
                    "Seçilen demirbaş türü bulunamadı.");
            }

            if (!kind.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif demirbaş türü kullanılamaz.");
            }

            // ==========================================
            // ASSET CATEGORY
            // ==========================================

            var category = await _assetCategoryRepository
                .GetByIdAsync(
                    kind.AssetCategoryId,
                    cancellationToken);

            if (category == null)
            {
                throw new InvalidOperationException(
                    "Türün bağlı olduğu kategori bulunamadı.");
            }

            if (!category.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif kategoriye ait " +
                    "demirbaş türü kullanılamaz.");
            }
        }

        #endregion
    }
}
