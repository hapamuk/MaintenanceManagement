using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkOrderPersonnelRepository
       : IGenericRepository<WorkOrderPersonnel>
    {
        /// <summary>
        /// Bütün personel atamalarını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen personel atamasını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<WorkOrderPersonnel?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait bütün geçmiş ve mevcut
        /// personel atamalarını getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde halen aktif olan
        /// personel atamalarını getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetActiveByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takım üyesine ait bütün görev
        /// geçmişini getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetByTeamMemberIdAsync(
            int teamMemberId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının bütün geçmiş ve mevcut
        /// iş emri görevlendirmelerini getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının aktif personel
        /// atamalarını getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takımın personel
        /// görevlendirmelerini getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// yapılan görevlendirmeleri getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetByAssignedByUserIdAsync(
            int assignedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki belirtilen görev rolüne
        /// sahip personel atamalarını getirir.
        /// </summary>
        Task<List<WorkOrderPersonnel>> GetByAssignmentRoleAsync(
            int workOrderId,
            AssignmentRoleEnum assignmentRole,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin aktif sorumlu
        /// personel atamasını getirir.
        /// </summary>
        Task<WorkOrderPersonnel?> GetActiveResponsibleAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takım üyesinin aynı iş emrinde
        /// aktif ataması olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> ExistsActiveAssignmentAsync(
            int workOrderId,
            int teamMemberId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının iş emrinde aktif
        /// görevli olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> IsUserAssignedAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde aktif sorumlu personel
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveResponsibleAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takım üyesinin aktif bir iş emri
        /// ataması olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActiveAssignmentsByTeamMemberIdAsync(
            int teamMemberId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki aktif personel
        /// atamalarının sayısını getirir.
        /// </summary>
        Task<int> GetActivePersonnelCountAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Takım üyesinin iş emrine atanmak için
        /// organizasyon, takım üyeliği ve
        /// iş alt türü yetkileri bakımından
        /// uygun olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> CanTeamMemberBeAssignedAsync(
            int workOrderId,
            int teamMemberId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Personeli iş emrine görevlendirir.
        /// </summary>
        Task<WorkOrderPersonnel> AssignPersonnelAsync(
            WorkOrderPersonnel assignment,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Personel görevlendirmesini sonlandırır.
        /// Kayıt fiziksel olarak silinmez.
        /// </summary>
        Task<bool> ReleasePersonnelAsync(
            int assignmentId,
            DateTime? releasedAtUtc = null,
            CancellationToken cancellationToken = default);
    }
}
