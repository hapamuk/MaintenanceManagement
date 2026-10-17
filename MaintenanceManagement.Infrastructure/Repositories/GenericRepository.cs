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
    public class GenericRepository<TEntity> :
         IGenericRepository<TEntity>
         where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext _context;

        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Tüm kayıtları ID sırasına göre getirir.
        /// </summary>
        public virtual async Task<List<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// ID bilgisine göre tek kayıt getirir.
        /// </summary>
        public virtual async Task<TEntity?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Kayıt varlığını kontrol eder.
        /// </summary>
        public virtual async Task<bool> ExistsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Yeni kayıt oluşturur ve veritabanına kaydeder.
        /// </summary>
        public virtual async Task<TEntity> CreateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = null;

            await _dbSet.AddAsync(
                entity,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);

            return entity;
        }

        /// <summary>
        /// Mevcut kaydı günceller ve veritabanına kaydeder.
        /// </summary>
        public virtual async Task<bool> UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var existingEntity = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == entity.Id,
                    cancellationToken);

            if (existingEntity == null)
            {
                return false;
            }

            var originalCreatedAt = existingEntity.CreatedAt;

            _context.Entry(existingEntity)
                .CurrentValues.SetValues(entity);

            existingEntity.CreatedAt = originalCreatedAt;

            existingEntity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }
    }
}
