using AutoMapper;
using MaintenanceManagement.Application.DTOS.PlannedMaintenancesDtos;
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
    public class PlannedMaintenanceService
       : IPlannedMaintenanceService
    {
        #region Constants

        private const string ManagePermission =
            "WORK_ORDER_CREATE";

        private const int MaximumUpcomingDays = 3650;

        #endregion

        #region Fields

        private readonly IPlannedMaintenanceRepository
            _plannedMaintenanceRepository;

        private readonly IAssetRepository
            _assetRepository;

        private readonly IUnitRepository
            _unitRepository;

        private readonly IWorkSubTypeRepository
            _workSubTypeRepository;

        private readonly IUserRepository
            _userRepository;

        private readonly IRoleWorkSubTypePermissionService
            _roleWorkSubTypePermissionService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public PlannedMaintenanceService(
            IPlannedMaintenanceRepository plannedMaintenanceRepository,
            IAssetRepository assetRepository,
            IUnitRepository unitRepository,
            IWorkSubTypeRepository workSubTypeRepository,
            IUserRepository userRepository,
            IRoleWorkSubTypePermissionService
                roleWorkSubTypePermissionService,
            IMapper mapper)
        {
            _plannedMaintenanceRepository =
                plannedMaintenanceRepository;

            _assetRepository =
                assetRepository;

            _unitRepository =
                unitRepository;

            _workSubTypeRepository =
                workSubTypeRepository;

            _userRepository =
                userRepository;

            _roleWorkSubTypePermissionService =
                roleWorkSubTypePermissionService;

            _mapper = mapper;
        }

        #endregion

        #region GetAll

        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances.OrderBy(x => x.Name));
        }

        #endregion

        #region GetById

        public async Task<GetByIdPlannedMaintenanceDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var maintenance = await _plannedMaintenanceRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (maintenance == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdPlannedMaintenanceDto>(
                maintenance);
        }

        #endregion

        #region GetActive

        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x => x.IsActive)
                    .OrderBy(x => x.NextMaintenanceDate));
        }

        #endregion

        #region GetByAssetId

        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x => x.AssetId == assetId)
                    .OrderBy(x => x.NextMaintenanceDate));
        }

        #endregion

        #region GetByUnitId

        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x => x.UnitId == unitId)
                    .OrderBy(x => x.NextMaintenanceDate));
        }

        #endregion

        #region GetByWorkSubTypeId

        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x => x.WorkSubTypeId == workSubTypeId)
                    .OrderBy(x => x.NextMaintenanceDate));
        }

        #endregion

        #region GetByCreatedByUserId

        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByCreatedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x => x.CreatedByUserId == userId)
                    .OrderBy(x => x.Name));
        }

        #endregion

        #region GetDueMaintenances

        /// <summary>
        /// Bakım tarihi bugün veya daha önce olan
        /// aktif bakım planlarını getirir.
        /// Geciken bakımları da kapsar.
        /// </summary>
        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetDueMaintenancesAsync(
                DateTime referenceDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateUtc(referenceDateUtc);

            DateTime today = referenceDateUtc.Date;

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x =>
                        x.IsActive &&
                        GetNextDate(x).HasValue &&
                        GetNextDate(x)!.Value.Date <= today)
                    .OrderBy(x => x.NextMaintenanceDate));
        }

        #endregion

        #region GetOverdueMaintenances

        /// <summary>
        /// Bakım tarihi bugünden önce olan
        /// aktif bakım planlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetOverdueMaintenancesAsync(
                DateTime referenceDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateUtc(referenceDateUtc);

            DateTime today = referenceDateUtc.Date;

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x =>
                        x.IsActive &&
                        GetNextDate(x).HasValue &&
                        GetNextDate(x)!.Value.Date < today)
                    .OrderBy(x => x.NextMaintenanceDate));
        }

        #endregion

        #region GetUpcomingMaintenances

        /// <summary>
        /// Bugün ile belirtilen gün sonrası arasındaki
        /// aktif bakım planlarını getirir.
        /// Gecikmiş planlar bu sorguya dahil değildir.
        /// </summary>
        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetUpcomingMaintenancesAsync(
                int days,
                DateTime referenceDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateUtc(referenceDateUtc);

            if (days < 0 || days > MaximumUpcomingDays)
            {
                throw new ValidationException(
                    "Gün sayısı 0 ile 3650 arasında olmalıdır.");
            }

            DateTime startDate = referenceDateUtc.Date;

            DateTime endDate = startDate.AddDays(days);

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x =>
                        x.IsActive &&
                        GetNextDate(x).HasValue &&
                        GetNextDate(x)!.Value.Date >= startDate &&
                        GetNextDate(x)!.Value.Date <= endDate)
                    .OrderBy(x => x.NextMaintenanceDate));
        }

        #endregion

        #region GetByNextMaintenanceDateRange

        public async Task<IReadOnlyList<ResultPlannedMaintenanceDto>>
            GetByNextMaintenanceDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return MapResults(
                maintenances
                    .Where(x =>
                        GetNextDate(x).HasValue &&
                        GetNextDate(x)!.Value >= startDateUtc &&
                        GetNextDate(x)!.Value <= endDateUtc)
                    .OrderBy(x => x.NextMaintenanceDate));
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni planlı bakım oluşturur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreatePlannedMaintenanceDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // ======================================
            // 1. DTO DOĞRULAMASI
            // ======================================

            ValidateDto(dto);

            ValidateId(currentUserId, "Kullanıcı");

            // ======================================
            // 2. DTO -> ENTITY
            // ======================================

            var maintenance =
                _mapper.Map<PlannedMaintenance>(dto);

            // ======================================
            // 3. TEMEL BİLGİLER
            // ======================================

            maintenance.Name = ValidateName(
                maintenance.Name);

            ValidatePeriodDays(
                maintenance.PeriodDays);

            maintenance.Description =
                NormalizeDescription(maintenance.Description);

            // ======================================
            // 4. İLİŞKİLERİ DOĞRULA
            // ======================================

            await ValidateRelationsAsync(
                maintenance.AssetId,
                maintenance.UnitId,
                maintenance.WorkSubTypeId,
                cancellationToken);

            // ======================================
            // 5. KULLANICI YETKİSİ
            // ======================================

            await ValidateManagerPermissionAsync(
                currentUserId,
                maintenance.UnitId,
                maintenance.WorkSubTypeId,
                cancellationToken);

            // ======================================
            // 6. BAKIM TARİHLERİ
            // ======================================

            PrepareMaintenanceDates(
                maintenance,
                createIfMissing: true);

            // ======================================
            // 7. SİSTEM ALANLARI
            // ======================================

            maintenance.CreatedByUserId = currentUserId;

            maintenance.IsActive = true;

            // ======================================
            // 8. VERİTABANINA KAYDET
            // ======================================

            var createdMaintenance =
                await _plannedMaintenanceRepository
                    .CreateAsync(
                        maintenance,
                        cancellationToken);

            return createdMaintenance.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Bakım planının düzenlenebilir alanlarını
        /// günceller.
        ///
        /// Demirbaş, birim, iş alt türü ve
        /// geçmiş bakım tarihi korunur.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdatePlannedMaintenanceDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // ======================================
            // 1. DOĞRULAMA
            // ======================================

            ValidateDto(dto);

            ValidateId(dto.Id, "Planlı Bakım");

            ValidateId(currentUserId, "Kullanıcı");

            // ======================================
            // 2. MEVCUT BAKIM PLANINI GETİR
            // ======================================

            var maintenance = await _plannedMaintenanceRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (maintenance == null)
            {
                return false;
            }

            // ======================================
            // 3. YETKİ KONTROLÜ
            // ======================================

            await ValidateManagerPermissionAsync(
                currentUserId,
                maintenance.UnitId,
                maintenance.WorkSubTypeId,
                cancellationToken);

            // ======================================
            // 4. KORUNACAK ALANLAR
            // ======================================

            int assetId = maintenance.AssetId;

            int unitId = maintenance.UnitId;

            int workSubTypeId = maintenance.WorkSubTypeId;

            int? createdByUserId =
                maintenance.CreatedByUserId;

            DateTime? lastMaintenanceDate =
                maintenance.LastMaintenanceDate;

            DateTime? oldNextMaintenanceDate =
                maintenance.NextMaintenanceDate;

            bool isActive = maintenance.IsActive;

            // ======================================
            // 5. DTO -> MEVCUT ENTITY
            // ======================================

            _mapper.Map(dto, maintenance);

            // ======================================
            // 6. İLİŞKİLERİ VE GEÇMİŞİ KORU
            // ======================================

            maintenance.AssetId = assetId;

            maintenance.UnitId = unitId;

            maintenance.WorkSubTypeId = workSubTypeId;

            maintenance.CreatedByUserId = createdByUserId;

            maintenance.LastMaintenanceDate =
                lastMaintenanceDate;

            maintenance.IsActive = isActive;

            // ======================================
            // 7. ALAN DOĞRULAMASI
            // ======================================

            maintenance.Name =
                ValidateName(maintenance.Name);

            maintenance.Description =
                NormalizeDescription(maintenance.Description);

            ValidatePeriodDays(maintenance.PeriodDays);

            // DTO sonraki tarihi değiştirmiyorsa
            // önceki geçerli tarih korunur.
            DateTime? mappedNextDate =
                maintenance.NextMaintenanceDate;

            if (!mappedNextDate.HasValue ||
                mappedNextDate.Value == DateTime.MinValue)
            {
                if (oldNextMaintenanceDate.HasValue)
                {
                    maintenance.NextMaintenanceDate =
                        oldNextMaintenanceDate.Value;
                }
            }

            PrepareMaintenanceDates(
                maintenance,
                createIfMissing: true);

            // ======================================
            // 8. GÜNCELLEME
            // ======================================

            return await _plannedMaintenanceRepository
                .UpdateAsync(
                    maintenance,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Planlı bakımı aktif veya pasif yapar.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "Planlı Bakım");

            ValidateId(currentUserId, "Kullanıcı");

            // ======================================
            // 1. BAKIM PLANINI GETİR
            // ======================================

            var maintenance = await _plannedMaintenanceRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (maintenance == null)
            {
                return false;
            }

            // ======================================
            // 2. YETKİ KONTROLÜ
            // ======================================

            await ValidateManagerPermissionAsync(
                currentUserId,
                maintenance.UnitId,
                maintenance.WorkSubTypeId,
                cancellationToken);

            // ======================================
            // 3. MEVCUT DURUM
            // ======================================

            if (maintenance.IsActive == isActive)
            {
                return true;
            }

            // ======================================
            // 4. AKTİFLEŞTİRME DOĞRULAMASI
            // ======================================

            if (isActive)
            {
                await ValidateRelationsAsync(
                    maintenance.AssetId,
                    maintenance.UnitId,
                    maintenance.WorkSubTypeId,
                    cancellationToken);

                ValidatePeriodDays(
                    maintenance.PeriodDays);

                PrepareMaintenanceDates(
                    maintenance,
                    createIfMissing: true);
            }

            // ======================================
            // 5. DURUM GÜNCELLEME
            // ======================================

            maintenance.IsActive = isActive;

            return await _plannedMaintenanceRepository
                .UpdateAsync(
                    maintenance,
                    cancellationToken);
        }

        #endregion

        #region GetDueMaintenanceCount

        public async Task<int> GetDueMaintenanceCountAsync(
            DateTime referenceDateUtc,
            CancellationToken cancellationToken = default)
        {
            ValidateUtc(referenceDateUtc);

            DateTime today = referenceDateUtc.Date;

            var maintenances = await GetAllEntitiesAsync(
                cancellationToken);

            return maintenances.Count(x =>
                x.IsActive &&
                GetNextDate(x).HasValue &&
                GetNextDate(x)!.Value.Date <= today);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Repository'deki mevcut detaylı liste
        /// sorgusunu kullanır.
        /// </summary>
        private async Task<List<PlannedMaintenance>>
            GetAllEntitiesAsync(
                CancellationToken cancellationToken)
        {
            var maintenances =
                await _plannedMaintenanceRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            return maintenances.ToList();
        }

        /// <summary>
        /// Entity listesini DTO listesine dönüştürür.
        /// </summary>
        private List<ResultPlannedMaintenanceDto> MapResults(
            IEnumerable<PlannedMaintenance> maintenances)
        {
            return _mapper.Map<List<ResultPlannedMaintenanceDto>>(
                maintenances.ToList());
        }

        /// <summary>
        /// Nullable veya nullable olmayan tarih
        /// alanlarıyla çalışabilen yardımcı metot.
        /// </summary>
        private static DateTime? GetNextDate(
            PlannedMaintenance maintenance)
        {
            DateTime? date = maintenance.NextMaintenanceDate;

            if (!date.HasValue ||
                date.Value == DateTime.MinValue)
            {
                return null;
            }

            return date;
        }

        /// <summary>
        /// Bakım tarihi girilmemişse
        /// belirlenen periyoda göre oluşturur.
        /// </summary>
        private static void PrepareMaintenanceDates(
            PlannedMaintenance maintenance,
            bool createIfMissing)
        {
            ValidatePeriodDays(maintenance.PeriodDays);

            DateTime? lastDate =
                maintenance.LastMaintenanceDate;

            DateTime? nextDate =
                GetNextDate(maintenance);

            if (!nextDate.HasValue && createIfMissing)
            {
                DateTime baseDate =
                    lastDate.HasValue &&
                    lastDate.Value != DateTime.MinValue
                        ? lastDate.Value
                        : DateTime.UtcNow;

                try
                {
                    maintenance.NextMaintenanceDate =
                        baseDate.AddDays(
                            maintenance.PeriodDays);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new ValidationException(
                        "Bakım periyoduna göre hesaplanan " +
                        "sonraki tarih geçersiz.");
                }

                nextDate = maintenance.NextMaintenanceDate;
            }

            if (!nextDate.HasValue)
            {
                throw new ValidationException(
                    "Sonraki bakım tarihi zorunludur.");
            }

            if (lastDate.HasValue &&
                lastDate.Value != DateTime.MinValue &&
                nextDate.Value <= lastDate.Value)
            {
                throw new ValidationException(
                    "Sonraki bakım tarihi son bakım " +
                    "tarihinden sonra olmalıdır.");
            }
        }

        /// <summary>
        /// Demirbaş, birim ve iş alt türü
        /// ilişkilerini kontrol eder.
        /// </summary>
        private async Task ValidateRelationsAsync(
            int assetId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken)
        {
            ValidateId(assetId, "Demirbaş");

            ValidateId(unitId, "Organizasyon Birimi");

            ValidateId(workSubTypeId, "İş Alt Türü");

            // ======================================
            // 1. DEMİRBAŞ
            // ======================================

            var asset = await _assetRepository
                .GetByIdAsync(
                    assetId,
                    cancellationToken);

            if (asset == null)
            {
                throw new InvalidOperationException(
                    "Demirbaş bulunamadı.");
            }

            if (asset.Status == AssetStatusEnum.Scrapped)
            {
                throw new InvalidOperationException(
                    "Hurdaya ayrılmış demirbaş için " +
                    "aktif bakım planı oluşturulamaz.");
            }

            // ======================================
            // 2. ORGANİZASYON BİRİMİ
            // ======================================

            var unit = await _unitRepository
                .GetByIdAsync(
                    unitId,
                    cancellationToken);

            if (unit == null || !unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Sorumlu organizasyon birimi " +
                    "bulunamadı veya pasif durumdadır.");
            }

            // ======================================
            // 3. İŞ ALT TÜRÜ
            // ======================================

            var workSubType = await _workSubTypeRepository
                .GetByIdAsync(
                    workSubTypeId,
                    cancellationToken);

            if (workSubType == null ||
                !workSubType.IsActive)
            {
                throw new InvalidOperationException(
                    "İş alt türü bulunamadı veya " +
                    "pasif durumdadır.");
            }

            // ======================================
            // 4. BİRİM - İŞ ALT TÜRÜ UYGUNLUĞU
            // ======================================

            bool isAvailable = await _workSubTypeRepository
                .IsAvailableForUnitAsync(
                    unitId,
                    workSubTypeId,
                    cancellationToken);

            if (!isAvailable)
            {
                throw new InvalidOperationException(
                    "Seçilen iş alt türü sorumlu " +
                    "organizasyon biriminde kullanılamaz.");
            }
        }

        /// <summary>
        /// Planlı bakım yönetimi işlemini yapan
        /// kullanıcının aktif ve yetkili olduğunu doğrular.
        /// </summary>
        private async Task ValidateManagerPermissionAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByIdAsync(
                    userId,
                    cancellationToken);

            if (user == null ||
                !user.IsActive ||
                !user.HasSystemAccess)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı aktif değildir veya " +
                    "sistem erişimi bulunmamaktadır.");
            }

            bool hasPermission =
                await _roleWorkSubTypePermissionService
                    .HasUserActivePermissionAsync(
                        userId,
                        unitId,
                        workSubTypeId,
                        ManagePermission,
                        cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının bu birim ve iş alt " +
                    "türünde bakım planı yönetme " +
                    "yetkisi bulunmamaktadır.");
            }
        }

        private static void ValidateDto<T>(T dto)
            where T : class
        {
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);
        }

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

        private static string ValidateName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Bakım planı adı zorunludur.");
            }

            return name.Trim();
        }

        private static string? NormalizeDescription(
            string? description)
        {
            return string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();
        }

        private static void ValidatePeriodDays(int periodDays)
        {
            if (periodDays <= 0)
            {
                throw new ValidationException(
                    "Bakım periyodu sıfırdan büyük olmalıdır.");
            }
        }

        private static void ValidateUtc(DateTime date)
        {
            if (date.Kind != DateTimeKind.Utc)
            {
                throw new ValidationException(
                    "Referans tarihi UTC olarak gönderilmelidir.");
            }
        }

        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            ValidateUtc(startDateUtc);
            ValidateUtc(endDateUtc);

            if (endDateUtc < startDateUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç " +
                    "tarihinden önce olamaz.");
            }
        }

        #endregion
    }
}
