using AutoMapper;
using MaintenanceManagement.Application.DTOS.SiteLocationDtos;
using MaintenanceManagement.Application.DTOS.WorkRequestDtos;
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
    public class SiteLocationService : ISiteLocationService
    {
        private readonly ISiteLocationRepository _siteLocationRepository;
        private readonly IMapper _mapper;

        public SiteLocationService(
            ISiteLocationRepository siteLocationRepository,
            IMapper mapper)
        {
            _siteLocationRepository = siteLocationRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün konumları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultSiteLocationDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var locations = await _siteLocationRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultSiteLocationDto>>(
                locations);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Yalnızca aktif konumları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultSiteLocationDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var locations = await _siteLocationRepository
                .GetActiveLocationsAsync(cancellationToken);

            return _mapper.Map<List<ResultSiteLocationDto>>(
                locations);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Konum detayını, ilişkili iş talebi
        /// sayısıyla birlikte getirir.
        /// </summary>
        public async Task<GetByIdSiteLocationDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var location = await _siteLocationRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (location == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdSiteLocationDto>(
                location);
        }

        #endregion

        #region GetByName

        /// <summary>
        /// Konum adına göre detay bilgilerini getirir.
        /// </summary>
        public async Task<GetByIdSiteLocationDto?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            string normalizedName = ValidateAndNormalizeName(
                name);

            var location = await _siteLocationRepository
                .GetByNameAsync(
                    normalizedName,
                    cancellationToken);

            if (location == null)
            {
                return null;
            }

            // Detay DTO'sundaki WorkRequestCount
            // değerinin doğru doldurulmasını sağlar.
            return await GetByIdAsync(
                location.Id,
                cancellationToken);
        }

        #endregion

        #region GetByBuilding

        /// <summary>
        /// Bina bilgisine göre konumları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultSiteLocationDto>>
            GetByBuildingAsync(
                string building,
                CancellationToken cancellationToken = default)
        {
            string normalizedBuilding = ValidateRequiredText(
                building,
                100,
                "Bina bilgisi");

            var locations = await _siteLocationRepository
                .GetLocationsByBuildingAsync(
                    normalizedBuilding,
                    cancellationToken);

            return _mapper.Map<List<ResultSiteLocationDto>>(
                locations);
        }

        #endregion

        #region SearchByAddress

        /// <summary>
        /// Adres içerisinde metin araması yapar.
        /// </summary>
        public async Task<IReadOnlyList<ResultSiteLocationDto>>
            SearchByAddressAsync(
                string address,
                CancellationToken cancellationToken = default)
        {
            string normalizedAddress = ValidateRequiredText(
                address,
                500,
                "Adres arama metni");

            var locations = await _siteLocationRepository
                .SearchByAddressAsync(
                    normalizedAddress,
                    cancellationToken);

            return _mapper.Map<List<ResultSiteLocationDto>>(
                locations);
        }

        #endregion

        #region GetWithCoordinates

        /// <summary>
        /// Koordinatları tanımlanmış aktif
        /// konumları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultSiteLocationDto>>
            GetWithCoordinatesAsync(
                CancellationToken cancellationToken = default)
        {
            var locations = await _siteLocationRepository
                .GetLocationsWithCoordinatesAsync(
                    cancellationToken);

            return _mapper.Map<List<ResultSiteLocationDto>>(
                locations);
        }

        #endregion

        #region GetWorkRequestsByLocationId

        /// <summary>
        /// Konuma bağlı bütün iş taleplerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetWorkRequestsByLocationIdAsync(
                int locationId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(locationId, "Konum");

            var workRequests = await _siteLocationRepository
                .GetWorkRequestsByLocationIdAsync(
                    locationId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                workRequests);
        }

        #endregion

        #region GetNewWorkRequestsByLocationId

        /// <summary>
        /// Konuma bağlı Draft (Yeni) durumundaki
        /// iş taleplerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetNewWorkRequestsByLocationIdAsync(
                int locationId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(locationId, "Konum");

            var workRequests = await _siteLocationRepository
                .GetNewWorkRequestsByLocationIdAsync(
                    locationId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                workRequests);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni konum oluşturur.
        /// Konum adı sistem genelinde benzersizdir.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateSiteLocationDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // ======================================
            // 1. DTO DOĞRULAMASI
            // ======================================

            ValidateDto(dto);

            // ======================================
            // 2. AD VE ADRES BİLGİLERİ
            // ======================================

            string name = ValidateAndNormalizeName(
                dto.Name);

            string? address = NormalizeOptionalText(
                dto.Address,
                500,
                "Adres");

            string? building = NormalizeOptionalText(
                dto.Building,
                100,
                "Bina");

            string? floor = NormalizeOptionalText(
                dto.Floor,
                50,
                "Kat");

            // ======================================
            // 3. KOORDİNAT DOĞRULAMASI
            // ======================================

            ValidateCoordinates(
                dto.Latitude,
                dto.Longitude);

            // ======================================
            // 4. MÜKERRER KONUM KONTROLÜ
            // ======================================

            bool existsName = await _siteLocationRepository
                .ExistsNameAsync(
                    name,
                    null,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu isimde bir konum daha önce " +
                    "oluşturulmuştur.");
            }

            // ======================================
            // 5. DTO -> ENTITY
            // ======================================

            var location = _mapper.Map<SiteLocation>(dto);

            location.Name = name;
            location.Address = address;
            location.Building = building;
            location.Floor = floor;

            location.Latitude = dto.Latitude;
            location.Longitude = dto.Longitude;

            // Yeni konum aktif oluşturulur.
            location.IsActive = true;

            // ======================================
            // 6. KAYIT İŞLEMİ
            // ======================================

            var createdLocation = await _siteLocationRepository
                .CreateAsync(
                    location,
                    cancellationToken);

            return createdLocation.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut konumun ad, adres, bina, kat
        /// ve koordinat bilgilerini günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateSiteLocationDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // ======================================
            // 1. DTO DOĞRULAMASI
            // ======================================

            ValidateDto(dto);

            ValidateId(dto.Id, "Konum");

            // ======================================
            // 2. ALANLARI NORMALİZE ET
            // ======================================

            string name = ValidateAndNormalizeName(
                dto.Name);

            string? address = NormalizeOptionalText(
                dto.Address,
                500,
                "Adres");

            string? building = NormalizeOptionalText(
                dto.Building,
                100,
                "Bina");

            string? floor = NormalizeOptionalText(
                dto.Floor,
                50,
                "Kat");

            ValidateCoordinates(
                dto.Latitude,
                dto.Longitude);

            // ======================================
            // 3. MEVCUT KONUMU GETİR
            // ======================================

            var existingLocation = await _siteLocationRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingLocation == null)
            {
                return false;
            }

            // ======================================
            // 4. İSİM BENZERSİZLİĞİ
            // ======================================

            bool existsName = await _siteLocationRepository
                .ExistsNameAsync(
                    name,
                    existingLocation.Id,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu konum adı başka bir kayıt " +
                    "tarafından kullanılmaktadır.");
            }

            // ======================================
            // 5. DTO -> MEVCUT ENTITY
            // ======================================

            _mapper.Map(dto, existingLocation);

            existingLocation.Name = name;
            existingLocation.Address = address;
            existingLocation.Building = building;
            existingLocation.Floor = floor;

            existingLocation.Latitude = dto.Latitude;
            existingLocation.Longitude = dto.Longitude;

            // Id, IsActive, CreatedAt, UpdatedAt
            // ve WorkRequests korunur.

            // ======================================
            // 6. GÜNCELLEME
            // ======================================

            return await _siteLocationRepository
                .UpdateAsync(
                    existingLocation,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Konumu aktif veya pasif yapar.
        /// Yeni iş talepleri bulunan konum
        /// pasife alınamaz.
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

            // ======================================
            // 1. KONUMU GETİR
            // ======================================

            var location = await _siteLocationRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (location == null)
            {
                return false;
            }

            // ======================================
            // 2. MEVCUT DURUMU KONTROL ET
            // ======================================

            if (location.IsActive == isActive)
            {
                return true;
            }

            // ======================================
            // 3. PASİFE ALMA KONTROLÜ
            // ======================================

            if (!isActive)
            {
                bool hasNewWorkRequests =
                    await _siteLocationRepository
                        .HasNewWorkRequestsAsync(
                            id,
                            cancellationToken);

                if (hasNewWorkRequests)
                {
                    throw new InvalidOperationException(
                        "Bu konuma bağlı yeni iş talepleri " +
                        "bulunmaktadır. Konum pasife " +
                        "alınmadan önce ilgili talepler " +
                        "sonuçlandırılmalıdır.");
                }
            }

            // ======================================
            // 4. DURUMU GÜNCELLE
            // ======================================

            return await _siteLocationRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Konum adının sistem genelinde
        /// benzersiz olup olmadığını kontrol eder.
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
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak Konum");
            }

            return await _siteLocationRepository
                .ExistsNameAsync(
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasWorkRequests

        /// <summary>
        /// Konumun herhangi bir iş talebinde
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasWorkRequestsAsync(
            int locationId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(locationId, "Konum");

            return await _siteLocationRepository
                .HasWorkRequestsAsync(
                    locationId,
                    cancellationToken);
        }

        #endregion

        #region HasNewWorkRequests

        /// <summary>
        /// Konumda Yeni (Draft) durumunda
        /// iş talebi bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasNewWorkRequestsAsync(
            int locationId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(locationId, "Konum");

            return await _siteLocationRepository
                .HasNewWorkRequestsAsync(
                    locationId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO DataAnnotations kurallarını çalıştırır.
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
        /// Pozitif ID kontrolü gerçekleştirir.
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
        /// Konum adını doğrular ve temizler.
        /// </summary>
        private static string ValidateAndNormalizeName(
            string? name)
        {
            return ValidateRequiredText(
                name,
                200,
                "Konum adı");
        }

        /// <summary>
        /// Zorunlu metinleri doğrular.
        /// </summary>
        private static string ValidateRequiredText(
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
                    $"{fieldName} en fazla " +
                    $"{maxLength} karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// İsteğe bağlı metinleri doğrular.
        /// Boş alanları null olarak döndürür.
        /// </summary>
        private static string? NormalizeOptionalText(
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
                    $"{fieldName} en fazla " +
                    $"{maxLength} karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// Enlem ve boylam değerlerini doğrular.
        /// Koordinat giriliyorsa iki değer de bulunmalıdır.
        /// </summary>
        private static void ValidateCoordinates(
            decimal? latitude,
            decimal? longitude)
        {
            // Koordinatlar isteğe bağlıdır.
            // Ancak biri girilmişse diğeri de girilmelidir.
            if (latitude.HasValue != longitude.HasValue)
            {
                throw new ValidationException(
                    "Enlem ve boylam bilgileri " +
                    "birlikte girilmelidir.");
            }

            if (latitude.HasValue)
            {
                if (latitude.Value < -90m ||
                    latitude.Value > 90m)
                {
                    throw new ValidationException(
                        "Enlem -90 ile +90 " +
                        "arasında olmalıdır.");
                }
            }

            if (longitude.HasValue)
            {
                if (longitude.Value < -180m ||
                    longitude.Value > 180m)
                {
                    throw new ValidationException(
                        "Boylam -180 ile +180 " +
                        "arasında olmalıdır.");
                }
            }
        }

        #endregion
    }
}
