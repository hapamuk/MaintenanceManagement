using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkOrderChecklistResultDtos;
using MaintenanceManagement.Application.Interfaces;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class WorkOrderChecklistResultService
        : IWorkOrderChecklistResultService
    {
        #region Constants

        private const string ExecutePermission =
            "WORK_ORDER_EXECUTE";

        private const int MaxNotesLength = 1000;

        #endregion

        #region Fields

        private readonly IWorkOrderChecklistResultRepository
            _resultRepository;

        private readonly IWorkOrderRepository
            _workOrderRepository;

        private readonly IMaintenanceChecklistRepository
            _checklistRepository;

        private readonly IWorkOrderPersonnelRepository
            _personnelRepository;

        private readonly IUserRepository
            _userRepository;

        private readonly IRoleWorkSubTypePermissionService
            _permissionService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WorkOrderChecklistResultService(
            IWorkOrderChecklistResultRepository resultRepository,
            IWorkOrderRepository workOrderRepository,
            IMaintenanceChecklistRepository checklistRepository,
            IWorkOrderPersonnelRepository personnelRepository,
            IUserRepository userRepository,
            IRoleWorkSubTypePermissionService permissionService,
            IMapper mapper)
        {
            _resultRepository =
                resultRepository;

            _workOrderRepository =
                workOrderRepository;

            _checklistRepository =
                checklistRepository;

            _personnelRepository =
                personnelRepository;

            _userRepository =
                userRepository;

            _permissionService =
                permissionService;

            _mapper =
                mapper;
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün kontrol sonucu kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var results = await _resultRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID değerine göre kontrol sonucu detayını getirir.
        /// </summary>
        public async Task<GetByIdWorkOrderChecklistResultDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var result = await _resultRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (result == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWorkOrderChecklistResultDto>(
                result);
        }

        #endregion

        #region GetByWorkOrderId

        /// <summary>
        /// İş emrine ait kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetByMaintenanceChecklistId

        /// <summary>
        /// Kontrol maddesine ait bütün sonuçları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByMaintenanceChecklistIdAsync(
                int maintenanceChecklistId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                maintenanceChecklistId,
                "Kontrol Maddesi");

            var results = await _resultRepository
                .GetByMaintenanceChecklistIdAsync(
                    maintenanceChecklistId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetByWorkOrderAndChecklistId

        /// <summary>
        /// İş emri ve kontrol maddesine ait
        /// tekil kontrol sonucunu getirir.
        /// </summary>
        public async Task<GetByIdWorkOrderChecklistResultDto?>
            GetByWorkOrderAndChecklistIdAsync(
                int workOrderId,
                int maintenanceChecklistId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateId(
                maintenanceChecklistId,
                "Kontrol Maddesi");

            var result = await _resultRepository
                .GetByWorkOrderAndChecklistIdAsync(
                    workOrderId,
                    maintenanceChecklistId,
                    cancellationToken);

            if (result == null)
            {
                return null;
            }

            return await GetByIdAsync(
                result.Id,
                cancellationToken);
        }

        #endregion

        #region GetByCheckedByUserId

        /// <summary>
        /// Kullanıcının kontrol ettiği
        /// bütün sonuçları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByCheckedByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                userId,
                "Kullanıcı");

            var results = await _resultRepository
                .GetByCheckedByUserIdAsync(
                    userId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetByWorkOrderAndUserId

        /// <summary>
        /// Kullanıcının belirli iş emrinde
        /// gerçekleştirdiği kontrolleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByWorkOrderAndUserIdAsync(
                int workOrderId,
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateId(
                userId,
                "Kullanıcı");

            var results = await _resultRepository
                .GetByWorkOrderAndUserIdAsync(
                    workOrderId,
                    userId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetByResponsibleUnitId

        /// <summary>
        /// Sorumlu organizasyon birimine ait
        /// kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Sorumlu Birim");

            var results = await _resultRepository
                .GetByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetByPlannedMaintenanceId

        /// <summary>
        /// Planlı bakım kaydına ait
        /// bütün kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByPlannedMaintenanceIdAsync(
                int plannedMaintenanceId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                plannedMaintenanceId,
                "Planlı Bakım");

            var results = await _resultRepository
                .GetByPlannedMaintenanceIdAsync(
                    plannedMaintenanceId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetCompletedByWorkOrderId

        /// <summary>
        /// İş emrindeki tamamlanmış
        /// kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetCompletedByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetCompletedByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetSuccessfulByWorkOrderId

        /// <summary>
        /// İş emrindeki başarılı kontrol
        /// sonuçlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetSuccessfulByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetSuccessfulByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetFailedByWorkOrderId

        /// <summary>
        /// İş emrindeki başarısız
        /// kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetFailedByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetFailedByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetIncompleteByWorkOrderId

        /// <summary>
        /// İş emrinde oluşturulmuş ancak henüz
        /// tamamlanmamış kontrol sonuçlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetIncompleteByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetIncompleteByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return MapResults(results);
        }

        #endregion

        #region GetByDateRange

        /// <summary>
        /// Kontrol tarihi belirtilen UTC
        /// tarih aralığında bulunan sonuçları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkOrderChecklistResultDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. TARİH DOĞRULAMASI
            // =====================================

            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            // =====================================
            // 2. KAYITLARI GETİR
            // =====================================

            var results = await _resultRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            // =====================================
            // 3. TARİH ARALIĞI FİLTRESİ
            // =====================================

            var filteredResults = results
                .Where(x =>
                    x.CheckedAt.HasValue &&
                    x.CheckedAt.Value >= startDateUtc &&
                    x.CheckedAt.Value <= endDateUtc)
                .OrderByDescending(x => x.CheckedAt)
                .ThenByDescending(x => x.Id)
                .ToList();

            // =====================================
            // 4. DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(filteredResults);
        }

        #endregion

        #region Create

        /// <summary>
        /// İş emrine bağlı kontrol maddesi için
        /// başlangıç sonuç kaydı oluşturur.
        ///
        /// Create DTO içerisinde IsCompleted
        /// ve IsSuccessful bulunmamaktadır.
        ///
        /// Başlangıç değerleri:
        /// IsCompleted = false
        /// IsSuccessful = null
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkOrderChecklistResultDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(
                dto.WorkOrderId,
                "İş Emri");

            ValidateId(
                dto.MaintenanceChecklistId,
                "Kontrol Maddesi");

            ValidateId(
                currentUserId,
                "Kullanıcı");

            // =====================================
            // 2. NOT BİLGİSİ
            // =====================================

            string? notes = NormalizeNotes(
                dto.Notes);

            // =====================================
            // 3. İŞ EMRİNİ DOĞRULA
            // =====================================

            var workOrder = await GetExecutableWorkOrderAsync(
                dto.WorkOrderId,
                cancellationToken);

            // =====================================
            // 4. KONTROL MADDESİNİ DOĞRULA
            // =====================================

            await ValidateChecklistAsync(
                workOrder,
                dto.MaintenanceChecklistId,
                cancellationToken);

            // =====================================
            // 5. KULLANICI VE YETKİ KONTROLÜ
            // =====================================

            await ValidateActorAsync(
                workOrder,
                currentUserId,
                cancellationToken);

            // =====================================
            // 6. MÜKERRER KAYIT KONTROLÜ
            // =====================================

            var existingResult = await _resultRepository
                .GetByWorkOrderAndChecklistIdAsync(
                    dto.WorkOrderId,
                    dto.MaintenanceChecklistId,
                    cancellationToken);

            if (existingResult != null)
            {
                throw new InvalidOperationException(
                    "Bu iş emrindeki kontrol maddesi " +
                    "için sonuç kaydı zaten bulunmaktadır. " +
                    "Güncelleme işlemini kullanınız.");
            }

            // =====================================
            // 7. BAŞLANGIÇ SONUÇ DEĞERLERİ
            // =====================================

            bool isCompleted = false;

            bool? isSuccessful = null;

            // =====================================
            // 8. REPOSITORY KAYIT İŞLEMİ
            // =====================================

            var createdResult = await _resultRepository
                .SaveResultAsync(
                    dto.WorkOrderId,
                    dto.MaintenanceChecklistId,
                    currentUserId,
                    isCompleted,
                    isSuccessful,
                    notes,
                    cancellationToken);

            // =====================================
            // 9. SONUÇ ID
            // =====================================

            return createdResult.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut kontrol sonucu kaydını günceller.
        ///
        /// Kontrol sonucu:
        /// - Tamamlanmadı -> null
        /// - Başarılı -> true
        /// - Başarısız -> false
        ///
        /// İş emri InProgress durumunda olmalıdır.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkOrderChecklistResultDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(
                dto.Id,
                "Kontrol Sonucu");

            ValidateId(
                currentUserId,
                "Kullanıcı");

            // =====================================
            // 2. SONUÇ DOĞRULAMASI
            // =====================================

            ValidateResult(
                dto.IsCompleted,
                dto.IsSuccessful);

            string? notes = NormalizeNotes(
                dto.Notes);

            // =====================================
            // 3. MEVCUT KAYDI GETİR
            // =====================================

            var existingResult = await _resultRepository
                .GetByIdWithDetailsAsync(
                    dto.Id,
                    cancellationToken);

            if (existingResult == null)
            {
                return false;
            }

            // =====================================
            // 4. KONTROLÜ YAPAN KULLANICI
            // =====================================

            if (existingResult.CheckedByUserId.HasValue &&
                existingResult.CheckedByUserId.Value !=
                    currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Bu kontrol sonucunu yalnızca " +
                    "kontrolü gerçekleştiren kullanıcı " +
                    "güncelleyebilir.");
            }

            // =====================================
            // 5. İŞ EMRİNİ DOĞRULA
            // =====================================

            var workOrder = await GetExecutableWorkOrderAsync(
                existingResult.WorkOrderId,
                cancellationToken);

            // =====================================
            // 6. KONTROL MADDESİNİ DOĞRULA
            // =====================================

            await ValidateChecklistAsync(
                workOrder,
                existingResult.MaintenanceChecklistId,
                cancellationToken);

            // =====================================
            // 7. PERSONEL VE YETKİ KONTROLÜ
            // =====================================

            await ValidateActorAsync(
                workOrder,
                currentUserId,
                cancellationToken);

            // =====================================
            // 8. REPOSITORY SONUÇ GÜNCELLEME
            // =====================================

            var updatedResult = await _resultRepository
                .SaveResultAsync(
                    existingResult.WorkOrderId,
                    existingResult.MaintenanceChecklistId,
                    currentUserId,
                    dto.IsCompleted,
                    dto.IsSuccessful,
                    notes,
                    cancellationToken);

            // =====================================
            // 9. GÜNCELLEME SONUCU
            // =====================================

            return updatedResult.Id == existingResult.Id;
        }

        #endregion

        #region HasResult

        /// <summary>
        /// İş emri ve kontrol maddesine ait
        /// sonuç kaydı bulunuyor mu?
        /// </summary>
        public async Task<bool> HasResultAsync(
            int workOrderId,
            int maintenanceChecklistId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            ValidateId(
                maintenanceChecklistId,
                "Kontrol Maddesi");

            var result = await _resultRepository
                .GetByWorkOrderAndChecklistIdAsync(
                    workOrderId,
                    maintenanceChecklistId,
                    cancellationToken);

            return result != null;
        }

        #endregion

        #region HasUncompletedRequiredChecklists

        /// <summary>
        /// İş emrindeki tamamlanmamış zorunlu
        /// kontrol maddelerini kontrol eder.
        /// </summary>
        public async Task<bool> HasUncompletedRequiredChecklistsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            return await _checklistRepository
                .HasUncompletedRequiredChecklistsAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region HasFailedRequiredChecklists

        /// <summary>
        /// İş emrindeki başarısız zorunlu
        /// kontrol maddelerini kontrol eder.
        /// </summary>
        public async Task<bool> HasFailedRequiredChecklistsAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            return await _checklistRepository
                .HasFailedRequiredChecklistsAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region AreRequiredChecklistsSuccessful

        /// <summary>
        /// İş emrindeki zorunlu bakım kontrollerinin
        /// tamamlanıp başarılı sonuçlanmasını kontrol eder.
        ///
        /// Planlı bakıma bağlı olmayan iş emirlerinde
        /// kontrol listesi zorunluluğu uygulanmaz.
        /// </summary>
        public async Task<bool> AreRequiredChecklistsSuccessfulAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. İŞ EMRİ ID DOĞRULAMASI
            // =====================================

            ValidateId(
                workOrderId,
                "İş Emri");

            // =====================================
            // 2. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await _workOrderRepository
                .GetByIdAsync(
                    workOrderId,
                    cancellationToken);

            if (workOrder == null)
            {
                return false;
            }

            // =====================================
            // 3. PLANLI BAKIM KONTROLÜ
            // =====================================

            if (!workOrder.PlannedMaintenanceId.HasValue)
            {
                return true;
            }

            // =====================================
            // 4. AKTİF KONTROL MADDESİ
            // =====================================

            bool hasActiveChecklists =
                await _checklistRepository
                    .HasActiveChecklistsAsync(
                        workOrder.PlannedMaintenanceId.Value,
                        cancellationToken);

            if (!hasActiveChecklists)
            {
                return false;
            }

            // =====================================
            // 5. TAMAMLANMAMIŞ ZORUNLU KONTROLLER
            // =====================================

            bool hasIncomplete =
                await _checklistRepository
                    .HasUncompletedRequiredChecklistsAsync(
                        workOrderId,
                        cancellationToken);

            if (hasIncomplete)
            {
                return false;
            }

            // =====================================
            // 6. BAŞARISIZ ZORUNLU KONTROLLER
            // =====================================

            bool hasFailed =
                await _checklistRepository
                    .HasFailedRequiredChecklistsAsync(
                        workOrderId,
                        cancellationToken);

            if (hasFailed)
            {
                return false;
            }

            // =====================================
            // 7. SONUÇ
            // =====================================

            return true;
        }

        #endregion

        #region GetResultCountByWorkOrderId

        /// <summary>
        /// İş emrindeki toplam sonuç kaydı sayısı.
        /// </summary>
        public async Task<int> GetResultCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return results.Count();
        }

        #endregion

        #region GetCompletedCountByWorkOrderId

        /// <summary>
        /// İş emrinde tamamlanan kontrol sayısı.
        /// </summary>
        public async Task<int> GetCompletedCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetCompletedByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return results.Count();
        }

        #endregion

        #region GetSuccessfulCountByWorkOrderId

        /// <summary>
        /// İş emrindeki başarılı kontrol sayısı.
        /// </summary>
        public async Task<int> GetSuccessfulCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetSuccessfulByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return results.Count();
        }

        #endregion

        #region GetFailedCountByWorkOrderId

        /// <summary>
        /// İş emrindeki başarısız kontrol sayısı.
        /// </summary>
        public async Task<int> GetFailedCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                workOrderId,
                "İş Emri");

            var results = await _resultRepository
                .GetFailedByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return results.Count();
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Entity koleksiyonunu Result DTO listesine dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultWorkOrderChecklistResultDto>
            MapResults(
                IEnumerable<WorkOrderChecklistResult> results)
        {
            return _mapper.Map<
                List<ResultWorkOrderChecklistResultDto>>(
                    results.ToList());
        }

        /// <summary>
        /// İş emrinin mevcut ve devam ediyor
        /// durumda olduğunu doğrular.
        /// </summary>
        private async Task<WorkOrder> GetExecutableWorkOrderAsync(
            int workOrderId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await _workOrderRepository
                .GetByIdAsync(
                    workOrderId,
                    cancellationToken);

            if (workOrder == null)
            {
                throw new InvalidOperationException(
                    "İş emri bulunamadı.");
            }

            // =====================================
            // 2. İŞ EMRİ DURUMU
            // =====================================

            if (workOrder.Status != WorkStatusEnum.InProgress)
            {
                throw new InvalidOperationException(
                    "Kontrol sonucu yalnızca " +
                    "İşe Başlandı durumundaki " +
                    "iş emirlerinde kaydedilebilir.");
            }

            // =====================================
            // 3. PLANLI BAKIM İLİŞKİSİ
            // =====================================

            if (!workOrder.PlannedMaintenanceId.HasValue)
            {
                throw new InvalidOperationException(
                    "Bu iş emri planlı bakım " +
                    "kaydına bağlı değildir.");
            }

            return workOrder;
        }

        /// <summary>
        /// Kontrol maddesinin aktif olduğunu
        /// ve iş emrinin planlı bakımına ait
        /// olduğunu doğrular.
        /// </summary>
        private async Task ValidateChecklistAsync(
            WorkOrder workOrder,
            int maintenanceChecklistId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. KONTROL MADDESİNİ GETİR
            // =====================================

            var checklist = await _checklistRepository
                .GetByIdAsync(
                    maintenanceChecklistId,
                    cancellationToken);

            if (checklist == null)
            {
                throw new InvalidOperationException(
                    "Bakım kontrol maddesi bulunamadı.");
            }

            // =====================================
            // 2. AKTİFLİK KONTROLÜ
            // =====================================

            if (!checklist.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif kontrol maddesine " +
                    "sonuç kaydedilemez.");
            }

            // =====================================
            // 3. PLANLI BAKIM EŞLEŞMESİ
            // =====================================

            if (workOrder.PlannedMaintenanceId !=
                checklist.PlannedMaintenanceId)
            {
                throw new InvalidOperationException(
                    "Seçilen kontrol maddesi " +
                    "iş emrinin bağlı olduğu " +
                    "bakım planına ait değildir.");
            }
        }

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının
        /// aktif, iş emrine atanmış ve yetkili
        /// olduğunu kontrol eder.
        /// </summary>
        private async Task ValidateActorAsync(
            WorkOrder workOrder,
            int currentUserId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. KULLANICIYI GETİR
            // =====================================

            var user = await _userRepository
                .GetByIdAsync(
                    currentUserId,
                    cancellationToken);

            if (user == null ||
                !user.IsActive ||
                !user.HasSystemAccess)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı aktif değildir " +
                    "veya sistem erişimi bulunmamaktadır.");
            }

            // =====================================
            // 2. PERSONEL ATAMASINI KONTROL ET
            // =====================================

            bool isAssigned = await _personnelRepository
                .IsUserAssignedAsync(
                    workOrder.Id,
                    currentUserId,
                    cancellationToken);

            if (!isAssigned)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı bu iş emrine aktif " +
                    "personel olarak atanmamıştır.");
            }

            // =====================================
            // 3. İŞ ALT TÜRÜ YETKİSİNİ KONTROL ET
            // =====================================

            bool hasPermission = await _permissionService
                .HasUserActivePermissionAsync(
                    currentUserId,
                    workOrder.ResponsibleUnitId,
                    workOrder.WorkSubTypeId,
                    ExecutePermission,
                    cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının bu iş alt türünde " +
                    "kontrol sonucu kaydetme " +
                    "yetkisi bulunmamaktadır.");
            }
        }

        /// <summary>
        /// Sonuç durumunun tutarlı olmasını sağlar.
        /// </summary>
        private static void ValidateResult(
            bool isCompleted,
            bool? isSuccessful)
        {
            // =====================================
            // TAMAMLANMADI
            // =====================================

            if (!isCompleted && isSuccessful.HasValue)
            {
                throw new ValidationException(
                    "Tamamlanmamış kontrol için " +
                    "başarılı veya başarısız " +
                    "sonuç bilgisi girilemez.");
            }

            // =====================================
            // TAMAMLANDI
            // =====================================

            if (isCompleted && !isSuccessful.HasValue)
            {
                throw new ValidationException(
                    "Tamamlanan kontrol için " +
                    "başarı durumu belirtilmelidir.");
            }
        }

        /// <summary>
        /// Kontrol notlarını temizler ve
        /// maksimum uzunluğunu doğrular.
        /// </summary>
        private static string? NormalizeNotes(
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
            {
                return null;
            }

            string normalized = notes.Trim();

            if (normalized.Length > MaxNotesLength)
            {
                throw new ValidationException(
                    "Kontrol açıklaması en fazla " +
                    "1000 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// DTO DataAnnotations doğrulaması.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);
        }

        /// <summary>
        /// Pozitif ID doğrulaması.
        /// </summary>
        private static void ValidateId(
            int id,
            string fieldName)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    $"Geçerli bir {fieldName} seçiniz.");
            }
        }

        /// <summary>
        /// UTC tarih aralığı doğrulaması.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ValidationException(
                    "Başlangıç ve bitiş tarihleri " +
                    "UTC formatında olmalıdır.");
            }

            if (endDateUtc < startDateUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç " +
                    "tarihinden önce olamaz.");
            }
        }

        #endregion
    }
}
