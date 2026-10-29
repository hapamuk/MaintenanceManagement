using MaintenanceManagement.Application.DTOS.AssetUserDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IAssetUserService
    {
        #region Get Methods

        /// <summary>
        /// Bütün zimmet kayıtlarını getirir.
        /// Aktif ve iade edilmiş kayıtlar dahildir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetUserDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre zimmet detayını getirir.
        /// </summary>
        Task<GetByIdAssetUserDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa ait bütün zimmet geçmişini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetUserDto>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının bütün zimmet kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetUserDto>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Active Assignment Methods

        /// <summary>
        /// Sistemdeki aktif zimmet kayıtlarını getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetUserDto>>
            GetActiveAssignmentsAsync(
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa ait aktif zimmet kaydını getirir.
        /// </summary>
        Task<GetByIdAssetUserDto?>
            GetActiveAssignmentByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının aktif zimmetlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetUserDto>>
            GetActiveAssignmentsByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli bir kullanıcının oluşturduğu
        /// zimmet işlemlerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultAssetUserDto>>
            GetAssignmentsMadeByUserIdAsync(
                int assignedByUserId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni zimmet oluşturur.
        /// Zimmeti oluşturan kullanıcının ID bilgisi
        /// güvenilir oturum bağlamından alınmalıdır.
        /// </summary>
        Task<int> CreateAsync(
            CreateAssetUserDto dto,
            int? assignedByUserId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Açık zimmet kaydının açıklamasını günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateAssetUserDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Return Method

        /// <summary>
        /// Zimmeti iade işlemiyle sonlandırır.
        /// EndDate alanı güncellenir.
        /// </summary>
        Task<bool> ReturnAssetAsync(
            int id,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Demirbaşa ait aktif zimmet var mı kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssignmentAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşa ait iade edilmemiş
        /// zimmet var mı kontrol eder.
        /// </summary>
        Task<bool> HasUnreturnedAssignmentAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının aktif zimmet kaydı var mı kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssignmentsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli demirbaşın kullanıcıya aktif
        /// olarak zimmetli olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> IsAssetAssignedToUserAsync(
            int assetId,
            int userId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
