using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkOrderActionDtos;
using MaintenanceManagement.Application.Interfaces;
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
    public class WorkOrderActionService : IWorkOrderActionService
    {
        #region Constants

        private const string ExecutePermission =
            "WORK_ORDER_EXECUTE";

        private const int MaxDescriptionLength = 2000;

        private const int MaxNotesLength = 1000;

        #endregion

        #region Fields

        private readonly IWorkOrderActionRepository
            _workOrderActionRepository;

        private readonly IWorkOrderRepository
            _workOrderRepository;

        private readonly IWorkOrderPersonnelRepository
            _workOrderPersonnelRepository;

        private readonly IUserRepository
            _userRepository;

        private readonly IRoleWorkSubTypePermissionService
            _roleWorkSubTypePermissionService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WorkOrderActionService(
            IWorkOrderActionRepository workOrderActionRepository,
            IWorkOrderRepository workOrderRepository,
            IWorkOrderPersonnelRepository workOrderPersonnelRepository,
            IUserRepository userRepository,
            IRoleWorkSubTypePermissionService
                roleWorkSubTypePermissionService,
            IMapper mapper)
        {
            _workOrderActionRepository =
                workOrderActionRepository;

            _workOrderRepository =
                workOrderRepository;

            _workOrderPersonnelRepository =
                workOrderPersonnelRepository;

            _userRepository =
                userRepository;

            _roleWorkSubTypePermissionService =
                roleWorkSubTypePermissionService;

            _mapper = mapper;
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün faaliyet kayıtlarını ilişkileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var actions = await _workOrderActionRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderActionDto>>(
                actions);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Faaliyet detayını iş emri ve kullanıcı
        /// bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<GetByIdWorkOrderActionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var action = await _workOrderActionRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (action == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWorkOrderActionDto>(
                action);
        }

        #endregion

        #region GetByWorkOrderId

        /// <summary>
        /// İş emrine ait bütün faaliyetleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var actions = await _workOrderActionRepository
                .GetByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderActionDto>>(
                actions);
        }

        #endregion

        #region GetByPerformedByUserId

        /// <summary>
        /// Kullanıcının gerçekleştirdiği faaliyetleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByPerformedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                userId,
                "Kullanıcı");

            var actions = await _workOrderActionRepository
                .GetByPerformedByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderActionDto>>(
                actions);
        }

        #endregion

        #region GetByWorkOrderAndUser

        /// <summary>
        /// Kullanıcının belirli iş emrindeki
        /// faaliyet kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByWorkOrderAndUserAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateId(
                userId,
                "Kullanıcı");

            var actions = await _workOrderActionRepository
                .GetByWorkOrderAndUserAsync(
                    workOrderId,
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderActionDto>>(
                actions);
        }

        #endregion

        #region GetByResponsibleUnitId

        /// <summary>
        /// Sorumlu organizasyon biriminin
        /// iş emri faaliyetlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Sorumlu Birim");

            var actions = await _workOrderActionRepository
                .GetByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderActionDto>>(
                actions);
        }

        #endregion

        #region GetByTeamId

        /// <summary>
        /// Takımın iş emirlerine ait
        /// faaliyet kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                teamId,
                "Takım");

            var actions = await _workOrderActionRepository
                .GetByTeamIdAsync(
                    teamId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderActionDto>>(
                actions);
        }

        #endregion

        #region GetByDateRange

        /// <summary>
        /// UTC tarih aralığındaki faaliyetleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var actions = await _workOrderActionRepository
                .GetByDateRangeAsync(
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderActionDto>>(
                actions);
        }

        #endregion

        #region GetByWorkOrderAndDateRange

        /// <summary>
        /// Belirli iş emrindeki faaliyetleri
        /// UTC tarih aralığında getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderActionDto>>
            GetByWorkOrderAndDateRangeAsync(
                int workOrderId,
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var actions = await _workOrderActionRepository
                .GetByWorkOrderAndDateRangeAsync(
                    workOrderId,
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderActionDto>>(
                actions);
        }

        #endregion

        #region GetLatestActionByWorkOrderId

        /// <summary>
        /// İş emrinin en son faaliyetini getirir.
        /// </summary>
        public async Task<GetByIdWorkOrderActionDto?>
            GetLatestActionByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var action = await _workOrderActionRepository
                .GetLatestActionByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            if (action == null)
            {
                return null;
            }

            // İlişkili alanların eksiksiz
            // doldurulması için detay sorgusu.
            return await GetByIdAsync(
                action.Id,
                cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Devam eden iş emrine faaliyet kaydı oluşturur.
        ///
        /// Kullanıcının aktif personel ataması ve
        /// WORK_ORDER_EXECUTE yetkisi bulunmalıdır.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkOrderActionDto dto,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(
                dto.WorkOrderId,
                "İş Emri");

            ValidateId(
                performedByUserId,
                "İşlemi Gerçekleştiren Kullanıcı");

            // =====================================
            // 2. FAALİYET BİLGİLERİ
            // =====================================

            string description = NormalizeDescription(
                dto.Description);

            string? notes = NormalizeNotes(
                dto.Notes);

            ValidateLaborMinutes(
                dto.LaborMinutes);

            // =====================================
            // 3. İŞ EMRİNİ DOĞRULA
            // =====================================

            var workOrder = await GetExecutableWorkOrderAsync(
                dto.WorkOrderId,
                cancellationToken);

            // =====================================
            // 4. KULLANICI VE YETKİ KONTROLLERİ
            // =====================================

            await ValidateActionActorAsync(
                workOrder,
                performedByUserId,
                cancellationToken);

            // =====================================
            // 5. DTO -> ENTITY
            // =====================================

            var action = _mapper.Map<WorkOrderAction>(dto);

            action.WorkOrderId =
                dto.WorkOrderId;

            action.PerformedByUserId =
                performedByUserId;

            action.Description =
                description;

            action.LaborMinutes =
                dto.LaborMinutes;

            action.Notes =
                notes;

            action.PerformedAt =
                DateTime.UtcNow;

            // =====================================
            // 6. REPOSITORY KAYIT İŞLEMİ
            // =====================================

            // RegisterActionAsync mevcut iş emri
            // ve personel uygunluğunu yeniden
            // doğrulayarak kaydı gerçekleştirir.

            var createdAction = await _workOrderActionRepository
                .RegisterActionAsync(
                    action,
                    cancellationToken);

            return createdAction.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Kullanıcının kendi faaliyet kaydını günceller.
        ///
        /// İş emri devam ediyor olmalıdır.
        /// Sistem tarafından yönetilen alanlar korunur.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkOrderActionDto dto,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(
                dto.Id,
                "Faaliyet Kaydı");

            ValidateId(
                performedByUserId,
                "Kullanıcı");

            string description = NormalizeDescription(
                dto.Description);

            string? notes = NormalizeNotes(
                dto.Notes);

            ValidateLaborMinutes(
                dto.LaborMinutes);

            // =====================================
            // 2. MEVCUT FAALİYETİ GETİR
            // =====================================

            var existingAction = await _workOrderActionRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingAction == null)
            {
                return false;
            }

            // =====================================
            // 3. FAALİYET SAHİBİ KONTROLÜ
            // =====================================

            if (existingAction.PerformedByUserId !=
                performedByUserId)
            {
                throw new UnauthorizedAccessException(
                    "Faaliyet kaydını yalnızca " +
                    "işlemi gerçekleştiren kullanıcı " +
                    "güncelleyebilir.");
            }

            // =====================================
            // 4. İŞ EMRİNİ KONTROL ET
            // =====================================

            var workOrder = await GetExecutableWorkOrderAsync(
                existingAction.WorkOrderId,
                cancellationToken);

            // =====================================
            // 5. KULLANICI VE YETKİ KONTROLLERİ
            // =====================================

            await ValidateActionActorAsync(
                workOrder,
                performedByUserId,
                cancellationToken);

            // =====================================
            // 6. DTO -> MEVCUT ENTITY
            // =====================================

            _mapper.Map(
                dto,
                existingAction);

            existingAction.Description =
                description;

            existingAction.LaborMinutes =
                dto.LaborMinutes;

            existingAction.Notes =
                notes;

            // WorkOrderId, PerformedByUserId,
            // PerformedAt ve CreatedAt korunur.

            existingAction.UpdatedAt =
                DateTime.UtcNow;

            // =====================================
            // 7. GÜNCELLEME
            // =====================================

            return await _workOrderActionRepository
                .UpdateAsync(
                    existingAction,
                    cancellationToken);
        }

        #endregion

        #region HasActions

        /// <summary>
        /// İş emrinde faaliyet kaydı var mı?
        /// </summary>
        public async Task<bool> HasActionsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            return await _workOrderActionRepository
                .HasActionsAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region HasActionsByUserId

        /// <summary>
        /// Kullanıcının belirtilen iş emrinde
        /// faaliyet kaydı bulunup bulunmadığını kontrol eder.
        ///
        /// Düzeltilen Repository parametre sırası:
        /// 1. workOrderId (int)
        /// 2. userId (int)
        /// 3. cancellationToken
        /// </summary>
        public async Task<bool> HasActionsByUserIdAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. İŞ EMRİ ID KONTROLÜ
            // =====================================

            ValidateId(
                workOrderId,
                "İş Emri");

            // =====================================
            // 2. KULLANICI ID KONTROLÜ
            // =====================================

            ValidateId(
                userId,
                "Kullanıcı");

            // =====================================
            // 3. REPOSITORY KONTROLÜ
            // =====================================

            return await _workOrderActionRepository
                .HasActionsByUserIdAsync(
                    workOrderId,
                    userId,
                    cancellationToken);
        }

        #endregion

        #region GetActionCountByWorkOrderId

        /// <summary>
        /// İş emrinin toplam faaliyet sayısını getirir.
        /// </summary>
        public async Task<int> GetActionCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            return await _workOrderActionRepository
                .GetActionCountByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalLaborMinutesByWorkOrderId

        /// <summary>
        /// İş emrindeki toplam çalışma süresini getirir.
        /// </summary>
        public async Task<long> GetTotalLaborMinutesByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            return await _workOrderActionRepository
                .GetTotalLaborMinutesByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalLaborMinutesByUserId

        /// <summary>
        /// Kullanıcının toplam çalışma süresini getirir.
        /// </summary>
        public async Task<long> GetTotalLaborMinutesByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                userId,
                "Kullanıcı");

            return await _workOrderActionRepository
                .GetTotalLaborMinutesByUserIdAsync(
                    userId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalLaborMinutesByWorkOrderAndUser

        /// <summary>
        /// Kullanıcının belirli iş emrindeki
        /// toplam çalışma süresini getirir.
        /// </summary>
        public async Task<long>
            GetTotalLaborMinutesByWorkOrderAndUserAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateId(
                userId,
                "Kullanıcı");

            return await _workOrderActionRepository
                .GetTotalLaborMinutesByWorkOrderAndUserAsync(
                    workOrderId,
                    userId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalLaborMinutesByTeamId

        /// <summary>
        /// Takımın toplam çalışma süresini getirir.
        /// </summary>
        public async Task<long> GetTotalLaborMinutesByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                teamId,
                "Takım");

            return await _workOrderActionRepository
                .GetTotalLaborMinutesByTeamIdAsync(
                    teamId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO DataAnnotations doğrulaması.
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
        /// Faaliyet açıklamasını doğrular.
        /// </summary>
        private static string NormalizeDescription(
            string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ValidationException(
                    "Faaliyet açıklaması zorunludur.");
            }

            string normalized =
                description.Trim();

            if (normalized.Length > MaxDescriptionLength)
            {
                throw new ValidationException(
                    "Faaliyet açıklaması en fazla " +
                    "2000 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// Faaliyet notlarını normalize eder.
        /// </summary>
        private static string? NormalizeNotes(
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
            {
                return null;
            }

            string normalized =
                notes.Trim();

            if (normalized.Length > MaxNotesLength)
            {
                throw new ValidationException(
                    "Faaliyet notu en fazla " +
                    "1000 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// Çalışma süresinin negatif olmamasını sağlar.
        /// </summary>
        private static void ValidateLaborMinutes(
            int laborMinutes)
        {
            if (laborMinutes < 0)
            {
                throw new ValidationException(
                    "Çalışma süresi negatif olamaz.");
            }
        }

        /// <summary>
        /// UTC tarih aralığı doğrulaması.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ValidationException(
                    "Başlangıç ve bitiş tarihleri " +
                    "UTC formatında olmalıdır.");
            }

            if (endDateUtc < startDateUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç " +
                    "tarihinden önce olamaz.");
            }
        }

        /// <summary>
        /// İş emrinin faaliyet işlemine uygun
        /// durumda olduğunu kontrol eder.
        /// </summary>
        private async Task<WorkOrder> GetExecutableWorkOrderAsync(
            int workOrderId,
            CancellationToken cancellationToken)
        {
            var workOrder = await _workOrderRepository
                .GetByIdAsync(
                    workOrderId,
                    cancellationToken);

            if (workOrder == null)
            {
                throw new InvalidOperationException(
                    "İş emri bulunamadı.");
            }

            if (workOrder.Status != WorkStatusEnum.InProgress)
            {
                throw new InvalidOperationException(
                    "Faaliyet işlemleri yalnızca " +
                    "İşe Başlandı durumundaki " +
                    "iş emirlerinde yapılabilir.");
            }

            if (!workOrder.StartedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "İş emrinin başlangıç " +
                    "tarihi bulunamadı.");
            }

            return workOrder;
        }

        /// <summary>
        /// Faaliyeti gerçekleştiren kullanıcının
        /// aktif personel atamasını ve iş alt türü
        /// yetkisini doğrular.
        /// </summary>
        private async Task ValidateActionActorAsync(
            WorkOrder workOrder,
            int performedByUserId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. KULLANICI KONTROLÜ
            // =====================================

            var user = await _userRepository
                .GetByIdAsync(
                    performedByUserId,
                    cancellationToken);

            if (user == null ||
                !user.IsActive ||
                !user.HasSystemAccess)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı aktif değildir veya " +
                    "sistem erişimi bulunmamaktadır.");
            }

            // =====================================
            // 2. AKTİF PERSONEL ATAMASI
            // =====================================

            bool isAssigned =
                await _workOrderPersonnelRepository
                    .IsUserAssignedAsync(
                        workOrder.Id,
                        performedByUserId,
                        cancellationToken);

            if (!isAssigned)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı iş emrine aktif " +
                    "personel olarak atanmamıştır.");
            }

            // =====================================
            // 3. İŞ ALT TÜRÜ YETKİSİ
            // =====================================

            bool hasPermission =
                await _roleWorkSubTypePermissionService
                    .HasUserActivePermissionAsync(
                        performedByUserId,
                        workOrder.ResponsibleUnitId,
                        workOrder.WorkSubTypeId,
                        ExecutePermission,
                        cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının ilgili iş alt " +
                    "türünde işlem yapma yetkisi " +
                    "bulunmamaktadır.");
            }
        }

        #endregion
    }
}
