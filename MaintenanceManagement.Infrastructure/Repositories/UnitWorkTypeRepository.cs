using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class UnitWorkTypeRepository :
       GenericRepository<UnitWorkType>,
       IUnitWorkTypeRepository
    {
        public UnitWorkTypeRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün organizasyon-iş türü eşleştirmelerini
        /// organizasyon ve iş türü detaylarıyla getirir.
        /// </summary>
        public async Task<List<UnitWorkType>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Unit.Name)
                .ThenBy(x => x.WorkType.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Eşleştirme kaydını ID bilgisine göre
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<UnitWorkType?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyona tanımlanmış aktif ve pasif
        /// bütün iş türü eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<UnitWorkType>> GetByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.UnitId == unitId)
                .OrderBy(x => x.WorkType.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyonun kullanılabilir durumdaki
        /// aktif iş türü eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<UnitWorkType>> GetActiveByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.UnitId == unitId &&
                    x.IsActive &&
                    x.Unit.IsActive &&
                    x.Unit.OrganizationType.IsActive &&
                    x.WorkType.IsActive)
                .OrderBy(x => x.WorkType.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş türünün tanımlandığı
        /// bütün organizasyon eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<UnitWorkType>> GetByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x => x.WorkTypeId == workTypeId)
                .OrderBy(x => x.Unit.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş türünün tanımlandığı aktif
        /// organizasyon eşleştirmelerini getirir.
        /// </summary>
        public async Task<List<UnitWorkType>> GetActiveByWorkTypeIdAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.WorkTypeId == workTypeId &&
                    x.IsActive &&
                    x.Unit.IsActive &&
                    x.Unit.OrganizationType.IsActive &&
                    x.WorkType.IsActive)
                .OrderBy(x => x.Unit.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyon ve iş türü ID değerlerine göre
        /// mevcut eşleştirmeyi getirir.
        /// Pasif kayıtlar da sorgulanır.
        /// </summary>
        public async Task<UnitWorkType?> GetByUnitAndWorkTypeAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.UnitId == unitId &&
                         x.WorkTypeId == workTypeId,
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyonda kullanılabilen
        /// aktif iş türlerini getirir.
        /// </summary>
        public async Task<List<WorkType>>
            GetActiveWorkTypesByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetActiveAssignmentsQuery()
                .Where(x => x.UnitId == unitId)
                .Select(x => x.WorkType)
                .Distinct()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona tanımlanmış aktif iş türlerine
        /// bağlı kullanılabilir iş alt türlerini getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetAvailableWorkSubTypesByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkSubTypes
                .AsNoTracking()
                .Include(x => x.WorkType)
                .Where(x =>
                    x.IsActive &&
                    x.WorkType.IsActive &&
                    x.WorkType.UnitWorkTypes.Any(uwt =>
                        uwt.UnitId == unitId &&
                        uwt.IsActive &&
                        uwt.Unit.IsActive &&
                        uwt.Unit.OrganizationType.IsActive))
                .OrderBy(x => x.WorkType.Name)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aynı organizasyon ve iş türü eşleştirmesinin
        /// daha önce oluşturulup oluşturulmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsAssignmentAsync(
            int unitId,
            int workTypeId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UnitId == unitId &&
                    x.WorkTypeId == workTypeId &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// İş türünün organizasyonda kullanılabilir
        /// durumda olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> IsWorkTypeAvailableForUnitAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await GetActiveAssignmentsQuery()
                .AnyAsync(
                    x => x.UnitId == unitId &&
                         x.WorkTypeId == workTypeId,
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyonun ilgili iş türünde
        /// "Yeni" durumundaki iş taleplerini kontrol eder.
        ///
        /// Yeni WorkRequestStatusEnum tanımına göre:
        ///
        /// Draft = 1                 : Yeni
        /// Rejected = 2              : Reddedildi
        /// ConvertedToWorkOrder = 3  : İş Emrine Dönüştürüldü
        /// Cancelled = 4             : İptal Edildi
        ///
        /// Yalnızca Draft durumundaki iş talepleri
        /// sonuçlandırılmamış kabul edilir.
        /// </summary>
        public async Task<bool> HasPendingWorkRequestsAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequests
                .AsNoTracking()
                .AnyAsync(x =>
                    x.ResponsibleUnitId == unitId &&
                    x.WorkSubType.WorkTypeId == workTypeId &&
                    x.Status == WorkRequestStatusEnum.Draft,
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyonun belirtilen iş türünde
        /// henüz tamamlanmamış veya iptal edilmemiş
        /// iş emri olup olmadığını kontrol eder.
        ///
        /// WorkStatusEnum yapısı değiştirilmemiştir.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrders
                .AsNoTracking()
                .AnyAsync(x =>
                    x.ResponsibleUnitId == unitId &&
                    x.WorkSubType.WorkTypeId == workTypeId &&
                    (
                        x.Status == WorkStatusEnum.New ||
                        x.Status == WorkStatusEnum.InProgress ||
                        x.Status == WorkStatusEnum.PendingApproval
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// Organizasyon-iş türü eşleştirmesinin
        /// aktiflik durumunu değiştirir
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var unitWorkType = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (unitWorkType == null)
            {
                return false;
            }

            if (unitWorkType.IsActive == isActive)
            {
                return true;
            }

            unitWorkType.IsActive = isActive;
            unitWorkType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Organizasyon, organizasyon türü,
        /// iş türü ve iş alt türleriyle birlikte
        /// detaylı eşleştirme sorgusunu oluşturur.
        /// </summary>
        private IQueryable<UnitWorkType> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                .Include(x => x.Unit)
                    .ThenInclude(x => x.OrganizationType)

                .Include(x => x.WorkType)
                    .ThenInclude(x => x.WorkSubTypes)

                .AsSplitQuery();
        }

        /// <summary>
        /// Yalnızca kullanılabilir durumdaki aktif
        /// organizasyon-iş türü eşleştirmelerini sorgular.
        /// </summary>
        private IQueryable<UnitWorkType> GetActiveAssignmentsQuery()
        {
            return _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Unit.IsActive &&
                    x.Unit.OrganizationType.IsActive &&
                    x.WorkType.IsActive);
        }
    }
}
