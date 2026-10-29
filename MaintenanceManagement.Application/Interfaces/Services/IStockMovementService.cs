using MaintenanceManagement.Application.DTOS.StockMovementDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IStockMovementService
    {
        #region Get Methods

        /// <summary>
        /// Bütün stok hareketlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre stok hareketi detayı.
        /// </summary>
        Task<GetByIdStockMovementDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambara ait son stok hareketlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetByWarehouseIdAsync(
                int warehouseId,
                int take = 100,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemeye ait son stok hareketlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetByMaterialIdAsync(
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve malzemeye ait hareketleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının gerçekleştirdiği hareketler.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetByPerformedByUserIdAsync(
                int userId,
                int take = 100,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// UTC tarih aralığındaki stok hareketleri.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Referans numarasına ait hareketleri getirir.
        /// Bir transferde iki hareket aynı referansı
        /// taşıyabilir.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetByReferenceNumberAsync(
                string referenceNumber,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı transfer grubundaki hareketleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetByTransferGroupIdAsync(
                Guid transferGroupId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Giriş veya çıkış türüne göre hareketler.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetByMovementTypeAsync(
                StockMovementTypeEnum movementType,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Transfer Methods

        /// <summary>
        /// Stok giriş veya çıkış hareketi oluşturur.
        /// Bakiye güncellemesi Repository tarafından
        /// atomik şekilde gerçekleştirilmelidir.
        /// </summary>
        Task<int> CreateAsync(
            CreateStockMovementDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İki ambar arasında transfer gerçekleştirir.
        /// Kaynak için Out, hedef için In hareketi
        /// oluşur ve aynı TransferGroupId kullanılır.
        /// </summary>
        Task<(int OutMovementId, int InMovementId)>
            TransferStockAsync(
                int fromWarehouseId,
                int toWarehouseId,
                int materialId,
                decimal quantity,
                int currentUserId,
                string? referenceNumber = null,
                string? description = null,
                CancellationToken cancellationToken = default);

        #endregion

        #region Update Methods

        /// <summary>
        /// Mevcut stok hareketinin yalnızca
        /// referans numarası ve açıklamasını günceller.
        /// Miktar ve hareket türü değiştirilemez.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateStockMovementDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Quantity Methods

        /// <summary>
        /// Ambar ve malzemeye ait toplam giriş miktarı.
        /// </summary>
        Task<decimal> GetTotalInQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve malzemeye ait toplam çıkış miktarı.
        /// </summary>
        Task<decimal> GetTotalOutQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Giriş toplamından çıkış toplamını çıkarır.
        /// Bu hareket geçmişi netidir;
        /// mevcut stok bakiyesi için WarehouseStock
        /// esas alınmalıdır.
        /// </summary>
        Task<decimal> GetNetMovementQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
