using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Repositories
{
    public class MaintenanceChecklistRepository :
       GenericRepository<MaintenanceChecklist>,
       IMaintenanceChecklistRepository
    {
        public MaintenanceChecklistRepository(
            ApplicationDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Bütün bakım kontrol maddelerini
        /// ilişkili detaylarıyla getirir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetAllWithDetailsAsync(
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .OrderBy(x => x.PlannedMaintenanceId)
                .ThenBy(x => x.SequenceNumber)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kontrol maddesini ID bilgisine göre
        /// bakım planı detaylarıyla getirir.
        /// </summary>
        public async Task<MaintenanceChecklist?>
            GetByIdWithDetailsAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        /// <summary>
        /// Bakım planına ait bütün kontrol
        /// maddelerini sıralı olarak getirir.
        ///
        /// Aktif ve pasif maddeler dahildir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId)
                .OrderBy(x => x.SequenceNumber)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bakım planındaki aktif kontrol
        /// maddelerini sıralı olarak getirir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetActiveByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.IsActive)
                .OrderBy(x => x.SequenceNumber)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bakım planındaki aktif ve
        /// zorunlu kontrol maddelerini getirir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetActiveRequiredByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.IsActive &&
                    x.IsRequired)
                .OrderBy(x => x.SequenceNumber)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen demirbaşın bütün
        /// bakım kontrol maddelerini getirir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedMaintenance.AssetId == assetId)
                .OrderBy(x => x.PlannedMaintenanceId)
                .ThenBy(x => x.SequenceNumber)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Belirtilen organizasyona ait
        /// bakım kontrol maddelerini getirir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedMaintenance.UnitId == unitId)
                .OrderBy(x => x.PlannedMaintenanceId)
                .ThenBy(x => x.SequenceNumber)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş alt türüne bağlı bakım
        /// kontrol maddelerini getirir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedMaintenance.WorkSubTypeId ==
                        workSubTypeId)
                .OrderBy(x => x.PlannedMaintenanceId)
                .ThenBy(x => x.SequenceNumber)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Bakım planındaki belirli sıra
        /// numarasına ait maddeyi getirir.
        /// </summary>
        public async Task<MaintenanceChecklist?>
            GetBySequenceNumberAsync(
                int plannedMaintenanceId,
                int sequenceNumber,
                CancellationToken cancellationToken = default)
        {
            return await GetDetailsQuery()
                .FirstOrDefaultAsync(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.SequenceNumber == sequenceNumber,
                    cancellationToken);
        }

        /// <summary>
        /// Aynı bakım planında aynı başlığa
        /// sahip başka kontrol maddesi olup
        /// olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsTitleAsync(
            int plannedMaintenanceId,
            string title,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return false;
            }

            string normalizedTitle = title.Trim();

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.Title == normalizedTitle &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Bakım planındaki sıra numarasının
        /// başka bir maddeye ait olup olmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> ExistsSequenceNumberAsync(
            int plannedMaintenanceId,
            int sequenceNumber,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (sequenceNumber <= 0)
            {
                return false;
            }

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.SequenceNumber == sequenceNumber &&
                    (!excludedId.HasValue ||
                     x.Id != excludedId.Value),
                    cancellationToken);
        }

        /// <summary>
        /// Kontrol maddesinin bütün iş emri
        /// kontrol sonuçlarını getirir.
        ///
        /// Geçmiş ve mevcut iş emirleri dahildir.
        /// </summary>
        public async Task<List<WorkOrderChecklistResult>>
            GetResultsByChecklistIdAsync(
                int checklistId,
                CancellationToken cancellationToken = default)
        {
            return await _context
                .Set<WorkOrderChecklistResult>()
                .AsNoTracking()
                .Where(x =>
                    x.MaintenanceChecklistId == checklistId)
                .OrderByDescending(x => x.CheckedAt)
                .ThenByDescending(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Kontrol maddesinin herhangi bir
        /// iş emrinde sonuç kaydı olup
        /// olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasResultsAsync(
            int checklistId,
            CancellationToken cancellationToken = default)
        {
            return await _context
                .Set<WorkOrderChecklistResult>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.MaintenanceChecklistId == checklistId,
                    cancellationToken);
        }

        /// <summary>
        /// Bakım planında en az bir aktif
        /// kontrol maddesi olup olmadığını
        /// kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveChecklistsAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// Bakım planındaki aktif kontrol
        /// maddelerinin sayısını getirir.
        /// </summary>
        public async Task<int> GetActiveChecklistCountAsync(
            int plannedMaintenanceId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .CountAsync(x =>
                    x.PlannedMaintenanceId == plannedMaintenanceId &&
                    x.IsActive,
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinin bakım planında bulunan
        /// henüz tamamlanmamış aktif ve zorunlu
        /// kontrol maddelerini getirir.
        ///
        /// Sonuç kaydı hiç bulunmayan maddeler de
        /// tamamlanmamış olarak değerlendirilir.
        /// </summary>
        public async Task<List<MaintenanceChecklist>>
            GetUncompletedRequiredChecklistsByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            int? plannedMaintenanceId =
                await GetPlannedMaintenanceIdAsync(
                    workOrderId,
                    cancellationToken);

            // İş emrine bağlı bakım planı yoksa
            // bakım kontrol listesi sorgulanamaz.
            if (!plannedMaintenanceId.HasValue)
            {
                return new List<MaintenanceChecklist>();
            }

            return await GetDetailsQuery()
                .Where(x =>
                    x.PlannedMaintenanceId ==
                        plannedMaintenanceId.Value &&

                    x.IsActive &&

                    x.IsRequired &&

                    // Bu madde için tamamlanmış bir
                    // sonuç kaydı bulunmamalı.
                    !_context.Set<WorkOrderChecklistResult>()
                        .Any(result =>
                            result.WorkOrderId == workOrderId &&

                            result.MaintenanceChecklistId == x.Id &&

                            result.IsCompleted))
                .OrderBy(x => x.SequenceNumber)
                .ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// İş emrinde tamamlanmamış aktif
        /// ve zorunlu bakım kontrol maddesi
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool>
            HasUncompletedRequiredChecklistsAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            int? plannedMaintenanceId =
                await GetPlannedMaintenanceIdAsync(
                    workOrderId,
                    cancellationToken);

            if (!plannedMaintenanceId.HasValue)
            {
                return false;
            }

            return await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.PlannedMaintenanceId ==
                        plannedMaintenanceId.Value &&

                    x.IsActive &&

                    x.IsRequired &&

                    !_context.Set<WorkOrderChecklistResult>()
                        .Any(result =>
                            result.WorkOrderId == workOrderId &&

                            result.MaintenanceChecklistId == x.Id &&

                            result.IsCompleted),
                    cancellationToken);
        }

        /// <summary>
        /// İş emrinde tamamlanmış ancak
        /// başarısız olarak işaretlenmiş aktif
        /// ve zorunlu kontrol bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool>
            HasFailedRequiredChecklistsAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            int? plannedMaintenanceId =
                await GetPlannedMaintenanceIdAsync(
                    workOrderId,
                    cancellationToken);

            if (!plannedMaintenanceId.HasValue)
            {
                return false;
            }

            return await _context
                .Set<WorkOrderChecklistResult>()
                .AsNoTracking()
                .AnyAsync(result =>
                    result.WorkOrderId == workOrderId &&

                    result.IsCompleted &&

                    result.IsSuccessful == false &&

                    _dbSet.Any(checklist =>
                        checklist.Id ==
                            result.MaintenanceChecklistId &&

                        checklist.PlannedMaintenanceId ==
                            plannedMaintenanceId.Value &&

                        checklist.IsActive &&

                        checklist.IsRequired),
                    cancellationToken);
        }

        /// <summary>
        /// Bakım kontrol maddesinin aktiflik
        /// durumunu günceller.
        ///
        /// Kayıt fiziksel olarak silinmez.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var checklist = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (checklist == null)
            {
                return false;
            }

            if (checklist.IsActive == isActive)
            {
                return true;
            }

            checklist.IsActive = isActive;

            checklist.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// Bakım kontrol maddesinin sırasını
        /// günceller.
        ///
        /// Aynı bakım planında yeni sıra numarası
        /// başka bir madde tarafından kullanılmamalıdır.
        /// </summary>
        public async Task<bool> UpdateSequenceNumberAsync(
            int id,
            int sequenceNumber,
            CancellationToken cancellationToken = default)
        {
            if (sequenceNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sequenceNumber),
                    "Sıra numarası sıfırdan büyük olmalıdır.");
            }

            var checklist = await _dbSet
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (checklist == null)
            {
                return false;
            }

            if (checklist.SequenceNumber == sequenceNumber)
            {
                return true;
            }

            bool sequenceExists = await _dbSet
                .AsNoTracking()
                .AnyAsync(x =>
                    x.PlannedMaintenanceId ==
                        checklist.PlannedMaintenanceId &&

                    x.SequenceNumber == sequenceNumber &&

                    x.Id != checklist.Id,
                    cancellationToken);

            if (sequenceExists)
            {
                throw new InvalidOperationException(
                    "Bu sıra numarası aynı bakım planında " +
                    "başka bir kontrol maddesi tarafından kullanılıyor.");
            }

            checklist.SequenceNumber = sequenceNumber;

            checklist.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            return true;
        }

        /// <summary>
        /// İş emrine bağlı planlı bakım ID
        /// bilgisini getirir.
        ///
        /// İş emri bulunmazsa veya planlı
        /// bakıma bağlı değilse null döner.
        /// </summary>
        private async Task<int?>
            GetPlannedMaintenanceIdAsync(
                int workOrderId,
                CancellationToken cancellationToken)
        {
            return await _context.Set<WorkOrder>()
                .AsNoTracking()
                .Where(x => x.Id == workOrderId)
                .Select(x => x.PlannedMaintenanceId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Kontrol maddelerini bakım planı,
        /// demirbaş, model, marka, organizasyon,
        /// iş alt türü ve iş türü detaylarıyla getirir.
        /// </summary>
        private IQueryable<MaintenanceChecklist> GetDetailsQuery()
        {
            return _dbSet
                .AsNoTracking()

                // Bakım planı ve demirbaş
                .Include(x => x.PlannedMaintenance)
                    .ThenInclude(x => x.Asset)

                // Demirbaş, model ve marka
                .Include(x => x.PlannedMaintenance)
                    .ThenInclude(x => x.Asset)
                        .ThenInclude(x => x.AssetModel)
                            .ThenInclude(x => x.Brand)

                // Bakım planından sorumlu organizasyon
                .Include(x => x.PlannedMaintenance)
                    .ThenInclude(x => x.Unit)

                // İş alt türü ve iş türü
                .Include(x => x.PlannedMaintenance)
                    .ThenInclude(x => x.WorkSubType)
                        .ThenInclude(x => x.WorkType);
        }
    }
}
