using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IAssetRepository : IGenericRepository<Asset>
    {
        /// <summary>
        /// Aktif durumdaki demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetActiveAssetsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bütün demirbaşları marka, model, kategori,
        /// tür ve organizasyon bilgileriyle getirir.
        /// </summary>
        Task<List<Asset>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşı ID bilgisine göre ilişkili
        /// detayları ve geçmiş kayıtlarıyla getirir.
        /// </summary>
        Task<Asset?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş koduna göre kayıt getirir.
        /// </summary>
        Task<Asset?> GetByAssetCodeAsync(
            string assetCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyona ait demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen markaya ait demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByBrandIdAsync(
            int brandId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen modele ait demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByModelIdAsync(
            int modelId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kategoriye ait demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaş türüne ait kayıtları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByKindIdAsync(
            int kindId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen durumdaki demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByStatusAsync(
            AssetStatusEnum status,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcıya halen atanmış
        /// demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen gün içerisinde garantisi
        /// sona erecek demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetAssetsWithExpiringWarrantyAsync(
            int days,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Garanti süresi dolmuş demirbaşları getirir.
        /// </summary>
        Task<List<Asset>> GetExpiredWarrantyAssetsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş kodunun başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsAssetCodeAsync(
            string assetCode,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Seri numarasının başka bir demirbaşta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsSerialNumberAsync(
            string serialNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Envanter numarasının başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsInventoryNumberAsync(
            string inventoryNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa bağlı açık iş emri bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşın halen bir kullanıcıya
        /// atanmış olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssignmentsAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa bağlı aktif periyodik bakım
        /// planı bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveMaintenancePlansAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşın durumunu AssetStatusEnum
        /// değerlerinden biriyle değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            AssetStatusEnum status,
            CancellationToken cancellationToken = default);
    }
}
