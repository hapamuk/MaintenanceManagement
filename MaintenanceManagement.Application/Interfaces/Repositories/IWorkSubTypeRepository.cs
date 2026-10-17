using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkSubTypeRepository
       : IGenericRepository<WorkSubType>
    {
        /// <summary>
        /// Aktif iş türlerine bağlı aktif
        /// iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetActiveWorkSubTypesAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Bütün iş alt türlerini iş türü ve
        /// rol yetki detaylarıyla getirir.
        /// </summary>
        Task<List<WorkSubType>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türünü ilişkili bütün
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<WorkSubType?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş türüne bağlı aktif
        /// ve pasif bütün alt türleri getirir.
        /// </summary>
        Task<List<WorkSubType>> GetByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş türüne bağlı aktif
        /// iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetActiveByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun kullanabileceği
        /// aktif iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetAvailableByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyonun belirtilen iş türündeki
        /// kullanılabilir iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetAvailableByUnitAndWorkTypeAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyonda, rolün
        /// verilen izin koduyla yetkili olduğu
        /// iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetAuthorizedByRoleAsync(
            int roleId,
            int unitId,
            string permissionCode = "WORK_ORDER_EXECUTE",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının organizasyondaki aktif
        /// rolleri üzerinden yetkili olduğu
        /// iş alt türlerini getirir.
        /// </summary>
        Task<List<WorkSubType>> GetAuthorizedByUserAsync(
            int userId,
            int unitId,
            string permissionCode = "WORK_ORDER_EXECUTE",
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türünün organizasyonda
        /// kullanılabilir olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> IsAvailableForUnitAsync(
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirtilen organizasyonda
        /// bu iş alt türünü uygulama yetkisini kontrol eder.
        /// </summary>
        Task<bool> CanUserExecuteAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı iş türü altında aynı alt tür adının
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            int workTypeId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne bağlı herhangi bir
        /// iş talebi olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasWorkRequestsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türünde Draft (Yeni) durumunda
        /// iş talebi bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasNewWorkRequestsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne bağlı açık iş emri
        /// olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne bağlı aktif planlı
        /// bakım bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveMaintenancePlansAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türüne tanımlanmış aktif rol
        /// yetkisi bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveRolePermissionsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş alt türünün aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
