using MaintenanceManagement.Application.DTOS.WorkRequestDtos;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkRequestService
    {
        #region Get Methods

        Task<IReadOnlyList<ResultWorkRequestDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<GetByIdWorkRequestDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<GetByIdWorkRequestDto?> GetByRequestNumberAsync(
            string requestNumber,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByRequestedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByRequesterUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetNewRequestsByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetSubmittedNewRequestsByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByManagerUserIdAsync(
                int managerUserId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>> GetByStatusAsync(
            WorkRequestStatusEnum status,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>> GetByPriorityAsync(
            WorkPriorityEnum priority,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>> GetByAssetIdAsync(
            int assetId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkRequestDto>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Yeni iş talebi oluşturur.
        /// Talebi oluşturan kullanıcı, güvenilir
        /// oturum bilgisinden alınmalıdır.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkRequestDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Henüz gönderilmemiş talebi günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkRequestDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Workflow Methods

        /// <summary>
        /// Talebi sorumlu birime gönderir.
        /// Status Draft olarak kalır,
        /// SubmittedAt doldurulur.
        /// </summary>
        Task<bool> SubmitAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gönderilmiş talebi reddeder.
        /// </summary>
        Task<bool> RejectAsync(
            int id,
            int performedByUserId,
            string reason,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talebi iptal eder.
        /// </summary>
        Task<bool> CancelAsync(
            int id,
            int performedByUserId,
            string? description = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İlgili iş emri oluşturulduktan sonra
        /// talebin durumunu günceller.
        /// İş emrini kendisi oluşturmaz.
        /// </summary>
        Task<bool> MarkConvertedToWorkOrderAsync(
            int id,
            int performedByUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        Task<bool> ExistsRequestNumberAsync(
            string requestNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        Task<bool> HasWorkOrderAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
