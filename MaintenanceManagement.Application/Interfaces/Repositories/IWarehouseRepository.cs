using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWarehouseRepository
         : IGenericRepository<Warehouse>
    {
        /// <summary>
        /// Bütün ambarları ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<Warehouse>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ambarı ilişkili detaylarıyla getirir.
        /// </summary>
        Task<Warehouse?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar koduna göre kayıt getirir.
        /// </summary>
        Task<Warehouse?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ada sahip ambarları getirir.
        /// </summary>
        Task<List<Warehouse>> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif ambarları listeler.
        /// </summary>
        Task<List<Warehouse>> GetActiveWarehousesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Pasif ambarları listeler.
        /// </summary>
        Task<List<Warehouse>> GetInactiveWarehousesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyona bağlı ambarları getirir.
        /// Ortak ambarlar dahil değildir.
        /// </summary>
        Task<List<Warehouse>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyona bağlı aktif ambarları getirir.
        /// </summary>
        Task<List<Warehouse>> GetActiveByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyondan bağımsız ortak
        /// ambarları getirir.
        /// </summary>
        Task<List<Warehouse>> GetSharedWarehousesAsync(
            bool onlyActive = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun kendi aktif ambarlarıyla
        /// ortak aktif ambarları getirir.
        ///
        /// Kullanıcının işlem yapma yetkisi
        /// ayrıca doğrulanmalıdır.
        /// </summary>
        Task<List<Warehouse>> GetAvailableForUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar kodu, adı, konumu veya açıklamasında
        /// belirtilen ifadeyi arar.
        /// </summary>
        Task<List<Warehouse>> SearchWarehousesAsync(
            string searchText,
            int? unitId = null,
            bool onlyActive = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambara ait bütün stok kayıtlarını
        /// ilişkili malzeme detaylarıyla getirir.
        /// </summary>
        Task<List<WarehouseStock>> GetStocksByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda pozitif stok bakiyesi
        /// bulunan kayıtları getirir.
        /// </summary>
        Task<List<WarehouseStock>>
            GetPositiveStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve malzemeye ait stok
        /// kaydını getirir.
        /// </summary>
        Task<WarehouseStock?> GetStockByWarehouseAndMaterialIdAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda pozitif stok bakiyesi
        /// bulunan aktif malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetMaterialsInStockAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki belirli malzemenin
        /// mevcut stok miktarını getirir.
        /// </summary>
        Task<decimal> GetStockQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki toplam stok
        /// kayıt sayısını getirir.
        /// </summary>
        Task<int> GetStockRecordCountAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda pozitif stok bakiyesi
        /// bulunan farklı malzeme sayısını getirir.
        /// </summary>
        Task<int> GetMaterialInStockCountAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambara ait son stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetStockMovementsByWarehouseIdAsync(
            int warehouseId,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki stok hareketlerini
        /// In veya Out türüne göre getirir.
        ///
        /// Transfer hareketleri de ilgili
        /// In/Out türünün içerisinde bulunur.
        /// </summary>
        Task<List<StockMovement>> GetMovementsByTypeAsync(
            int warehouseId,
            StockMovementTypeEnum movementType,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambara ait transfer giriş ve
        /// çıkış hareketlerini getirir.
        ///
        /// TransferGroupId dolu kayıtlar
        /// transfer olarak değerlendirilir.
        /// </summary>
        Task<List<StockMovement>> GetTransferMovementsAsync(
            int warehouseId,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki belirli malzemenin
        /// stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetMovementsByMaterialIdAsync(
            int warehouseId,
            int materialId,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki belirtilen UTC tarih aralığında
        /// gerçekleşmiş stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetMovementsByDateRangeAsync(
            int warehouseId,
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar kodunun başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda herhangi bir pozitif
        /// stok bakiyesi olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasPositiveStockAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambara ait geçmiş stok hareketi
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasStockMovementsAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarın açık iş emirlerinde malzeme
        /// kullanım kaydı olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrderUsageAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarın aktiflik durumunu değiştirir.
        ///
        /// Pozitif stok veya açık iş emri
        /// malzeme bağlantısı varsa
        /// pasife alma işlemini engeller.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
