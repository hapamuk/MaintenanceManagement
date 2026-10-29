using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class WorkOrderPersonnelRepository :
        GenericRepository<WorkOrderPersonnel>,
        IWorkOrderPersonnelRepository
    {
        public WorkOrderPersonnelRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün personel atamalarını iş emri,
        /// takım, personel ve görevlendiren
        /// kullanıcı detaylarıyla getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen personel atamasını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<WorkOrderPersonnel?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrine ait geçmiş ve mevcut
        /// bütün personel atamalarını getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde halen aktif olan
        /// personel atamalarını getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetActiveByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetActiveAssignmentsQuery()
                .Include(x => x.TeamMember)
                    .ThenInclude(x => x.User)
                .Include(x => x.AssignedByUser)
                .Where(x => x.WorkOrderId == workOrderId)
                .OrderBy(x => x.AssignmentRole)
                .ThenBy(x => x.AssignedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Takım üyesinin bütün geçmiş
        /// ve mevcut görevlendirmelerini getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetByTeamMemberIdAsync(
                int teamMemberId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.TeamMemberId == teamMemberId)
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının farklı takım üyelikleri
        /// üzerinden aldığı bütün görevleri getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.TeamMember.UserId == userId)
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının halen aktif olan
        /// personel atamalarını getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetActiveByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.TeamMember.UserId == userId &&
                    x.IsActive &&
                    x.ReleasedAt == null)
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen takıma ait personellerin
        /// iş emri görevlendirmelerini getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.TeamMember.TeamId == teamId)
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// gerçekleştirilen personel atamalarını getirir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetByAssignedByUserIdAsync(
                int assignedByUserId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.AssignedByUserId == assignedByUserId)
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrindeki belirtilen görev rolüne
        /// ait bütün görevlendirmeleri getirir.
        ///
        /// Geçmiş kayıtlar da sorguya dahildir.
        /// </summary>
        public async Task<List<WorkOrderPersonnel>>
            GetByAssignmentRoleAsync(
                int workOrderId,
                AssignmentRoleEnum assignmentRole,
                CancellationToken cancellationToken = default)
        {
            ValidateAssignmentRole(assignmentRole);

            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.AssignmentRole == assignmentRole)
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinin halen aktif olan
        /// sorumlu personelini getirir.
        /// </summary>
        public async Task<WorkOrderPersonnel?>
            GetActiveResponsibleAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkOrderId == workOrderId &&
                    x.AssignmentRole ==
                        AssignmentRoleEnum.Responsible &&
                    x.IsActive &&
                    x.ReleasedAt == null)
                .OrderByDescending(x => x.AssignedAt)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Aynı takım üyesinin iş emrinde
        /// aktif görevlendirmesi olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsActiveAssignmentAsync(
            int workOrderId,
            int teamMemberId,
            CancellationToken cancellationToken = default)
        {
            return await GetActiveAssignmentsQuery()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.TeamMemberId == teamMemberId,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirtilen iş emrinde
        /// aktif olarak görevli olup olmadığını
        /// kontrol eder.
        ///
        /// Farklı takım üyelikleri de dikkate alınır.
        /// </summary>
        public async Task<bool> IsUserAssignedAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await GetActiveAssignmentsQuery()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.TeamMember.UserId == userId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinde aktif sorumlu personel
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveResponsibleAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await GetActiveAssignmentsQuery()
                .AnyAsync(x =>
                    x.WorkOrderId == workOrderId &&
                    x.AssignmentRole ==
                        AssignmentRoleEnum.Responsible,
                    cancellationToken);
        }

        /// <summary>
        /// Takım üyesinin aktif iş emri ataması
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool>
            HasActiveAssignmentsByTeamMemberIdAsync(
                int teamMemberId,
                CancellationToken cancellationToken = default)
        {
            return await GetActiveAssignmentsQuery()
                .AnyAsync(
                    x => x.TeamMemberId == teamMemberId,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrindeki aktif personel
        /// atamalarının sayısını getirir.
        /// </summary>
        public async Task<int> GetActivePersonnelCountAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            return await GetActiveAssignmentsQuery()
                .CountAsync(
                    x => x.WorkOrderId == workOrderId,
                    cancellationToken);
        }

        /// <summary>
        /// Takım üyesinin iş emrine atanmak için
        /// uygun olup olmadığını kontrol eder.
        ///
        /// Şu koşulları doğrular:
        /// - İş emri New veya InProgress olmalı.
        /// - Takım iş emrinin sorumlu birimine ait olmalı.
        /// - Takım, personel ve organizasyon aktif olmalı.
        /// - Takım üyeliği geçerli olmalı.
        /// - İş türü organizasyonda kullanılabilir olmalı.
        /// - İş alt türü aktif olmalı.
        /// - Kullanıcının ilgili birimde aktif rolü bulunmalı.
        /// - Rolün WORK_ORDER_EXECUTE yetkisi bulunmalı.
        /// </summary>
        public async Task<bool> CanTeamMemberBeAssignedAsync(
            int workOrderId,
            int teamMemberId,
            CancellationToken cancellationToken = default)
        {
            if (workOrderId <= 0 || teamMemberId <= 0)
            {
                return false;
            }

            var workOrder = await _context.Set<WorkOrder>()
                .AsNoTracking()
                .Where(x => x.Id == workOrderId)
                .Select(x => new
                {
                    x.TeamId,
                    x.ResponsibleUnitId,
                    x.WorkSubTypeId,
                    WorkTypeId = x.WorkSubType.WorkTypeId,
                    WorkSubTypeIsActive = x.WorkSubType.IsActive,
                    WorkTypeIsActive = x.WorkSubType.WorkType.IsActive,
                    x.Status
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (workOrder == null)
            {
                return false;
            }

            // Tamamlanan, iptal edilen veya onay
            // bekleyen işlere yeni personel atanmaz.
            if (workOrder.Status != WorkStatusEnum.New &&
                workOrder.Status != WorkStatusEnum.InProgress)
            {
                return false;
            }

            if (!workOrder.WorkSubTypeIsActive ||
                !workOrder.WorkTypeIsActive)
            {
                return false;
            }

            // İş türünün organizasyona aktif
            // olarak tanımlandığını kontrol et.
            bool isWorkTypeAvailable =
                await _context.Set<UnitWorkType>()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.UnitId == workOrder.ResponsibleUnitId &&
                        x.WorkTypeId == workOrder.WorkTypeId &&
                        x.IsActive &&
                        x.Unit.IsActive &&
                        x.Unit.OrganizationType.IsActive &&
                        x.WorkType.IsActive,
                        cancellationToken);

            if (!isWorkTypeAvailable)
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;

            const string permissionCode = "WORK_ORDER_EXECUTE";

            // Takım üyesinin aktifliği,
            // organizasyon ilişkisi ve
            // rol bazlı iş alt türü yetkisi kontrol edilir.
            return await _context.Set<TeamMember>()
                .AsNoTracking()
                .AnyAsync(tm =>
                    tm.Id == teamMemberId &&

                    tm.TeamId == workOrder.TeamId &&

                    tm.IsActive &&

                    tm.JoinedAt <= now &&

                    (!tm.LeftAt.HasValue ||
                     tm.LeftAt.Value > now) &&

                    tm.Team.IsActive &&

                    tm.Team.UnitId ==
                        workOrder.ResponsibleUnitId &&

                    tm.Team.Unit.IsActive &&

                    tm.Team.Unit.OrganizationType.IsActive &&

                    tm.User.IsActive &&

                    // Kullanıcının ilgili organizasyonda
                    // aktif rolü ve iş alt türü izni olmalı.
                    tm.User.UserUnitRoles.Any(ur =>
                        ur.IsActive &&

                        ur.Role.IsActive &&

                        ur.Role.UnitId ==
                            workOrder.ResponsibleUnitId &&

                        _context.Set<RoleWorkSubTypePermission>()
                            .Any(rp =>
                                rp.RoleId == ur.RoleId &&

                                rp.WorkSubTypeId ==
                                    workOrder.WorkSubTypeId &&

                                rp.IsActive &&

                                rp.Permission.IsActive &&

                                rp.Permission.Code ==
                                    permissionCode)),
                    cancellationToken);
        }

        /// <summary>
        /// İş emrine personel ataması oluşturur.
        ///
        /// Mükerrer aktif görev, aynı kullanıcının
        /// farklı üyeliklerle ikinci kez atanması,
        /// sorumlu personel çakışması ve personel
        /// uygunluğu kontrol edilir.
        /// </summary>
        public async Task<WorkOrderPersonnel> AssignPersonnelAsync(
            WorkOrderPersonnel assignment,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(assignment);

            if (assignment.Id != 0)
            {
                throw new ArgumentException(
                    "Yeni görevlendirmenin ID değeri sıfır olmalıdır.",
                    nameof(assignment));
            }

            if (assignment.WorkOrderId <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir iş emri seçilmelidir.",
                    nameof(assignment));
            }

            if (assignment.TeamMemberId <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir takım üyesi seçilmelidir.",
                    nameof(assignment));
            }

            if (assignment.AssignedByUserId <= 0)
            {
                throw new ArgumentException(
                    "Görevlendirmeyi yapan kullanıcı belirtilmelidir.",
                    nameof(assignment));
            }

            ValidateAssignmentRole(assignment.AssignmentRole);

            // Görevlendiren kullanıcının mevcut
            // ve aktif olduğunu kontrol et.
            bool assigningUserExists = await _context.Set<User>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == assignment.AssignedByUserId &&
                    x.IsActive,
                    cancellationToken);

            if (!assigningUserExists)
            {
                throw new KeyNotFoundException(
                    "Görevlendirmeyi yapan aktif kullanıcı bulunamadı.");
            }

            bool isEligible =
                await CanTeamMemberBeAssignedAsync(
                    assignment.WorkOrderId,
                    assignment.TeamMemberId,
                    cancellationToken);

            if (!isEligible)
            {
                throw new InvalidOperationException(
                    "Personelin takım üyeliği, organizasyonu " +
                    "veya iş alt türü yetkisi bu iş emrine " +
                    "atanması için uygun değildir.");
            }

            // Aynı takım üyesi aynı iş emrine
            // ikinci kez aktif olarak atanamaz.
            bool hasActiveAssignment =
                await ExistsActiveAssignmentAsync(
                    assignment.WorkOrderId,
                    assignment.TeamMemberId,
                    cancellationToken);

            if (hasActiveAssignment)
            {
                throw new InvalidOperationException(
                    "Bu takım üyesi iş emrine zaten atanmış.");
            }

            // Personelin UserId bilgisini getir.
            int userId = await _context.Set<TeamMember>()
                .AsNoTracking()
                .Where(x => x.Id == assignment.TeamMemberId)
                .Select(x => x.UserId)
                .FirstAsync(cancellationToken);

            // Aynı personelin farklı takım üyelikleri
            // üzerinden aynı iş emrine ikinci kez
            // atanmasını engelle.
            bool isUserAlreadyAssigned =
                await IsUserAssignedAsync(
                    assignment.WorkOrderId,
                    userId,
                    cancellationToken);

            if (isUserAlreadyAssigned)
            {
                throw new InvalidOperationException(
                    "Bu personel iş emrinde zaten aktif görevli.");
            }

            // İş emrinde aynı anda tek bir aktif
            // sorumlu personel bulunabilir.
            if (assignment.AssignmentRole ==
                AssignmentRoleEnum.Responsible)
            {
                bool hasResponsible =
                    await HasActiveResponsibleAsync(
                        assignment.WorkOrderId,
                        cancellationToken);

                if (hasResponsible)
                {
                    throw new InvalidOperationException(
                        "Bu iş emrinde zaten aktif " +
                        "bir sorumlu personel bulunmaktadır.");
                }
            }

            DateTime now = DateTime.UtcNow;

            assignment.AssignedAt = now;
            assignment.ReleasedAt = null;
            assignment.IsActive = true;

            assignment.Description =
                assignment.Description?.Trim();

            assignment.CreatedAt = now;
            assignment.UpdatedAt = null;

            await _dbSet.AddAsync(
                assignment,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            return assignment;
        }

        /// <summary>
        /// Aktif personel görevlendirmesini sonlandırır.
        ///
        /// IsActive false yapılır ve ReleasedAt doldurulur.
        /// Kayıt fiziksel olarak silinmez.
        /// </summary>
        public async Task<bool> ReleasePersonnelAsync(
            int assignmentId,
            DateTime? releasedAtUtc = null,
            CancellationToken cancellationToken = default)
        {
            var assignment = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == assignmentId,
                    cancellationToken);

            if (assignment == null)
            {
                return false;
            }

            // Zaten sona erdirilmiş kayıt
            // ikinci kez sonlandırılmaz.
            if (!assignment.IsActive ||
                assignment.ReleasedAt.HasValue)
            {
                return false;
            }

            DateTime now = DateTime.UtcNow;

            DateTime releaseDate = releasedAtUtc ?? now;

            if (releaseDate.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Görevden alınma tarihi UTC olmalıdır.",
                    nameof(releasedAtUtc));
            }

            if (releaseDate < assignment.AssignedAt)
            {
                throw new InvalidOperationException(
                    "Görevden alınma tarihi atama tarihinden önce olamaz.");
            }

            if (releaseDate > now)
            {
                throw new InvalidOperationException(
                    "Görevden alınma tarihi gelecekte olamaz.");
            }

            assignment.IsActive = false;
            assignment.ReleasedAt = releaseDate;
            assignment.UpdatedAt = now;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Yalnızca aktif durumda bulunan
        /// personel atamalarını sorgular.
        /// </summary>
        private IQueryable<WorkOrderPersonnel>
            GetActiveAssignmentsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.ReleasedAt == null);
        }

        /// <summary>
        /// Personel atamasıyla ilişkili iş emri,
        /// iş türü, takım, personel ve görevlendiren
        /// kullanıcı detaylarını getirir.
        /// </summary>
        private IQueryable<WorkOrderPersonnel> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş emri ve sorumlu organizasyon
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.ResponsibleUnit)

                // İş emrinin takımı
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.Team)

                // İş alt türü ve iş türü
                .Include(x => x.WorkOrder)
                    .ThenInclude(x => x.WorkSubType)
                        .ThenInclude(x => x.WorkType)

                // Takım üyesi ve personel
                .Include(x => x.TeamMember)
                    .ThenInclude(x => x.User)
                        .ThenInclude(x => x.PrimaryUnit)

                // Takım üyesinin takımı
                .Include(x => x.TeamMember)
                    .ThenInclude(x => x.Team)
                        .ThenInclude(x => x.Unit)

                // Görevlendirmeyi yapan kullanıcı
                .Include(x => x.AssignedByUser);
        }

        /// <summary>
        /// Görev rolünün AssignmentRoleEnum
        /// içerisinde tanımlı olduğunu doğrular.
        /// </summary>
        private static void ValidateAssignmentRole(
            AssignmentRoleEnum assignmentRole)
        {
            if (!Enum.IsDefined(
                typeof(AssignmentRoleEnum),
                assignmentRole))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(assignmentRole),
                    "Geçersiz personel görev rolü.");
            }
        }

    }
}
