using MaintenanceManagement.Application.DTOS.TeamMemberDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface ITeamMemberService
    {
        #region Get Methods

        /// <summary>
        /// Aktif, pasif ve sona ermiş bütün
        /// ekip üyeliklerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamMemberDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// ID bilgisine göre üyelik detayını getirir.
        /// </summary>
        Task<GetByIdTeamMemberDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ekibin bütün üyeliklerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamMemberDto>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ekibin aktif üyelerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamMemberDto>> GetActiveByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının bütün ekip üyeliklerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamMemberDto>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının aktif ekip üyeliklerini getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamMemberDto>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcı ve ekibin aktif üyelik
        /// kaydını getirir.
        /// </summary>
        Task<GetByIdTeamMemberDto?> GetActiveByUserAndTeamAsync(
            int userId,
            int teamId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Work Order Methods

        /// <summary>
        /// Seçilen ekipten, belirtilen organizasyon
        /// birimi ve iş alt türüne uygun personelleri getirir.
        /// </summary>
        Task<IReadOnlyList<ResultTeamMemberDto>>
            GetEligibleForWorkOrderAsync(
                int teamId,
                int unitId,
                int workSubTypeId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Bir ekip üyesinin ilgili iş emrine
        /// atanma şartlarını sağlayıp sağlamadığını kontrol eder.
        /// </summary>
        Task<bool> IsEligibleForWorkOrderAsync(
            int teamMemberId,
            int teamId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ekip üyesine ait açık iş emri
        /// görevlendirmesi bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrderAssignmentsAsync(
            int teamMemberId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni ekip üyeliği oluşturur.
        /// Oluşturulan üyelik kaydının ID bilgisini döndürür.
        /// </summary>
        Task<int> CreateAsync(
            CreateTeamMemberDto dto,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut ekip üyeliğini kontrollü şekilde günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateTeamMemberDto dto,
            CancellationToken cancellationToken = default);

        #endregion

        #region Status Methods

        /// <summary>
        /// Ekip üyeliğinin aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ekip üyeliğini sonlandırır.
        /// LeftAt alanı sonlandırma tarihiyle doldurulur.
        /// </summary>
        Task<bool> EndMembershipAsync(
            int id,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        /// <summary>
        /// Aynı ekip ve kullanıcıya ait başka
        /// aktif üyelik olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsActiveMembershipAsync(
            int teamId,
            int userId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
