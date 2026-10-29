using AutoMapper;
using MaintenanceManagement.Application.DTOS.UnitWorkTypeDtos;
using MaintenanceManagement.Application.DTOS.WorkSubTypeDtos;
using MaintenanceManagement.Application.DTOS.WorkTypeDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class UnitWorkTypeService : IUnitWorkTypeService
    {
        private readonly IUnitWorkTypeRepository _unitWorkTypeRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IWorkTypeRepository _workTypeRepository;
        private readonly IMapper _mapper;

        public UnitWorkTypeService(
            IUnitWorkTypeRepository unitWorkTypeRepository,
            IUnitRepository unitRepository,
            IWorkTypeRepository workTypeRepository,
            IMapper mapper)
        {
            _unitWorkTypeRepository = unitWorkTypeRepository;
            _unitRepository = unitRepository;
            _workTypeRepository = workTypeRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün atamaları
        /// birim ve iş türü bilgileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var assignments = await _unitWorkTypeRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultUnitWorkTypeDto>>(
                assignments);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Belirtilen ID değerine ait atama detayını getirir.
        /// </summary>
        public async Task<GetByIdUnitWorkTypeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var assignment = await _unitWorkTypeRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdUnitWorkTypeDto>(
                assignment);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Birime ait bütün iş türü atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var assignments = await _unitWorkTypeRepository
                .GetByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultUnitWorkTypeDto>>(
                assignments);
        }

        #endregion

        #region GetActiveByUnitId

        /// <summary>
        /// Organizasyon biriminin aktif
        /// iş türü atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetActiveByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var unit = await _unitRepository
                .GetByIdAsync(
                    unitId,
                    cancellationToken);

            if (unit == null || !unit.IsActive)
            {
                return Array.Empty<ResultUnitWorkTypeDto>();
            }

            var assignments = await _unitWorkTypeRepository
                .GetActiveByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultUnitWorkTypeDto>>(
                assignments);
        }

        #endregion

        #region GetByWorkTypeId

        /// <summary>
        /// İş türünün atandığı bütün
        /// organizasyon birimlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            var assignments = await _unitWorkTypeRepository
                .GetByWorkTypeIdAsync(
                    workTypeId,
                    cancellationToken);

            return _mapper.Map<List<ResultUnitWorkTypeDto>>(
                assignments);
        }

        #endregion

        #region GetActiveByWorkTypeId

        /// <summary>
        /// İş türüne ait aktif birim atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitWorkTypeDto>>
            GetActiveByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            var workType = await _workTypeRepository
                .GetByIdAsync(
                    workTypeId,
                    cancellationToken);

            if (workType == null || !workType.IsActive)
            {
                return Array.Empty<ResultUnitWorkTypeDto>();
            }

            var assignments = await _unitWorkTypeRepository
                .GetActiveByWorkTypeIdAsync(
                    workTypeId,
                    cancellationToken);

            return _mapper.Map<List<ResultUnitWorkTypeDto>>(
                assignments);
        }

        #endregion

        #region GetByUnitAndWorkType

        /// <summary>
        /// Belirtilen birim ve iş türü arasındaki
        /// atamanın detaylarını getirir.
        /// </summary>
        public async Task<GetByIdUnitWorkTypeDto?>
            GetByUnitAndWorkTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workTypeId, "İş Türü");

            var assignment = await _unitWorkTypeRepository
                .GetByUnitAndWorkTypeAsync(
                    unitId,
                    workTypeId,
                    cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            // Detay DTO'sunda ilişkisel alanların
            // eksiksiz doldurulması için detay sorgusu.
            return await GetByIdAsync(
                assignment.Id,
                cancellationToken);
        }

        #endregion

        #region GetActiveWorkTypesByUnitId

        /// <summary>
        /// Organizasyon biriminde kullanılabilecek
        /// aktif iş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkTypeDto>>
            GetActiveWorkTypesByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var unit = await _unitRepository
                .GetByIdAsync(
                    unitId,
                    cancellationToken);

            if (unit == null || !unit.IsActive)
            {
                return Array.Empty<ResultWorkTypeDto>();
            }

            // Repository aşağıdaki durumları kontrol eder:
            //
            // Unit.IsActive
            // WorkType.IsActive
            // UnitWorkType.IsActive

            var workTypes = await _unitWorkTypeRepository
                .GetActiveWorkTypesByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkTypeDto>>(
                workTypes);
        }

        #endregion

        #region GetAvailableWorkSubTypesByUnitId

        /// <summary>
        /// Organizasyon biriminde kullanılabilen
        /// aktif iş alt türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAvailableWorkSubTypesByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var unit = await _unitRepository
                .GetByIdAsync(
                    unitId,
                    cancellationToken);

            if (unit == null || !unit.IsActive)
            {
                return Array.Empty<ResultWorkSubTypeDto>();
            }

            // İş alt türleri doğrudan takıma bağlanmaz.
            //
            // Uygunluk:
            // Unit -> UnitWorkType -> WorkType -> WorkSubType

            var workSubTypes = await _unitWorkTypeRepository
                .GetAvailableWorkSubTypesByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);
        }

        #endregion

        #region Create

        /// <summary>
        /// Organizasyon birimine yeni iş türü ataması oluşturur.
        /// Aynı birim-iş türü ikilisi tekrar oluşturulamaz.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateUnitWorkTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // ======================================
            // 1. DTO DOĞRULAMASI
            // ======================================

            ValidateDto(dto);

            ValidateId(dto.UnitId, "Organizasyon Birimi");
            ValidateId(dto.WorkTypeId, "İş Türü");

            // ======================================
            // 2. BİRİM VE İŞ TÜRÜ DOĞRULAMASI
            // ======================================

            await ValidateRelationsAsync(
                dto.UnitId,
                dto.WorkTypeId,
                cancellationToken);

            // ======================================
            // 3. MÜKERRER ATAMA KONTROLÜ
            // ======================================

            bool existsAssignment = await _unitWorkTypeRepository
                .ExistsAssignmentAsync(
                    dto.UnitId,
                    dto.WorkTypeId,
                    null,
                    cancellationToken);

            if (existsAssignment)
            {
                throw new InvalidOperationException(
                    "Bu iş türü organizasyon birimine " +
                    "daha önce atanmıştır. Atama pasifse " +
                    "mevcut kayıt yeniden aktif yapılmalıdır.");
            }

            // ======================================
            // 4. DTO -> ENTITY
            // ======================================

            var assignment = _mapper.Map<UnitWorkType>(dto);

            assignment.UnitId = dto.UnitId;
            assignment.WorkTypeId = dto.WorkTypeId;

            // Yeni atama aktif oluşturulur.
            assignment.IsActive = true;

            // ======================================
            // 5. KAYIT İŞLEMİ
            // ======================================

            var createdAssignment = await _unitWorkTypeRepository
                .CreateAsync(
                    assignment,
                    cancellationToken);

            return createdAssignment.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut birim-iş türü atamasını günceller.
        ///
        /// Atama ilişkisi değişecekse eski birim-iş türü
        /// için bekleyen talep veya açık iş emri bulunamaz.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateUnitWorkTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // ======================================
            // 1. DTO DOĞRULAMASI
            // ======================================

            ValidateDto(dto);

            ValidateId(dto.Id, "Atama");
            ValidateId(dto.UnitId, "Organizasyon Birimi");
            ValidateId(dto.WorkTypeId, "İş Türü");

            // ======================================
            // 2. MEVCUT ATAMAYI GETİR
            // ======================================

            var existingAssignment = await _unitWorkTypeRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingAssignment == null)
            {
                return false;
            }

            bool relationChanged =
                existingAssignment.UnitId != dto.UnitId ||
                existingAssignment.WorkTypeId != dto.WorkTypeId;

            // İlişkide değişiklik yoksa
            // herhangi bir güncelleme yapma.
            if (!relationChanged)
            {
                return true;
            }

            // ======================================
            // 3. ESKİ İLİŞKİNİN KULLANIMINI KONTROL ET
            // ======================================

            await ValidateCanDeactivateAsync(
                existingAssignment.UnitId,
                existingAssignment.WorkTypeId,
                cancellationToken);

            // ======================================
            // 4. YENİ İLİŞKİYİ DOĞRULA
            // ======================================

            await ValidateRelationsAsync(
                dto.UnitId,
                dto.WorkTypeId,
                cancellationToken);

            // ======================================
            // 5. MÜKERRERLİK KONTROLÜ
            // ======================================

            bool existsAssignment = await _unitWorkTypeRepository
                .ExistsAssignmentAsync(
                    dto.UnitId,
                    dto.WorkTypeId,
                    existingAssignment.Id,
                    cancellationToken);

            if (existsAssignment)
            {
                throw new InvalidOperationException(
                    "Seçilen organizasyon birimi ve " +
                    "iş türü arasında başka bir atama " +
                    "kaydı bulunmaktadır.");
            }

            // ======================================
            // 6. DTO -> MEVCUT ENTITY
            // ======================================

            _mapper.Map(dto, existingAssignment);

            existingAssignment.UnitId = dto.UnitId;
            existingAssignment.WorkTypeId = dto.WorkTypeId;

            // Id, IsActive, CreatedAt ve navigation
            // özellikleri AutoMapper tarafından korunur.

            // ======================================
            // 7. GÜNCELLE
            // ======================================

            return await _unitWorkTypeRepository
                .UpdateAsync(
                    existingAssignment,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Birim-iş türü atamasının aktiflik durumunu değiştirir.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            // ======================================
            // 1. ATAMAYI GETİR
            // ======================================

            var assignment = await _unitWorkTypeRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (assignment == null)
            {
                return false;
            }

            // Durum zaten istenilen değerdeyse
            // yeniden kaydetme.
            if (assignment.IsActive == isActive)
            {
                return true;
            }

            if (isActive)
            {
                // ==================================
                // 2. AKTİF HALE GETİRME
                // ==================================

                // Birim ve iş türü aktif olmalıdır.
                await ValidateRelationsAsync(
                    assignment.UnitId,
                    assignment.WorkTypeId,
                    cancellationToken);

                // Diğer kayıtlarla çakışma olmamalıdır.
                bool existsAssignment =
                    await _unitWorkTypeRepository
                        .ExistsAssignmentAsync(
                            assignment.UnitId,
                            assignment.WorkTypeId,
                            assignment.Id,
                            cancellationToken);

                if (existsAssignment)
                {
                    throw new InvalidOperationException(
                        "Bu birim ve iş türü için başka " +
                        "bir atama kaydı bulunmaktadır.");
                }
            }
            else
            {
                // ==================================
                // 3. PASİFE ALMA
                // ==================================

                // Bekleyen iş talepleri veya
                // açık iş emirleri varsa engelle.
                await ValidateCanDeactivateAsync(
                    assignment.UnitId,
                    assignment.WorkTypeId,
                    cancellationToken);
            }

            // ======================================
            // 4. DURUMU GÜNCELLE
            // ======================================

            return await _unitWorkTypeRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsAssignment

        /// <summary>
        /// Aynı birim ve iş türü eşleştirmesinin
        /// mevcut olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsAssignmentAsync(
            int unitId,
            int workTypeId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workTypeId, "İş Türü");

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak Atama");
            }

            return await _unitWorkTypeRepository
                .ExistsAssignmentAsync(
                    unitId,
                    workTypeId,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region IsWorkTypeAvailableForUnit

        /// <summary>
        /// İş türünün belirtilen birimde
        /// kullanılabilir olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> IsWorkTypeAvailableForUnitAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workTypeId, "İş Türü");

            return await _unitWorkTypeRepository
                .IsWorkTypeAvailableForUnitAsync(
                    unitId,
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasPendingWorkRequests

        /// <summary>
        /// Birim ve iş türüne ait bekleyen
        /// iş taleplerini kontrol eder.
        /// </summary>
        public async Task<bool> HasPendingWorkRequestsAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workTypeId, "İş Türü");

            return await _unitWorkTypeRepository
                .HasPendingWorkRequestsAsync(
                    unitId,
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasOpenWorkOrders

        /// <summary>
        /// Birim ve iş türüne ait açık
        /// iş emirlerini kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workTypeId, "İş Türü");

            return await _unitWorkTypeRepository
                .HasOpenWorkOrdersAsync(
                    unitId,
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations
        /// doğrulamalarını gerçekleştirir.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            var validationContext = new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                validationContext,
                validateAllProperties: true);
        }

        /// <summary>
        /// ID değerinin geçerli olduğunu kontrol eder.
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
        /// Organizasyon biriminin ve iş türünün
        /// mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateRelationsAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken)
        {
            // ======================================
            // 1. ORGANİZASYON BİRİMİ
            // ======================================

            var unit = await _unitRepository
                .GetByIdAsync(
                    unitId,
                    cancellationToken);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    "Seçilen organizasyon birimi bulunamadı.");
            }

            if (!unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif organizasyon birimine " +
                    "iş türü atanamaz.");
            }

            // ======================================
            // 2. İŞ TÜRÜ
            // ======================================

            var workType = await _workTypeRepository
                .GetByIdAsync(
                    workTypeId,
                    cancellationToken);

            if (workType == null)
            {
                throw new InvalidOperationException(
                    "Seçilen iş türü bulunamadı.");
            }

            if (!workType.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif iş türü organizasyon " +
                    "birimine atanamaz.");
            }
        }

        /// <summary>
        /// Birim-iş türü bağlantısının pasife
        /// alınmasına engel açık işlem var mı kontrol eder.
        /// </summary>
        private async Task ValidateCanDeactivateAsync(
            int unitId,
            int workTypeId,
            CancellationToken cancellationToken)
        {
            // ======================================
            // 1. BEKLEYEN İŞ TALEPLERİ
            // ======================================

            bool hasPendingRequests =
                await _unitWorkTypeRepository
                    .HasPendingWorkRequestsAsync(
                        unitId,
                        workTypeId,
                        cancellationToken);

            if (hasPendingRequests)
            {
                throw new InvalidOperationException(
                    "Bu organizasyon birimi ve iş türüne " +
                    "ait bekleyen iş talepleri bulunmaktadır. " +
                    "Atama pasife alınmadan önce talepler " +
                    "sonuçlandırılmalıdır.");
            }

            // ======================================
            // 2. AÇIK İŞ EMİRLERİ
            // ======================================

            bool hasOpenWorkOrders =
                await _unitWorkTypeRepository
                    .HasOpenWorkOrdersAsync(
                        unitId,
                        workTypeId,
                        cancellationToken);

            if (hasOpenWorkOrders)
            {
                throw new InvalidOperationException(
                    "Bu organizasyon birimi ve iş türüne " +
                    "ait açık iş emirleri bulunmaktadır. " +
                    "Atama pasife alınmadan önce ilgili " +
                    "iş emirleri sonuçlandırılmalıdır.");
            }
        }

        #endregion
    }
}
