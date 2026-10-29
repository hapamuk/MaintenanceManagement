using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.DTOS.WarehouseStockDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IMaterialService
    {
        #region Get Methods

        /// <summary>
        /// Bütün malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre malzeme detayını getirir.
        /// </summary>
        Task<GetByIdMaterialDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzeme koduna göre kayıt getirir.
        /// </summary>
        Task<GetByIdMaterialDto?> GetByMaterialCodeAsync(
            string materialCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Pasif malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>> GetInactiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzeme kategorisine göre listeleme.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>> GetByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimine göre listeleme.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetByMeasurementUnitIdAsync(
                int measurementUnitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimine göre listeleme.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori ve organizasyon birimine
        /// göre malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetByCategoryAndUnitIdAsync(
                int categoryId,
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimi ve kategoriye göre listeleme.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetByMeasurementUnitAndCategoryIdAsync(
                int measurementUnitId,
                int categoryId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kategori atanmamış malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetWithoutCategoryAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon birimi atanmamış
        /// malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetWithoutUnitAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzeme kodu veya adına göre arama yapar.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>> SearchAsync(
            string searchText,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);

        #endregion

        #region Warehouse Stock Methods

        /// <summary>
        /// Malzemenin ambarlardaki stok kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetWarehouseStocksByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni malzeme oluşturur.
        /// </summary>
        Task<int> CreateAsync(
            CreateMaterialDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin düzenlenebilir
        /// bilgilerini günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateMaterialDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemeyi aktif veya pasif yapar.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemenin kritik stok seviyesini günceller.
        /// </summary>
        Task<bool> UpdateMinimumStockLevelAsync(
            int id,
            decimal minimumStockLevel,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Malzeme kodu kullanılıyor mu?
        /// </summary>
        Task<bool> ExistsMaterialCodeAsync(
            string materialCode,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimi değiştirilebilir mi?
        /// </summary>
        Task<bool> CanChangeMeasurementUnitAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
