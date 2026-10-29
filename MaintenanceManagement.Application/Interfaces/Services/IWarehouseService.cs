using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.DTOS.StockMovementDtos;
using MaintenanceManagement.Application.DTOS.WarehouseDtos;
using MaintenanceManagement.Application.DTOS.WarehouseStockDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWarehouseService
    {
        #region Get Methods

        /// <summary>
        /// Bütün ambarları ilişkileriyle getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre ambar detayını getirir.
        /// </summary>
        Task<GetByIdWarehouseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar koduna göre detay getirir.
        /// </summary>
        Task<GetByIdWarehouseDto?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar adına göre detay getirir.
        /// </summary>
        Task<GetByIdWarehouseDto?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif ambarları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Pasif ambarları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseDto>> GetInactiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimine doğrudan
        /// bağlı ambarları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseDto>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon biriminin aktif ambarları.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseDto>>
            GetActiveByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Birime bağlı olmayan ortak ambarları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseDto>>
            GetSharedWarehousesAsync(
                bool onlyActive = true,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli birim tarafından kullanılabilecek
        /// ambarları getirir.
        ///
        /// Birime özel ambarlar ve ortak ambarlar.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseDto>>
            GetAvailableForUnitAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar kodu, adı veya diğer arama
        /// alanlarına göre filtreleme yapar.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseDto>> SearchAsync(
            string searchText,
            int? unitId = null,
            bool onlyActive = true,
            CancellationToken cancellationToken = default);

        #endregion

        #region Warehouse Stock Methods

        /// <summary>
        /// Ambardaki bütün stok kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Miktarı sıfırdan büyük stokları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetPositiveStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambar ve malzemeye göre stok kaydı getirir.
        /// </summary>
        Task<ResultWarehouseStockDto?>
            GetStockByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda pozitif stoğu bulunan malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetMaterialsInStockAsync(
                int warehouseId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki belirli malzemenin miktarını getirir.
        /// </summary>
        Task<decimal> GetStockQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambardaki toplam stok kayıt sayısını getirir.
        /// </summary>
        Task<int> GetStockRecordCountAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda pozitif stoğu bulunan
        /// farklı malzeme sayısını getirir.
        /// </summary>
        Task<int> GetMaterialInStockCountAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Stock Movement Methods

        /// <summary>
        /// Ambarın son stok hareketlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultStockMovementDto>>
            GetStockMovementsByWarehouseIdAsync(
                int warehouseId,
                int take = 100,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni ambar oluşturur.
        /// </summary>
        Task<int> CreateAsync(
            CreateWarehouseDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarın düzenlenebilir bilgilerini günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWarehouseDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarı aktif veya pasif yapar.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Ambar kodu daha önce kullanılmış mı?
        /// </summary>
        Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ambarda pozitif stok bakiyesi bulunuyor mu?
        /// </summary>
        Task<bool> HasPositiveStockAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
