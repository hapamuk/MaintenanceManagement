using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkTypeRepository
          : IGenericRepository<WorkType>
    {
        /// <summary>
        /// Aktif iş türlerini getirir.
        /// </summary>
        Task<List<WorkType>> GetActiveWorkTypesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türlerini alt türleri ve organizasyon
        /// eşleştirmeleriyle birlikte getirir.
        /// </summary>
        Task<List<WorkType>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş türünü ilişkili
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<WorkType?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyona tanımlanmış
        /// aktif ve pasif bütün iş türlerini getirir.
        /// </summary>
        Task<List<WorkType>> GetWorkTypesByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyonda kullanılabilen
        /// aktif iş türlerini getirir.
        /// </summary>
        Task<List<WorkType>> GetActiveWorkTypesByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı bütün iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetWorkSubTypesByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı aktif iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetActiveWorkSubTypesByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun kullanabildiği iş türüne bağlı
        /// aktif iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>>
            GetAvailableWorkSubTypesByUnitAndTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türü adının daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı alt tür bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasWorkSubTypesAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı aktif alt tür bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveWorkSubTypesAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türünün herhangi bir organizasyona
        /// tanımlanıp tanımlanmadığını kontrol eder.
        /// </summary>
        Task<bool> HasUnitAssignmentsAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı iş taleplerinin
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasWorkRequestsAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı açık iş emirlerinin
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türüne bağlı aktif bakım planlarının
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveMaintenancePlansAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türünün aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
