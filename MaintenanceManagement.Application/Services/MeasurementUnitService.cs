using AutoMapper;
using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.DTOS.MeasurementUnitDtos;
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
    public class MeasurementUnitService
         : IMeasurementUnitService
    {
        #region Constants

        private const int MaxNameLength = 100;

        private const int MaxSymbolLength = 20;

        #endregion

        #region Fields

        private readonly IMeasurementUnitRepository
            _measurementUnitRepository;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public MeasurementUnitService(
            IMeasurementUnitRepository measurementUnitRepository,
            IMapper mapper)
        {
            _measurementUnitRepository =
                measurementUnitRepository;

            _mapper = mapper;
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün ölçü birimlerini malzeme ilişkileriyle
        /// birlikte getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var measurementUnits =
                await _measurementUnitRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            return MapResults(
                measurementUnits
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region GetById

        /// <summary>
        /// Ölçü biriminin detayını getirir.
        /// </summary>
        public async Task<GetByIdMeasurementUnitDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var measurementUnit =
                await _measurementUnitRepository
                    .GetByIdWithDetailsAsync(
                        id,
                        cancellationToken);

            if (measurementUnit == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdMeasurementUnitDto>(
                measurementUnit);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif ölçü birimlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var measurementUnits =
                await _measurementUnitRepository
                    .GetActiveMeasurementUnitsAsync(
                        cancellationToken);

            return await MapResultsWithDetailsAsync(
                measurementUnits,
                cancellationToken);
        }

        #endregion

        #region GetInactive

        /// <summary>
        /// Pasif ölçü birimlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetInactiveAsync(
                CancellationToken cancellationToken = default)
        {
            var measurementUnits =
                await _measurementUnitRepository
                    .GetInactiveMeasurementUnitsAsync(
                        cancellationToken);

            return await MapResultsWithDetailsAsync(
                measurementUnits,
                cancellationToken);
        }

        #endregion

        #region GetByName

        /// <summary>
        /// Ölçü birimi adına göre kayıt getirir.
        /// </summary>
        public async Task<GetByIdMeasurementUnitDto?>
            GetByNameAsync(
                string name,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. AD DOĞRULAMASI
            // =====================================

            string normalizedName = ValidateName(name);

            // =====================================
            // 2. REPOSITORY SORGUSU
            // =====================================

            var measurementUnit =
                await _measurementUnitRepository
                    .GetByNameAsync(
                        normalizedName,
                        cancellationToken);

            if (measurementUnit == null)
            {
                return null;
            }

            // =====================================
            // 3. DETAYLI DTO DÖNÜŞÜMÜ
            // =====================================

            return await GetByIdAsync(
                measurementUnit.Id,
                cancellationToken);
        }

        #endregion

        #region GetBySymbol

        /// <summary>
        /// Ölçü birimi sembolüne göre kayıt getirir.
        /// </summary>
        public async Task<GetByIdMeasurementUnitDto?>
            GetBySymbolAsync(
                string symbol,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. SEMBOL DOĞRULAMASI
            // =====================================

            string normalizedSymbol =
                ValidateSymbol(symbol);

            // =====================================
            // 2. REPOSITORY SORGUSU
            // =====================================

            var measurementUnit =
                await _measurementUnitRepository
                    .GetBySymbolAsync(
                        normalizedSymbol,
                        cancellationToken);

            if (measurementUnit == null)
            {
                return null;
            }

            // =====================================
            // 3. DETAYLI DTO DÖNÜŞÜMÜ
            // =====================================

            return await GetByIdAsync(
                measurementUnit.Id,
                cancellationToken);
        }

        #endregion

        #region GetByMaterialId

        /// <summary>
        /// Malzemenin bağlı olduğu ölçü birimini getirir.
        /// </summary>
        public async Task<GetByIdMeasurementUnitDto?>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                materialId,
                "Malzeme");

            var measurementUnit =
                await _measurementUnitRepository
                    .GetByMaterialIdAsync(
                        materialId,
                        cancellationToken);

            if (measurementUnit == null)
            {
                return null;
            }

            return await GetByIdAsync(
                measurementUnit.Id,
                cancellationToken);
        }

        #endregion

        #region Search

        /// <summary>
        /// Ölçü birimi adı veya sembolüne göre arama yapar.
        /// </summary>
        public async Task<IReadOnlyList<ResultMeasurementUnitDto>>
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
                    "Arama metni boş olamaz.");
            }

            string normalizedText = searchText.Trim();

            // =====================================
            // 2. REPOSITORY ARAMASI
            // =====================================

            var measurementUnits =
                await _measurementUnitRepository
                    .SearchByNameOrSymbolAsync(
                        normalizedText,
                        onlyActive,
                        cancellationToken);

            // =====================================
            // 3. DETAYLI DTO DÖNÜŞÜMÜ
            // =====================================

            return await MapResultsWithDetailsAsync(
                measurementUnits,
                cancellationToken);
        }

        #endregion

        #region GetUnitsWithMaterials

        /// <summary>
        /// En az bir malzeme bağlantısı olan
        /// ölçü birimlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetUnitsWithMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            var measurementUnits =
                await _measurementUnitRepository
                    .GetUnitsWithMaterialsAsync(
                        cancellationToken);

            return MapResults(
                measurementUnits
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region GetUnitsWithoutMaterials

        /// <summary>
        /// Hiçbir malzeme bağlantısı bulunmayan
        /// ölçü birimlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetUnitsWithoutMaterialsAsync(
                CancellationToken cancellationToken = default)
        {
            var measurementUnits =
                await _measurementUnitRepository
                    .GetUnitsWithoutMaterialsAsync(
                        cancellationToken);

            return await MapResultsWithDetailsAsync(
                measurementUnits,
                cancellationToken);
        }

        #endregion

        #region GetMaterialsByMeasurementUnitId

        /// <summary>
        /// Ölçü birimine bağlı bütün
        /// malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetMaterialsByMeasurementUnitIdAsync(
                int measurementUnitId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            // =====================================
            // 2. MALZEMELERİ GETİR
            // =====================================

            var materials =
                await _measurementUnitRepository
                    .GetMaterialsByMeasurementUnitIdAsync(
                        measurementUnitId,
                        cancellationToken);

            // =====================================
            // 3. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<List<ResultMaterialDto>>(
                materials);
        }

        #endregion

        #region GetActiveMaterialsByMeasurementUnitId

        /// <summary>
        /// Ölçü birimine bağlı aktif
        /// malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetActiveMaterialsByMeasurementUnitIdAsync(
                int measurementUnitId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMASI
            // =====================================

            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            // =====================================
            // 2. AKTİF MALZEMELERİ GETİR
            // =====================================

            var materials =
                await _measurementUnitRepository
                    .GetActiveMaterialsByMeasurementUnitIdAsync(
                        measurementUnitId,
                        cancellationToken);

            // =====================================
            // 3. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<List<ResultMaterialDto>>(
                materials);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni ölçü birimi oluşturur.
        ///
        /// Name ve Symbol benzersiz olmalıdır.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateMeasurementUnitDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            // =====================================
            // 2. AD VE SEMBOL NORMALİZASYONU
            // =====================================

            string name = ValidateName(
                dto.Name);

            string symbol = ValidateSymbol(
                dto.Symbol);

            // =====================================
            // 3. AD BENZERSİZLİĞİ
            // =====================================

            bool nameExists =
                await _measurementUnitRepository
                    .ExistsNameAsync(
                        name,
                        null,
                        cancellationToken);

            if (nameExists)
            {
                throw new ValidationException(
                    "Bu ölçü birimi adı sistemde " +
                    "zaten kayıtlıdır.");
            }

            // =====================================
            // 4. SEMBOL BENZERSİZLİĞİ
            // =====================================

            bool symbolExists =
                await _measurementUnitRepository
                    .ExistsSymbolAsync(
                        symbol,
                        null,
                        cancellationToken);

            if (symbolExists)
            {
                throw new ValidationException(
                    "Bu ölçü birimi sembolü sistemde " +
                    "zaten kayıtlıdır.");
            }

            // =====================================
            // 5. DTO -> ENTITY
            // =====================================

            var measurementUnit =
                _mapper.Map<MeasurementUnit>(dto);

            measurementUnit.Name = name;

            measurementUnit.Symbol = symbol;

            // =====================================
            // 6. BAŞLANGIÇ DURUMU
            // =====================================

            measurementUnit.IsActive = true;

            // CreatedAt ve UpdatedAt alanları
            // GenericRepository tarafından yönetilir.

            // =====================================
            // 7. REPOSITORY KAYIT İŞLEMİ
            // =====================================

            var createdMeasurementUnit =
                await _measurementUnitRepository
                    .CreateAsync(
                        measurementUnit,
                        cancellationToken);

            // =====================================
            // 8. ID DÖNDÜR
            // =====================================

            return createdMeasurementUnit.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Ölçü biriminin adını ve sembolünü günceller.
        ///
        /// ID, IsActive, CreatedAt ve Materials
        /// bilgileri korunur.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateMeasurementUnitDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(
                dto.Id,
                "Ölçü Birimi");

            string name = ValidateName(
                dto.Name);

            string symbol = ValidateSymbol(
                dto.Symbol);

            // =====================================
            // 2. MEVCUT KAYDI GETİR
            // =====================================

            var measurementUnit =
                await _measurementUnitRepository
                    .GetByIdAsync(
                        dto.Id,
                        cancellationToken);

            if (measurementUnit == null)
            {
                return false;
            }

            // =====================================
            // 3. AD BENZERSİZLİĞİ
            // =====================================

            bool nameExists =
                await _measurementUnitRepository
                    .ExistsNameAsync(
                        name,
                        dto.Id,
                        cancellationToken);

            if (nameExists)
            {
                throw new ValidationException(
                    "Bu ölçü birimi adı başka " +
                    "bir kayıt tarafından kullanılmaktadır.");
            }

            // =====================================
            // 4. SEMBOL BENZERSİZLİĞİ
            // =====================================

            bool symbolExists =
                await _measurementUnitRepository
                    .ExistsSymbolAsync(
                        symbol,
                        dto.Id,
                        cancellationToken);

            if (symbolExists)
            {
                throw new ValidationException(
                    "Bu sembol başka bir ölçü " +
                    "birimi tarafından kullanılmaktadır.");
            }

            // =====================================
            // 5. DTO -> MEVCUT ENTITY
            // =====================================

            _mapper.Map(
                dto,
                measurementUnit);

            // Normalize edilmiş değerler kullanılır.
            measurementUnit.Name = name;

            measurementUnit.Symbol = symbol;

            // IsActive, Id, CreatedAt ve Materials
            // mapping içerisinde Ignore edilir.

            // =====================================
            // 6. REPOSITORY GÜNCELLEME
            // =====================================

            return await _measurementUnitRepository
                .UpdateAsync(
                    measurementUnit,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Ölçü birimini aktif veya pasif yapar.
        ///
        /// Aktif malzemelerin kullandığı ölçü
        /// birimleri pasif hale getirilemez.
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
                "Ölçü Birimi");

            // =====================================
            // 2. MEVCUT KAYDI GETİR
            // =====================================

            var measurementUnit =
                await _measurementUnitRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (measurementUnit == null)
            {
                return false;
            }

            // =====================================
            // 3. DURUM DEĞİŞMEDİYSE
            // =====================================

            if (measurementUnit.IsActive == isActive)
            {
                return true;
            }

            // =====================================
            // 4. PASİFLEŞTİRME KONTROLÜ
            // =====================================

            if (!isActive)
            {
                bool hasActiveMaterials =
                    await _measurementUnitRepository
                        .HasActiveMaterialsAsync(
                            id,
                            cancellationToken);

                if (hasActiveMaterials)
                {
                    throw new InvalidOperationException(
                        "Bu ölçü birimi aktif malzemeler " +
                        "tarafından kullanılmaktadır. " +
                        "Öncelikle ilişkili aktif " +
                        "malzemeleri düzenleyiniz.");
                }
            }

            // =====================================
            // 5. REPOSITORY DURUM DEĞİŞİKLİĞİ
            // =====================================

            return await _measurementUnitRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Ölçü birimi adının kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedName =
                ValidateName(name);

            ValidateExcludedId(excludedId);

            return await _measurementUnitRepository
                .ExistsNameAsync(
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsSymbol

        /// <summary>
        /// Ölçü birimi sembolünün kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsSymbolAsync(
            string symbol,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedSymbol =
                ValidateSymbol(symbol);

            ValidateExcludedId(excludedId);

            return await _measurementUnitRepository
                .ExistsSymbolAsync(
                    normalizedSymbol,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasMaterials

        /// <summary>
        /// Ölçü biriminin en az bir
        /// malzemeyle ilişkisi var mı?
        /// </summary>
        public async Task<bool> HasMaterialsAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            return await _measurementUnitRepository
                .HasMaterialsAsync(
                    measurementUnitId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveMaterials

        /// <summary>
        /// Ölçü biriminin aktif bir
        /// malzemeyle ilişkisi var mı?
        /// </summary>
        public async Task<bool> HasActiveMaterialsAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            return await _measurementUnitRepository
                .HasActiveMaterialsAsync(
                    measurementUnitId,
                    cancellationToken);
        }

        #endregion

        #region GetMaterialCount

        /// <summary>
        /// Ölçü birimine bağlı toplam
        /// malzeme sayısını getirir.
        /// </summary>
        public async Task<int> GetMaterialCountAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            return await _measurementUnitRepository
                .GetMaterialCountAsync(
                    measurementUnitId,
                    cancellationToken);
        }

        #endregion

        #region GetActiveMaterialCount

        /// <summary>
        /// Ölçü birimine bağlı aktif
        /// malzeme sayısını getirir.
        /// </summary>
        public async Task<int> GetActiveMaterialCountAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                measurementUnitId,
                "Ölçü Birimi");

            return await _measurementUnitRepository
                .GetActiveMaterialCountAsync(
                    measurementUnitId,
                    cancellationToken);
        }

        #endregion

        #region Private Mapping Methods

        /// <summary>
        /// Navigation özellikleri yüklenmiş
        /// Entity kayıtlarını Result DTO'ya dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultMeasurementUnitDto> MapResults(
            IEnumerable<MeasurementUnit> measurementUnits)
        {
            return _mapper.Map<List<ResultMeasurementUnitDto>>(
                measurementUnits.ToList());
        }

        /// <summary>
        /// Repository'den gelen ölçü birimlerini
        /// Materials ilişkisi yüklenmiş kayıtlarla
        /// eşleştirip Result DTO'ya dönüştürür.
        ///
        /// Böylece MaterialCount doğru hesaplanır.
        /// </summary>
        private async Task<IReadOnlyList<ResultMeasurementUnitDto>>
            MapResultsWithDetailsAsync(
                IEnumerable<MeasurementUnit> measurementUnits,
                CancellationToken cancellationToken)
        {
            // =====================================
            // 1. SEÇİLEN KAYIT ID'LERİ
            // =====================================

            var selectedIds = measurementUnits
                .Select(x => x.Id)
                .ToHashSet();

            if (selectedIds.Count == 0)
            {
                return new List<ResultMeasurementUnitDto>();
            }

            // =====================================
            // 2. DETAYLI KAYITLAR
            // =====================================

            var detailedUnits =
                await _measurementUnitRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            // =====================================
            // 3. SEÇİLEN KAYITLARI FİLTRELE
            // =====================================

            var filteredUnits = detailedUnits
                .Where(x => selectedIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            // =====================================
            // 4. DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(filteredUnits);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DataAnnotations doğrulamalarını çalıştırır.
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
        /// Hariç tutulan ID değerini doğrular.
        /// </summary>
        private static void ValidateExcludedId(
            int? excludedId)
        {
            if (excludedId.HasValue && excludedId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(excludedId),
                    "Hariç tutulacak kayıt ID geçersiz.");
            }
        }

        /// <summary>
        /// Ölçü birimi adını doğrular ve temizler.
        /// </summary>
        private static string ValidateName(
            string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Ölçü birimi adı zorunludur.");
            }

            string normalized = name.Trim();

            if (normalized.Length > MaxNameLength)
            {
                throw new ValidationException(
                    "Ölçü birimi adı en fazla " +
                    "100 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// Ölçü birimi sembolünü doğrular ve temizler.
        /// </summary>
        private static string ValidateSymbol(
            string? symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
            {
                throw new ValidationException(
                    "Ölçü birimi sembolü zorunludur.");
            }

            string normalized = symbol.Trim();

            if (normalized.Length > MaxSymbolLength)
            {
                throw new ValidationException(
                    "Ölçü birimi sembolü en fazla " +
                    "20 karakter olabilir.");
            }

            return normalized;
        }

        #endregion
    }
}
