using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkSubTypeDtos;
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
    public class WorkSubTypeService : IWorkSubTypeService
    {
        private readonly IWorkSubTypeRepository _workSubTypeRepository;
        private readonly IWorkTypeRepository _workTypeRepository;
        private readonly IMapper _mapper;

        public WorkSubTypeService(
            IWorkSubTypeRepository workSubTypeRepository,
            IWorkTypeRepository workTypeRepository,
            IMapper mapper)
        {
            _workSubTypeRepository = workSubTypeRepository;
            _workTypeRepository = workTypeRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün iş alt türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var workSubTypes = await _workSubTypeRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif iş alt türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var workSubTypes = await _workSubTypeRepository
                .GetActiveWorkSubTypesAsync(cancellationToken);

            return _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);
        }

        #endregion

        #region GetById

        /// <summary>
        /// İş alt türü detaylarını ilişkili
        /// kayıt sayılarıyla getirir.
        /// </summary>
        public async Task<GetByIdWorkSubTypeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var workSubType = await _workSubTypeRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (workSubType == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWorkSubTypeDto>(
                workSubType);
        }

        #endregion

        #region GetByWorkTypeId

        /// <summary>
        /// İş türüne bağlı aktif ve pasif
        /// bütün alt türleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetByWorkTypeIdAsync(
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            var workType = await _workTypeRepository
                .GetByIdAsync(
                    workTypeId,
                    cancellationToken);

            if (workType == null)
            {
                return Array.Empty<ResultWorkSubTypeDto>();
            }

            var workSubTypes = await _workSubTypeRepository
                .GetByWorkTypeIdAsync(
                    workTypeId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);

            // WorkType navigation alanı yüklenmemişse bile
            // listeye iş türü bilgilerini doğru aktar.
            foreach (var item in result)
            {
                item.WorkTypeName = workType.Name;
                item.WorkTypeIsActive = workType.IsActive;
            }

            return result;
        }

        #endregion

        #region GetActiveByWorkTypeId

        /// <summary>
        /// Aktif iş türüne bağlı aktif
        /// iş alt türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
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
                return Array.Empty<ResultWorkSubTypeDto>();
            }

            var workSubTypes = await _workSubTypeRepository
                .GetActiveByWorkTypeIdAsync(
                    workTypeId,
                    cancellationToken);

            var result = _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);

            foreach (var item in result)
            {
                item.WorkTypeName = workType.Name;
                item.WorkTypeIsActive = workType.IsActive;
            }

            return result;
        }

        #endregion

        #region GetAvailableByUnitId

        /// <summary>
        /// Organizasyon biriminde kullanılabilecek
        /// aktif iş alt türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAvailableByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var workSubTypes = await _workSubTypeRepository
                .GetAvailableByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);
        }

        #endregion

        #region GetAvailableByUnitAndWorkType

        /// <summary>
        /// Birim ve iş türüne göre kullanılabilir
        /// aktif iş alt türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAvailableByUnitAndWorkTypeAsync(
                int unitId,
                int workTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workTypeId, "İş Türü");

            var workSubTypes = await _workSubTypeRepository
                .GetAvailableByUnitAndWorkTypeAsync(
                    unitId,
                    workTypeId,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);
        }

        #endregion

        #region GetAuthorizedByRole

        /// <summary>
        /// Rolün ilgili birimde ve işlem yetkisinde
        /// kullanabileceği iş alt türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAuthorizedByRoleAsync(
                int roleId,
                int unitId,
                string permissionCode = "WORK_ORDER_EXECUTE",
                CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");
            ValidateId(unitId, "Organizasyon Birimi");

            string normalizedPermissionCode =
                ValidatePermissionCode(permissionCode);

            var workSubTypes = await _workSubTypeRepository
                .GetAuthorizedByRoleAsync(
                    roleId,
                    unitId,
                    normalizedPermissionCode,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);
        }

        #endregion

        #region GetAuthorizedByUser

        /// <summary>
        /// Kullanıcının ilgili birimde ve işlem
        /// yetkisinde kullanabileceği alt türleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWorkSubTypeDto>>
            GetAuthorizedByUserAsync(
                int userId,
                int unitId,
                string permissionCode = "WORK_ORDER_EXECUTE",
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(unitId, "Organizasyon Birimi");

            string normalizedPermissionCode =
                ValidatePermissionCode(permissionCode);

            var workSubTypes = await _workSubTypeRepository
                .GetAuthorizedByUserAsync(
                    userId,
                    unitId,
                    normalizedPermissionCode,
                    cancellationToken);

            return _mapper.Map<List<ResultWorkSubTypeDto>>(
                workSubTypes);
        }

        #endregion

        #region IsAvailableForUnit

        /// <summary>
        /// Alt türün ilgili organizasyon biriminde
        /// kullanılabilir olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> IsAvailableForUnitAsync(
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workSubTypeId, "İş Alt Türü");

            return await _workSubTypeRepository
                .IsAvailableForUnitAsync(
                    unitId,
                    workSubTypeId,
                    cancellationToken);
        }

        #endregion

        #region CanUserExecute

        /// <summary>
        /// Kullanıcının ilgili birimdeki iş alt türünde
        /// iş emri işlemi gerçekleştirme yetkisini kontrol eder.
        /// </summary>
        public async Task<bool> CanUserExecuteAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workSubTypeId, "İş Alt Türü");

            return await _workSubTypeRepository
                .CanUserExecuteAsync(
                    userId,
                    unitId,
                    workSubTypeId,
                    cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni iş alt türü oluşturur.
        /// İş türü aktif olmalıdır ve aynı iş türü
        /// altında aynı isimle ikinci kayıt oluşturulamaz.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkSubTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.WorkTypeId, "İş Türü");

            string name = ValidateAndNormalizeName(
                dto.Name);

            string? description =
                ValidateAndNormalizeDescription(
                    dto.Description);

            // =====================================
            // 2. İŞ TÜRÜ DOĞRULAMASI
            // =====================================

            await ValidateWorkTypeAsync(
                dto.WorkTypeId,
                cancellationToken);

            // =====================================
            // 3. MÜKERRER KAYIT KONTROLÜ
            // =====================================

            bool existsName = await _workSubTypeRepository
                .ExistsNameAsync(
                    dto.WorkTypeId,
                    name,
                    null,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Seçilen iş türünde aynı isimle " +
                    "başka bir iş alt türü bulunmaktadır.");
            }

            // =====================================
            // 4. DTO -> ENTITY
            // =====================================

            var workSubType =
                _mapper.Map<WorkSubType>(dto);

            workSubType.WorkTypeId = dto.WorkTypeId;
            workSubType.Name = name;
            workSubType.Description = description;

            // Yeni alt tür aktif oluşturulur.
            workSubType.IsActive = true;

            // =====================================
            // 5. KAYIT İŞLEMİ
            // =====================================

            var createdWorkSubType =
                await _workSubTypeRepository
                    .CreateAsync(
                        workSubType,
                        cancellationToken);

            return createdWorkSubType.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// İş alt türünün adını ve açıklamasını günceller.
        /// İş türü ilişkisi normal güncellemede korunur.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkSubTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.Id, "İş Alt Türü");
            ValidateId(dto.WorkTypeId, "İş Türü");

            string name = ValidateAndNormalizeName(
                dto.Name);

            string? description =
                ValidateAndNormalizeDescription(
                    dto.Description);

            // =====================================
            // 2. MEVCUT KAYDI GETİR
            // =====================================

            var existingWorkSubType =
                await _workSubTypeRepository
                    .GetByIdAsync(
                        dto.Id,
                        cancellationToken);

            if (existingWorkSubType == null)
            {
                return false;
            }

            // =====================================
            // 3. İŞ TÜRÜ İLİŞKİSİNİ KORU
            // =====================================

            if (existingWorkSubType.WorkTypeId != dto.WorkTypeId)
            {
                throw new InvalidOperationException(
                    "İş alt türünün bağlı olduğu iş türü " +
                    "normal güncelleme işleminde değiştirilemez. " +
                    "Farklı bir iş türü için yeni alt tür " +
                    "oluşturulmalıdır.");
            }

            // =====================================
            // 4. İŞ TÜRÜ DOĞRULAMASI
            // =====================================

            await ValidateWorkTypeAsync(
                existingWorkSubType.WorkTypeId,
                cancellationToken);

            // =====================================
            // 5. BENZERSİZLİK KONTROLÜ
            // =====================================

            bool existsName = await _workSubTypeRepository
                .ExistsNameAsync(
                    existingWorkSubType.WorkTypeId,
                    name,
                    existingWorkSubType.Id,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu iş türü altında aynı isimli " +
                    "başka bir iş alt türü bulunmaktadır.");
            }

            // =====================================
            // 6. DTO -> MEVCUT ENTITY
            // =====================================

            _mapper.Map(dto, existingWorkSubType);

            existingWorkSubType.Name = name;
            existingWorkSubType.Description = description;

            // Id, WorkTypeId, IsActive, tarih
            // alanları ve navigation koleksiyonları korunur.

            // =====================================
            // 7. GÜNCELLEME İŞLEMİ
            // =====================================

            return await _workSubTypeRepository
                .UpdateAsync(
                    existingWorkSubType,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// İş alt türünün aktiflik durumunu değiştirir.
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

            var workSubType = await _workSubTypeRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (workSubType == null)
            {
                return false;
            }

            // Durum zaten aynıysa işlem yapma.
            if (workSubType.IsActive == isActive)
            {
                return true;
            }

            if (isActive)
            {
                // Pasif alt tür yeniden açılırken
                // bağlı iş türü aktif olmalıdır.
                await ValidateWorkTypeAsync(
                    workSubType.WorkTypeId,
                    cancellationToken);
            }
            else
            {
                // Açık süreçler veya aktif yetkiler
                // varsa alt tür pasife alınamaz.
                await ValidateCanDeactivateAsync(
                    workSubType.Id,
                    cancellationToken);
            }

            return await _workSubTypeRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// İş türü bazında alt tür adı
        /// benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            int workTypeId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workTypeId, "İş Türü");

            string normalizedName =
                ValidateAndNormalizeName(name);

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak İş Alt Türü");
            }

            return await _workSubTypeRepository
                .ExistsNameAsync(
                    workTypeId,
                    normalizedName,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasWorkRequests

        /// <summary>
        /// Alt türe bağlı iş talebi var mı?
        /// </summary>
        public async Task<bool> HasWorkRequestsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            return await _workSubTypeRepository
                .HasWorkRequestsAsync(
                    workSubTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasNewWorkRequests

        /// <summary>
        /// Alt türe bağlı yeni iş talebi var mı?
        /// </summary>
        public async Task<bool> HasNewWorkRequestsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            return await _workSubTypeRepository
                .HasNewWorkRequestsAsync(
                    workSubTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasOpenWorkOrders

        /// <summary>
        /// Alt türe bağlı açık iş emri var mı?
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            return await _workSubTypeRepository
                .HasOpenWorkOrdersAsync(
                    workSubTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveMaintenancePlans

        /// <summary>
        /// Alt türe bağlı aktif bakım planı var mı?
        /// </summary>
        public async Task<bool> HasActiveMaintenancePlansAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            return await _workSubTypeRepository
                .HasActiveMaintenancePlansAsync(
                    workSubTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveRolePermissions

        /// <summary>
        /// Alt türe bağlı aktif rol yetkisi var mı?
        /// </summary>
        public async Task<bool> HasActiveRolePermissionsAsync(
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            return await _workSubTypeRepository
                .HasActiveRolePermissionsAsync(
                    workSubTypeId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations
        /// kurallarını çalıştırır.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            var validationContext =
                new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                validationContext,
                validateAllProperties: true);
        }

        /// <summary>
        /// ID değerinin pozitif olduğunu kontrol eder.
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
        /// İş alt türü adını doğrular ve normalize eder.
        /// </summary>
        private static string ValidateAndNormalizeName(
            string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "İş alt türü adı zorunludur.");
            }

            string normalizedName = name.Trim();

            if (normalizedName.Length > 150)
            {
                throw new ValidationException(
                    "İş alt türü adı en fazla 150 " +
                    "karakter olabilir.");
            }

            return normalizedName;
        }

        /// <summary>
        /// Açıklama bilgisini doğrular ve temizler.
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
                    "İş alt türü açıklaması en fazla " +
                    "500 karakter olabilir.");
            }

            return normalizedDescription;
        }

        /// <summary>
        /// Yetki kodunun boş olmamasını sağlar.
        /// </summary>
        private static string ValidatePermissionCode(
            string? permissionCode)
        {
            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                throw new ValidationException(
                    "İşlem yetkisi kodu zorunludur.");
            }

            return permissionCode.Trim();
        }

        /// <summary>
        /// İş türünün mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateWorkTypeAsync(
            int workTypeId,
            CancellationToken cancellationToken)
        {
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
                    "Pasif iş türüne bağlı iş alt türü " +
                    "oluşturulamaz veya aktif hale getirilemez.");
            }
        }

        /// <summary>
        /// İş alt türünün pasife alınmasını engelleyen
        /// açık süreçleri ve aktif yetkileri kontrol eder.
        /// </summary>
        private async Task ValidateCanDeactivateAsync(
            int workSubTypeId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. YENİ İŞ TALEPLERİ
            // =====================================

            bool hasNewWorkRequests =
                await _workSubTypeRepository
                    .HasNewWorkRequestsAsync(
                        workSubTypeId,
                        cancellationToken);

            if (hasNewWorkRequests)
            {
                throw new InvalidOperationException(
                    "Bu iş alt türüne bağlı yeni iş talepleri " +
                    "bulunmaktadır. Öncelikle ilgili talepler " +
                    "sonuçlandırılmalıdır.");
            }

            // =====================================
            // 2. AÇIK İŞ EMİRLERİ
            // =====================================

            bool hasOpenWorkOrders =
                await _workSubTypeRepository
                    .HasOpenWorkOrdersAsync(
                        workSubTypeId,
                        cancellationToken);

            if (hasOpenWorkOrders)
            {
                throw new InvalidOperationException(
                    "Bu iş alt türüne bağlı açık iş emirleri " +
                    "bulunmaktadır. İş alt türü pasife alınamaz.");
            }

            // =====================================
            // 3. AKTİF BAKIM PLANLARI
            // =====================================

            bool hasActiveMaintenancePlans =
                await _workSubTypeRepository
                    .HasActiveMaintenancePlansAsync(
                        workSubTypeId,
                        cancellationToken);

            if (hasActiveMaintenancePlans)
            {
                throw new InvalidOperationException(
                    "Bu iş alt türüne bağlı aktif bakım " +
                    "planları bulunmaktadır. Öncelikle " +
                    "bakım planları sonlandırılmalıdır.");
            }

            // =====================================
            // 4. AKTİF ROL YETKİLERİ
            // =====================================

            bool hasActiveRolePermissions =
                await _workSubTypeRepository
                    .HasActiveRolePermissionsAsync(
                        workSubTypeId,
                        cancellationToken);

            if (hasActiveRolePermissions)
            {
                throw new InvalidOperationException(
                    "Bu iş alt türü için aktif rol " +
                    "yetkilendirmeleri bulunmaktadır. " +
                    "Öncelikle ilgili yetkileri pasife alınız.");
            }
        }

        #endregion
    }
}
