using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWarehouseStockRepository
      : IGenericRepository<WarehouseStock>
    {
        /// <summary>
        /// Bütün stok kayıtlarını ambar, malzeme,
        /// ölçü birimi ve kategori detaylarıyla getirir.
        /// </summary>
        Task<List<WarehouseStock>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen stok kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<WarehouseStock?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve malzeme bilgilerine göre
        /// stok kaydını getirir.
        /// </summary>
        Task<WarehouseStock?> GetByWarehouseAndMaterialIdAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ambardaki bütün
        /// stok kayıtlarını getirir.
        /// </summary>
        Task<List<WarehouseStock>> GetByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin bütün ambarlardaki
        /// stok kayıtlarını getirir.
        /// </summary>
        Task<List<WarehouseStock>> GetByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda pozitif miktara sahip
        /// stok kayıtlarını getirir.
        /// </summary>
        Task<List<WarehouseStock>> GetPositiveStocksByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin pozitif stok
        /// bakiyesi bulunan ambarlarını getirir.
        /// </summary>
        Task<List<WarehouseStock>> GetPositiveStocksByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin aktif ambarlardaki
        /// stok kayıtlarını getirir.
        /// </summary>
        Task<List<WarehouseStock>>
            GetActiveWarehouseStocksByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun kendi ambarları ile
        /// ortak aktif ambarlarındaki stokları getirir.
        /// </summary>
        Task<List<WarehouseStock>> GetAvailableStocksByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki belirli malzemenin
        /// stok miktarını getirir.
        /// </summary>
        Task<decimal> GetStockQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin bütün ambarlardaki
        /// toplam stok miktarını getirir.
        /// </summary>
        Task<decimal> GetTotalStockQuantityByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin aktif ambarlardaki
        /// kullanılabilir toplam stok miktarını getirir.
        /// </summary>
        Task<decimal> GetAvailableStockQuantityByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki toplam stok
        /// satırı sayısını getirir.
        /// </summary>
        Task<int> GetStockRecordCountByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki pozitif stok miktarına
        /// sahip farklı malzeme sayısını getirir.
        /// </summary>
        Task<int> GetPositiveMaterialCountByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanılabilir toplam stoğu minimum
        /// seviyesinde veya altında bulunan
        /// aktif malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetCriticalStockMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif ambarlarda kullanılabilir
        /// stoku kalmayan aktif malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetOutOfStockMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı ambarda aynı malzemenin
        /// stok kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsStockRecordAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda malzemenin pozitif stok
        /// bakiyesi olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasPositiveStockAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İstenen miktarda stok çıkışı
        /// yapılabilecek yeterli bakiye var mı?
        /// </summary>
        Task<bool> HasSufficientStockAsync(
            int warehouseId,
            int materialId,
            decimal requiredQuantity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda herhangi bir malzemenin
        /// pozitif stok bakiyesi var mı?
        /// </summary>
        Task<bool> HasAnyPositiveStockByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin bütün ambarlardaki
        /// stok hareketi geçmişini getirir.
        /// </summary>
        Task<List<StockMovement>> GetMovementsByMaterialIdAsync(
            int materialId,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ambar ve malzemenin
        /// stok hareketi geçmişini getirir.
        /// </summary>
        Task<List<StockMovement>>
            GetMovementsByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve malzemenin en son
        /// stok hareketini getirir.
        /// </summary>
        Task<StockMovement?> GetLatestMovementAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif ambar ve malzeme için başlangıç
        /// miktarı sıfır olan stok kartı oluşturur.
        ///
        /// Aynı ambar ve malzeme için ikinci
        /// stok kartı oluşturulamaz.
        /// </summary>
        Task<WarehouseStock> CreateStockRecordAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki mevcut stok miktarını artırır.
        ///
        /// Stok hareketi ile birlikte bir transaction
        /// kapsamında çağrılmalıdır.
        /// </summary>
        Task<WarehouseStock> IncreaseStockAsync(
            int warehouseId,
            int materialId,
            decimal quantity,
            DateTime movementDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki stok miktarını azaltır.
        ///
        /// Yetersiz stok durumunda işlem yapılmaz.
        /// Stok hareketi ile birlikte bir transaction
        /// kapsamında çağrılmalıdır.
        /// </summary>
        Task<WarehouseStock> DecreaseStockAsync(
            int warehouseId,
            int materialId,
            decimal quantity,
            DateTime movementDateUtc,
            CancellationToken cancellationToken = default);
    }
}
