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
    public class WorkTypeRepository :
           GenericRepository<WorkType>, IWorkTypeRepository
    {
        public WorkTypeRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Aktif iş türlerini alfabetik sırayla getirir.
        /// </summary>
        public async Task<List<WorkType>> GetActiveWorkTypesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bütün iş türlerini alt türleri ve
        /// organizasyon eşleştirmeleriyle getirir.
        /// </summary>
        public async Task<List<WorkType>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen iş türünü ilişkili alt türleri
        /// ve organizasyon bilgileriyle getirir.
        /// </summary>
        public async Task<WorkType?> GetByIdWithDetailsAsync(
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
        /// bütün iş türlerini getirir.
        /// </summary>
        public async Task<List<WorkType>> GetWorkTypesByUnitIdAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.UnitWorkTypes.Any(uwt =>
                    uwt.UnitId == unitId))
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyonda kullanılabilecek aktif iş
        /// türlerini getirir.
        ///
        /// İş türü, eşleştirme ve organizasyon birimi
        /// aktif olmalıdır.
        /// </summary>
        public async Task<List<WorkType>>
            GetActiveWorkTypesByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.UnitWorkTypes.Any(uwt =>
                        uwt.UnitId == unitId &&
                        uwt.IsActive &&
                        uwt.Unit.IsActive))
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş türüne bağlı aktif ve pasif
        /// bütün iş alt türlerini getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetWorkSubTypesByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkSubTypes
                .AsNoTracking()
                .Include(x => x.WorkType)
                .Where(x => x.WorkTypeId == workTypeId)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Aktif iş türüne bağlı aktif
        /// iş alt türlerini getirir.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetActiveWorkSubTypesByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkSubTypes
                .AsNoTracking()
                .Include(x => x.WorkType)
                .Where(x =>
                    x.WorkTypeId == workTypeId &&
                    x.IsActive &&
                    x.WorkType.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Organizasyona tanımlanmış iş türüne ait
        /// kullanılabilir aktif iş alt türlerini getirir.
        ///
        /// İş talebi oluşturma ekranındaki
        /// bağımlı seçim alanlarında kullanılır.
        /// </summary>
        public async Task<List<WorkSubType>>
            GetAvailableWorkSubTypesByUnitAndTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            return await _context.WorkSubTypes
                .AsNoTracking()
                .Include(x => x.WorkType)
                .Where(x =>
                    x.WorkTypeId == workTypeId &&
                    x.IsActive &&
                    x.WorkType.IsActive &&

                    x.WorkType.UnitWorkTypes.Any(uwt =>
                        uwt.UnitId == unitId &&
                        uwt.IsActive &&
                        uwt.Unit.IsActive))
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş türü adının başka bir kayıtta
        /// kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string normalizedName = name.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Name == normalizedName &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// İş türüne bağlı herhangi bir
        /// alt tür olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasWorkSubTypesAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkSubTypes
                .AsNoTracking()
                .AnyAsync(
                    x => x.WorkTypeId == workTypeId,
                    cancellationToken);
        }

        /// <summary>
        /// İş türüne bağlı aktif alt tür
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveWorkSubTypesAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkSubTypes
                .AsNoTracking()
                .AnyAsync(
                    x => x.WorkTypeId == workTypeId &&
                         x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// İş türünün herhangi bir organizasyona
        /// tanımlanıp tanımlanmadığını kontrol eder.
        ///
        /// Pasif eşleştirmeler de dikkate alınır.
        /// </summary>
        public async Task<bool> HasUnitAssignmentsAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UnitWorkTypes
                .AsNoTracking()
                .AnyAsync(
                    x => x.WorkTypeId == workTypeId,
                    cancellationToken);
        }

        /// <summary>
        /// İş türüne bağlı iş alt türleri üzerinden
        /// oluşturulmuş iş talebi olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasWorkRequestsAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkRequests
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkSubType.WorkTypeId == workTypeId,
                    cancellationToken);
        }

        /// <summary>
        /// İş türüne bağlı tamamlanmamış veya
        /// iptal edilmemiş iş emri olup olmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.WorkOrders
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkSubType.WorkTypeId == workTypeId &&
                    (
                        x.Status == WorkStatusEnum.New ||
                        x.Status == WorkStatusEnum.InProgress ||
                        x.Status == WorkStatusEnum.PendingApproval
                    ),
                    cancellationToken);
        }

        /// <summary>
        /// İş türüne bağlı aktif planlı
        /// bakım olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveMaintenancePlansAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.PlannedMaintenances
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WorkSubType.WorkTypeId == workTypeId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// İş türünün aktiflik durumunu günceller
        /// ve veritabanına kaydeder.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var workType = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (workType == null)
            {
                return false;
            }

            if (workType.IsActive == isActive)
            {
                return true;
            }

            workType.IsActive = isActive;
            workType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// İş türünün alt türlerini ve organizasyon
        /// eşleştirmelerini içeren ortak detay sorgusudur.
        /// </summary>
        private IQueryable<WorkType> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // İş alt türleri
                .Include(x => x.WorkSubTypes)

                // Organizasyon eşleştirmeleri
                .Include(x => x.UnitWorkTypes)
                    .ThenInclude(x => x.Unit)

                .AsSplitQuery();
        }
    }
}
