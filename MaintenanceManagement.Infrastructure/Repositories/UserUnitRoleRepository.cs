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
    public class UserUnitRoleRepository :
        GenericRepository<UserUnitRole>,
        IUserUnitRoleRepository
    {
        public UserUnitRoleRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün kullanıcı-rol eşleştirmelerini kullanıcı,
        /// rol ve organizasyon detaylarıyla getirir.
        /// </summary>
        public async Task<List<UserUnitRole>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Role.Unit.Name)
                .ThenBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ThenBy(x => x.Role.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Eşleştirme kaydını ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<UserUnitRole?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcıya ait aktif ve pasif
        /// bütün rol eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<UserUnitRole>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.Role.Unit.Name)
                .ThenBy(x => x.Role.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının yalnızca aktif ve
        /// kullanılabilir rol eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<UserUnitRole>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive &&
                    x.User.IsActive &&
                    x.Role.IsActive &&
                    x.Role.Unit.IsActive)
                .OrderBy(x => x.Role.Unit.Name)
                .ThenBy(x => x.Role.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen role atanmış kullanıcıların
        /// aktif ve pasif eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<UserUnitRole>> GetByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.RoleId == roleId)
                .OrderBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyonun rollerine atanmış bütün
        /// kullanıcıların eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<UserUnitRole>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.Role.UnitId == unitId)
                .OrderBy(x => x.Role.Name)
                .ThenBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirtilen organizasyondaki
        /// aktif ve kullanılabilir rollerini getirir.
        /// </summary>
        public async Task<List<UserUnitRole>>
            GetActiveByUserAndUnitAsync(
                int userId,
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.UserId == userId &&
                    x.Role.UnitId == unitId &&
                    x.IsActive &&
                    x.User.IsActive &&
                    x.Role.IsActive &&
                    x.Role.Unit.IsActive)
                .OrderBy(x => x.Role.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kullanıcı ve rol ID değerlerine göre
        /// mevcut eşleştirme kaydını getirir.
        /// Pasif kayıtlar da sorgulanır.
        /// </summary>
        public async Task<UserUnitRole?> GetByUserAndRoleAsync(
            int userId,
            int roleId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.UserId == userId &&
                         x.RoleId == roleId,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcı-rol eşleştirmesinin
        /// daha önce oluşturulup oluşturulmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsAssignmentAsync(
            int userId,
            int roleId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.RoleId == roleId &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirtilen organizasyonda
        /// en az bir aktif rolü olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveRoleInUnitAsync(
            int userId,
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.Role.UnitId == unitId &&
                    x.IsActive &&
                    x.User.IsActive &&
                    x.Role.IsActive &&
                    x.Role.Unit.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcının belirtilen role aktif olarak
        /// sahip olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveRoleAsync(
            int userId,
            int roleId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.RoleId == roleId &&
                    x.IsActive &&
                    x.User.IsActive &&
                    x.Role.IsActive &&
                    x.Role.Unit.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Kullanıcı-rol eşleştirmesinin aktiflik
        /// durumunu değiştirir ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (entity == null)
            {
                return false;
            }

            entity.IsActive = isActive;

            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Kullanıcı, kullanıcının asıl birimi,
        /// rol ve rolün bağlı olduğu organizasyon
        /// bilgilerini içeren ortak sorgudur.
        /// </summary>
        private IQueryable<UserUnitRole> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Include(x => x.User)
                    .ThenInclude(x => x.PrimaryUnit)
                .Include(x => x.Role)
                    .ThenInclude(x => x.Unit);
        }
    }
}
