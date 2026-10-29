using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkOrderActionRepository
           : IGenericRepository<WorkOrderAction>
    {
        /// <summary>
        /// Bütün iş emri faaliyetlerini ilişkili
        /// detaylarıyla birlikte getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen faaliyeti iş emri ve
        /// kullanıcı detaylarıyla getirir.
        /// </summary>
        Task<WorkOrderAction?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait bütün faaliyet
        /// kayıtlarını kronolojik sırayla getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen kullanıcı tarafından
        /// gerçekleştirilen faaliyetleri getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetByPerformedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli bir iş emrinde
        /// gerçekleştirdiği faaliyetleri getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetByWorkOrderAndUserAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerinin
        /// faaliyet kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetByResponsibleUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takıma atanmış iş emirlerinin
        /// faaliyet kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığındaki
        /// faaliyet kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin belirtilen tarih aralığındaki
        /// faaliyet kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderAction>> GetByWorkOrderAndDateRangeAsync(
            int workOrderId,
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait en son
        /// faaliyet kaydını getirir.
        /// </summary>
        Task<WorkOrderAction?> GetLatestActionByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait herhangi bir faaliyet
        /// kaydı bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActionsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirtilen iş emrinde
        /// faaliyet kaydı olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> HasActionsByUserIdAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki toplam faaliyet
        /// kaydı sayısını getirir.
        /// </summary>
        Task<int> GetActionCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinin toplam işçilik süresini
        /// dakika cinsinden getirir.
        /// </summary>
        Task<long> GetTotalLaborMinutesByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının bütün iş emirlerindeki
        /// toplam işçilik süresini getirir.
        /// </summary>
        Task<long> GetTotalLaborMinutesByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Kullanıcının belirli bir iş emrindeki
        /// toplam işçilik süresini getirir.
        /// </summary>
        Task<long> GetTotalLaborMinutesByWorkOrderAndUserAsync(
            int workOrderId,
            int userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takımın iş emirlerindeki
        /// toplam kayıtlı işçilik süresini getirir.
        /// </summary>
        Task<long> GetTotalLaborMinutesByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Devam eden iş emrine, halen görevli
        /// aktif personelin faaliyetini kaydeder.
        /// </summary>
        Task<WorkOrderAction> RegisterActionAsync(
            WorkOrderAction action,
            CancellationToken cancellationToken = default);
    }
}
