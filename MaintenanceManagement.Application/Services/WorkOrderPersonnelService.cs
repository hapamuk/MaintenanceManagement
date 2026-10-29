using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkOrderPersonnelDtos;
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
    public class WorkOrderPersonnelService
       : IWorkOrderPersonnelService
    {
        #region Constants

        private const string AssignPermission =
            "WORK_ORDER_CREATE";

        private const string ExecutePermission =
            "WORK_ORDER_EXECUTE";

        private const int MaxDescriptionLength = 500;

        #endregion

        #region Fields

        private readonly IWorkOrderPersonnelRepository
            _workOrderPersonnelRepository;

        private readonly IWorkOrderRepository
            _workOrderRepository;

        private readonly ITeamMemberRepository
            _teamMemberRepository;

        private readonly IUserRepository
            _userRepository;

        private readonly IRoleWorkSubTypePermissionService
            _roleWorkSubTypePermissionService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WorkOrderPersonnelService(
            IWorkOrderPersonnelRepository workOrderPersonnelRepository,
            IWorkOrderRepository workOrderRepository,
            ITeamMemberRepository teamMemberRepository,
            IUserRepository userRepository,
            IRoleWorkSubTypePermissionService
                roleWorkSubTypePermissionService,
            IMapper mapper)
        {
            _workOrderPersonnelRepository =
                workOrderPersonnelRepository;

            _workOrderRepository =
                workOrderRepository;

            _teamMemberRepository =
                teamMemberRepository;

            _userRepository =
                userRepository;

            _roleWorkSubTypePermissionService =
                roleWorkSubTypePermissionService;

            _mapper = mapper;
        }

        #endregion

        #region GetAll

        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var assignments =
                await _workOrderPersonnelRepository
                    .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetById

        public async Task<GetByIdWorkOrderPersonnelDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var assignment =
                await _workOrderPersonnelRepository
                    .GetByIdWithDetailsAsync(
                        id,
                        cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWorkOrderPersonnelDto>(
                assignment);
        }

        #endregion

        #region GetByWorkOrderId

        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var assignments =
                await _workOrderPersonnelRepository
                    .GetByWorkOrderIdAsync(
                        workOrderId,
                        cancellationToken);

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetActiveByWorkOrderId

        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetActiveByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var assignments =
                await _workOrderPersonnelRepository
                    .GetActiveByWorkOrderIdAsync(
                        workOrderId,
                        cancellationToken);

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetByTeamMemberId

        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByTeamMemberIdAsync(
                int teamMemberId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(teamMemberId, "Takım Üyesi");

            var assignments =
                await _workOrderPersonnelRepository
                    .GetByTeamMemberIdAsync(
                        teamMemberId,
                        cancellationToken);

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetByUserId

        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var assignments =
                await _workOrderPersonnelRepository
                    .GetByUserIdAsync(
                        userId,
                        cancellationToken);

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetActiveByUserId

        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetActiveByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var assignments =
                await _workOrderPersonnelRepository
                    .GetActiveByUserIdAsync(
                        userId,
                        cancellationToken);

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetByTeamId

        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Takım");

            var assignments =
                await _workOrderPersonnelRepository
                    .GetByTeamIdAsync(
                        teamId,
                        cancellationToken);

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetByAssignedByUserId

        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByAssignedByUserIdAsync(
                int assignedByUserId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                assignedByUserId,
                "Atamayı Yapan Kullanıcı");

            var assignments =
                await _workOrderPersonnelRepository
                    .GetByAssignedByUserIdAsync(
                        assignedByUserId,
                        cancellationToken);

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetByAssignmentRole

        /// <summary>
        /// Belirli iş emrindeki personelleri
        /// görev rollerine göre getirir.
        ///
        /// ÖNEMLİ:
        /// Repository metodunun parametre sırası:
        /// 1. workOrderId
        /// 2. assignmentRole
        /// 3. cancellationToken
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByAssignmentRoleAsync(
                int workOrderId,
                AssignmentRoleEnum assignmentRole,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. İŞ EMRİ ID KONTROLÜ
            // =====================================

            ValidateId(workOrderId, "İş Emri");

            // =====================================
            // 2. GÖREV ROLÜ KONTROLÜ
            // =====================================

            ValidateAssignmentRole(assignmentRole);

            // =====================================
            // 3. REPOSITORY SORGUSU
            // =====================================

            var assignments =
                await _workOrderPersonnelRepository
                    .GetByAssignmentRoleAsync(
                        workOrderId,
                        assignmentRole,
                        cancellationToken);

            // =====================================
            // 4. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<List<ResultWorkOrderPersonnelDto>>(
                assignments);
        }

        #endregion

        #region GetActiveResponsible

        public async Task<GetByIdWorkOrderPersonnelDto?>
            GetActiveResponsibleAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var assignment =
                await _workOrderPersonnelRepository
                    .GetActiveResponsibleAsync(
                        workOrderId,
                        cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            return await GetByIdAsync(
                assignment.Id,
                cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// İş emrine personel atar.
        ///
        /// Atanacak kişi:
        /// - İş emrinin takımında olmalıdır.
        /// - Aktif takım üyesi olmalıdır.
        /// - Aktif kullanıcı olmalıdır.
        /// - İlgili birim ve iş alt türünde
        ///   WORK_ORDER_EXECUTE yetkisi bulunmalıdır.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkOrderPersonnelDto dto,
            int assignedByUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.WorkOrderId, "İş Emri");
            ValidateId(dto.TeamMemberId, "Takım Üyesi");
            ValidateId(assignedByUserId, "Atamayı Yapan Kullanıcı");

            ValidateAssignmentRole(dto.AssignmentRole);

            string? description =
                NormalizeDescription(dto.Description);

            // =====================================
            // 2. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await GetWorkOrderOrThrowAsync(
                dto.WorkOrderId,
                cancellationToken);

            ValidateAssignableStatus(workOrder.Status);

            // =====================================
            // 3. ATAMAYI YAPAN KULLANICI
            // =====================================

            await ValidateAssignmentManagerAsync(
                assignedByUserId,
                workOrder,
                cancellationToken);

            // =====================================
            // 4. TAKIM ÜYESİNİ GETİR
            // =====================================

            var teamMember = await _teamMemberRepository
                .GetByIdAsync(
                    dto.TeamMemberId,
                    cancellationToken);

            if (teamMember == null)
            {
                throw new InvalidOperationException(
                    "Seçilen takım üyesi bulunamadı.");
            }

            // =====================================
            // 5. TAKIM VE PERSONEL UYGUNLUĞU
            // =====================================

            bool isEligible = await IsTeamMemberEligibleAsync(
                workOrder,
                teamMember,
                cancellationToken);

            if (!isEligible)
            {
                throw new InvalidOperationException(
                    "Personelin takım üyeliği, kullanıcı " +
                    "durumu veya iş alt türü yetkisi " +
                    "atama için uygun değildir.");
            }

            // =====================================
            // 6. MÜKERRER TAKIM ÜYELİĞİ
            // =====================================

            bool existsActiveAssignment =
                await _workOrderPersonnelRepository
                    .ExistsActiveAssignmentAsync(
                        dto.WorkOrderId,
                        dto.TeamMemberId,
                        cancellationToken);

            if (existsActiveAssignment)
            {
                throw new InvalidOperationException(
                    "Bu takım üyesi iş emrine " +
                    "zaten aktif olarak atanmıştır.");
            }

            // =====================================
            // 7. MÜKERRER KULLANICI
            // =====================================

            bool userAssigned =
                await _workOrderPersonnelRepository
                    .IsUserAssignedAsync(
                        dto.WorkOrderId,
                        teamMember.UserId,
                        cancellationToken);

            if (userAssigned)
            {
                throw new InvalidOperationException(
                    "Bu kullanıcı iş emrine başka bir " +
                    "aktif takım üyeliği üzerinden atanmıştır.");
            }

            // =====================================
            // 8. REPOSITORY UYGUNLUK KONTROLÜ
            // =====================================

            bool canBeAssigned =
                await _workOrderPersonnelRepository
                    .CanTeamMemberBeAssignedAsync(
                        dto.WorkOrderId,
                        dto.TeamMemberId,
                        cancellationToken);

            if (!canBeAssigned)
            {
                throw new InvalidOperationException(
                    "Personel iş emrine atanma " +
                    "şartlarını sağlamamaktadır.");
            }

            // =====================================
            // 9. SORUMLU PERSONEL KONTROLÜ
            // =====================================

            if (dto.AssignmentRole ==
                AssignmentRoleEnum.Responsible)
            {
                bool hasResponsible =
                    await _workOrderPersonnelRepository
                        .HasActiveResponsibleAsync(
                            dto.WorkOrderId,
                            cancellationToken);

                if (hasResponsible)
                {
                    throw new InvalidOperationException(
                        "Bu iş emrinde zaten aktif " +
                        "bir sorumlu personel bulunmaktadır.");
                }
            }

            // =====================================
            // 10. DTO -> ENTITY
            // =====================================

            var assignment =
                _mapper.Map<WorkOrderPersonnel>(dto);

            assignment.WorkOrderId = dto.WorkOrderId;
            assignment.TeamMemberId = dto.TeamMemberId;

            assignment.AssignmentRole = dto.AssignmentRole;
            assignment.Description = description;

            assignment.AssignedByUserId = assignedByUserId;

            // AssignedAt, ReleasedAt, IsActive
            // Repository tarafından yönetilir.

            // =====================================
            // 11. PERSONEL ATAMA İŞLEMİ
            // =====================================

            var createdAssignment =
                await _workOrderPersonnelRepository
                    .AssignPersonnelAsync(
                        assignment,
                        cancellationToken);

            return createdAssignment.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Aktif personel atamasının görev rolünü
        /// ve açıklamasını günceller.
        ///
        /// İş emri New durumunda olmalıdır.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkOrderPersonnelDto dto,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Personel Ataması");
            ValidateId(performedByUserId, "İşlemi Yapan Kullanıcı");

            ValidateAssignmentRole(dto.AssignmentRole);

            string? description =
                NormalizeDescription(dto.Description);

            // =====================================
            // 1. MEVCUT ATAMAYI GETİR
            // =====================================

            var existingAssignment =
                await _workOrderPersonnelRepository
                    .GetByIdAsync(
                        dto.Id,
                        cancellationToken);

            if (existingAssignment == null)
            {
                return false;
            }

            // =====================================
            // 2. ATAMANIN AKTİFLİĞİ
            // =====================================

            if (!existingAssignment.IsActive ||
                existingAssignment.ReleasedAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Görevi sonlandırılmış personel " +
                    "ataması güncellenemez.");
            }

            // =====================================
            // 3. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await GetWorkOrderOrThrowAsync(
                existingAssignment.WorkOrderId,
                cancellationToken);

            if (workOrder.Status != WorkStatusEnum.New)
            {
                throw new InvalidOperationException(
                    "Personel görev rolü yalnızca " +
                    "Yeni durumundaki iş emirlerinde " +
                    "güncellenebilir.");
            }

            // =====================================
            // 4. YÖNETİCİ YETKİSİ
            // =====================================

            await ValidateAssignmentManagerAsync(
                performedByUserId,
                workOrder,
                cancellationToken);

            // =====================================
            // 5. MEVCUT TAKIM ÜYESİ KONTROLÜ
            // =====================================

            var teamMember = await _teamMemberRepository
                .GetByIdAsync(
                    existingAssignment.TeamMemberId,
                    cancellationToken);

            if (teamMember == null)
            {
                throw new InvalidOperationException(
                    "Atamaya bağlı takım üyesi bulunamadı.");
            }

            bool isEligible = await IsTeamMemberEligibleAsync(
                workOrder,
                teamMember,
                cancellationToken);

            if (!isEligible)
            {
                throw new InvalidOperationException(
                    "Personelin mevcut takım veya " +
                    "iş alt türü yetkisi uygun değildir.");
            }

            // =====================================
            // 6. GÖREV ROLÜ DEĞİŞİYOR MU?
            // =====================================

            bool roleChanged =
                existingAssignment.AssignmentRole !=
                dto.AssignmentRole;

            // =====================================
            // 7. SORUMLU PERSONEL KONTROLÜ
            // =====================================

            if (roleChanged &&
                dto.AssignmentRole ==
                    AssignmentRoleEnum.Responsible)
            {
                var activeResponsible =
                    await _workOrderPersonnelRepository
                        .GetActiveResponsibleAsync(
                            workOrder.Id,
                            cancellationToken);

                // Mevcut sorumlu başka bir atama ise
                // ikinci sorumlu oluşturulamaz.
                if (activeResponsible != null &&
                    activeResponsible.Id !=
                        existingAssignment.Id)
                {
                    throw new InvalidOperationException(
                        "İş emrinde başka bir aktif " +
                        "sorumlu personel bulunmaktadır.");
                }
            }

            // =====================================
            // 8. DTO -> ENTITY
            // =====================================

            _mapper.Map(dto, existingAssignment);

            existingAssignment.AssignmentRole =
                dto.AssignmentRole;

            existingAssignment.Description =
                description;

            // WorkOrderId, TeamMemberId,
            // AssignedByUserId, AssignedAt,
            // ReleasedAt, IsActive ve
            // diğer sistem alanları korunur.

            // =====================================
            // 9. GÜNCELLEME
            // =====================================

            return await _workOrderPersonnelRepository
                .UpdateAsync(
                    existingAssignment,
                    cancellationToken);
        }

        #endregion

        #region ReleasePersonnel

        /// <summary>
        /// Personelin iş emrindeki görevini sonlandırır.
        /// Kayıt fiziksel olarak silinmez.
        /// </summary>
        public async Task<bool> ReleasePersonnelAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(id, "Personel Ataması");
            ValidateId(performedByUserId, "İşlemi Yapan Kullanıcı");

            // =====================================
            // 1. MEVCUT ATAMAYI GETİR
            // =====================================

            var assignment =
                await _workOrderPersonnelRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (assignment == null)
            {
                return false;
            }

            if (!assignment.IsActive ||
                assignment.ReleasedAt.HasValue)
            {
                return false;
            }

            // =====================================
            // 2. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await GetWorkOrderOrThrowAsync(
                assignment.WorkOrderId,
                cancellationToken);

            ValidateAssignableStatus(workOrder.Status);

            // =====================================
            // 3. YETKİ KONTROLÜ
            // =====================================

            await ValidateAssignmentManagerAsync(
                performedByUserId,
                workOrder,
                cancellationToken);

            // =====================================
            // 4. DEVAM EDEN İŞ EMRİ KONTROLLERİ
            // =====================================

            if (workOrder.Status == WorkStatusEnum.InProgress)
            {
                int activePersonnelCount =
                    await _workOrderPersonnelRepository
                        .GetActivePersonnelCountAsync(
                            workOrder.Id,
                            cancellationToken);

                if (activePersonnelCount <= 1)
                {
                    throw new InvalidOperationException(
                        "Devam eden iş emrinin son " +
                        "aktif personeli görevden alınamaz.");
                }

                if (assignment.AssignmentRole ==
                    AssignmentRoleEnum.Responsible)
                {
                    throw new InvalidOperationException(
                        "Devam eden iş emrinin sorumlu " +
                        "personeli doğrudan görevden alınamaz.");
                }
            }

            // =====================================
            // 5. GÖREVDEN ALMA
            // =====================================

            return await _workOrderPersonnelRepository
                .ReleasePersonnelAsync(
                    id,
                    DateTime.UtcNow,
                    cancellationToken);
        }

        #endregion

        #region ExistsActiveAssignment

        public async Task<bool> ExistsActiveAssignmentAsync(
            int workOrderId,
            int teamMemberId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");
            ValidateId(teamMemberId, "Takım Üyesi");

            return await _workOrderPersonnelRepository
                .ExistsActiveAssignmentAsync(
                    workOrderId,
                    teamMemberId,
                    cancellationToken);
        }

        #endregion

        #region IsUserAssigned

        public async Task<bool> IsUserAssignedAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");
            ValidateId(userId, "Kullanıcı");

            return await _workOrderPersonnelRepository
                .IsUserAssignedAsync(
                    workOrderId,
                    userId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveResponsible

        public async Task<bool> HasActiveResponsibleAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            return await _workOrderPersonnelRepository
                .HasActiveResponsibleAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveAssignmentsByTeamMemberId

        public async Task<bool> HasActiveAssignmentsByTeamMemberIdAsync(
            int teamMemberId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(teamMemberId, "Takım Üyesi");

            return await _workOrderPersonnelRepository
                .HasActiveAssignmentsByTeamMemberIdAsync(
                    teamMemberId,
                    cancellationToken);
        }

        #endregion

        #region GetActivePersonnelCount

        public async Task<int> GetActivePersonnelCountAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            return await _workOrderPersonnelRepository
                .GetActivePersonnelCountAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region CanTeamMemberBeAssigned

        /// <summary>
        /// Personelin iş emrine atanabilirliğini kontrol eder.
        /// </summary>
        public async Task<bool> CanTeamMemberBeAssignedAsync(
            int workOrderId,
            int teamMemberId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");
            ValidateId(teamMemberId, "Takım Üyesi");

            // =====================================
            // 1. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await _workOrderRepository
                .GetByIdAsync(
                    workOrderId,
                    cancellationToken);

            if (workOrder == null)
            {
                return false;
            }

            if (workOrder.Status != WorkStatusEnum.New &&
                workOrder.Status != WorkStatusEnum.InProgress)
            {
                return false;
            }

            // =====================================
            // 2. TAKIM ÜYESİNİ GETİR
            // =====================================

            var teamMember = await _teamMemberRepository
                .GetByIdAsync(
                    teamMemberId,
                    cancellationToken);

            if (teamMember == null)
            {
                return false;
            }

            // =====================================
            // 3. UYGUNLUK
            // =====================================

            bool isEligible = await IsTeamMemberEligibleAsync(
                workOrder,
                teamMember,
                cancellationToken);

            if (!isEligible)
            {
                return false;
            }

            // =====================================
            // 4. MÜKERRER TAKIM ÜYELİĞİ
            // =====================================

            bool existsActiveAssignment =
                await _workOrderPersonnelRepository
                    .ExistsActiveAssignmentAsync(
                        workOrderId,
                        teamMemberId,
                        cancellationToken);

            if (existsActiveAssignment)
            {
                return false;
            }

            // =====================================
            // 5. MÜKERRER KULLANICI
            // =====================================

            bool userAssigned =
                await _workOrderPersonnelRepository
                    .IsUserAssignedAsync(
                        workOrderId,
                        teamMember.UserId,
                        cancellationToken);

            if (userAssigned)
            {
                return false;
            }

            // =====================================
            // 6. REPOSITORY DOĞRULAMASI
            // =====================================

            return await _workOrderPersonnelRepository
                .CanTeamMemberBeAssignedAsync(
                    workOrderId,
                    teamMemberId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

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
        /// Görev rolünün geçerli olduğunu doğrular.
        /// </summary>
        private static void ValidateAssignmentRole(
            AssignmentRoleEnum assignmentRole)
        {
            if (!Enum.IsDefined(
                    typeof(AssignmentRoleEnum),
                    assignmentRole))
            {
                throw new ValidationException(
                    "Geçersiz personel görev rolü.");
            }
        }

        /// <summary>
        /// Atama açıklamasını normalize eder.
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
                    "Atama açıklaması en fazla " +
                    "500 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// İş emrini getirir.
        /// </summary>
        private async Task<WorkOrder> GetWorkOrderOrThrowAsync(
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

            return workOrder;
        }

        /// <summary>
        /// Personel atama ve görevden alma işlemlerinin
        /// yapılabileceği iş emri durumlarını kontrol eder.
        /// </summary>
        private static void ValidateAssignableStatus(
            WorkStatusEnum status)
        {
            if (status != WorkStatusEnum.New &&
                status != WorkStatusEnum.InProgress)
            {
                throw new InvalidOperationException(
                    "Personel işlemleri yalnızca " +
                    "Yeni veya İşe Başlandı durumundaki " +
                    "iş emirlerinde gerçekleştirilebilir.");
            }
        }

        /// <summary>
        /// Atama yapan kullanıcının aktif olduğunu ve
        /// iş emri oluşturma yetkisinin bulunduğunu doğrular.
        /// </summary>
        private async Task ValidateAssignmentManagerAsync(
            int userId,
            WorkOrder workOrder,
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
                    "İşlemi gerçekleştiren kullanıcı aktif " +
                    "değildir veya sistem erişimi yoktur.");
            }

            bool hasPermission =
                await _roleWorkSubTypePermissionService
                    .HasUserActivePermissionAsync(
                        userId,
                        workOrder.ResponsibleUnitId,
                        workOrder.WorkSubTypeId,
                        AssignPermission,
                        cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının bu iş emrinde " +
                    "personel atama veya görevden alma " +
                    "yetkisi bulunmamaktadır.");
            }
        }

        /// <summary>
        /// Takım üyesinin iş emrine atanabilecek
        /// durumda olup olmadığını kontrol eder.
        ///
        /// Bu metot mükerrer atama sorgusu yapmaz.
        /// Böylece mevcut aktif atamanın rolü
        /// güncellenirken yanlışlıkla reddedilmez.
        /// </summary>
        private async Task<bool> IsTeamMemberEligibleAsync(
            WorkOrder workOrder,
            TeamMember teamMember,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. TAKIM ÜYELİĞİ
            // =====================================

            if (!teamMember.IsActive ||
                teamMember.LeftAt.HasValue)
            {
                return false;
            }

            if (teamMember.TeamId != workOrder.TeamId)
            {
                return false;
            }

            // =====================================
            // 2. KULLANICI DURUMU
            // =====================================

            var user = await _userRepository
                .GetByIdAsync(
                    teamMember.UserId,
                    cancellationToken);

            if (user == null ||
                !user.IsActive ||
                !user.HasSystemAccess)
            {
                return false;
            }

            // =====================================
            // 3. İŞ ALT TÜRÜ YETKİSİ
            // =====================================

            return await _roleWorkSubTypePermissionService
                .HasUserActivePermissionAsync(
                    teamMember.UserId,
                    workOrder.ResponsibleUnitId,
                    workOrder.WorkSubTypeId,
                    ExecutePermission,
                    cancellationToken);
        }

        #endregion
    }
}
