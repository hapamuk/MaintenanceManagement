using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkOrdersDtos;
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
    public class WorkOrderService : IWorkOrderService
    {
        #region Constants

        private const string CreatePermission =
            "WORK_ORDER_CREATE";

        private const string ExecutePermission =
            "WORK_ORDER_EXECUTE";

        private const string ApprovePermission =
            "WORK_ORDER_APPROVE";

        #endregion

        #region Fields

        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IWorkSubTypeRepository _workSubTypeRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IPlannedMaintenanceRepository
            _plannedMaintenanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleWorkSubTypePermissionService
            _roleWorkSubTypePermissionService;
        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WorkOrderService(
            IWorkOrderRepository workOrderRepository,
            IUnitRepository unitRepository,
            ITeamRepository teamRepository,
            ITeamMemberRepository teamMemberRepository,
            IWorkSubTypeRepository workSubTypeRepository,
            IAssetRepository assetRepository,
            IPlannedMaintenanceRepository plannedMaintenanceRepository,
            IUserRepository userRepository,
            IRoleWorkSubTypePermissionService
                roleWorkSubTypePermissionService,
            IMapper mapper)
        {
            _workOrderRepository = workOrderRepository;
            _unitRepository = unitRepository;
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _workSubTypeRepository = workSubTypeRepository;
            _assetRepository = assetRepository;
            _plannedMaintenanceRepository = plannedMaintenanceRepository;
            _userRepository = userRepository;
            _roleWorkSubTypePermissionService =
                roleWorkSubTypePermissionService;
            _mapper = mapper;
        }

        #endregion

        #region GetAll

        public async Task<IReadOnlyList<ResultWorkOrderDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var workOrders = await _workOrderRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetById

        public async Task<GetByIdWorkOrderDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var workOrder = await _workOrderRepository
                .GetByIdWithDetailsAsync(id, cancellationToken);

            return workOrder == null
                ? null
                : _mapper.Map<GetByIdWorkOrderDto>(workOrder);
        }

        #endregion

        #region GetByWorkOrderNumber

        public async Task<GetByIdWorkOrderDto?>
            GetByWorkOrderNumberAsync(
                string workOrderNumber,
                CancellationToken cancellationToken = default)
        {
            string number = ValidateRequiredText(
                workOrderNumber,
                "İş emri numarası");

            var workOrder = await _workOrderRepository
                .GetByWorkOrderNumberAsync(number, cancellationToken);

            return workOrder == null
                ? null
                : await GetByIdAsync(workOrder.Id, cancellationToken);
        }

        #endregion

        #region GetByWorkRequestId

        public async Task<GetByIdWorkOrderDto?>
            GetByWorkRequestIdAsync(
                int workRequestId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");

            var workOrder = await _workOrderRepository
                .GetByWorkRequestIdAsync(
                    workRequestId,
                    cancellationToken);

            return workOrder == null
                ? null
                : await GetByIdAsync(workOrder.Id, cancellationToken);
        }

        #endregion

        #region GetByPlannedMaintenanceId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(plannedMaintenanceId, "Planlı Bakım");

            var workOrders = await _workOrderRepository
                .GetByPlannedMaintenanceIdAsync(
                    plannedMaintenanceId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetByResponsibleUnitId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Sorumlu Birim");

            var workOrders = await _workOrderRepository
                .GetByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetByTeamId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Takım");

            var workOrders = await _workOrderRepository
                .GetByTeamIdAsync(teamId, cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetByWorkSubTypeId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            var workOrders = await _workOrderRepository
                .GetByWorkSubTypeIdAsync(
                    workSubTypeId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetByAssetId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            var workOrders = await _workOrderRepository
                .GetByAssetIdAsync(assetId, cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetByStatus

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByStatusAsync(
                WorkStatusEnum status,
                CancellationToken cancellationToken = default)
        {
            ValidateStatus(status);

            var workOrders = await _workOrderRepository
                .GetByStatusAsync(status, cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetByPriority

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByPriorityAsync(
                WorkPriorityEnum priority,
                CancellationToken cancellationToken = default)
        {
            ValidatePriority(priority);

            var workOrders = await _workOrderRepository
                .GetByPriorityAsync(priority, cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetByCreatedByUserId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByCreatedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var workOrders = await _workOrderRepository
                .GetByCreatedByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetAssignedByUserId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetAssignedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var workOrders = await _workOrderRepository
                .GetAssignedWorkOrdersByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetOpenByUnitId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetOpenByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Sorumlu Birim");

            var workOrders = await _workOrderRepository
                .GetOpenWorkOrdersByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetPendingApprovalByUnitId

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetPendingApprovalByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Sorumlu Birim");

            var workOrders = await _workOrderRepository
                .GetPendingApprovalByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region GetByPlannedStartDateRange

        public async Task<IReadOnlyList<ResultWorkOrderDto>>
            GetByPlannedStartDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(startDateUtc, endDateUtc);

            var workOrders = await _workOrderRepository
                .GetByPlannedStartDateRangeAsync(
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkOrderDto>>(workOrders);
        }

        #endregion

        #region Create

        /// <summary>
        /// Doğrudan veya planlı bakım ilişkili iş emrini
        /// ilk durum geçmişiyle birlikte oluşturur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkOrderDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);
            ValidateId(currentUserId, "İşlemi Yapan Kullanıcı");

            string title = ValidateRequiredText(
                dto.Title,
                "İş Emri Başlığı");

            string? description = NormalizeOptionalText(
                dto.Description);

            ValidatePriority(dto.Priority);

            ValidatePlannedDates(
                dto.PlannedStartDate,
                dto.PlannedEndDate);

            // İş talebinden dönüşüm işlemi, WorkRequest
            // durumunu ve hareket geçmişini de değiştirir.
            // Bu nedenle mevcut CreateWithInitialHistoryAsync
            // metodu tek başına yeterli değildir.
            if (dto.WorkRequestId.HasValue)
            {
                throw new InvalidOperationException(
                    "İş talebinden iş emrine dönüşüm " +
                    "ortak transaction ile yapılmalıdır. " +
                    "Doğrudan iş emri oluşturma metodu " +
                    "bu dönüşüm için kullanılamaz.");
            }

            await ValidateWorkOrderRelationsAsync(
                dto.ResponsibleUnitId,
                dto.TeamId,
                dto.WorkSubTypeId,
                dto.AssetId,
                dto.PlannedMaintenanceId,
                cancellationToken);

            await ValidateActorPermissionAsync(
                currentUserId,
                dto.ResponsibleUnitId,
                dto.WorkSubTypeId,
                CreatePermission,
                cancellationToken);

            string workOrderNumber =
                await GenerateUniqueWorkOrderNumberAsync(
                    cancellationToken);

            var workOrder = _mapper.Map<WorkOrder>(dto);

            workOrder.WorkOrderNumber = workOrderNumber;
            workOrder.Title = title;
            workOrder.Description = description;

            workOrder.ResponsibleUnitId = dto.ResponsibleUnitId;
            workOrder.TeamId = dto.TeamId;
            workOrder.WorkSubTypeId = dto.WorkSubTypeId;

            workOrder.AssetId = dto.AssetId;
            workOrder.WorkRequestId = null;
            workOrder.PlannedMaintenanceId = dto.PlannedMaintenanceId;

            workOrder.Priority = dto.Priority;
            workOrder.PlannedStartDate = dto.PlannedStartDate;
            workOrder.PlannedEndDate = dto.PlannedEndDate;

            workOrder.CreatedByUserId = currentUserId;
            workOrder.ApprovedByUserId = null;

            workOrder.Status = WorkStatusEnum.New;

            workOrder.StartedAt = null;
            workOrder.SubmittedForApprovalAt = null;
            workOrder.CompletedAt = null;
            workOrder.CancelledAt = null;
            workOrder.ResultDescription = null;

            // İlk durum geçmişi Repository tarafından
            // aynı transaction içerisinde oluşturulur.
            var createdWorkOrder = await _workOrderRepository
                .CreateWithInitialHistoryAsync(
                    workOrder,
                    cancellationToken);

            return createdWorkOrder.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Yeni durumundaki iş emrinin
        /// düzenlenebilir bilgilerini günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkOrderDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "İş Emri");
            ValidateId(currentUserId, "Kullanıcı");

            var workOrder = await _workOrderRepository
                .GetByIdAsync(dto.Id, cancellationToken);

            if (workOrder == null)
            {
                return false;
            }

            if (workOrder.Status != WorkStatusEnum.New)
            {
                throw new InvalidOperationException(
                    "Yalnızca Yeni durumundaki " +
                    "iş emirleri normal yöntemle güncellenebilir.");
            }

            // İş emrinin sorumlu birimi, takımı ve
            // iş alt türü geçmiş ilişkilerin temelidir.
            // Bunları normal Update ile değiştirmiyoruz.
            if (dto.ResponsibleUnitId != workOrder.ResponsibleUnitId ||
                dto.TeamId != workOrder.TeamId ||
                dto.WorkSubTypeId != workOrder.WorkSubTypeId)
            {
                throw new InvalidOperationException(
                    "İş emrinin sorumlu birimi, takımı veya " +
                    "iş alt türü normal güncelleme " +
                    "işleminde değiştirilemez.");
            }

            if (dto.WorkRequestId != workOrder.WorkRequestId ||
                dto.PlannedMaintenanceId != workOrder.PlannedMaintenanceId)
            {
                throw new InvalidOperationException(
                    "İş emrinin kaynak talep veya " +
                    "planlı bakım ilişkisi değiştirilemez.");
            }

            string title = ValidateRequiredText(
                dto.Title,
                "İş Emri Başlığı");

            string? description = NormalizeOptionalText(
                dto.Description);

            ValidatePriority(dto.Priority);

            ValidatePlannedDates(
                dto.PlannedStartDate,
                dto.PlannedEndDate);

            await ValidateActorPermissionAsync(
                currentUserId,
                workOrder.ResponsibleUnitId,
                workOrder.WorkSubTypeId,
                CreatePermission,
                cancellationToken);

            if (dto.AssetId != workOrder.AssetId)
            {
                var assignedPersonnel = await _workOrderRepository
                    .GetPersonnelByWorkOrderIdAsync(
                        workOrder.Id,
                        cancellationToken);

                if (assignedPersonnel.Any())
                {
                    throw new InvalidOperationException(
                        "Personel ataması bulunan iş emrinin " +
                        "demirbaşı normal güncellemeyle " +
                        "değiştirilemez.");
                }
            }

            await ValidateAssetAsync(
                dto.AssetId,
                cancellationToken);

            // Mapping, sistem alanlarını ve
            // ilişkili koleksiyonları korur.
            _mapper.Map(dto, workOrder);

            workOrder.Title = title;
            workOrder.Description = description;
            workOrder.AssetId = dto.AssetId;
            workOrder.Priority = dto.Priority;
            workOrder.PlannedStartDate = dto.PlannedStartDate;
            workOrder.PlannedEndDate = dto.PlannedEndDate;

            return await _workOrderRepository
                .UpdateAsync(workOrder, cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// İş emrinin durumunu yetki ve iş kurallarına
        /// uygun şekilde değiştirir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            WorkStatusEnum newStatus,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "İş Emri");
            ValidateId(performedByUserId, "Kullanıcı");
            ValidateStatus(newStatus);

            var workOrder = await _workOrderRepository
                .GetByIdAsync(id, cancellationToken);

            if (workOrder == null)
            {
                return false;
            }

            if (workOrder.Status == newStatus)
            {
                return true;
            }

            if (!IsValidTransition(workOrder.Status, newStatus))
            {
                throw new InvalidOperationException(
                    $"İş emri durumu {workOrder.Status} " +
                    $"değerinden {newStatus} değerine " +
                    "geçirilemez.");
            }

            string? normalizedDescription =
                NormalizeOptionalText(description);

            switch (newStatus)
            {
                case WorkStatusEnum.InProgress:

                    if (workOrder.Status ==
                        WorkStatusEnum.PendingApproval)
                    {
                        ValidateReason(
                            normalizedDescription,
                            "İade nedeni");

                        await ValidateActorPermissionAsync(
                            performedByUserId,
                            workOrder.ResponsibleUnitId,
                            workOrder.WorkSubTypeId,
                            ApprovePermission,
                            cancellationToken);
                    }
                    else
                    {
                        await ValidateAssignedPersonnelAsync(
                            workOrder,
                            performedByUserId,
                            cancellationToken);

                        await ValidateActorPermissionAsync(
                            performedByUserId,
                            workOrder.ResponsibleUnitId,
                            workOrder.WorkSubTypeId,
                            ExecutePermission,
                            cancellationToken);
                    }

                    break;

                case WorkStatusEnum.PendingApproval:

                    await ValidateAssignedPersonnelAsync(
                        workOrder,
                        performedByUserId,
                        cancellationToken);

                    await ValidateActorPermissionAsync(
                        performedByUserId,
                        workOrder.ResponsibleUnitId,
                        workOrder.WorkSubTypeId,
                        ExecutePermission,
                        cancellationToken);

                    break;

                case WorkStatusEnum.Completed:

                    await ValidateActorPermissionAsync(
                        performedByUserId,
                        workOrder.ResponsibleUnitId,
                        workOrder.WorkSubTypeId,
                        ApprovePermission,
                        cancellationToken);

                    break;

                case WorkStatusEnum.Cancelled:

                    ValidateReason(
                        normalizedDescription,
                        "İptal nedeni");

                    await ValidateCancelPermissionAsync(
                        workOrder,
                        performedByUserId,
                        cancellationToken);

                    break;
            }

            // Repository hem durum alanlarını hem de
            // WorkOrderStatusHistory kaydını yönetir.
            return await _workOrderRepository
                .ChangeStatusAsync(
                    id,
                    newStatus,
                    performedByUserId,
                    normalizedDescription,
                    cancellationToken);
        }

        #endregion

        #region StartWork

        public Task<bool> StartWorkAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            return ChangeStatusAsync(
                id,
                WorkStatusEnum.InProgress,
                performedByUserId,
                "İş emrine başlandı.",
                cancellationToken);
        }

        #endregion

        #region SubmitForApproval

        public Task<bool> SubmitForApprovalAsync(
            int id,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            return ChangeStatusAsync(
                id,
                WorkStatusEnum.PendingApproval,
                performedByUserId,
                description,
                cancellationToken);
        }

        #endregion

        #region Approve

        public Task<bool> ApproveAsync(
            int id,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            return ChangeStatusAsync(
                id,
                WorkStatusEnum.Completed,
                performedByUserId,
                description,
                cancellationToken);
        }

        #endregion

        #region ReturnToInProgress

        public Task<bool> ReturnToInProgressAsync(
            int id,
            int performedByUserId,
            string reason,
            CancellationToken cancellationToken = default)
        {
            return ChangeStatusAsync(
                id,
                WorkStatusEnum.InProgress,
                performedByUserId,
                reason,
                cancellationToken);
        }

        #endregion

        #region Cancel

        public Task<bool> CancelAsync(
            int id,
            int performedByUserId,
            string reason,
            CancellationToken cancellationToken = default)
        {
            return ChangeStatusAsync(
                id,
                WorkStatusEnum.Cancelled,
                performedByUserId,
                reason,
                cancellationToken);
        }

        #endregion

        #region ExistsWorkOrderNumber

        public async Task<bool> ExistsWorkOrderNumberAsync(
            string workOrderNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string number = ValidateRequiredText(
                workOrderNumber,
                "İş Emri Numarası");

            if (excludedId.HasValue)
            {
                ValidateId(excludedId.Value, "Hariç Tutulacak İş Emri");
            }

            return await _workOrderRepository
                .ExistsWorkOrderNumberAsync(
                    number,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasWorkOrderForRequest

        public async Task<bool> HasWorkOrderForRequestAsync(
            int workRequestId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workRequestId, "İş Talebi");

            return await _workOrderRepository
                .HasWorkOrderForRequestAsync(
                    workRequestId,
                    cancellationToken);
        }

        #endregion

        #region HasOpenWorkOrdersByTeamId

        public async Task<bool> HasOpenWorkOrdersByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Takım");

            return await _workOrderRepository
                .HasOpenWorkOrdersByTeamIdAsync(
                    teamId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

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

        private static void ValidateReason(
            string? reason,
            string fieldName)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new ValidationException(
                    $"{fieldName} zorunludur.");
            }
        }

        private static void ValidateStatus(WorkStatusEnum status)
        {
            if (!Enum.IsDefined(typeof(WorkStatusEnum), status))
            {
                throw new ValidationException(
                    "Geçersiz iş emri durumu.");
            }
        }

        private static void ValidatePriority(WorkPriorityEnum priority)
        {
            if (!Enum.IsDefined(typeof(WorkPriorityEnum), priority))
            {
                throw new ValidationException(
                    "Geçersiz iş önceliği.");
            }
        }

        private static void ValidatePlannedDates(
            DateTime? start,
            DateTime? end)
        {
            if (start.HasValue &&
                end.HasValue &&
                end.Value < start.Value)
            {
                throw new ValidationException(
                    "Planlanan bitiş tarihi, başlangıç " +
                    "tarihinden önce olamaz.");
            }
        }

        private static void ValidateDateRange(
            DateTime startUtc,
            DateTime endUtc)
        {
            if (startUtc.Kind != DateTimeKind.Utc ||
                endUtc.Kind != DateTimeKind.Utc)
            {
                throw new ValidationException(
                    "Tarihler UTC olarak gönderilmelidir.");
            }

            if (endUtc < startUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç tarihinden önce olamaz.");
            }
        }

        private static bool IsValidTransition(
            WorkStatusEnum current,
            WorkStatusEnum next)
        {
            return current switch
            {
                WorkStatusEnum.New =>
                    next == WorkStatusEnum.InProgress ||
                    next == WorkStatusEnum.Cancelled,

                WorkStatusEnum.InProgress =>
                    next == WorkStatusEnum.PendingApproval ||
                    next == WorkStatusEnum.Cancelled,

                WorkStatusEnum.PendingApproval =>
                    next == WorkStatusEnum.InProgress ||
                    next == WorkStatusEnum.Completed ||
                    next == WorkStatusEnum.Cancelled,

                _ => false
            };
        }

        /// <summary>
        /// İşlemi yapan kullanıcının belirli
        /// iş alt türündeki aktif yetkisini doğrular.
        /// </summary>
        private async Task ValidateActorPermissionAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByIdAsync(userId, cancellationToken);

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
                        permissionCode,
                        cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    $"Kullanıcının {permissionCode} " +
                    "yetkisi bulunmamaktadır.");
            }
        }

        /// <summary>
        /// Birim, takım, iş alt türü, demirbaş
        /// ve planlı bakım ilişkilerini doğrular.
        /// </summary>
        private async Task ValidateWorkOrderRelationsAsync(
            int responsibleUnitId,
            int teamId,
            int workSubTypeId,
            int? assetId,
            int? plannedMaintenanceId,
            CancellationToken cancellationToken)
        {
            ValidateId(responsibleUnitId, "Sorumlu Birim");
            ValidateId(teamId, "Takım");
            ValidateId(workSubTypeId, "İş Alt Türü");

            var unit = await _unitRepository
                .GetByIdAsync(
                    responsibleUnitId,
                    cancellationToken);

            if (unit == null || !unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Sorumlu organizasyon birimi " +
                    "bulunamadı veya pasif durumdadır.");
            }

            var team = await _teamRepository
                .GetByIdAsync(teamId, cancellationToken);

            if (team == null || !team.IsActive)
            {
                throw new InvalidOperationException(
                    "Takım bulunamadı veya pasif durumdadır.");
            }

            if (team.UnitId != responsibleUnitId)
            {
                throw new InvalidOperationException(
                    "Takım, iş emrinin sorumlu " +
                    "organizasyon birimine ait olmalıdır.");
            }

            bool isAvailable = await _workSubTypeRepository
                .IsAvailableForUnitAsync(
                    responsibleUnitId,
                    workSubTypeId,
                    cancellationToken);

            if (!isAvailable)
            {
                throw new InvalidOperationException(
                    "Seçilen iş alt türü sorumlu " +
                    "birimde kullanılamaz.");
            }

            await ValidateAssetAsync(assetId, cancellationToken);

            if (plannedMaintenanceId.HasValue)
            {
                ValidateId(
                    plannedMaintenanceId.Value,
                    "Planlı Bakım");

                var maintenance = await _plannedMaintenanceRepository
                    .GetByIdAsync(
                        plannedMaintenanceId.Value,
                        cancellationToken);

                if (maintenance == null)
                {
                    throw new InvalidOperationException(
                        "Seçilen planlı bakım kaydı bulunamadı.");
                }
            }
        }

        private async Task ValidateAssetAsync(
            int? assetId,
            CancellationToken cancellationToken)
        {
            if (!assetId.HasValue)
            {
                return;
            }

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

            // Kullanım dışı demirbaş için iş emri açılabilir.
            // Hurda demirbaş için normal iş emri açılamaz.
            if (asset.Status == AssetStatusEnum.Scrapped)
            {
                throw new InvalidOperationException(
                    "Hurdaya ayrılmış demirbaş için " +
                    "yeni iş emri oluşturulamaz.");
            }
        }

        /// <summary>
        /// İş emri personel ilişkisindeki TeamMemberId
        /// üzerinden aktörün aktif atamasını kontrol eder.
        /// </summary>
        private async Task ValidateAssignedPersonnelAsync(
            WorkOrder workOrder,
            int userId,
            CancellationToken cancellationToken)
        {
            var personnel = await _workOrderRepository
                .GetPersonnelByWorkOrderIdAsync(
                    workOrder.Id,
                    cancellationToken);

            bool hasActivePersonnel = false;
            bool actorAssigned = false;

            foreach (var assignment in personnel)
            {
                if (!assignment.IsActive ||
                    assignment.ReleasedAt.HasValue)
                {
                    continue;
                }

                var teamMember = await _teamMemberRepository
                    .GetByIdAsync(
                        assignment.TeamMemberId,
                        cancellationToken);

                if (teamMember == null ||
                    !teamMember.IsActive ||
                    teamMember.LeftAt.HasValue ||
                    teamMember.TeamId != workOrder.TeamId)
                {
                    continue;
                }

                hasActivePersonnel = true;

                if (teamMember.UserId == userId)
                {
                    actorAssigned = true;
                }
            }

            if (!hasActivePersonnel)
            {
                throw new InvalidOperationException(
                    "İş emrine aktif takım personeli " +
                    "atanmadan iş başlatılamaz veya " +
                    "onaya gönderilemez.");
            }

            if (!actorAssigned)
            {
                throw new UnauthorizedAccessException(
                    "Bu işlem yalnızca iş emrine " +
                    "atanmış aktif personel tarafından " +
                    "gerçekleştirilebilir.");
            }
        }

        /// <summary>
        /// İptale iş emrini oluşturan yetkili kullanıcı
        /// veya onay yetkilisi tarafından izin verir.
        /// </summary>
        private async Task ValidateCancelPermissionAsync(
            WorkOrder workOrder,
            int userId,
            CancellationToken cancellationToken)
        {
            string permissionCode =
                workOrder.CreatedByUserId == userId
                    ? CreatePermission
                    : ApprovePermission;

            await ValidateActorPermissionAsync(
                userId,
                workOrder.ResponsibleUnitId,
                workOrder.WorkSubTypeId,
                permissionCode,
                cancellationToken);
        }

        /// <summary>
        /// Benzersiz iş emri numarası üretir.
        /// Veritabanında ayrıca unique index olmalıdır.
        /// </summary>
        private async Task<string> GenerateUniqueWorkOrderNumberAsync(
            CancellationToken cancellationToken)
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                string number =
                    $"ISE-{DateTime.UtcNow:yyyyMMdd}-" +
                    Guid.NewGuid()
                        .ToString("N")[..12]
                        .ToUpperInvariant();

                bool exists = await _workOrderRepository
                    .ExistsWorkOrderNumberAsync(
                        number,
                        null,
                        cancellationToken);

                if (!exists)
                {
                    return number;
                }
            }

            throw new InvalidOperationException(
                "Benzersiz iş emri numarası üretilemedi.");
        }

        #endregion
    }
}
