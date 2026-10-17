using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IRolePermissionRepository
        : IGenericRepository<RolePermission>
    {
        /// <summary>
        /// Rol-yetki eşleştirmelerini ilişkili
        /// rol ve yetki bilgileriyle getirir.
        /// </summary>
        Task<List<RolePermission>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen rol-yetki kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<RolePermission?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role tanımlanmış
        /// bütün yetki eşleştirmelerini getirir.
        /// </summary>
        Task<List<RolePermission>> GetByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role ait aktif yetki
        /// eşleştirmelerini getirir.
        /// </summary>
        Task<List<RolePermission>> GetActiveByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen yetkinin hangi rollere
        /// tanımlandığını getirir.
        /// </summary>
        Task<List<RolePermission>> GetByPermissionIdAsync(
            int permissionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rol ve yetki bilgilerine göre
        /// mevcut eşleştirme kaydını getirir.
        /// </summary>
        Task<RolePermission?> GetByRoleAndPermissionAsync(
            int roleId,
            int permissionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rol ile yetki arasında daha önce
        /// eşleştirme yapılıp yapılmadığını kontrol eder.
        /// Pasif kayıtlar da dikkate alınır.
        /// </summary>
        Task<bool> ExistsAssignmentAsync(
            int roleId,
            int permissionId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolün belirtilen genel sistem yetkisine
        /// aktif olarak sahip olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActivePermissionAsync(
            int roleId,
            string permissionCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rol-yetki eşleştirmesinin
        /// aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
