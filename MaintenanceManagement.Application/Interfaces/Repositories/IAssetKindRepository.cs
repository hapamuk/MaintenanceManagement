using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IAssetKindRepository
           : IGenericRepository<AssetKind>
    {
        /// <summary>
        /// Aktif kategorilere bağlı aktif
        /// demirbaş türlerini getirir.
        /// </summary>
        Task<List<AssetKind>> GetActiveKindsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bütün demirbaş türlerini kategori ve
        /// demirbaş bilgileriyle birlikte getirir.
        /// </summary>
        Task<List<AssetKind>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaş türünü ilişkili
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<AssetKind?> GetByIdWithDetailsAsync(
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
        /// Belirtilen kategoriye bağlı
        /// aktif demirbaş türlerini getirir.
        /// </summary>
        Task<List<AssetKind>> GetActiveKindsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori ve tür adına göre
        /// mevcut demirbaş türünü getirir.
        /// </summary>
        Task<AssetKind?> GetByCategoryAndNameAsync(
            int categoryId,
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı kategori içerisinde demirbaş
        /// türü adının kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            int categoryId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaş türüne bağlı
        /// bütün demirbaş kayıtlarını getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByKindIdAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaş türüne bağlı
        /// aktif demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetActiveAssetsByKindIdAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş türüne bağlı herhangi
        /// bir demirbaş olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssetsAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş türüne bağlı aktif
        /// demirbaş bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssetsAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş türünün aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
