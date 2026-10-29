using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.DTOS.MeasurementUnitDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IMeasurementUnitService
    {
        #region Get Methods

        /// <summary>
        /// Bütün ölçü birimlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre ölçü birimi detayını getirir.
        /// </summary>
        Task<GetByIdMeasurementUnitDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif ölçü birimlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Pasif ölçü birimlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetInactiveAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimi adına göre kayıt getirir.
        /// </summary>
        Task<GetByIdMeasurementUnitDto?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimi sembolüne göre kayıt getirir.
        /// </summary>
        Task<GetByIdMeasurementUnitDto?> GetBySymbolAsync(
            string symbol,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzemeye bağlı ölçü birimini getirir.
        /// </summary>
        Task<GetByIdMeasurementUnitDto?> GetByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ad veya sembole göre arama yapar.
        /// </summary>
        Task<IReadOnlyList<ResultMeasurementUnitDto>>
            SearchAsync(
                string searchText,
                bool onlyActive = false,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Malzeme bağlantısı bulunan
        /// ölçü birimlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetUnitsWithMaterialsAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Hiçbir malzemeyle ilişkisi bulunmayan
        /// ölçü birimlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMeasurementUnitDto>>
            GetUnitsWithoutMaterialsAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimini kullanan bütün
        /// malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetMaterialsByMeasurementUnitIdAsync(
                int measurementUnitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimini kullanan aktif
        /// malzemeleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultMaterialDto>>
            GetActiveMaterialsByMeasurementUnitIdAsync(
                int measurementUnitId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni ölçü birimi oluşturur.
        /// </summary>
        Task<int> CreateAsync(
            CreateMeasurementUnitDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü biriminin adını ve sembolünü günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateMeasurementUnitDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimini aktif veya pasif yapar.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Ölçü birimi adı kullanılıyor mu?
        /// excludedId güncelleme sırasında
        /// mevcut kaydı hariç tutar.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimi sembolü kullanılıyor mu?
        /// </summary>
        Task<bool> ExistsSymbolAsync(
            string symbol,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimine bağlı malzeme var mı?
        /// </summary>
        Task<bool> HasMaterialsAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimine bağlı aktif malzeme var mı?
        /// </summary>
        Task<bool> HasActiveMaterialsAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count Methods

        /// <summary>
        /// Bağlı toplam malzeme sayısını getirir.
        /// </summary>
        Task<int> GetMaterialCountAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bağlı aktif malzeme sayısını getirir.
        /// </summary>
        Task<int> GetActiveMaterialCountAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
