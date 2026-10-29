using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkRequestDtos;
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
    public class WorkRequestService : IWorkRequestService
    {
        private const string WorkOrderCreatePermission =
            "WORK_ORDER_CREATE";

        private readonly IWorkRequestRepository _workRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IWorkSubTypeRepository _workSubTypeRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly ISiteLocationRepository _siteLocationRepository;
        private readonly IUserUnitRoleService _userUnitRoleService;
        private readonly IRoleWorkSubTypePermissionService
            _roleWorkSubTypePermissionService;
        private readonly IMapper _mapper;

        public WorkRequestService(
            IWorkRequestRepository workRequestRepository,
            IUserRepository userRepository,
            IUnitRepository unitRepository,
            IWorkSubTypeRepository workSubTypeRepository,
            IAssetRepository assetRepository,
            ISiteLocationRepository siteLocationRepository,
            IUserUnitRoleService userUnitRoleService,
            IRoleWorkSubTypePermissionService
                roleWorkSubTypePermissionService,
            IMapper mapper)
        {
            _workRequestRepository = workRequestRepository;
            _userRepository = userRepository;
            _unitRepository = unitRepository;
            _workSubTypeRepository = workSubTypeRepository;
            _assetRepository = assetRepository;
            _siteLocationRepository = siteLocationRepository;
            _userUnitRoleService = userUnitRoleService;
            _roleWorkSubTypePermissionService =
                roleWorkSubTypePermissionService;
            _mapper = mapper;
        }

        #region GetAll

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var requests = await _workRequestRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetById

        public async Task<GetByIdWorkRequestDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var request = await _workRequestRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (request == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWorkRequestDto>(
                request);
        }

        #endregion

        #region GetByRequestNumber

        public async Task<GetByIdWorkRequestDto?>
            GetByRequestNumberAsync(
                string requestNumber,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(requestNumber))
            {
                return null;
            }

            var request = await _workRequestRepository
                .GetByRequestNumberAsync(
                    requestNumber.Trim(),
                    cancellationToken);

            if (request == null)
            {
                return null;
            }

            return await GetByIdAsync(
                request.Id,
                cancellationToken);
        }

        #endregion

        #region GetByRequestedByUserId

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByRequestedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var requests = await _workRequestRepository
                .GetByRequestedByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetByRequesterUnitId

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByRequesterUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Talep Eden Birim");

            var requests = await _workRequestRepository
                .GetByRequesterUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetByResponsibleUnitId

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Sorumlu Birim");

            var requests = await _workRequestRepository
                .GetByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetNewRequestsByResponsibleUnitId

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetNewRequestsByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Sorumlu Birim");

            var requests = await _workRequestRepository
                .GetNewRequestsByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetSubmittedNewRequestsByResponsibleUnitId

        /// <summary>
        /// Sorumlu birime gönderilmiş,
        /// henüz sonuçlandırılmamış talepleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetSubmittedNewRequestsByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Sorumlu Birim");

            var requests = await _workRequestRepository
                .GetSubmittedNewRequestsByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetByManagerUserId

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByManagerUserIdAsync(
                int managerUserId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(managerUserId, "Yönetici");

            var requests = await _workRequestRepository
                .GetByManagerUserIdAsync(
                    managerUserId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetByStatus

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByStatusAsync(
                WorkRequestStatusEnum status,
                CancellationToken cancellationToken = default)
        {
            ValidateRequestStatus(status);

            var requests = await _workRequestRepository
                .GetByStatusAsync(
                    status,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetByPriority

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByPriorityAsync(
                WorkPriorityEnum priority,
                CancellationToken cancellationToken = default)
        {
            ValidatePriority(priority);

            var requests = await _workRequestRepository
                .GetByPriorityAsync(
                    priority,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetByAssetId

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            var requests = await _workRequestRepository
                .GetByAssetIdAsync(
                    assetId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetByWorkSubTypeId

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            var requests = await _workRequestRepository
                .GetByWorkSubTypeIdAsync(
                    workSubTypeId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region GetByDateRange

        public async Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ValidationException(
                    "Tarih aralığı UTC formatında olmalıdır.");
            }

            if (endDateUtc < startDateUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç tarihinden önce olamaz.");
            }

            var requests = await _workRequestRepository
                .GetByDateRangeAsync(
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkRequestDto>>(
                requests);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni iş talebi oluşturur.
        /// Talep Draft durumunda ve gönderilmemiş
        /// olarak kaydedilir.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkRequestDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateId(currentUserId, "Talep Oluşturan Kullanıcı");

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            // =====================================
            // 2. METİN ALANLARI
            // =====================================

            string title = ValidateRequiredText(
                dto.Title,
                "İş talebi başlığı");

            string description = ValidateRequiredText(
                dto.Description,
                "İş talebi açıklaması");

            string? locationDescription =
                NormalizeOptionalText(dto.LocationDescription);

            // =====================================
            // 3. ÖNCELİK DOĞRULAMASI
            // =====================================

            ValidatePriority(dto.Priority);

            // =====================================
            // 4. KULLANICI VE İLİŞKİ KONTROLLERİ
            // =====================================

            await ValidateRequesterAsync(
                currentUserId,
                dto.RequesterUnitId,
                cancellationToken);

            await ValidateRequestRelationsAsync(
                dto.RequesterUnitId,
                dto.ResponsibleUnitId,
                dto.WorkSubTypeId,
                dto.AssetId,
                dto.SiteLocationId,
                cancellationToken);

            // =====================================
            // 5. TALEP NUMARASI
            // =====================================

            string requestNumber =
                await GenerateUniqueRequestNumberAsync(
                    cancellationToken);

            // =====================================
            // 6. DTO -> ENTITY
            // =====================================

            var request = _mapper.Map<WorkRequest>(dto);

            request.RequestNumber = requestNumber;

            request.Title = title;
            request.Description = description;

            request.RequestedByUserId = currentUserId;

            request.RequesterUnitId = dto.RequesterUnitId;
            request.ResponsibleUnitId = dto.ResponsibleUnitId;
            request.WorkSubTypeId = dto.WorkSubTypeId;

            request.AssetId = dto.AssetId;
            request.SiteLocationId = dto.SiteLocationId;

            request.LocationDescription = locationDescription;

            request.Priority = dto.Priority;

            // Yönetici ataması ayrı bir süreçtir.
            request.ManagerUserId = null;

            // =====================================
            // 7. BAŞLANGIÇ DURUMU
            // =====================================

            request.Status = WorkRequestStatusEnum.Draft;

            request.RequestDate = DateTime.UtcNow;

            request.SubmittedAt = null;

            // =====================================
            // 8. KAYIT İŞLEMİ
            // =====================================

            var createdRequest = await _workRequestRepository
                .CreateAsync(
                    request,
                    cancellationToken);

            return createdRequest.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Yalnızca talep sahibinin henüz
        /// göndermediği Draft kaydı güncellenebilir.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkRequestDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "İş Talebi");
            ValidateId(currentUserId, "Kullanıcı");

            // =====================================
            // 1. MEVCUT TALEBİ GETİR
            // =====================================

            var existingRequest = await _workRequestRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingRequest == null)
            {
                return false;
            }

            // =====================================
            // 2. TALEP SAHİBİ KONTROLÜ
            // =====================================

            if (existingRequest.RequestedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Yalnızca talebi oluşturan kullanıcı " +
                    "talep bilgilerini güncelleyebilir.");
            }

            // =====================================
            // 3. DURUM KONTROLÜ
            // =====================================

            if (existingRequest.Status != WorkRequestStatusEnum.Draft ||
                existingRequest.SubmittedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Yalnızca henüz gönderilmemiş " +
                    "yeni iş talepleri düzenlenebilir.");
            }

            // =====================================
            // 4. METİN VE ÖNCELİK
            // =====================================

            string title = ValidateRequiredText(
                dto.Title,
                "İş talebi başlığı");

            string description = ValidateRequiredText(
                dto.Description,
                "İş talebi açıklaması");

            string? locationDescription =
                NormalizeOptionalText(dto.LocationDescription);

            ValidatePriority(dto.Priority);

            // =====================================
            // 5. İLİŞKİLERİ DOĞRULA
            // =====================================

            await ValidateRequesterAsync(
                currentUserId,
                dto.RequesterUnitId,
                cancellationToken);

            await ValidateRequestRelationsAsync(
                dto.RequesterUnitId,
                dto.ResponsibleUnitId,
                dto.WorkSubTypeId,
                dto.AssetId,
                dto.SiteLocationId,
                cancellationToken);

            // =====================================
            // 6. MEVCUT ENTITY'Yİ GÜNCELLE
            // =====================================

            _mapper.Map(dto, existingRequest);

            existingRequest.Title = title;
            existingRequest.Description = description;

            existingRequest.RequesterUnitId = dto.RequesterUnitId;
            existingRequest.ResponsibleUnitId = dto.ResponsibleUnitId;
            existingRequest.WorkSubTypeId = dto.WorkSubTypeId;

            existingRequest.AssetId = dto.AssetId;
            existingRequest.SiteLocationId = dto.SiteLocationId;
            existingRequest.LocationDescription = locationDescription;

            existingRequest.Priority = dto.Priority;

            // RequestNumber, RequestedByUserId,
            // ManagerUserId, Status, RequestDate,
            // SubmittedAt ve geçmiş ilişkileri korunur.

            return await _workRequestRepository
                .UpdateAsync(
                    existingRequest,
                    cancellationToken);
        }

        #endregion

        #region Submit

        /// <summary>
        /// Talebi sorumlu birime gönderir.
        /// SubmittedAt ve Submitted action kaydı
        /// Repository tarafından oluşturulur.
        /// </summary>
        public async Task<bool> SubmitAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "İş Talebi");
            ValidateId(performedByUserId, "Kullanıcı");

            var request = await _workRequestRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (request == null)
            {
                return false;
            }

            if (request.RequestedByUserId != performedByUserId)
            {
                throw new UnauthorizedAccessException(
                    "İş talebi yalnızca talebi oluşturan " +
                    "kullanıcı tarafından gönderilebilir.");
            }

            if (request.Status != WorkRequestStatusEnum.Draft)
            {
                throw new InvalidOperationException(
                    "Yalnızca yeni durumundaki " +
                    "iş talepleri gönderilebilir.");
            }

            if (request.SubmittedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Bu iş talebi daha önce gönderilmiştir.");
            }

            // Gönderim öncesinde ilişkilerin güncel
            // aktiflik durumlarını yeniden kontrol et.
            await ValidateRequesterAsync(
                performedByUserId,
                request.RequesterUnitId,
                cancellationToken);

            await ValidateRequestRelationsAsync(
                request.RequesterUnitId,
                request.ResponsibleUnitId,
                request.WorkSubTypeId,
                request.AssetId,
                request.SiteLocationId,
                cancellationToken);

            // Repository:
            // SubmittedAt = UTC
            // WorkRequestAction = Submitted
            // Status = Draft olarak kalır.
            return await _workRequestRepository
                .SubmitAsync(
                    id,
                    performedByUserId,
                    cancellationToken);
        }

        #endregion

        #region Reject

        /// <summary>
        /// Gönderilmiş iş talebini reddeder.
        /// Ret nedeni zorunludur.
        /// </summary>
        public async Task<bool> RejectAsync(
            int id,
            int performedByUserId,
            string reason,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "İş Talebi");

            string normalizedReason = ValidateRequiredText(
                reason,
                "Ret nedeni");

            var request = await _workRequestRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (request == null)
            {
                return false;
            }

            await ValidateResponsibleUnitActorAsync(
                performedByUserId,
                request.ResponsibleUnitId,
                cancellationToken);

            if (request.Status != WorkRequestStatusEnum.Draft ||
                !request.SubmittedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Yalnızca gönderilmiş ve henüz " +
                    "sonuçlandırılmamış talepler reddedilebilir.");
            }

            // Repository:
            // Status = Rejected
            // WorkRequestAction = Rejected
            return await _workRequestRepository
                .ChangeStatusAsync(
                    id,
                    WorkRequestStatusEnum.Rejected,
                    performedByUserId,
                    normalizedReason,
                    cancellationToken);
        }

        #endregion

        #region Cancel

        /// <summary>
        /// İş talebini iptal eder.
        /// Gönderim öncesi veya sonrası mümkündür.
        /// </summary>
        public async Task<bool> CancelAsync(
            int id,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "İş Talebi");

            var request = await _workRequestRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (request == null)
            {
                return false;
            }

            // Bu sürümde iptal işlemi talep sahibine aittir.
            if (request.RequestedByUserId != performedByUserId)
            {
                throw new UnauthorizedAccessException(
                    "İş talebi yalnızca talep sahibi " +
                    "tarafından iptal edilebilir.");
            }

            await ValidateSystemUserAsync(
                performedByUserId,
                cancellationToken);

            if (request.Status != WorkRequestStatusEnum.Draft)
            {
                throw new InvalidOperationException(
                    "Sonuçlandırılmış iş talebi iptal edilemez.");
            }

            string? normalizedDescription =
                NormalizeOptionalText(description);

            // Repository:
            // Status = Cancelled
            // WorkRequestAction = Cancelled
            return await _workRequestRepository
                .ChangeStatusAsync(
                    id,
                    WorkRequestStatusEnum.Cancelled,
                    performedByUserId,
                    normalizedDescription,
                    cancellationToken);
        }

        #endregion

        #region MarkConvertedToWorkOrder

        /// <summary>
        /// İlgili iş emri oluşturulduktan sonra
        /// talebi ConvertedToWorkOrder yapar.
        ///
        /// Bu metot iş emri oluşturmaz.
        /// </summary>
        public async Task<bool> MarkConvertedToWorkOrderAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "İş Talebi");
            ValidateId(performedByUserId, "Kullanıcı");

            var request = await _workRequestRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (request == null)
            {
                return false;
            }

            if (request.Status != WorkRequestStatusEnum.Draft ||
                !request.SubmittedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Yalnızca gönderilmiş ve " +
                    "sonuçlandırılmamış iş talepleri " +
                    "iş emrine dönüştürülebilir.");
            }

            // İşlemi gerçekleştiren kullanıcı
            // sorumlu birimde aktif role sahip olmalıdır.
            await ValidateResponsibleUnitActorAsync(
                performedByUserId,
                request.ResponsibleUnitId,
                cancellationToken);

            // İş emri oluşturma yetkisi.
            bool hasPermission =
                await _roleWorkSubTypePermissionService
                    .HasUserActivePermissionAsync(
                        performedByUserId,
                        request.ResponsibleUnitId,
                        request.WorkSubTypeId,
                        WorkOrderCreatePermission,
                        cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının bu iş alt türünde " +
                    "iş emri oluşturma yetkisi bulunmamaktadır.");
            }

            // İlişkili iş emri gerçekten
            // oluşturulmuş olmalıdır.
            bool hasWorkOrder = await _workRequestRepository
                .HasWorkOrderAsync(
                    id,
                    cancellationToken);

            if (!hasWorkOrder)
            {
                throw new InvalidOperationException(
                    "Talebe bağlı iş emri oluşturulmadan " +
                    "talep dönüştürüldü durumuna alınamaz.");
            }

            // Repository:
            // Status = ConvertedToWorkOrder
            // WorkRequestAction = Converted
            return await _workRequestRepository
                .ChangeStatusAsync(
                    id,
                    WorkRequestStatusEnum.ConvertedToWorkOrder,
                    performedByUserId,
                    "İş talebi iş emrine dönüştürüldü.",
                    cancellationToken);
        }

        #endregion

        #region ExistsRequestNumber

        public async Task<bool> ExistsRequestNumberAsync(
            string requestNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalized = ValidateRequiredText(
                requestNumber,
                "Talep numarası");

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak İş Talebi");
            }

            return await _workRequestRepository
                .ExistsRequestNumberAsync(
                    normalized,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasWorkOrder

        public async Task<bool> HasWorkOrderAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");

            return await _workRequestRepository
                .HasWorkOrderAsync(
                    workRequestId,
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
            var context = new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                context,
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

        private static string ValidateRequiredText(
            string? value,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException(
                    $"{fieldName} zorunludur.");
            }

            return value.Trim();
        }

        private static string? NormalizeOptionalText(
            string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        private static void ValidatePriority(
            WorkPriorityEnum priority)
        {
            if (!Enum.IsDefined(
                    typeof(WorkPriorityEnum),
                    priority))
            {
                throw new ValidationException(
                    "Geçersiz iş önceliği.");
            }
        }

        private static void ValidateRequestStatus(
            WorkRequestStatusEnum status)
        {
            if (!Enum.IsDefined(
                    typeof(WorkRequestStatusEnum),
                    status))
            {
                throw new ValidationException(
                    "Geçersiz iş talebi durumu.");
            }
        }

        /// <summary>
        /// Kullanıcının mevcut, aktif ve
        /// sistem erişimine sahip olduğunu doğrular.
        /// </summary>
        private async Task ValidateSystemUserAsync(
            int userId,
            CancellationToken cancellationToken)
        {
            ValidateId(userId, "Kullanıcı");

            var user = await _userRepository
                .GetByIdAsync(
                    userId,
                    cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Kullanıcı bulunamadı.");
            }

            if (!user.IsActive || !user.HasSystemAccess)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı aktif değildir veya " +
                    "sistem erişimi bulunmamaktadır.");
            }
        }

        /// <summary>
        /// Talep sahibinin talep eden birimde
        /// aktif rol ilişkisi bulunduğunu doğrular.
        /// </summary>
        private async Task ValidateRequesterAsync(
            int userId,
            int requesterUnitId,
            CancellationToken cancellationToken)
        {
            ValidateId(requesterUnitId, "Talep Eden Birim");

            await ValidateSystemUserAsync(
                userId,
                cancellationToken);

            bool hasActiveRole = await _userUnitRoleService
                .HasActiveRoleInUnitAsync(
                    userId,
                    requesterUnitId,
                    cancellationToken);

            if (!hasActiveRole)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının talep eden birimde " +
                    "aktif rol ataması bulunmamaktadır.");
            }
        }

        /// <summary>
        /// Talebi değerlendiren kullanıcının
        /// sorumlu birimde aktif rolü olduğunu kontrol eder.
        /// </summary>
        private async Task ValidateResponsibleUnitActorAsync(
            int userId,
            int responsibleUnitId,
            CancellationToken cancellationToken)
        {
            await ValidateSystemUserAsync(
                userId,
                cancellationToken);

            bool hasActiveRole = await _userUnitRoleService
                .HasActiveRoleInUnitAsync(
                    userId,
                    responsibleUnitId,
                    cancellationToken);

            if (!hasActiveRole)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının sorumlu organizasyon " +
                    "biriminde aktif rolü bulunmamaktadır.");
            }
        }

        /// <summary>
        /// Birimler, iş alt türü, demirbaş ve
        /// konum ilişkilerini doğrular.
        /// </summary>
        private async Task ValidateRequestRelationsAsync(
            int requesterUnitId,
            int responsibleUnitId,
            int workSubTypeId,
            int? assetId,
            int? siteLocationId,
            CancellationToken cancellationToken)
        {
            ValidateId(requesterUnitId, "Talep Eden Birim");
            ValidateId(responsibleUnitId, "Sorumlu Birim");
            ValidateId(workSubTypeId, "İş Alt Türü");

            // =====================================
            // 1. TALEP EDEN BİRİM
            // =====================================

            var requesterUnit = await _unitRepository
                .GetByIdAsync(
                    requesterUnitId,
                    cancellationToken);

            if (requesterUnit == null || !requesterUnit.IsActive)
            {
                throw new InvalidOperationException(
                    "Talep eden organizasyon birimi " +
                    "bulunamadı veya pasif durumdadır.");
            }

            // =====================================
            // 2. SORUMLU BİRİM
            // =====================================

            var responsibleUnit = await _unitRepository
                .GetByIdAsync(
                    responsibleUnitId,
                    cancellationToken);

            if (responsibleUnit == null ||
                !responsibleUnit.IsActive)
            {
                throw new InvalidOperationException(
                    "Sorumlu organizasyon birimi " +
                    "bulunamadı veya pasif durumdadır.");
            }

            // =====================================
            // 3. İŞ ALT TÜRÜ
            // =====================================

            var workSubType = await _workSubTypeRepository
                .GetByIdAsync(
                    workSubTypeId,
                    cancellationToken);

            if (workSubType == null || !workSubType.IsActive)
            {
                throw new InvalidOperationException(
                    "İş alt türü bulunamadı veya pasif durumdadır.");
            }

            // =====================================
            // 4. BİRİM–İŞ TÜRÜ UYGUNLUĞU
            // =====================================

            // Repository şu ilişkileri dikkate alır:
            // Unit -> UnitWorkType -> WorkType -> WorkSubType
            bool isAvailable = await _workSubTypeRepository
                .IsAvailableForUnitAsync(
                    responsibleUnitId,
                    workSubTypeId,
                    cancellationToken);

            if (!isAvailable)
            {
                throw new InvalidOperationException(
                    "Seçilen iş alt türü sorumlu " +
                    "organizasyon biriminde kullanılamaz.");
            }

            // =====================================
            // 5. DEMİRBAŞ (İSTEĞE BAĞLI)
            // =====================================

            if (assetId.HasValue)
            {
                ValidateId(assetId.Value, "Demirbaş");

                var asset = await _assetRepository
                    .GetByIdAsync(
                        assetId.Value,
                        cancellationToken);

                if (asset == null)
                {
                    throw new InvalidOperationException(
                        "Seçilen demirbaş bulunamadı.");
                }

                // Arıza talepleri kullanım dışı
                // demirbaşlar için de açılabileceğinden
                // burada yalnızca varlık kontrol edilir.
            }

            // =====================================
            // 6. KONUM (İSTEĞE BAĞLI)
            // =====================================

            if (siteLocationId.HasValue)
            {
                ValidateId(siteLocationId.Value, "Konum");

                var location = await _siteLocationRepository
                    .GetByIdAsync(
                        siteLocationId.Value,
                        cancellationToken);

                if (location == null || !location.IsActive)
                {
                    throw new InvalidOperationException(
                        "Seçilen konum bulunamadı " +
                        "veya pasif durumdadır.");
                }
            }
        }

        /// <summary>
        /// Benzersiz talep numarası üretir.
        /// </summary>
        private async Task<string> GenerateUniqueRequestNumberAsync(
            CancellationToken cancellationToken)
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                string number =
                    $"TLP-{DateTime.UtcNow:yyyyMMdd}-" +
                    Guid.NewGuid()
                        .ToString("N")[..12]
                        .ToUpperInvariant();

                bool exists = await _workRequestRepository
                    .ExistsRequestNumberAsync(
                        number,
                        null,
                        cancellationToken);

                if (!exists)
                {
                    return number;
                }
            }

            throw new InvalidOperationException(
                "Benzersiz iş talebi numarası üretilemedi.");
        }

        #endregion
    }
}
