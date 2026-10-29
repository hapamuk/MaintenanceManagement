using MaintenanceManagement.Application.DTOS.WorkOrderPersonnelDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkOrderPersonnelService
    {
        #region Get Methods

        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default);

        Task<GetByIdWorkOrderPersonnelDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetActiveByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByTeamMemberIdAsync(
                int teamMemberId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetActiveByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByAssignedByUserIdAsync(
                int assignedByUserId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli iş emrinin personel atamalarını
        /// görev rolüne göre getirir.
        ///
        /// Repository imzasıyla uyumludur.
        /// </summary>
        Task<IReadOnlyList<ResultWorkOrderPersonnelDto>>
            GetByAssignmentRoleAsync(
                int workOrderId,
                AssignmentRoleEnum assignmentRole,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin aktif sorumlu personelini getirir.
        /// </summary>
        Task<GetByIdWorkOrderPersonnelDto?>
            GetActiveResponsibleAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// İş emrine yeni personel atar.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkOrderPersonnelDto dto,
            int assignedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Aktif atamanın görev rolünü ve
        /// açıklamasını günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkOrderPersonnelDto dto,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Personelin görevini sonlandırır.
        /// Geçmiş kayıt silinmez.
        /// </summary>
        Task<bool> ReleasePersonnelAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        Task<bool> ExistsActiveAssignmentAsync(
            int workOrderId,
            int teamMemberId,
            CancellationToken cancellationToken = default);

        Task<bool> IsUserAssignedAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveResponsibleAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveAssignmentsByTeamMemberIdAsync(
            int teamMemberId,
            CancellationToken cancellationToken = default);

        Task<int> GetActivePersonnelCountAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        Task<bool> CanTeamMemberBeAssignedAsync(
            int workOrderId,
            int teamMemberId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
