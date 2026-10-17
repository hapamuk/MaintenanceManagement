using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        /// <summary>
        /// Aktif markaları alfabetik sırayla getirir.
        /// </summary>
        Task<List<Brand>> GetActiveBrandsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bütün markaları ilişkili modelleriyle getirir.
        /// </summary>
        Task<List<Brand>> GetAllWithModelsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markayı ilişkili modelleriyle getirir.
        /// </summary>
        Task<Brand?> GetByIdWithModelsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markaya ait modelleri getirir.
        /// Aktif ve pasif modeller dahildir.
        /// </summary>
        Task<List<AssetModel>> GetModelsByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markaya ait aktif modelleri getirir.
        /// </summary>
        Task<List<AssetModel>> GetActiveModelsByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Marka adının daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markaya bağlı herhangi bir model bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasModelsAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markaya bağlı aktif model bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveModelsAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markanın modellerine bağlı kayıtlı
        /// demirbaş olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssetsAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Markanın aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
