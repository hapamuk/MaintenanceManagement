using MaintenanceManagement.Application.DTOS.WorkOrderFilesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Services
{
    public interface IWorkOrderFileService
    {
        #region Get Methods

        Task<IReadOnlyList<ResultWorkOrderFileDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<GetByIdWorkOrderFileDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<GetByIdWorkOrderFileDto?>
            GetByIdAndWorkOrderIdAsync(
                int id,
                int workOrderId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByUploadedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByWorkOrderAndUserAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByContentTypeAsync(
                string contentType,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResultWorkOrderFileDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default);

        Task<GetByIdWorkOrderFileDto?> GetByFilePathAsync(
            string filePath,
            CancellationToken cancellationToken = default);

        #endregion

        #region Create / Update Methods

        /// <summary>
        /// Güvenilir depolama altyapısında bulunan
        /// dosyanın metadata kaydını oluşturur.
        /// </summary>
        Task<int> CreateAsync(
            CreateWorkOrderFileDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Dosyanın yalnızca görünen adını günceller.
        /// </summary>
        Task<bool> UpdateAsync(
            UpdateWorkOrderFileDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Validation Methods

        Task<bool> ExistsFilePathAsync(
            string filePath,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsFileNameInWorkOrderAsync(
            int workOrderId,
            string fileName,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        Task<bool> HasFilesAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        #endregion

        #region Count / Size Methods

        Task<int> GetFileCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        Task<long> GetTotalFileSizeByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
