
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
    public class TeamRepository :
       GenericRepository<Team>, ITeamRepository
    {
        #region Constructor

        public TeamRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        #endregion

        #region GetActiveTeams

        /// <summary>
        /// Aktif organizasyon birimlerinde bulunan
        /// aktif takımları organizasyon adına göre
        /// sıralayarak getirir.
        /// </summary>
        public async Task<List<Team>> GetActiveTeamsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Unit)
                .Where(x =>
                    x.IsActive &&
                    x.Unit.IsActive)
                .OrderBy(x => x.Unit.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetAllWithDetails

        /// <summary>
        /// Bütün takımları organizasyon, takım üyeleri
        /// ve iş emirleriyle birlikte getirir.
        /// </summary>
        public async Task<List<Team>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Unit.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetByIdWithDetails

        /// <summary>
        /// Belirtilen takımı organizasyon,
        /// personel ve iş emri detaylarıyla getirir.
        /// </summary>
        public async Task<Team?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        #endregion

        #region GetTeamsByUnitId

        /// <summary>
        /// Belirtilen organizasyona ait aktif ve
        /// pasif bütün takımları getirir.
        /// </summary>
        public async Task<List<Team>> GetTeamsByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Unit)
                .Where(x => x.UnitId == unitId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetActiveTeamsByUnitId

        /// <summary>
        /// Belirtilen organizasyona ait aktif takımları getirir.
        /// Takımın ve bağlı olduğu organizasyon biriminin
        /// aktif olması gerekir.
        ///
        /// Organizasyon türü artık OrganizationTypeEnum
        /// üzerinden yönetilir.
        /// </summary>
        public async Task<List<Team>> GetActiveTeamsByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Unit)
                .Where(x =>
                    x.UnitId == unitId &&
                    x.IsActive &&
                    x.Unit.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region GetActiveTeamMembers

        /// <summary>
        /// Takıma ait aktif personel üyeliklerini getirir.
        /// Takımdan ayrılmış veya pasif personeller listelenmez.
        /// </summary>
        public async Task<List<TeamMember>>
            GetActiveTeamMembersAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            return await _context.TeamMembers
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Team)
                .Where(x =>
                    x.TeamId == teamId &&
                    x.IsActive &&
                    x.LeftAt == null &&
                    x.User.IsActive &&
                    x.Team.IsActive &&
                    x.Team.Unit.IsActive)
                .OrderBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Aynı organizasyonda takım adının mevcut
        /// olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            int unitId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string normalizedName = name.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UnitId == unitId &&
                    x.Name == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Aynı organizasyonda takım kodunun mevcut
        /// olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            string normalizedCode = code.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UnitId == unitId &&
                    x.Code == normalizedCode &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        #endregion

        #region HasActiveMembers

        /// <summary>
        /// Belirtilen takımda aktif personel
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveMembersAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            return await _context.TeamMembers
                .AsNoTracking()
                .AnyAsync(x =>
                    x.TeamId == teamId &&
                    x.IsActive &&
                    x.LeftAt == null &&
                    x.User.IsActive &&
                    x.Team.IsActive &&
                    x.Team.Unit.IsActive,
                    cancellationToken);
        }

        #endregion

        #region HasOpenWorkOrders

        /// <summary>
        /// Takımın henüz tamamlanmamış veya iptal edilmemiş
        /// iş emri olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrders
                .AsNoTracking()
                .AnyAsync(x =>
                    x.TeamId == teamId &&
                    (
                        x.Status == WorkStatusEnum.New ||
                        x.Status == WorkStatusEnum.InProgress ||
                        x.Status == WorkStatusEnum.PendingApproval
                    ),
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Takımın aktiflik durumunu değiştirir
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var team = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (team == null)
            {
                return false;
            }

            if (team.IsActive == isActive)
            {
                return true;
            }

            team.IsActive = isActive;
            team.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        #endregion

        #region GetDetailsQuery

        /// <summary>
        /// Organizasyon birimi, takım üyeleri
        /// ve iş emirlerini içeren ortak detay sorgusu.
        ///
        /// OrganizationType navigation kaldırılmıştır.
        /// Organizasyon türü Unit.OrganizationTypeEnum
        /// üzerinden okunur.
        /// </summary>
        private IQueryable<Team> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.Unit)
                .Include(x => x.TeamMembers)
                    .ThenInclude(x => x.User)
                .Include(x => x.WorkOrders)
                .AsSplitQuery();
        }

        #endregion
    }
}
