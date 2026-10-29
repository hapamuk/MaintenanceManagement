using MaintenanceManagement.Application.DTOS.UserUnitRoleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IUserUnitRoleService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün kullanıcı-rol
        /// atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserUnitRoleDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID değerine göre atama detaylarını getirir.
        /// </summary>
        Task<GetByIdUserUnitRoleDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcıya ait bütün rol atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserUnitRoleDto>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcıya ait aktif rol atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserUnitRoleDto>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role ait kullanıcı atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserUnitRoleDto>> GetByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon birimindeki
        /// rollere ait kullanıcı atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserUnitRoleDto>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli organizasyon
        /// birimindeki aktif rol atamalarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultUserUnitRoleDto>>
            GetActiveByUserAndUnitAsync(
                int userId,
                int unitId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı ve rol ID değerlerine göre
        /// mevcut atamayı getirir.
        /// </summary>
        Task<GetByIdUserUnitRoleDto?> GetByUserAndRoleAsync(
            int userId,
            int roleId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Kullanıcıya yeni rol ataması oluşturur.
        /// Oluşturulan atamanın ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateUserUnitRoleDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut kullanıcı-rol atamasını günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateUserUnitRoleDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Method

        /// <summary>
        /// Kullanıcı-rol atamasının aktiflik
        /// durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Kullanıcı-rol atamasının daha önce
        /// oluşturulup oluşturulmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsAssignmentAsync(
            int userId,
            int roleId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirtilen organizasyon
        /// biriminde aktif rolü olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveRoleInUnitAsync(
            int userId,
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirtilen role ait
        /// aktif ataması olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveRoleAsync(
            int userId,
            int roleId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
