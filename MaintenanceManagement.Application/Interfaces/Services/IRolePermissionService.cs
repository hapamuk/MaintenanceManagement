using MaintenanceManagement.Application.DTOS.RolePermissionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IRolePermissionService
    {
        #region Get Methods

        /// <summary>
        /// Bütün rol-yetki ilişkilerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRolePermissionDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre rol-yetki detayını getirir.
        /// </summary>
        Task<GetByIdRolePermissionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role ait bütün yetki atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRolePermissionDto>> GetByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role ait aktif yetki atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRolePermissionDto>> GetActiveByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen yetkinin atandığı rolleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRolePermissionDto>> GetByPermissionIdAsync(
            int permissionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rol ve yetki ikilisine ait ilişkiyi getirir.
        /// </summary>
        Task<GetByIdRolePermissionDto?> GetByRoleAndPermissionAsync(
            int roleId,
            int permissionId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Role yeni yetki ataması oluşturur.
        /// Oluşturulan ilişkinin ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateRolePermissionDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut rol-yetki ilişkisini günceller.
        /// Kayıt yoksa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateRolePermissionDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Rol-yetki ilişkisinin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Rol ve yetki ilişkisinin önceden
        /// oluşturulup oluşturulmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsAssignmentAsync(
            int roleId,
            int permissionId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolün verilen yetki koduna ait
        /// aktif ve kullanılabilir yetkisi olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActivePermissionAsync(
            int roleId,
            string permissionCode,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
