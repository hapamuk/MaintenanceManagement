using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class UserRepository :
          GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif kullanıcıları ad ve soyada göre sıralayarak getirir.
        /// </summary>
        public async Task<List<User>> GetActiveUsersAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcıları organizasyon, rol ve takım
        /// bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<List<User>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetUsersWithDetailsQuery()
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen kullanıcıyı organizasyon, rol ve takım
        /// bilgileriyle birlikte getirir.
        /// </summary>
        public async Task<User?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await GetUsersWithDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcı detay sorgularında ortak kullanılan
        /// Navigation Property ilişkilerini oluşturur.
        /// </summary>
        private IQueryable<User> GetUsersWithDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.PrimaryUnit)
                .Include(x => x.UserUnitRoles)
                    .ThenInclude(x => x.Role)
                .Include(x => x.TeamMemberships)
                    .ThenInclude(x => x.Team)
                .AsSplitQuery();
        }

        /// <summary>
        /// Belirtilen organizasyona asıl birimi üzerinden
        /// bağlı olan aktif kullanıcıları getirir.
        /// </summary>
        public async Task<List<User>> GetUsersByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.PrimaryUnitId == unitId &&
                    x.IsActive &&
                    x.PrimaryUnit.IsActive)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen takımda aktif olarak görev yapan
        /// kullanıcıları getirir.
        /// </summary>
        public async Task<List<User>> GetUsersByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.TeamMemberships.Any(tm =>
                        tm.TeamId == teamId &&
                        tm.IsActive &&
                        tm.LeftAt == null &&
                        tm.Team.IsActive &&
                        tm.Team.Unit.IsActive))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen role aktif olarak atanmış
        /// kullanıcıları getirir.
        /// </summary>
        public async Task<List<User>> GetUsersByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.UserUnitRoles.Any(ur =>
                        ur.RoleId == roleId &&
                        ur.IsActive &&
                        ur.Role.IsActive &&
                        ur.Role.Unit.IsActive))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// E-posta adresine göre kullanıcı getirir.
        /// </summary>
        public async Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            string normalizedEmail = email.Trim();

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Email == normalizedEmail,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcı adına göre kayıt getirir.
        /// </summary>
        public async Task<User?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            string normalizedUsername = username.Trim();

            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Username == normalizedUsername,
                    cancellationToken);
        }

        /// <summary>
        /// E-posta adresinin başka bir kullanıcı tarafından
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsEmailAsync(
            string email,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string normalizedEmail = email.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Email == normalizedEmail &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcı adının başka bir kayıt tarafından
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsUsernameAsync(
            string username,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            string normalizedUsername = username.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Username == normalizedUsername &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcının aktiflik durumunu değiştirir
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var user = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (user == null)
                return false;

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Kullanıcının sisteme giriş yetkisini değiştirir
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeSystemAccessAsync(
            int id,
            bool hasSystemAccess,
            CancellationToken cancellationToken = default)
        {
            var user = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (user == null)
                return false;

            user.HasSystemAccess = hasSystemAccess;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
