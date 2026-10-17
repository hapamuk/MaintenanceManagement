using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces
{
    public interface IRoleWorkSubTypePermissionRepository
       : IGenericRepository<RoleWorkSubTypePermission>
    {
        /// <summary>
        /// Tüm rol-iş alt türü-yetki eşleştirmelerini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<RoleWorkSubTypePermission>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre eşleştirmeyi
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<RoleWorkSubTypePermission?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role ait bütün iş alt türü
        /// yetkilendirmelerini getirir.
        /// </summary>
        Task<List<RoleWorkSubTypePermission>> GetByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role ait aktif iş alt türü
        /// yetkilendirmelerini getirir.
        /// </summary>
        Task<List<RoleWorkSubTypePermission>> GetActiveByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş alt türüne tanımlanmış
        /// rol ve yetki eşleştirmelerini getirir.
        /// </summary>
        Task<List<RoleWorkSubTypePermission>> GetByWorkSubTypeIdAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyona bağlı rollere ait
        /// iş alt türü yetkilendirmelerini getirir.
        /// </summary>
        Task<List<RoleWorkSubTypePermission>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rol, iş alt türü ve yetki bilgilerine göre
        /// eşleştirme kaydını getirir.
        /// </summary>
        Task<RoleWorkSubTypePermission?>
            GetByRoleWorkSubTypeAndPermissionAsync(
                int roleId,
                int workSubTypeId,
                int permissionId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı rol, iş alt türü ve yetki eşleştirmesinin
        /// daha önce oluşturulup oluşturulmadığını kontrol eder.
        /// Pasif kayıtlar da dikkate alınır.
        /// </summary>
        Task<bool> ExistsAssignmentAsync(
            int roleId,
            int workSubTypeId,
            int permissionId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolün ilgili iş alt türünde belirtilen
        /// aktif yetkiye sahip olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActivePermissionAsync(
            int roleId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının ilgili organizasyonda ve iş alt türünde
        /// belirtilen yetkiye sahip olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasUserActivePermissionAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rol-iş alt türü-yetki eşleştirmesinin
        /// aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
