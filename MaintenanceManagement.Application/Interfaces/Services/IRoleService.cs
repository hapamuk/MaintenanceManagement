using MaintenanceManagement.Application.DTOS.RoleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IRoleService
    {
        /// <summary>
        /// Sistemde tanımlı bütün rolleri getirir.
        /// Aktif ve pasif roller dahildir.
        /// </summary>
        Task<IReadOnlyList<ResultRoleDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ID değerine ait rolün
        /// detay bilgilerini getirir.
        /// </summary>
        Task<GetByIdRoleDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon biriminin
        /// aktif rollerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultRoleDto>> GetByUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni rol oluşturur ve oluşturulan
        /// rolün ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateRoleDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut rolü günceller.
        /// Kayıt bulunamazsa false döndürür.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateRoleDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolün aktiflik durumunu belirtilen
        /// değere göre değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyon birimi içinde
        /// rol kodunun kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);
    }
}
