using MaintenanceManagement.Application.DTOS.WarehouseStockDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWarehouseStockService
    {
        #region Get Methods

        /// <summary>
        /// Bütün ambar stok kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre detay getirir.
        /// </summary>
        Task<GetByIdWarehouseStockDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve malzemeye göre stok kaydı getirir.
        /// </summary>
        Task<GetByIdWarehouseStockDto?>
            GetByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki stok kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin ambarlardaki stoklarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki pozitif stok kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetPositiveStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki sıfır bakiyeli kayıtları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetZeroStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Minimum stok seviyesinde veya altında
        /// bulunan stok kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetCriticalStocksAsync(
                int? warehouseId = null,
                CancellationToken cancellationToken = default);

        #endregion

        #region Quantity Methods

        /// <summary>
        /// Ambardaki malzemenin stok miktarı.
        /// </summary>
        Task<decimal> GetStockQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin kullanılabilir toplam
        /// stok miktarını Repository'den getirir.
        /// </summary>
        Task<decimal> GetAvailableStockQuantityByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki stok miktarlarının toplamı.
        /// Farklı ölçü birimleri içerebileceğinden
        /// yalnızca sayısal raporlama amaçlıdır.
        /// </summary>
        Task<decimal> GetTotalQuantityByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İstenen miktarda stok mevcut mu?
        /// </summary>
        Task<bool> HasSufficientStockAsync(
            int warehouseId,
            int materialId,
            decimal requiredQuantity,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create Methods

        /// <summary>
        /// Ambar ve malzeme için sıfır bakiyeli
        /// başlangıç stok kaydı oluşturur.
        ///
        /// İlk stok miktarı StockMovement
        /// üzerinden girilmelidir.
        /// </summary>
        Task<int> CreateAsync(
            CreateWarehouseStockDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Ambar ve malzeme için stok
        /// kaydı mevcut mu?
        /// </summary>
        Task<bool> ExistsAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        Task<int> GetStockRecordCountByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        Task<int> GetStockRecordCountByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
