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
    public class TeamMemberRepository :
      GenericRepository<TeamMember>,
      ITeamMemberRepository
    {
        public TeamMemberRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün takım üyeliklerini ilişkili
        /// kullanıcı ve organizasyon bilgileriyle getirir.
        /// </summary>
        public async Task<List<TeamMember>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Team.Unit.Name)
                .ThenBy(x => x.Team.Name)
                .ThenBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Üyelik kaydını kullanıcı, takım, organizasyon
        /// ve iş emri atamalarıyla birlikte getirir.
        /// </summary>
        public async Task<TeamMember?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Include(x => x.WorkOrderAssignments)
                    .ThenInclude(x => x.WorkOrder)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Takıma ait geçmiş ve mevcut
        /// bütün üyelikleri getirir.
        /// </summary>
        public async Task<List<TeamMember>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.TeamId == teamId)
                .OrderBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ThenByDescending(x => x.JoinedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Takımda halen görev yapan
        /// aktif personelleri getirir.
        /// </summary>
        public async Task<List<TeamMember>> GetActiveByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            return await GetActiveMembershipQuery()
                .Include(x => x.User)
                .Include(x => x.Team)
                .Where(x => x.TeamId == teamId)
                .OrderBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının mevcut ve geçmiş
        /// bütün takım üyeliklerini getirir.
        /// </summary>
        public async Task<List<TeamMember>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Team.Unit.Name)
                .ThenBy(x => x.Team.Name)
                .ThenByDescending(x => x.JoinedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının aktif olarak görev yaptığı
        /// takımlardaki üyeliklerini getirir.
        /// </summary>
        public async Task<List<TeamMember>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await GetActiveMembershipQuery()
                .Include(x => x.Team)
                    .ThenInclude(x => x.Unit)
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Team.Unit.Name)
                .ThenBy(x => x.Team.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirli takımdaki
        /// mevcut aktif üyeliğini getirir.
        /// </summary>
        public async Task<TeamMember?> GetActiveByUserAndTeamAsync(
            int userId,
            int teamId,
            CancellationToken cancellationToken = default)
        {
            return await GetActiveMembershipQuery()
                .Include(x => x.User)
                .Include(x => x.Team)
                .FirstOrDefaultAsync(
                    x => x.UserId == userId &&
                         x.TeamId == teamId,
                    cancellationToken);
        }

        /// <summary>
        /// Aynı kullanıcı ve takım için halen
        /// aktif bir üyelik olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsActiveMembershipAsync(
            int teamId,
            int userId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.TeamId == teamId &&
                    x.UserId == userId &&
                    x.IsActive &&
                    x.LeftAt == null &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Takım içerisindeki ilgili iş alt türünde
        /// WORK_ORDER_EXECUTE yetkisine sahip
        /// aktif personelleri getirir.
        /// </summary>
        public async Task<List<TeamMember>>
            GetEligibleForWorkOrderAsync(
                int teamId,
                int unitId,
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            return await GetEligibleForWorkOrderQuery(
                    teamId,
                    unitId,
                    workSubTypeId)
                .Include(x => x.User)
                .Include(x => x.Team)
                    .ThenInclude(x => x.Unit)
                .OrderBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirli bir takım üyesinin iş emrine
        /// atanmak için bütün yetki şartlarını
        /// sağlayıp sağlamadığını kontrol eder.
        /// </summary>
        public async Task<bool> IsEligibleForWorkOrderAsync(
            int teamMemberId,
            int teamId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            return await GetEligibleForWorkOrderQuery(
                    teamId,
                    unitId,
                    workSubTypeId)
                .AnyAsync(
                    x => x.Id == teamMemberId,
                    cancellationToken);
        }

        /// <summary>
        /// Personelin henüz kapatılmamış iş emirlerinde
        /// aktif görevlendirmesi bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrderAssignmentsAsync(
            int teamMemberId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrderPersonnels
                .AsNoTracking()
                .AnyAsync(x =>
                    x.TeamMemberId == teamMemberId &&
                    x.IsActive &&
                    x.ReleasedAt == null &&
                    (
                        x.WorkOrder.Status == WorkStatusEnum.New ||
                        x.WorkOrder.Status == WorkStatusEnum.InProgress ||
                        x.WorkOrder.Status == WorkStatusEnum.PendingApproval
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// Takım üyeliğinin aktiflik durumunu değiştirir.
        /// Ayrılışı tamamlanmış üyelikler bu metotla
        /// yeniden aktif hale getirilemez.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var member = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (member == null)
            {
                return false;
            }

            if (isActive && member.LeftAt.HasValue)
            {
                return false;
            }

            if (!isActive &&
                member.IsActive &&
                await HasOpenWorkOrderAssignmentsAsync(
                    id, cancellationToken))
            {
                return false;
            }

            if (member.IsActive == isActive)
            {
                return true;
            }

            member.IsActive = isActive;
            member.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Takım üyeliğini sonlandırır.
        /// Ayrılış tarihini doldurur ve üyeliği pasife alır.
        /// Açık iş emri görevlendirmesi varsa işlem yapılmaz.
        /// </summary>
        public async Task<bool> EndMembershipAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var member = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (member == null)
            {
                return false;
            }

            if (member.LeftAt.HasValue)
            {
                return false;
            }

            bool hasOpenAssignments =
                await HasOpenWorkOrderAssignmentsAsync(
                    id, cancellationToken);

            if (hasOpenAssignments)
            {
                return false;
            }

            member.LeftAt = DateTime.UtcNow;
            member.IsActive = false;
            member.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Takım, organizasyon, kullanıcı ve kullanıcının
        /// asıl birimini içeren ortak detay sorgusu.
        /// </summary>
        private IQueryable<TeamMember> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.Team)
                    .ThenInclude(x => x.Unit)
                .Include(x => x.User)
                    .ThenInclude(x => x.PrimaryUnit);
        }

        /// <summary>
        /// Fiilen kullanılabilir durumda bulunan
        /// aktif takım üyeliklerini sorgular.
        /// </summary>
        private IQueryable<TeamMember> GetActiveMembershipQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.LeftAt == null &&
                    x.User.IsActive &&
                    x.Team.IsActive &&
                    x.Team.Unit.IsActive);
        }

        /// <summary>
        /// Takım üyeliği, organizasyonun iş türü,
        /// kullanıcının rolü ve iş alt türündeki
        /// uygulama yetkisini birlikte doğrular.
        /// </summary>
        private IQueryable<TeamMember> GetEligibleForWorkOrderQuery(
            int teamId,
            int unitId,
            int workSubTypeId)
        {
            return GetActiveMembershipQuery()
                .Where(x =>
                    x.TeamId == teamId &&
                    x.Team.UnitId == unitId &&

                    // İş alt türü organizasyonun
                    // izin verdiği iş türüne bağlı olmalı.
                    _context.UnitWorkTypes.Any(uwt =>
                        uwt.UnitId == unitId &&
                        uwt.IsActive &&
                        uwt.WorkType.IsActive &&
                        uwt.WorkType.WorkSubTypes.Any(st =>
                            st.Id == workSubTypeId &&
                            st.IsActive)) &&

                    // Kullanıcının bu organizasyonda
                    // aktif ve yetkili bir rolü bulunmalı.
                    x.User.UserUnitRoles.Any(ur =>
                        ur.IsActive &&
                        ur.Role.IsActive &&
                        ur.Role.UnitId == unitId &&

                        // Rol ilgili iş alt türünde
                        // çalışma yetkisine sahip olmalı.
                        ur.Role.WorkSubTypePermissions.Any(rp =>
                            rp.IsActive &&
                            rp.WorkSubTypeId == workSubTypeId &&
                            rp.WorkSubType.IsActive &&
                            rp.Permission.IsActive &&
                            rp.Permission.Code ==
                                "WORK_ORDER_EXECUTE")));
        }
    }
}
