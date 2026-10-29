using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Interfaces.Repositories
{
    public interface IWorkOrderMaterialRepository
         : IGenericRepository<WorkOrderMaterial>
    {
        /// <summary>
        /// Bütün iş emri malzeme kayıtlarını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen iş emri malzeme kaydını
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        Task<WorkOrderMaterial?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrine ait bütün malzeme
        /// kullanım kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen malzemenin kullanıldığı
        /// bütün iş emri kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen ambardan iş emirlerine
        /// verilen malzemeleri getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetByWarehouseIdAsync(
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde kullanılan belirli
        /// malzemeye ait kayıtları getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetByWorkOrderAndMaterialIdAsync(
            int workOrderId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde belirtilen ambardan
        /// kullanılan malzemeleri getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetByWorkOrderAndWarehouseIdAsync(
            int workOrderId,
            int warehouseId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen stok hareketine bağlı
        /// malzeme kullanım kaydını getirir.
        /// </summary>
        Task<WorkOrderMaterial?> GetByStockMovementIdAsync(
            int stockMovementId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sorumlu organizasyona ait iş emirlerinde
        /// kullanılan malzemeleri getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetByResponsibleUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen takıma atanmış iş emirlerinde
        /// kullanılan malzemeleri getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetByTeamIdAsync(
            int teamId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen UTC tarih aralığında oluşturulan
        /// iş emri malzeme kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>> GetByDateRangeAsync(
            DateTime startDateUtc,
            DateTime endDateUtc,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde malzeme kullanım kaydı
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        Task<bool> HasMaterialsByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Stok hareketinin başka bir malzeme
        /// kullanım kaydına bağlanıp bağlanmadığını
        /// kontrol eder.
        /// </summary>
        Task<bool> ExistsStockMovementLinkAsync(
            int stockMovementId,
            int? excludedId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki toplam malzeme kullanım
        /// kayıtlarının sayısını getirir.
        /// </summary>
        Task<int> GetMaterialRecordCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde belirtilen malzemeden
        /// toplam ne kadar kullanıldığını getirir.
        /// </summary>
        Task<decimal> GetTotalQuantityByWorkOrderAndMaterialIdAsync(
            int workOrderId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirtilen malzemenin bütün
        /// iş emirlerindeki toplam kullanımını getirir.
        /// </summary>
        Task<decimal> GetTotalQuantityByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrindeki bilinen malzeme
        /// maliyetlerinin toplamını getirir.
        /// </summary>
        Task<decimal> GetTotalCostByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde belirtilen malzemeye ait
        /// bilinen toplam maliyeti getirir.
        /// </summary>
        Task<decimal> GetTotalCostByWorkOrderAndMaterialIdAsync(
            int workOrderId,
            int materialId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde birim maliyeti henüz
        /// belirlenmemiş malzeme var mı?
        /// </summary>
        Task<bool> HasMaterialsWithoutUnitCostAsync(
            int workOrderId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// İş emrinde birim maliyeti bulunmayan
        /// malzeme kullanım kayıtlarını getirir.
        /// </summary>
        Task<List<WorkOrderMaterial>>
            GetMaterialsWithoutUnitCostAsync(
                int workOrderId,
                CancellationToken cancellationToken = default);

        /// <summary>
        /// Gerçekleştirilmiş stok çıkışına ait
        /// malzeme kullanım kaydını oluşturur.
        ///
        /// Bu metot ambar stok miktarını düşürmez.
        /// </summary>
        Task<WorkOrderMaterial> RegisterIssuedMaterialAsync(
            WorkOrderMaterial workOrderMaterial,
            CancellationToken cancellationToken = default);
    }
}
