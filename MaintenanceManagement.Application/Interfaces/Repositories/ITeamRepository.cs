using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        /// <summary>
        /// Aktif organizasyonlara bağlı aktif takımları getirir.
        /// </summary>
        Task<List<Team>> GetActiveTeamsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takımları organizasyon, personel ve
        /// iş emri bilgileriyle birlikte getirir.
        /// </summary>
        Task<List<Team>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takımı ilişkili detaylarıyla getirir.
        /// </summary>
        Task<Team?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyona ait bütün takımları getirir.
        /// Aktif ve pasif kayıtlar dahildir.
        /// </summary>
        Task<List<Team>> GetTeamsByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen organizasyona ait aktif takımları getirir.
        /// </summary>
        Task<List<Team>> GetActiveTeamsByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takımın aktif personel üyeliklerini getirir.
        /// </summary>
        Task<List<TeamMember>> GetActiveTeamMembersAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı organizasyonda belirtilen takım adının
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsNameAsync(
            int unitId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aynı organizasyonda takım kodunun
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takımda aktif personel bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveMembersAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takıma bağlı henüz tamamlanmamış veya
        /// iptal edilmemiş iş emri olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasOpenWorkOrdersAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takımın aktiflik durumunu değiştirir.
        /// </summary>
        Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default);
    }
}
