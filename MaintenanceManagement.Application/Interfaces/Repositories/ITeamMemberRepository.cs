using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface ITeamMemberRepository
         : IGenericRepository<TeamMember>
    {
        /// <summary>
        /// Takım üyeliklerini takım, organizasyon ve
        /// kullanıcı bilgileriyle birlikte getirir.
        /// </summary>
        Task<List<TeamMember>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takım üyesini ilişkili bilgiler ve
        /// iş emri atamalarıyla birlikte getirir.
        /// </summary>
        Task<TeamMember?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takıma ait aktif ve pasif bütün
        /// üyelik kayıtlarını getirir.
        /// </summary>
        Task<List<TeamMember>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takımda halen görev yapan aktif
        /// personelleri getirir.
        /// </summary>
        Task<List<TeamMember>> GetActiveByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının geçmiş ve mevcut
        /// takım üyeliklerini getirir.
        /// </summary>
        Task<List<TeamMember>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının mevcut aktif takım
        /// üyeliklerini getirir.
        /// </summary>
        Task<List<TeamMember>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirtilen takımdaki
        /// aktif üyeliğini getirir.
        /// </summary>
        Task<TeamMember?> GetActiveByUserAndTeamAsync(
            int userId,
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı kullanıcı ve takım için aktif
        /// üyelik kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsActiveMembershipAsync(
            int teamId,
            int userId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Seçilen takımda, ilgili iş alt türünü
        /// uygulama yetkisine sahip aktif üyeleri getirir.
        /// </summary>
        Task<List<TeamMember>> GetEligibleForWorkOrderAsync(
            int teamId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takım üyesinin iş emrine
        /// atanmak için uygunluğunu kontrol eder.
        /// </summary>
        Task<bool> IsEligibleForWorkOrderAsync(
            int teamMemberId,
            int teamId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takım üyesinin devam eden aktif iş emri
        /// görevlendirmesi bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrderAssignmentsAsync(
            int teamMemberId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takım üyeliğinin aktif/pasif
        /// durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Personelin takımdan ayrılışını kaydeder.
        /// Üyeliği pasife alır ve ayrılış tarihini doldurur.
        /// </summary>
        Task<bool> EndMembershipAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}
