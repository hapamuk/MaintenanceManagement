using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IUnitWorkTypeRepository
       : IGenericRepository<UnitWorkType>
    {
        /// <summary>
        /// Bütün organizasyon-iş türü eşleştirmelerini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<UnitWorkType>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Eşleştirme kaydını organizasyon ve iş türü
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<UnitWorkType?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyona tanımlanmış aktif ve pasif
        /// bütün iş türü eşleştirmelerini getirir.
        /// </summary>
        Task<List<UnitWorkType>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonda kullanılabilir durumdaki
        /// aktif iş türü eşleştirmelerini getirir.
        /// </summary>
        Task<List<UnitWorkType>> GetActiveByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türünün tanımlandığı bütün
        /// organizasyon eşleştirmelerini getirir.
        /// </summary>
        Task<List<UnitWorkType>> GetByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türünün tanımlandığı aktif
        /// organizasyon eşleştirmelerini getirir.
        /// </summary>
        Task<List<UnitWorkType>> GetActiveByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon ve iş türüne ait mevcut
        /// eşleştirme kaydını getirir.
        /// Pasif kayıtlar da dahildir.
        /// </summary>
        Task<UnitWorkType?> GetByUnitAndWorkTypeAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun kullanabildiği
        /// aktif iş türlerini getirir.
        /// </summary>
        Task<List<WorkType>> GetActiveWorkTypesByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonda kullanılabilen
        /// aktif iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetAvailableWorkSubTypesByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı organizasyon ve iş türü eşleştirmesinin
        /// mevcut olup olmadığını kontrol eder.
        /// Pasif kayıtlar da dikkate alınır.
        /// </summary>
        Task<bool> ExistsAssignmentAsync(
            int unitId,
            int workTypeId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş türünün belirtilen organizasyonda
        /// kullanılabilir olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> IsWorkTypeAvailableForUnitAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun belirtilen iş türünde
        /// "Yeni" (Draft) durumunda iş talebi
        /// bulunup bulunmadığını kontrol eder.
        ///
        /// Rejected, ConvertedToWorkOrder ve Cancelled
        /// durumları bu sorguya dahil edilmez.
        /// </summary>
        Task<bool> HasPendingWorkRequestsAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun belirtilen iş türünde
        /// açık iş emri bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyon-iş türü eşleştirmesinin
        /// aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
