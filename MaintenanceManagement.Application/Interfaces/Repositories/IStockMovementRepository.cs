using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IStockMovementRepository
        : IGenericRepository<StockMovement>
    {
        /// <summary>
        /// Bütün stok hareketlerini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<StockMovement>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen stok hareketini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<StockMovement?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambara ait son stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetByWarehouseIdAsync(
            int warehouseId,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemeye ait son stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetByMaterialIdAsync(
            int materialId,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli ambar ve malzemeye ait
        /// stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>>
            GetByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcıya
        /// ait stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>>
            GetByPerformedByUserIdAsync(
                int userId,
                int take = 100,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// In veya Out hareket türüne göre
        /// stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetByMovementTypeAsync(
            StockMovementTypeEnum movementType,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki In veya Out türündeki
        /// stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetByWarehouseAndTypeAsync(
            int warehouseId,
            StockMovementTypeEnum movementType,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığındaki
        /// stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Referans numarasına bağlı
        /// bütün stok hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetByReferenceNumberAsync(
            string referenceNumber,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı transfer grubuna bağlı
        /// giriş ve çıkış hareketlerini getirir.
        /// </summary>
        Task<List<StockMovement>> GetByTransferGroupIdAsync(
            Guid transferGroupId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambara ait transfer hareketlerini getirir.
        ///
        /// TransferGroupId dolu olan In ve Out
        /// hareketleri transfer olarak kabul edilir.
        /// </summary>
        Task<List<StockMovement>> GetTransfersByWarehouseIdAsync(
            int warehouseId,
            int take = 100,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin malzeme kullanımına
        /// bağlanan stok çıkışlarını getirir.
        /// </summary>
        Task<List<StockMovement>> GetByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli ambar ve malzemenin
        /// en son stok hareketini getirir.
        /// </summary>
        Task<StockMovement?> GetLatestMovementAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve malzemeye ait hareket
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasMovementsAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki toplam stok
        /// hareketi sayısını getirir.
        /// </summary>
        Task<int> GetMovementCountByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki malzemenin toplam
        /// giriş miktarını hesaplar.
        ///
        /// Transfer girişleri de dahildir.
        /// </summary>
        Task<decimal> GetTotalIncomingQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki malzemenin toplam
        /// çıkış miktarını hesaplar.
        ///
        /// Transfer çıkışları da dahildir.
        /// </summary>
        Task<decimal> GetTotalOutgoingQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Toplam giriş miktarından toplam
        /// çıkış miktarını çıkarır.
        /// </summary>
        Task<decimal> GetNetQuantityChangeAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Normal stok girişi veya çıkışını
        /// WarehouseStock ile birlikte kaydeder.
        ///
        /// Transfer işlemleri için
        /// TransferStockAsync kullanılmalıdır.
        /// </summary>
        Task<StockMovement> RecordMovementAsync(
            int warehouseId,
            int materialId,
            int performedByUserId,
            StockMovementTypeEnum movementType,
            decimal quantity,
            DateTime movementDateUtc,
            string? referenceNumber = null,
            string? description = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kaynak ambardan hedef ambara
        /// stok transferini gerçekleştirir.
        ///
        /// TransferOut yerine Out,
        /// TransferIn yerine In kullanılır.
        ///
        /// Her iki hareket aynı TransferGroupId
        /// değeriyle ilişkilendirilir.
        /// </summary>
        Task<(StockMovement OutMovement, StockMovement InMovement)>
            TransferStockAsync(
                int fromWarehouseId,
                int toWarehouseId,
                int materialId,
                int performedByUserId,
                decimal quantity,
                DateTime movementDateUtc,
                string? referenceNumber = null,
                string? description = null,
                CancellationToken cancellationToken = default);
    }
}
