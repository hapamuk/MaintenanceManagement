using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IAssetModelRepository
         : IGenericRepository<AssetModel>
    {
        /// <summary>
        /// Aktif markalara bağlı aktif demirbaş modellerini getirir.
        /// </summary>
        Task<List<AssetModel>> GetActiveModelsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bütün demirbaş modellerini marka ve
        /// ilişkili demirbaş bilgileriyle getirir.
        /// </summary>
        Task<List<AssetModel>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen modeli marka ve demirbaş
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<AssetModel?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markaya ait aktif ve pasif
        /// bütün modelleri getirir.
        /// </summary>
        Task<List<AssetModel>> GetModelsByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markaya bağlı aktif modelleri getirir.
        /// </summary>
        Task<List<AssetModel>> GetActiveModelsByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Modelin bağlı olduğu marka ve model adına
        /// göre mevcut kaydı getirir.
        /// </summary>
        Task<AssetModel?> GetByBrandAndNameAsync(
            int brandId,
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı marka altında aynı model adının daha önce
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            int brandId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen modele bağlı bütün
        /// demirbaş kayıtlarını getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByModelIdAsync(
            int modelId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen modele bağlı herhangi bir
        /// demirbaş bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssetsAsync(
            int modelId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen modele bağlı aktif demirbaş
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssetsAsync(
            int modelId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Modelin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
