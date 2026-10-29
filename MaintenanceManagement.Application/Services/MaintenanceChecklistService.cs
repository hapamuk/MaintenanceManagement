using AutoMapper;
using MaintenanceManagement.Application.DTOS.MaintenanceChecklistDtos;
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
    public class MaintenanceChecklistService
      : IMaintenanceChecklistService
    {
        #region Constants

        private const string ManagePermission =
            "WORK_ORDER_CREATE";

        private const int MaxTitleLength = 200;

        private const int MaxDescriptionLength = 1000;

        #endregion

        #region Fields

        private readonly IMaintenanceChecklistRepository
            _maintenanceChecklistRepository;

        private readonly IPlannedMaintenanceRepository
            _plannedMaintenanceRepository;

        private readonly IUserRepository _userRepository;

        private readonly IRoleWorkSubTypePermissionService
            _roleWorkSubTypePermissionService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public MaintenanceChecklistService(
            IMaintenanceChecklistRepository
                maintenanceChecklistRepository,
            IPlannedMaintenanceRepository
                plannedMaintenanceRepository,
            IUserRepository userRepository,
            IRoleWorkSubTypePermissionService
                roleWorkSubTypePermissionService,
            IMapper mapper)
        {
            _maintenanceChecklistRepository =
                maintenanceChecklistRepository;

            _plannedMaintenanceRepository =
                plannedMaintenanceRepository;

            _userRepository =
                userRepository;

            _roleWorkSubTypePermissionService =
                roleWorkSubTypePermissionService;

            _mapper = mapper;
        }

        #endregion

        #region GetAll

        public async Task<
            IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var checklists =
                await _maintenanceChecklistRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            return MapResults(
                checklists
                    .OrderBy(x => x.PlannedMaintenanceId)
                    .ThenBy(x => x.SequenceNumber)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region GetById

        public async Task<GetByIdMaintenanceChecklistDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var checklist =
                await _maintenanceChecklistRepository
                    .GetByIdWithDetailsAsync(
                        id,
                        cancellationToken);

            if (checklist == null)
            {
                return null;
            }

            return _mapper.Map<
                GetByIdMaintenanceChecklistDto>(
                    checklist);
        }

        #endregion

        #region GetByPlannedMaintenanceId

        public async Task<
            IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            var checklists =
                await _maintenanceChecklistRepository
                    .GetByPlannedMaintenanceIdAsync(
                        plannedMaintenanceId,
                        cancellationToken);

            return MapResults(
                checklists
                    .OrderBy(x => x.SequenceNumber)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region GetActiveByPlannedMaintenanceId

        public async Task<
            IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetActiveByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            var checklists =
                await _maintenanceChecklistRepository
                    .GetActiveByPlannedMaintenanceIdAsync(
                        plannedMaintenanceId,
                        cancellationToken);

            return MapResults(
                checklists
                    .OrderBy(x => x.SequenceNumber)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region GetActiveRequiredByPlannedMaintenanceId

        public async Task<
            IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetActiveRequiredByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            var checklists =
                await _maintenanceChecklistRepository
                    .GetActiveRequiredByPlannedMaintenanceIdAsync(
                        plannedMaintenanceId,
                        cancellationToken);

            return MapResults(
                checklists
                    .OrderBy(x => x.SequenceNumber)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region GetByAssetId

        public async Task<
            IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            var checklists =
                await _maintenanceChecklistRepository
                    .GetByAssetIdAsync(
                        assetId,
                        cancellationToken);

            return MapResults(checklists);
        }

        #endregion

        #region GetByUnitId

        public async Task<
            IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var checklists =
                await _maintenanceChecklistRepository
                    .GetByUnitIdAsync(
                        unitId,
                        cancellationToken);

            return MapResults(checklists);
        }

        #endregion

        #region GetByWorkSubTypeId

        public async Task<
            IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            var checklists =
                await _maintenanceChecklistRepository
                    .GetByWorkSubTypeIdAsync(
                        workSubTypeId,
                        cancellationToken);

            return MapResults(checklists);
        }

        #endregion

        #region GetBySequenceNumber

        public async Task<GetByIdMaintenanceChecklistDto?>
            GetBySequenceNumberAsync(
                int plannedMaintenanceId,
                int sequenceNumber,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            ValidateSequenceNumber(sequenceNumber);

            var checklist =
                await _maintenanceChecklistRepository
                    .GetBySequenceNumberAsync(
                        plannedMaintenanceId,
                        sequenceNumber,
                        cancellationToken);

            if (checklist == null)
            {
                return null;
            }

            // İlişkili alanların da yüklenmesi
            // için detay sorgusunu kullanıyoruz.
            return await GetByIdAsync(
                checklist.Id,
                cancellationToken);
        }

        #endregion

        #region GetUncompletedRequiredByWorkOrderId

        public async Task<
            IReadOnlyList<ResultMaintenanceChecklistDto>>
            GetUncompletedRequiredByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var checklists =
                await _maintenanceChecklistRepository
                    .GetUncompletedRequiredChecklistsByWorkOrderIdAsync(
                        workOrderId,
                        cancellationToken);

            return MapResults(
                checklists
                    .OrderBy(x => x.SequenceNumber)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region Create

        /// <summary>
        /// Planlı bakıma yeni kontrol maddesi ekler.
        /// Başlık ve sıra numarası aynı plan
        /// içerisinde benzersiz olmalıdır.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateMaintenanceChecklistDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(
                dto.PlannedMaintenanceId,
                "Planlı Bakım");

            ValidateId(currentUserId, "Kullanıcı");

            string title = ValidateTitle(dto.Title);

            string? description =
                NormalizeDescription(dto.Description);

            ValidateSequenceNumber(dto.SequenceNumber);

            // =====================================
            // 2. PLANLI BAKIMI GETİR
            // =====================================

            var maintenance =
                await GetPlannedMaintenanceOrThrowAsync(
                    dto.PlannedMaintenanceId,
                    requireActive: true,
                    cancellationToken);

            // =====================================
            // 3. KULLANICI YETKİSİ
            // =====================================

            await ValidateManagePermissionAsync(
                maintenance,
                currentUserId,
                cancellationToken);

            // =====================================
            // 4. AYNI BAŞLIK KONTROLÜ
            // =====================================

            bool titleExists =
                await _maintenanceChecklistRepository
                    .ExistsTitleAsync(
                        dto.PlannedMaintenanceId,
                        title,
                        null,
                        cancellationToken);

            if (titleExists)
            {
                throw new ValidationException(
                    "Bu bakım planında aynı başlığa " +
                    "sahip kontrol maddesi bulunmaktadır.");
            }

            // =====================================
            // 5. SIRA NUMARASI KONTROLÜ
            // =====================================

            bool sequenceExists =
                await _maintenanceChecklistRepository
                    .ExistsSequenceNumberAsync(
                        dto.PlannedMaintenanceId,
                        dto.SequenceNumber,
                        null,
                        cancellationToken);

            if (sequenceExists)
            {
                throw new ValidationException(
                    "Bu sıra numarası aynı bakım " +
                    "planında kullanılmaktadır.");
            }

            // =====================================
            // 6. DTO -> ENTITY
            // =====================================

            var checklist =
                _mapper.Map<MaintenanceChecklist>(dto);

            checklist.PlannedMaintenanceId =
                dto.PlannedMaintenanceId;

            checklist.Title = title;

            checklist.Description = description;

            checklist.SequenceNumber =
                dto.SequenceNumber;

            checklist.IsRequired =
                dto.IsRequired;

            checklist.IsActive = true;

            checklist.CreatedAt = DateTime.UtcNow;

            checklist.UpdatedAt = null;

            // =====================================
            // 7. REPOSITORY KAYDI
            // =====================================

            var createdChecklist =
                await _maintenanceChecklistRepository
                    .CreateAsync(
                        checklist,
                        cancellationToken);

            return createdChecklist.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Kontrol maddesinin düzenlenebilir
        /// özelliklerini günceller.
        ///
        /// İş emri sonuç kaydı bulunan maddeler
        /// normal yöntemle değiştirilemez.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateMaintenanceChecklistDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.Id, "Kontrol Maddesi");

            ValidateId(currentUserId, "Kullanıcı");

            string title = ValidateTitle(dto.Title);

            string? description =
                NormalizeDescription(dto.Description);

            ValidateSequenceNumber(dto.SequenceNumber);

            // =====================================
            // 2. MEVCUT KAYDI GETİR
            // =====================================

            var checklist =
                await _maintenanceChecklistRepository
                    .GetByIdAsync(
                        dto.Id,
                        cancellationToken);

            if (checklist == null)
            {
                return false;
            }

            // =====================================
            // 3. BAKIM PLANI VE YETKİ
            // =====================================

            var maintenance =
                await GetPlannedMaintenanceOrThrowAsync(
                    checklist.PlannedMaintenanceId,
                    requireActive: true,
                    cancellationToken);

            await ValidateManagePermissionAsync(
                maintenance,
                currentUserId,
                cancellationToken);

            // =====================================
            // 4. GEÇMİŞ SONUÇ KONTROLÜ
            // =====================================

            await EnsureChecklistHasNoResultsAsync(
                checklist.Id,
                cancellationToken);

            // =====================================
            // 5. BAŞLIK BENZERSİZLİĞİ
            // =====================================

            bool titleExists =
                await _maintenanceChecklistRepository
                    .ExistsTitleAsync(
                        checklist.PlannedMaintenanceId,
                        title,
                        checklist.Id,
                        cancellationToken);

            if (titleExists)
            {
                throw new ValidationException(
                    "Bu bakım planında aynı başlığa " +
                    "sahip başka bir kontrol maddesi var.");
            }

            // =====================================
            // 6. SIRA BENZERSİZLİĞİ
            // =====================================

            bool sequenceExists =
                await _maintenanceChecklistRepository
                    .ExistsSequenceNumberAsync(
                        checklist.PlannedMaintenanceId,
                        dto.SequenceNumber,
                        checklist.Id,
                        cancellationToken);

            if (sequenceExists)
            {
                throw new ValidationException(
                    "Bu sıra numarası başka bir " +
                    "kontrol maddesinde kullanılmaktadır.");
            }

            // =====================================
            // 7. DTO -> MEVCUT ENTITY
            // =====================================

            _mapper.Map(dto, checklist);

            checklist.Title = title;

            checklist.Description = description;

            checklist.SequenceNumber =
                dto.SequenceNumber;

            checklist.IsRequired =
                dto.IsRequired;

            // Planlı bakım ilişkisi,
            // IsActive ve CreatedAt korunur.

            checklist.UpdatedAt = DateTime.UtcNow;

            // =====================================
            // 8. GÜNCELLEME
            // =====================================

            return await _maintenanceChecklistRepository
                .UpdateAsync(
                    checklist,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Kontrol maddesinin aktiflik
        /// durumunu değiştirir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "Kontrol Maddesi");

            ValidateId(currentUserId, "Kullanıcı");

            // =====================================
            // 1. KONTROL MADDESİNİ GETİR
            // =====================================

            var checklist =
                await _maintenanceChecklistRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (checklist == null)
            {
                return false;
            }

            // =====================================
            // 2. PLANLI BAKIM
            // =====================================

            var maintenance =
                await GetPlannedMaintenanceOrThrowAsync(
                    checklist.PlannedMaintenanceId,
                    requireActive: isActive,
                    cancellationToken);

            // =====================================
            // 3. YETKİ KONTROLÜ
            // =====================================

            await ValidateManagePermissionAsync(
                maintenance,
                currentUserId,
                cancellationToken);

            // =====================================
            // 4. MEVCUT DURUM KONTROLÜ
            // =====================================

            if (checklist.IsActive == isActive)
            {
                return true;
            }

            // =====================================
            // 5. SONUÇ GEÇMİŞİNİ KORU
            // =====================================

            await EnsureChecklistHasNoResultsAsync(
                checklist.Id,
                cancellationToken);

            // =====================================
            // 6. REPOSITORY DURUM DEĞİŞİKLİĞİ
            // =====================================

            return await _maintenanceChecklistRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region UpdateSequenceNumber

        /// <summary>
        /// Kontrol maddesinin sıra numarasını
        /// aynı plan içerisinde değiştirir.
        /// </summary>
        public async Task<bool> UpdateSequenceNumberAsync(
            int id,
            int sequenceNumber,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "Kontrol Maddesi");

            ValidateId(currentUserId, "Kullanıcı");

            ValidateSequenceNumber(sequenceNumber);

            // =====================================
            // 1. MEVCUT KAYDI GETİR
            // =====================================

            var checklist =
                await _maintenanceChecklistRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (checklist == null)
            {
                return false;
            }

            // =====================================
            // 2. BAKIM PLANI
            // =====================================

            var maintenance =
                await GetPlannedMaintenanceOrThrowAsync(
                    checklist.PlannedMaintenanceId,
                    requireActive: true,
                    cancellationToken);

            // =====================================
            // 3. YETKİ KONTROLÜ
            // =====================================

            await ValidateManagePermissionAsync(
                maintenance,
                currentUserId,
                cancellationToken);

            // =====================================
            // 4. SIRA DEĞİŞMEMİŞSE
            // =====================================

            if (checklist.SequenceNumber == sequenceNumber)
            {
                return true;
            }

            // =====================================
            // 5. SONUÇ GEÇMİŞİ KONTROLÜ
            // =====================================

            await EnsureChecklistHasNoResultsAsync(
                checklist.Id,
                cancellationToken);

            // =====================================
            // 6. SIRA ÇAKIŞMASI
            // =====================================

            bool exists =
                await _maintenanceChecklistRepository
                    .ExistsSequenceNumberAsync(
                        checklist.PlannedMaintenanceId,
                        sequenceNumber,
                        checklist.Id,
                        cancellationToken);

            if (exists)
            {
                throw new ValidationException(
                    "Bu sıra numarası aynı plan " +
                    "içerisinde kullanılmaktadır.");
            }

            // =====================================
            // 7. REPOSITORY SIRA GÜNCELLEMESİ
            // =====================================

            return await _maintenanceChecklistRepository
                .UpdateSequenceNumberAsync(
                    id,
                    sequenceNumber,
                    cancellationToken);
        }

        #endregion

        #region ExistsTitle

        public async Task<bool> ExistsTitleAsync(
            int plannedMaintenanceId,
            string title,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            ValidateExcludedId(excludedId);

            string normalizedTitle = ValidateTitle(title);

            return await _maintenanceChecklistRepository
                .ExistsTitleAsync(
                    plannedMaintenanceId,
                    normalizedTitle,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsSequenceNumber

        public async Task<bool> ExistsSequenceNumberAsync(
            int plannedMaintenanceId,
            int sequenceNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            ValidateSequenceNumber(sequenceNumber);

            ValidateExcludedId(excludedId);

            return await _maintenanceChecklistRepository
                .ExistsSequenceNumberAsync(
                    plannedMaintenanceId,
                    sequenceNumber,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasResults

        public async Task<bool> HasResultsAsync(
            int checklistId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(checklistId, "Kontrol Maddesi");

            return await _maintenanceChecklistRepository
                .HasResultsAsync(
                    checklistId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveChecklists

        public async Task<bool> HasActiveChecklistsAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            return await _maintenanceChecklistRepository
                .HasActiveChecklistsAsync(
                    plannedMaintenanceId,
                    cancellationToken);
        }

        #endregion

        #region HasUncompletedRequiredChecklists

        public async Task<bool>
            HasUncompletedRequiredChecklistsAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            return await _maintenanceChecklistRepository
                .HasUncompletedRequiredChecklistsAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region HasFailedRequiredChecklists

        public async Task<bool>
            HasFailedRequiredChecklistsAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            return await _maintenanceChecklistRepository
                .HasFailedRequiredChecklistsAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region GetActiveChecklistCount

        public async Task<int> GetActiveChecklistCountAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            return await _maintenanceChecklistRepository
                .GetActiveChecklistCountAsync(
                    plannedMaintenanceId,
                    cancellationToken);
        }

        #endregion

        #region GetResultCountByChecklistId

        /// <summary>
        /// Kontrol maddesinin farklı iş emirlerinde
        /// oluşturulmuş sonuç kayıtlarını sayar.
        /// </summary>
        public async Task<int> GetResultCountByChecklistIdAsync(
            int checklistId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(checklistId, "Kontrol Maddesi");

            var results =
                await _maintenanceChecklistRepository
                    .GetResultsByChecklistIdAsync(
                        checklistId,
                        cancellationToken);

            return results.Count;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Listeleme Entity kayıtlarını DTO'lara dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultMaintenanceChecklistDto>
            MapResults(
                IEnumerable<MaintenanceChecklist> checklists)
        {
            return _mapper.Map<
                List<ResultMaintenanceChecklistDto>>(
                    checklists.ToList());
        }

        /// <summary>
        /// Planlı bakımın mevcut ve gerekiyorsa
        /// aktif olduğunu kontrol eder.
        /// </summary>
        private async Task<PlannedMaintenance>
            GetPlannedMaintenanceOrThrowAsync(
                int plannedMaintenanceId,
                bool requireActive,
                CancellationToken cancellationToken)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            var maintenance =
                await _plannedMaintenanceRepository
                    .GetByIdAsync(
                        plannedMaintenanceId,
                        cancellationToken);

            if (maintenance == null)
            {
                throw new InvalidOperationException(
                    "İlgili planlı bakım bulunamadı.");
            }

            if (requireActive && !maintenance.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif planlı bakıma kontrol " +
                    "maddesi eklenemez veya " +
                    "kontrol maddesi aktifleştirilemez.");
            }

            return maintenance;
        }

        /// <summary>
        /// Kullanıcının aktifliğini ve ilgili birim/
        /// iş alt türünde yönetim yetkisini kontrol eder.
        /// </summary>
        private async Task ValidateManagePermissionAsync(
            PlannedMaintenance maintenance,
            int currentUserId,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByIdAsync(
                    currentUserId,
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
                        currentUserId,
                        maintenance.UnitId,
                        maintenance.WorkSubTypeId,
                        ManagePermission,
                        cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının bu bakım planında " +
                    "kontrol maddesi yönetme " +
                    "yetkisi bulunmamaktadır.");
            }
        }

        /// <summary>
        /// İş emri sonucu oluşmuş kontrol
        /// maddelerinin geçmiş anlamını korur.
        /// </summary>
        private async Task EnsureChecklistHasNoResultsAsync(
            int checklistId,
            CancellationToken cancellationToken)
        {
            bool hasResults =
                await _maintenanceChecklistRepository
                    .HasResultsAsync(
                        checklistId,
                        cancellationToken);

            if (hasResults)
            {
                throw new InvalidOperationException(
                    "Bu kontrol maddesi için daha önce " +
                    "iş emri sonucu kaydedildiğinden " +
                    "normal güncelleme, sıra veya " +
                    "aktiflik değişikliği yapılamaz.");
            }
        }

        /// <summary>
        /// DataAnnotations doğrulaması.
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
        /// Pozitif ID doğrulaması.
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
        /// Hariç tutulacak kayıt ID doğrulaması.
        /// </summary>
        private static void ValidateExcludedId(
            int? excludedId)
        {
            if (excludedId.HasValue &&
                excludedId.Value <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(excludedId),
                    "Hariç tutulacak ID geçersiz.");
            }
        }

        /// <summary>
        /// Başlık doğrulaması.
        /// </summary>
        private static string ValidateTitle(string? title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ValidationException(
                    "Kontrol maddesi başlığı zorunludur.");
            }

            string normalized = title.Trim();

            if (normalized.Length > MaxTitleLength)
            {
                throw new ValidationException(
                    "Kontrol maddesi başlığı en fazla " +
                    "200 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// Açıklama doğrulaması.
        /// </summary>
        private static string? NormalizeDescription(
            string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            string normalized = description.Trim();

            if (normalized.Length > MaxDescriptionLength)
            {
                throw new ValidationException(
                    "Kontrol maddesi açıklaması " +
                    "en fazla 1000 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// Sıra numarası doğrulaması.
        /// </summary>
        private static void ValidateSequenceNumber(
            int sequenceNumber)
        {
            if (sequenceNumber <= 0)
            {
                throw new ValidationException(
                    "Kontrol maddesi sıra numarası " +
                    "sıfırdan büyük olmalıdır.");
            }
        }

        #endregion
    }
}
