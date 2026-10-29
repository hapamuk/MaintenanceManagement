using MaintenanceManagement.Application.DTOS.PermissionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IPermissionService
    {
        #region Get Methods

        /// <summary>
        /// Aktif ve pasif bütün yetkileri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPermissionDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yalnızca aktif yetkileri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPermissionDto>> GetActiveAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine ait
        /// yetkinin detaylarını getirir.
        /// </summary>
        Task<GetByIdPermissionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen modüle ait aktif
        /// yetkileri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultPermissionDto>> GetByModuleAsync(
            string module,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni yetki oluşturur ve oluşturulan
        /// yetkinin ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreatePermissionDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut yetkinin bilgilerini günceller.
        /// Kayıt bulunamadığında false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdatePermissionDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region ChangeStatus Method

        /// <summary>
        /// Yetkinin aktif veya pasif durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı modülde belirtilen yetki
        /// adının kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            string name,
            string module,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yetki kodunun başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yetkiye bağlı rol veya iş alt türü
        /// yetki ataması bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasAssignmentsAsync(
            int id,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
