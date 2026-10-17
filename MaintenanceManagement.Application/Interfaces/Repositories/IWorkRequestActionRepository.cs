using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkRequestActionRepository
        : IGenericRepository<WorkRequestAction>
    {
        /// <summary>
        /// Bütün iş talebi hareketlerini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen hareketi iş talebi ve
        /// kullanıcı detaylarıyla getirir.
        /// </summary>
        Task<WorkRequestAction?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Hareketin belirtilen iş talebine
        /// ait olduğunu doğrulayarak kaydı getirir.
        /// </summary>
        Task<WorkRequestAction?> GetByIdAndWorkRequestIdAsync(
            int id,
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinin bütün hareket geçmişini
        /// kronolojik sırayla getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetActionsByWorkRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Talep numarasına göre işlem
        /// geçmişi kayıtlarını getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetActionsByRequestNumberAsync(
            string requestNumber,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// gerçekleştirilen hareketleri getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetActionsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyona ait iş taleplerinin
        /// hareket geçmişlerini getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetActionsByResponsibleUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen işlem türüne ait
        /// bütün hareketleri getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetActionsByTypeAsync(
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine ait belirtilen türdeki
        /// hareketleri getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetActionsByRequestAndTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// UTC tarih aralığındaki
        /// hareket kayıtlarını getirir.
        /// </summary>
        Task<List<WorkRequestAction>> GetActionsByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinin en son hareketini getirir.
        /// </summary>
        Task<WorkRequestAction?> GetLatestActionByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine ait belirtilen türdeki
        /// en son hareketi getirir.
        /// </summary>
        Task<WorkRequestAction?> GetLatestActionByTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinde herhangi bir
        /// hareket olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActionsAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebinde belirtilen türde
        /// hareket bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActionTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebine ait toplam
        /// hareket sayısını getirir.
        /// </summary>
        Task<int> GetActionCountByRequestIdAsync(
            int workRequestId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş talebindeki belirtilen türdeki
        /// hareketlerin sayısını getirir.
        /// </summary>
        Task<int> GetActionCountByTypeAsync(
            int workRequestId,
            WorkRequestActionTypeEnum actionType,
            CancellationToken cancellationToken = default);
    }
}
