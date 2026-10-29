using MaintenanceManagement.Application.DTOS.RoleWorkSubTypePermissionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IRoleWorkSubTypePermissionService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün rol-iş alt türü-yetki
        /// atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre atamanın detayını getirir.
        /// </summary>
        Task<GetByIdRoleWorkSubTypePermissionDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role ait bütün atamaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role ait aktif atamaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetActiveByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş alt türüne ait atamaları getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon birimine bağlı
        /// rollerin iş alt türü yetkilerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Rol, iş alt türü ve yetki ID bilgilerine
        /// göre ilgili atamayı getirir.
        /// </summary>
        Task<GetByIdRoleWorkSubTypePermissionDto?>
            GetByRoleWorkSubTypeAndPermissionAsync(
                int roleId,
                int workSubTypeId,
                int permissionId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni rol-iş alt türü-yetki ataması oluşturur.
        /// Oluşturulan kaydın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateRoleWorkSubTypePermissionDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut atamanın bilgilerini günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateRoleWorkSubTypePermissionDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Atamanın aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı rol, iş alt türü ve yetki ilişkisi
        /// daha önce oluşturulmuş mu kontrol eder.
        /// </summary>
        Task<bool> ExistsAssignmentAsync(
            int roleId,
            int workSubTypeId,
            int permissionId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolün belirli iş alt türünde
        /// aktif yetkiye sahip olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActivePermissionAsync(
            int roleId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli organizasyon biriminde
        /// ilgili iş alt türü yetkisine sahip olup
        /// olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasUserActivePermissionAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
