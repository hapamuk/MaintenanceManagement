using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IAssetCategoryRepository
    : IGenericRepository<AssetCategory>
    {
        /// <summary>
        /// Aktif demirbaş kategorilerini getirir.
        /// </summary>
        Task<List<AssetCategory>> GetActiveCategoriesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bütün kategorileri ilişkili demirbaş
        /// türleriyle birlikte getirir.
        /// </summary>
        Task<List<AssetCategory>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoriyi demirbaş türleriyle
        /// birlikte getirir.
        /// </summary>
        Task<AssetCategory?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoriye bağlı aktif ve
        /// pasif bütün demirbaş türlerini getirir.
        /// </summary>
        Task<List<AssetKind>> GetKindsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoriye bağlı aktif
        /// demirbaş türlerini getirir.
        /// </summary>
        Task<List<AssetKind>> GetActiveKindsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori adının daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı herhangi bir demirbaş
        /// türü bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasKindsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif demirbaş
        /// türü bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveKindsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı herhangi bir
        /// demirbaş bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssetsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı aktif demirbaş
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssetsAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategorinin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
