using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IMaterialRepository
            : IGenericRepository<Material>
    {
        /// <summary>
        /// Bütün malzemeleri ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<Material>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemeyi ilişkili detaylarıyla getirir.
        /// </summary>
        Task<Material?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzeme koduna göre kayıt getirir.
        /// </summary>
        Task<Material?> GetByMaterialCodeAsync(
            string materialCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ada sahip malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetActiveMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Pasif malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetInactiveMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategoriye bağlı malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ölçü birimini kullanan
        /// malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetByMeasurementUnitIdAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyona bağlı malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli bir organizasyona bağlanmamış
        /// ortak malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetSharedMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun kendi malzemelerini ve
        /// ortak malzemeleri birlikte getirir.
        /// </summary>
        Task<List<Material>> GetAvailableForUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategorisi bulunmayan malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetWithoutCategoryAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzeme kodu, adı veya açıklaması
        /// içerisinde arama yapar.
        /// </summary>
        Task<List<Material>> SearchMaterialsAsync(
            string searchText,
            int? unitId = null,
            bool onlyActive = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin bütün ambarlardaki
        /// stok kayıtlarını getirir.
        /// </summary>
        Task<List<WarehouseStock>> GetWarehouseStocksByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin belirli ambardaki
        /// stok kaydını getirir.
        /// </summary>
        Task<WarehouseStock?> GetWarehouseStockAsync(
            int materialId,
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemeye ait son stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetStockMovementsByMaterialIdAsync(
            int materialId,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin iş emirlerindeki
        /// kullanım kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>>
            GetWorkOrderMaterialsByMaterialIdAsync(
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin aktif ve pasif bütün
        /// ambarlardaki toplam stok miktarını getirir.
        /// </summary>
        Task<decimal> GetTotalStockQuantityAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin yalnızca aktif ambarlardaki
        /// kullanılabilir stok miktarını getirir.
        /// </summary>
        Task<decimal> GetAvailableStockQuantityAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin belirli ambardaki
        /// stok miktarını getirir.
        /// </summary>
        Task<decimal> GetStockQuantityInWarehouseAsync(
            int materialId,
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanılabilir stok miktarı minimum
        /// seviyeye eşit veya altında olan
        /// aktif malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetCriticalStockMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon ve ortak malzemeler arasından
        /// kritik stok seviyesinde olanları getirir.
        /// </summary>
        Task<List<Material>> GetCriticalStockMaterialsByUnitIdAsync(
            int unitId,
            bool includeShared = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif ambarlarda kullanılabilir
        /// stoku kalmamış malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetOutOfStockMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kritik stok seviyesindeki aktif
        /// malzemelerin sayısını getirir.
        /// </summary>
        Task<int> GetCriticalStockCountAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzeme kodunun başka kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsMaterialCodeAsync(
            string materialCode,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemeye ait pozitif stok
        /// bakiyesi bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasStockAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin stok hareketi
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasStockMovementsAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin iş emirlerinde
        /// kullanım kaydı olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasWorkOrderUsageAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin ölçü birimini değiştirmenin
        /// geçmiş stok kayıtları bakımından
        /// uygun olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> CanChangeMeasurementUnitAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin minimum stok
        /// seviyesini günceller.
        /// </summary>
        Task<bool> UpdateMinimumStockLevelAsync(
            int id,
            decimal minimumStockLevel,
            CancellationToken cancellationToken = default);
    }
}
