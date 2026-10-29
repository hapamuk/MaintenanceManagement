using AutoMapper;
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
    public class WorkTypeService : IWorkTypeService
    {
        private readonly IWorkTypeRepository _workTypeRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public WorkTypeService(
            IWorkTypeRepository workTypeRepository,
            IUnitRepository unitRepository,
            IMapper mapper)
        {
            _workTypeRepository = workTypeRepository;
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün iş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkTypeDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var workTypes = await _workTypeRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkTypeDto>>(
                workTypes);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Yalnızca aktif iş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkTypeDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var workTypes = await _workTypeRepository
                .GetActiveWorkTypesAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkTypeDto>>(
                workTypes);
        }

        #endregion

        #region GetById

        /// <summary>
        /// İş türünün detaylarını, alt tür ve
        /// organizasyon birimi atama sayılarını getirir.
        /// </summary>
        public async Task<GetByIdWorkTypeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var workType = await _workTypeRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (workType == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWorkTypeDto>(
                workType);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Organizasyon birimine tanımlı aktif
        /// ve pasif bütün iş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkTypeDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Organizasyon Birimi");

            var workTypes = await _workTypeRepository
                .GetWorkTypesByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkTypeDto>>(
                workTypes);
        }

        #endregion

        #region GetActiveByUnitId

        /// <summary>
        /// Aktif organizasyon birimine tanımlı
        /// kullanılabilir iş türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkTypeDto>>
            GetActiveByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Organizasyon Birimi");

            var unit = await _unitRepository
                .GetByIdAsync(
                    unitId,
                    cancellationToken);

            if (unit == null || !unit.IsActive)
            {
                return Array.Empty<ResultWorkTypeDto>();
            }

            var workTypes = await _workTypeRepository
                .GetActiveWorkTypesByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkTypeDto>>(
                workTypes);
        }

        #endregion

        #region GetWorkSubTypes

        /// <summary>
        /// İş türüne bağlı aktif ve pasif
        /// bütün alt türleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetWorkSubTypesAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workTypeId,
                "İş Türü");

            var workType = await _workTypeRepository
                .GetByIdAsync(
                    workTypeId,
                    cancellationToken);

            if (workType == null)
            {
                return Array.Empty<ResultWorkSubTypeDto>();
            }

            var workSubTypes = await _workTypeRepository
                .GetWorkSubTypesByWorkTypeIdAsync(
                    workTypeId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);

            // WorkType navigation özelliği yüklenmemiş
            // olsa bile DTO üzerinde iş türü adını doldur.
            foreach (var item in result)
            {
                item.WorkTypeName = workType.Name;
            }

            return result;
        }

        #endregion

        #region GetActiveWorkSubTypes

        /// <summary>
        /// Aktif iş türüne bağlı aktif alt türleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetActiveWorkSubTypesAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                workTypeId,
                "İş Türü");

            var workType = await _workTypeRepository
                .GetByIdAsync(
                    workTypeId,
                    cancellationToken);

            if (workType == null || !workType.IsActive)
            {
                return Array.Empty<ResultWorkSubTypeDto>();
            }

            var workSubTypes = await _workTypeRepository
                .GetActiveWorkSubTypesByWorkTypeIdAsync(
                    workTypeId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);

            foreach (var item in result)
            {
                item.WorkTypeName = workType.Name;
            }

            return result;
        }

        #endregion

        #region GetAvailableWorkSubTypesByUnitAndType

        /// <summary>
        /// Organizasyon biriminde kullanılabilecek
        /// iş alt türlerini getirir.
        ///
        /// UnitWorkType ilişkisi üzerinden filtreleme yapılır.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAvailableWorkSubTypesByUnitAndTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Organizasyon Birimi");

            ValidateId(
                workTypeId,
                "İş Türü");

            // Organizasyon birimi aktif olmalıdır.
            var unit = await _unitRepository
                .GetByIdAsync(
                    unitId,
                    cancellationToken);

            if (unit == null || !unit.IsActive)
            {
                return Array.Empty<ResultWorkSubTypeDto>();
            }

            // İş türü aktif olmalıdır.
            var workType = await _workTypeRepository
                .GetByIdAsync(
                    workTypeId,
                    cancellationToken);

            if (workType == null || !workType.IsActive)
            {
                return Array.Empty<ResultWorkSubTypeDto>();
            }

            // Repository, aktif UnitWorkType bağlantısını
            // ve aktif WorkSubType kayıtlarını filtreler.
            var workSubTypes = await _workTypeRepository
                .GetAvailableWorkSubTypesByUnitAndTypeAsync(
                    unitId,
                    workTypeId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);

            foreach (var item in result)
            {
                item.WorkTypeName = workType.Name;
            }

            return result;
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni iş türü oluşturur.
        /// Aynı isimde ikinci bir iş türü oluşturulamaz.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // 1. DTO doğrulaması
            ValidateDto(dto);

            // 2. İş türü adını düzenle
            string name = ValidateAndNormalizeName(
                dto.Name);

            // 3. Açıklama alanını düzenle
            string? description = ValidateAndNormalizeDescription(
                dto.Description);

            // 4. İş türü adı benzersizlik kontrolü
            bool existsName = await _workTypeRepository
                .ExistsNameAsync(
                    name,
                    null,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu isimde bir iş türü daha önce " +
                    "oluşturulmuştur.");
            }

            // 5. DTO -> Entity
            var workType = _mapper.Map<WorkType>(dto);

            workType.Name = name;
            workType.Description = description;

            // 6. Yeni iş türü aktif oluşturulur.
            workType.IsActive = true;

            // 7. Kayıt işlemi
            var createdWorkType = await _workTypeRepository
                .CreateAsync(
                    workType,
                    cancellationToken);

            return createdWorkType.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// İş türünün adını ve açıklamasını günceller.
        /// İlişkili alt tür ve birim atamaları korunur.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // 1. DTO doğrulaması
            ValidateDto(dto);

            ValidateId(
                dto.Id,
                "İş Türü");

            // 2. Alanları düzenle
            string name = ValidateAndNormalizeName(
                dto.Name);

            string? description = ValidateAndNormalizeDescription(
                dto.Description);

            // 3. Mevcut iş türünü getir
            var existingWorkType = await _workTypeRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingWorkType == null)
            {
                return false;
            }

            // 4. Benzersizlik kontrolü
            // Kendi ID değerini sorgudan hariç tut.
            bool existsName = await _workTypeRepository
                .ExistsNameAsync(
                    name,
                    existingWorkType.Id,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu iş türü adı başka bir kayıt " +
                    "tarafından kullanılmaktadır.");
            }

            // 5. DTO -> Mevcut Entity
            _mapper.Map(dto, existingWorkType);

            existingWorkType.Name = name;
            existingWorkType.Description = description;

            // Id, IsActive, CreatedAt ve navigation
            // koleksiyonları AutoMapper ile korunur.

            // 6. Güncelleme işlemi
            return await _workTypeRepository
                .UpdateAsync(
                    existingWorkType,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// İş türünün aktiflik durumunu değiştirir.
        ///
        /// Aktif alt tür, açık iş emri veya aktif bakım
        /// planı bulunduğunda pasife alma engellenir.
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

            // 1. Mevcut iş türünü getir.
            var existingWorkType = await _workTypeRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingWorkType == null)
            {
                return false;
            }

            // 2. Durum zaten aynıysa yeniden işlem yapma.
            if (existingWorkType.IsActive == isActive)
            {
                return true;
            }

            // 3. Pasife alma kontrolleri
            if (!isActive)
            {
                // Aktif iş alt türü kontrolü
                bool hasActiveWorkSubTypes =
                    await _workTypeRepository
                        .HasActiveWorkSubTypesAsync(
                            id,
                            cancellationToken);

                if (hasActiveWorkSubTypes)
                {
                    throw new InvalidOperationException(
                        "İş türüne bağlı aktif iş alt türleri " +
                        "bulunmaktadır. Öncelikle ilgili " +
                        "alt türleri pasife alınız.");
                }

                // Açık iş emri kontrolü
                bool hasOpenWorkOrders =
                    await _workTypeRepository
                        .HasOpenWorkOrdersAsync(
                            id,
                            cancellationToken);

                if (hasOpenWorkOrders)
                {
                    throw new InvalidOperationException(
                        "Bu iş türüne bağlı açık iş emirleri " +
                        "bulunduğu için pasife alınamaz.");
                }

                // Aktif planlı bakım kontrolü
                bool hasActiveMaintenancePlans =
                    await _workTypeRepository
                        .HasActiveMaintenancePlansAsync(
                            id,
                            cancellationToken);

                if (hasActiveMaintenancePlans)
                {
                    throw new InvalidOperationException(
                        "Bu iş türüne bağlı aktif bakım " +
                        "planları bulunmaktadır. Öncelikle " +
                        "bakım planlarını sonlandırınız.");
                }
            }

            // 4. Durum değişikliği
            return await _workTypeRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// İş türü adının benzersiz olup
        /// olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedName = ValidateAndNormalizeName(
                name);

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak İş Türü");
            }

            return await _workTypeRepository
                .ExistsNameAsync(
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasWorkSubTypes

        /// <summary>
        /// İş türüne bağlı alt tür bulunuyor mu?
        /// </summary>
        public async Task<bool> HasWorkSubTypesAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            return await _workTypeRepository
                .HasWorkSubTypesAsync(
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveWorkSubTypes

        /// <summary>
        /// İş türüne bağlı aktif alt tür bulunuyor mu?
        /// </summary>
        public async Task<bool> HasActiveWorkSubTypesAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            return await _workTypeRepository
                .HasActiveWorkSubTypesAsync(
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasUnitAssignments

        /// <summary>
        /// İş türünün organizasyon birimi
        /// ataması bulunuyor mu?
        /// </summary>
        public async Task<bool> HasUnitAssignmentsAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            return await _workTypeRepository
                .HasUnitAssignmentsAsync(
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasWorkRequests

        /// <summary>
        /// İş türüne bağlı iş talebi var mı?
        /// </summary>
        public async Task<bool> HasWorkRequestsAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            return await _workTypeRepository
                .HasWorkRequestsAsync(
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasOpenWorkOrders

        /// <summary>
        /// İş türüne bağlı açık iş emri var mı?
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            return await _workTypeRepository
                .HasOpenWorkOrdersAsync(
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveMaintenancePlans

        /// <summary>
        /// İş türüne bağlı aktif bakım planı var mı?
        /// </summary>
        public async Task<bool> HasActiveMaintenancePlansAsync(
            int workTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            return await _workTypeRepository
                .HasActiveMaintenancePlansAsync(
                    workTypeId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations
        /// doğrulamalarını çalıştırır.
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
        /// ID değerinin geçerli olduğunu doğrular.
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
        /// İş türü adını doğrular ve normalize eder.
        /// </summary>
        private static string ValidateAndNormalizeName(
            string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "İş türü adı zorunludur.");
            }

            string normalizedName = name.Trim();

            if (normalizedName.Length > 150)
            {
                throw new ValidationException(
                    "İş türü adı en fazla 150 " +
                    "karakter olabilir.");
            }

            return normalizedName;
        }

        /// <summary>
        /// Açıklama alanını kontrol eder ve normalize eder.
        /// </summary>
        private static string? ValidateAndNormalizeDescription(
            string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            string normalizedDescription =
                description.Trim();

            if (normalizedDescription.Length > 500)
            {
                throw new ValidationException(
                    "İş türü açıklaması en fazla " +
                    "500 karakter olabilir.");
            }

            return normalizedDescription;
        }

        #endregion
    }
}
