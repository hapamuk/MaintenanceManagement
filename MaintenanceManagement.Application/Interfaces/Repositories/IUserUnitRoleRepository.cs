using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IUserUnitRoleRepository
         : IGenericRepository<UserUnitRole>
    {
        /// <summary>
        /// Bütün kullanıcı-rol eşleştirmelerini kullanıcı,
        /// rol ve organizasyon detaylarıyla getirir.
        /// </summary>
        Task<List<UserUnitRole>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı-rol eşleştirmesini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<UserUnitRole?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcıya ait aktif ve pasif
        /// bütün rol eşleştirmelerini getirir.
        /// </summary>
        Task<List<UserUnitRole>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının yalnızca aktif ve
        /// kullanılabilir rollerini getirir.
        /// </summary>
        Task<List<UserUnitRole>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen role atanmış kullanıcıların
        /// eşleştirme kayıtlarını getirir.
        /// </summary>
        Task<List<UserUnitRole>> GetByRoleIdAsync(
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Organizasyona bağlı bütün kullanıcı-rol
        /// eşleştirmelerini getirir.
        /// </summary>
        Task<List<UserUnitRole>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli organizasyondaki
        /// aktif rol eşleştirmelerini getirir.
        /// </summary>
        Task<List<UserUnitRole>> GetActiveByUserAndUnitAsync(
            int userId,
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı ve rol bilgilerine göre
        /// eşleştirme kaydını getirir.
        /// Pasif kayıtlar da dikkate alınır.
        /// </summary>
        Task<UserUnitRole?> GetByUserAndRoleAsync(
            int userId,
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı ve rol eşleştirmesinin daha önce
        /// oluşturulup oluşturulmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsAssignmentAsync(
            int userId,
            int roleId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirtilen organizasyonda
        /// aktif rolü bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveRoleInUnitAsync(
            int userId,
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirtilen role aktif olarak
        /// sahip olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveRoleAsync(
            int userId,
            int roleId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı-rol eşleştirmesinin
        /// aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
