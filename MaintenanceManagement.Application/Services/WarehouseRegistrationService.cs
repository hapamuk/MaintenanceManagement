
using MaintenanceManagement.Application.DTOS.WarehouseDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class WarehouseRegistrationService
        : IWarehouseRegistrationService
    {
        #region Fields

        private readonly IWarehouseService
            _warehouseService;

        private readonly IUnitService
            _unitService;

        private readonly IWarehouseRegistrationRepository
            _warehouseRegistrationRepository;

        #endregion

        #region Constructor

        public WarehouseRegistrationService(
            IWarehouseService warehouseService,
            IUnitService unitService,
            IWarehouseRegistrationRepository
                warehouseRegistrationRepository)
        {
            _warehouseService = warehouseService
                ?? throw new ArgumentNullException(
                    nameof(warehouseService));

            _unitService = unitService
                ?? throw new ArgumentNullException(
                    nameof(unitService));

            _warehouseRegistrationRepository =
                warehouseRegistrationRepository
                ?? throw new ArgumentNullException(
                    nameof(warehouseRegistrationRepository));
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni ambar ve görevli personelleri
        /// tek transaction içerisinde oluşturur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWarehouseDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. ORGANİZASYON DOĞRULAMASI
            // =====================================

            int unitId = await ValidateUnitAsync(
                dto.UnitId,
                cancellationToken);

            // =====================================
            // 2. PERSONEL ID LİSTESİNİ DÜZENLE
            // =====================================

            List<int> personnelIds =
                NormalizePersonnelIds(
                    dto.SelectedPersonnelIds);

            // =====================================
            // 3. PERSONEL YETKİLERİNİ DOĞRULA
            // =====================================

            await ValidatePersonnelAsync(
                unitId,
                personnelIds,
                cancellationToken);

            // =====================================
            // 4. AMBAR KODUNU BELİRLE
            // =====================================

            // Create ekranında otomatik oluşturulan
            // kod geçerliyse kullanılmaya çalışılır.
            // Kod kullanılmışsa yenisi oluşturulur.

            string requestedCode =
                dto.Code?.Trim() ?? string.Empty;

            bool validCode =
                IsValidWarehouseCode(requestedCode);

            bool codeExists = false;

            if (validCode)
            {
                codeExists = await _warehouseService
                    .ExistsCodeAsync(
                        requestedCode,
                        cancellationToken: cancellationToken);
            }

            string warehouseCode =
                validCode && !codeExists
                    ? requestedCode
                    : await GenerateUniqueWarehouseCodeAsync(
                        cancellationToken);

            dto.Code = warehouseCode;

            // =====================================
            // 5. VERİLERİ NORMALİZE ET
            // =====================================

            dto.Name = dto.Name?.Trim()
                ?? string.Empty;

            dto.Location = NormalizeOptional(
                dto.Location);

            dto.Description = NormalizeOptional(
                dto.Description);

            dto.UnitId = unitId;

            dto.SelectedPersonnelIds = personnelIds;

            // =====================================
            // 6. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            // =====================================
            // 7. ATOMİK KAYIT
            // =====================================

            // Repository tek transaction içinde:
            // - Warehouse kaydını oluşturur.
            // - WarehousePersonnel kayıtlarını oluşturur.
            // - Herhangi bir hata olursa rollback yapar.

            return await _warehouseRegistrationRepository
                .CreateWithPersonnelAsync(
                    dto,
                    personnelIds,
                    cancellationToken);
        }

        #endregion

        #region Update

        /// <summary>
        /// Ambarı ve görevli personelleri günceller.
        /// Ambar kodunu değiştirmez.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWarehouseDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            if (dto.Id <= 0)
            {
                throw new ValidationException(
                    "Geçerli bir ambar seçiniz.");
            }

            // =====================================
            // 2. MEVCUT AMBARI GETİR
            // =====================================

            var currentWarehouse =
                await _warehouseService.GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (currentWarehouse == null)
            {
                return false;
            }

            // =====================================
            // 3. AMBAR KODUNU KORU
            // =====================================

            // Dışarıdan gönderilen Code dikkate alınmaz.
            dto.Code = currentWarehouse.Code;

            // =====================================
            // 4. ORGANİZASYON DOĞRULAMASI
            // =====================================

            int unitId = await ValidateUnitAsync(
                dto.UnitId,
                cancellationToken);

            // =====================================
            // 5. BİRİM DEĞİŞİKLİĞİ KONTROLÜ
            // =====================================

            bool unitChanged =
                currentWarehouse.UnitId != unitId;

            if (unitChanged)
            {
                // Stok kaydı bulunan ambarda
                // organizasyon değiştirilemez.
                if (currentWarehouse.WarehouseStockCount > 0)
                {
                    throw new InvalidOperationException(
                        "Stok kayıtları bulunan ambarın " +
                        "organizasyon birimi değiştirilemez.");
                }

                // Stok hareketi geçmişi kontrolü.
                if (currentWarehouse.StockMovementCount > 0)
                {
                    throw new InvalidOperationException(
                        "Stok hareketi geçmişi bulunan " +
                        "ambarın organizasyon birimi " +
                        "değiştirilemez.");
                }

                // İş emri malzeme geçmişi kontrolü.
                if (currentWarehouse.WorkOrderMaterialCount > 0)
                {
                    throw new InvalidOperationException(
                        "İş emirlerinde malzeme kullanımı " +
                        "bulunan ambarın organizasyon " +
                        "birimi değiştirilemez.");
                }
            }

            // =====================================
            // 6. PERSONEL ID LİSTESİNİ DÜZENLE
            // =====================================

            List<int> personnelIds =
                NormalizePersonnelIds(
                    dto.SelectedPersonnelIds);

            // =====================================
            // 7. PERSONEL YETKİLERİNİ DOĞRULA
            // =====================================

            await ValidatePersonnelAsync(
                unitId,
                personnelIds,
                cancellationToken);

            // =====================================
            // 8. GÜNCELLENEBİLİR ALANLAR
            // =====================================

            dto.Name = dto.Name?.Trim()
                ?? string.Empty;

            dto.Location = NormalizeOptional(
                dto.Location);

            dto.Description = NormalizeOptional(
                dto.Description);

            dto.UnitId = unitId;

            dto.SelectedPersonnelIds = personnelIds;

            // =====================================
            // 9. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            // =====================================
            // 10. ATOMİK GÜNCELLEME
            // =====================================

            // Repository:
            // - Mevcut Code değerini korur.
            // - Ambar alanlarını günceller.
            // - Personel atamalarını eşitler.
            // - İşlem başarısızsa rollback yapar.

            return await _warehouseRegistrationRepository
                .UpdateWithPersonnelAsync(
                    dto,
                    personnelIds,
                    cancellationToken);
        }

        #endregion

        #region GetEligiblePersonnel

        /// <summary>
        /// Seçilen organizasyondaki aktif
        /// personelleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<WarehousePersonnelCandidateDto>>
            GetEligiblePersonnelAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            await ValidateUnitAsync(
                unitId,
                cancellationToken);

            var personnel =
                await _warehouseRegistrationRepository
                    .GetEligiblePersonnelByUnitAsync(
                        unitId,
                        cancellationToken);

            return personnel
                .Where(x =>
                    x.IsActive &&
                    x.UnitId == unitId)
                .OrderBy(x => x.FullName)
                .ToList();
        }

        #endregion

        #region GetAssignedPersonnelIds

        /// <summary>
        /// Ambara atanmış personel ID bilgilerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<int>>
            GetAssignedPersonnelIdsAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            if (warehouseId <= 0)
            {
                throw new ValidationException(
                    "Geçerli bir ambar seçiniz.");
            }

            var warehouse = await _warehouseService
                .GetByIdAsync(
                    warehouseId,
                    cancellationToken);

            if (warehouse == null)
            {
                throw new InvalidOperationException(
                    "Ambar bulunamadı.");
            }

            var ids = await _warehouseRegistrationRepository
                .GetAssignedPersonnelIdsAsync(
                    warehouseId,
                    cancellationToken);

            return ids
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        #endregion

        #region GenerateUniqueWarehouseCode

        /// <summary>
        /// Benzersiz, 6 haneli sayısal ambar kodu üretir.
        /// </summary>
        private async Task<string>
            GenerateUniqueWarehouseCodeAsync(
                CancellationToken cancellationToken)
        {
            const int maxAttempts = 100;

            for (int attempt = 0;
                 attempt < maxAttempts;
                 attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                int number =
                    RandomNumberGenerator.GetInt32(
                        100000,
                        1000000);

                string code = number.ToString(
                    CultureInfo.InvariantCulture);

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

        #region ValidateUnit

        /// <summary>
        /// Birimin mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task<int> ValidateUnitAsync(
            int? unitId,
            CancellationToken cancellationToken)
        {
            if (!unitId.HasValue || unitId.Value <= 0)
            {
                throw new ValidationException(
                    "Ambar için organizasyon seçimi zorunludur.");
            }

            var activeUnits = await _unitService
                .GetActiveAsync(cancellationToken);

            bool unitExists = activeUnits.Any(
                x => x.Id == unitId.Value);

            if (!unitExists)
            {
                throw new ValidationException(
                    "Seçilen organizasyon bulunamadı " +
                    "veya aktif değildir.");
            }

            return unitId.Value;
        }

        #endregion

        #region ValidatePersonnel

        /// <summary>
        /// Seçilen personellerin tamamının
        /// ilgili organizasyonda aktif olduğunu kontrol eder.
        /// </summary>
        private async Task ValidatePersonnelAsync(
            int unitId,
            IReadOnlyCollection<int> personnelIds,
            CancellationToken cancellationToken)
        {
            if (personnelIds.Count == 0)
            {
                throw new ValidationException(
                    "Ambara en az bir görevli personel seçiniz.");
            }

            var eligiblePersonnel =
                await _warehouseRegistrationRepository
                    .GetEligiblePersonnelByUnitAsync(
                        unitId,
                        cancellationToken);

            var eligibleIds = eligiblePersonnel
                .Where(x =>
                    x.IsActive &&
                    x.UnitId == unitId)
                .Select(x => x.UserId)
                .ToHashSet();

            var invalidIds = personnelIds
                .Where(x => !eligibleIds.Contains(x))
                .ToList();

            if (invalidIds.Count > 0)
            {
                throw new ValidationException(
                    "Seçilen personellerden en az biri " +
                    "ilgili organizasyonda görevli değildir " +
                    "veya aktif değildir.");
            }
        }

        #endregion

        #region NormalizePersonnelIds

        private static List<int> NormalizePersonnelIds(
            IEnumerable<int>? personnelIds)
        {
            var ids = personnelIds?
                .Distinct()
                .ToList()
                ?? new List<int>();

            if (ids.Any(x => x <= 0))
            {
                throw new ValidationException(
                    "Geçersiz personel seçimi yapıldı.");
            }

            return ids;
        }

        #endregion

        #region IsValidWarehouseCode

        private static bool IsValidWarehouseCode(
            string? code)
        {
            return !string.IsNullOrEmpty(code)
                && code.Length == 6
                && code[0] >= '1'
                && code[0] <= '9'
                && code.All(x =>
                    x >= '0' && x <= '9');
        }

        #endregion

        #region NormalizeOptional

        private static string? NormalizeOptional(
            string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        #endregion

        #region ValidateDto

        private static void ValidateDto<T>(T dto)
            where T : class
        {
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);
        }

        #endregion
    }
}
