using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IAssetUserRepository
          : IGenericRepository<AssetUser>
    {
        /// <summary>
        /// Bütün zimmet kayıtlarını demirbaş,
        /// personel ve zimmeti veren kullanıcı
        /// bilgileriyle birlikte getirir.
        /// </summary>
        Task<List<AssetUser>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen zimmet kaydını bütün
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<AssetUser?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaşın geçmiş ve mevcut
        /// bütün zimmet kayıtlarını getirir.
        /// </summary>
        Task<List<AssetUser>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcıya ait bütün
        /// zimmet geçmişini getirir.
        /// </summary>
        Task<List<AssetUser>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Halen aktif olan bütün zimmet kayıtlarını getirir.
        /// </summary>
        Task<List<AssetUser>> GetActiveAssignmentsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaşın mevcut aktif
        /// zimmet kaydını getirir.
        /// </summary>
        Task<AssetUser?> GetActiveAssignmentByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcının halen zimmetinde
        /// bulunan demirbaş kayıtlarını getirir.
        /// </summary>
        Task<List<AssetUser>> GetActiveAssignmentsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// oluşturulan zimmet kayıtlarını getirir.
        /// </summary>
        Task<List<AssetUser>> GetAssignmentsMadeByUserIdAsync(
            int assignedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaşın halen aktif bir zimmeti
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssignmentAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Demirbaş için henüz iade edilmemiş
        /// veya ileri tarihli zimmet bulunup bulunmadığını
        /// kontrol eder.
        /// </summary>
        Task<bool> HasUnreturnedAssignmentAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının üzerinde halen aktif
        /// zimmet bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssignmentsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen demirbaşın belirtilen
        /// kullanıcıya aktif olarak zimmetli
        /// olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> IsAssetAssignedToUserAsync(
            int assetId,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Zimmet iade işlemini gerçekleştirir.
        /// İade tarihini doldurur ve kaydeder.
        /// </summary>
        Task<bool> ReturnAssetAsync(
            int assignmentId,
            DateTime? returnedAtUtc = null,
            CancellationToken cancellationToken = default);
    }
}
