using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        /// <summary>
        /// Aktif kullanıcı ve personelleri getirir.
        /// </summary>
        Task<List<User>> GetActiveUsersAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcıları asıl birim, organizasyon rolleri
        /// ve takım üyelikleriyle birlikte getirir.
        /// </summary>
        Task<List<User>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcıyı ilişkili organizasyon,
        /// rol ve takım bilgileriyle getirir.
        /// </summary>
        Task<User?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyonu asıl birimi olarak
        /// kullanan aktif personelleri getirir.
        /// </summary>
        Task<List<User>> GetUsersByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takımın aktif üyelerini getirir.
        /// </summary>
        Task<List<User>> GetUsersByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon rolüne sahip
        /// aktif kullanıcıları getirir.
        /// </summary>
        Task<List<User>> GetUsersByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// E-posta adresine göre kullanıcı getirir.
        /// </summary>
        Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı adına göre kayıt getirir.
        /// </summary>
        Task<User?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// E-posta adresinin daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsEmailAsync(
            string email,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı adının daha önce kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsUsernameAsync(
            string username,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının sisteme giriş yetkisini değiştirir.
        /// </summary>
        Task<bool> ChangeSystemAccessAsync(
            int id,
            bool hasSystemAccess,
            CancellationToken cancellationToken = default);
    }
}
