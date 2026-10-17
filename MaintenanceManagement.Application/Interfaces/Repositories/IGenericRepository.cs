using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity>
       where TEntity : BaseEntity
    {
        /// <summary>
        /// Tüm kayıtları getirir.
        /// </summary>
        Task<List<TEntity>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine sahip kaydı getirir.
        /// </summary>
        Task<TEntity?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine sahip kayıt var mı kontrol eder.
        /// </summary>
        Task<bool> ExistsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni kayıt oluşturur ve veritabanına kaydeder.
        /// </summary>
        Task<TEntity> CreateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut kaydı günceller ve veritabanına kaydeder.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);
    }
}
