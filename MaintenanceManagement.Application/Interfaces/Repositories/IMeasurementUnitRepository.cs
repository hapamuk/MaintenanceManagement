using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IMeasurementUnitRepository
          : IGenericRepository<MeasurementUnit>
    {
        /// <summary>
        /// Bütün ölçü birimlerini ilişkili
        /// malzeme detaylarıyla getirir.
        /// </summary>
        Task<List<MeasurementUnit>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ölçü birimini ilişkili
        /// malzemeleriyle birlikte getirir.
        /// </summary>
        Task<MeasurementUnit?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif ölçü birimlerini getirir.
        /// </summary>
        Task<List<MeasurementUnit>> GetActiveMeasurementUnitsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Pasif ölçü birimlerini getirir.
        /// </summary>
        Task<List<MeasurementUnit>> GetInactiveMeasurementUnitsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü biriminin adına göre
        /// mevcut kaydı getirir.
        /// </summary>
        Task<MeasurementUnit?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü biriminin sembolüne göre
        /// mevcut kaydı getirir.
        /// </summary>
        Task<MeasurementUnit?> GetBySymbolAsync(
            string symbol,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen malzemenin
        /// ölçü birimini getirir.
        /// </summary>
        Task<MeasurementUnit?> GetByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ad veya sembol içerisinde geçen
        /// ifadeye göre ölçü birimlerini arar.
        /// </summary>
        Task<List<MeasurementUnit>> SearchByNameOrSymbolAsync(
            string searchText,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// En az bir malzemeyle ilişkili
        /// ölçü birimlerini getirir.
        /// </summary>
        Task<List<MeasurementUnit>> GetUnitsWithMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Hiçbir malzemeyle ilişkisi
        /// bulunmayan ölçü birimlerini getirir.
        /// </summary>
        Task<List<MeasurementUnit>> GetUnitsWithoutMaterialsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ölçü birimini kullanan
        /// bütün malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetMaterialsByMeasurementUnitIdAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ölçü birimini kullanan
        /// aktif malzemeleri getirir.
        /// </summary>
        Task<List<Material>> GetActiveMaterialsByMeasurementUnitIdAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı isimde başka ölçü birimi
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı sembole sahip başka ölçü birimi
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsSymbolAsync(
            string symbol,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimini kullanan herhangi
        /// bir malzeme olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasMaterialsAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimini kullanan aktif
        /// malzeme olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveMaterialsAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimine bağlı toplam
        /// malzeme sayısını getirir.
        /// </summary>
        Task<int> GetMaterialCountAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü birimini kullanan aktif
        /// malzemelerin sayısını getirir.
        /// </summary>
        Task<int> GetActiveMaterialCountAsync(
            int measurementUnitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ölçü biriminin aktiflik durumunu
        /// değiştirir.
        ///
        /// Aktif malzemeler tarafından kullanılan
        /// bir ölçü birimi pasife alınamaz.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
