using AutoMapper;
using MaintenanceManagement.Application.DTOS.RoleWorkSubTypePermissionDtos;
using MaintenanceManagement.Application.Interfaces;
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
    public class RoleWorkSubTypePermissionService
         : IRoleWorkSubTypePermissionService
    {
        private readonly IRoleWorkSubTypePermissionRepository
            _roleWorkSubTypePermissionRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IWorkSubTypeRepository _workSubTypeRepository;

        private readonly IPermissionRepository _permissionRepository;

        private readonly IMapper _mapper;

        public RoleWorkSubTypePermissionService(
            IRoleWorkSubTypePermissionRepository
                roleWorkSubTypePermissionRepository,
            IRoleRepository roleRepository,
            IWorkSubTypeRepository workSubTypeRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper)
        {
            _roleWorkSubTypePermissionRepository =
                roleWorkSubTypePermissionRepository;

            _roleRepository = roleRepository;
            _workSubTypeRepository = workSubTypeRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün atamaları
        /// ilişkili bilgilerle birlikte getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var assignments =
                await _roleWorkSubTypePermissionRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            return _mapper.Map<
                List<ResultRoleWorkSubTypePermissionDto>>(
                    assignments);
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID bilgisine göre atama detayını getirir.
        /// </summary>
        public async Task<GetByIdRoleWorkSubTypePermissionDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var assignment =
                await _roleWorkSubTypePermissionRepository
                    .GetByIdWithDetailsAsync(
                        id,
                        cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            return _mapper.Map<
                GetByIdRoleWorkSubTypePermissionDto>(
                    assignment);
        }

        #endregion

        #region GetByRoleId

        /// <summary>
        /// Belirtilen role ait bütün yetki
        /// atamalarını getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");

            var assignments =
                await _roleWorkSubTypePermissionRepository
                    .GetByRoleIdAsync(
                        roleId,
                        cancellationToken);

            return _mapper.Map<
                List<ResultRoleWorkSubTypePermissionDto>>(
                    assignments);
        }

        #endregion

        #region GetActiveByRoleId

        /// <summary>
        /// Belirtilen role ait aktif iş alt türü
        /// yetki atamalarını getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetActiveByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");

            var assignments =
                await _roleWorkSubTypePermissionRepository
                    .GetActiveByRoleIdAsync(
                        roleId,
                        cancellationToken);

            return _mapper.Map<
                List<ResultRoleWorkSubTypePermissionDto>>(
                    assignments);
        }

        #endregion

        #region GetByWorkSubTypeId

        /// <summary>
        /// Seçilen iş alt türüne ait bütün
        /// rol ve yetki atamalarını getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetByWorkSubTypeIdAsync(
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workSubTypeId, "İş Alt Türü");

            var assignments =
                await _roleWorkSubTypePermissionRepository
                    .GetByWorkSubTypeIdAsync(
                        workSubTypeId,
                        cancellationToken);

            return _mapper.Map<
                List<ResultRoleWorkSubTypePermissionDto>>(
                    assignments);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Belirtilen organizasyon biriminin
        /// rollerine ait iş alt türü yetkilerini getirir.
        /// </summary>
        public async Task<
            IReadOnlyList<ResultRoleWorkSubTypePermissionDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var assignments =
                await _roleWorkSubTypePermissionRepository
                    .GetByUnitIdAsync(
                        unitId,
                        cancellationToken);

            return _mapper.Map<
                List<ResultRoleWorkSubTypePermissionDto>>(
                    assignments);
        }

        #endregion

        #region GetByRoleWorkSubTypeAndPermission

        /// <summary>
        /// Rol, iş alt türü ve yetki üçlüsüne
        /// göre mevcut atamayı getirir.
        /// </summary>
        public async Task<GetByIdRoleWorkSubTypePermissionDto?>
            GetByRoleWorkSubTypeAndPermissionAsync(
                int roleId,
                int workSubTypeId,
                int permissionId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");
            ValidateId(workSubTypeId, "İş Alt Türü");
            ValidateId(permissionId, "Yetki");

            var assignment =
                await _roleWorkSubTypePermissionRepository
                    .GetByRoleWorkSubTypeAndPermissionAsync(
                        roleId,
                        workSubTypeId,
                        permissionId,
                        cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            // Detay DTO için ilişkileri yükleyen
            // Repository metodunu kullan.
            return await GetByIdAsync(
                assignment.Id,
                cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni rol-iş alt türü-yetki ilişkisi oluşturur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateRoleWorkSubTypePermissionDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.RoleId, "Rol");
            ValidateId(dto.WorkSubTypeId, "İş Alt Türü");
            ValidateId(dto.PermissionId, "Yetki");

            // Rol, iş alt türü ve yetki
            // mevcut ve aktif olmalıdır.
            await ValidateRelationsAsync(
                dto.RoleId,
                dto.WorkSubTypeId,
                dto.PermissionId,
                cancellationToken);

            // Aynı üçlü ilişki ikinci kez oluşturulamaz.
            bool existsAssignment =
                await _roleWorkSubTypePermissionRepository
                    .ExistsAssignmentAsync(
                        dto.RoleId,
                        dto.WorkSubTypeId,
                        dto.PermissionId,
                        null,
                        cancellationToken);

            if (existsAssignment)
            {
                throw new InvalidOperationException(
                    "Bu rol, iş alt türü ve yetki ilişkisi " +
                    "daha önce oluşturulmuştur.");
            }

            // DTO -> Entity
            var assignment =
                _mapper.Map<RoleWorkSubTypePermission>(dto);

            assignment.RoleId = dto.RoleId;
            assignment.WorkSubTypeId = dto.WorkSubTypeId;
            assignment.PermissionId = dto.PermissionId;

            // Yeni atama varsayılan olarak aktiftir.
            assignment.IsActive = true;

            // Generic Repository kayıt işlemini gerçekleştirir.
            var createdAssignment =
                await _roleWorkSubTypePermissionRepository
                    .CreateAsync(
                        assignment,
                        cancellationToken);

            return createdAssignment.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut rol-iş alt türü-yetki atamasını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateRoleWorkSubTypePermissionDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Atama");
            ValidateId(dto.RoleId, "Rol");
            ValidateId(dto.WorkSubTypeId, "İş Alt Türü");
            ValidateId(dto.PermissionId, "Yetki");

            var existingAssignment =
                await _roleWorkSubTypePermissionRepository
                    .GetByIdAsync(
                        dto.Id,
                        cancellationToken);

            if (existingAssignment == null)
            {
                return false;
            }

            // Yeni seçilen ilişkileri doğrula.
            await ValidateRelationsAsync(
                dto.RoleId,
                dto.WorkSubTypeId,
                dto.PermissionId,
                cancellationToken);

            // Kendi kaydını benzersizlik kontrolünden
            // hariç tut.
            bool existsAssignment =
                await _roleWorkSubTypePermissionRepository
                    .ExistsAssignmentAsync(
                        dto.RoleId,
                        dto.WorkSubTypeId,
                        dto.PermissionId,
                        existingAssignment.Id,
                        cancellationToken);

            if (existsAssignment)
            {
                throw new InvalidOperationException(
                    "Bu rol, iş alt türü ve yetki üçlüsü " +
                    "başka bir atamada kullanılmaktadır.");
            }

            // DTO -> Mevcut Entity
            // Id, IsActive ve tarih alanları korunur.
            _mapper.Map(dto, existingAssignment);

            return await _roleWorkSubTypePermissionRepository
                .UpdateAsync(
                    existingAssignment,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Rol-iş alt türü-yetki ilişkisinin
        /// aktiflik durumunu değiştirir.
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

            var assignment =
                await _roleWorkSubTypePermissionRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (assignment == null)
            {
                return false;
            }

            if (assignment.IsActive == isActive)
            {
                return true;
            }

            // Pasif atama yeniden aktifleştirilirken
            // ilişkili rol, iş alt türü ve yetkinin
            // halen aktif olduğu doğrulanmalıdır.
            if (isActive)
            {
                await ValidateRelationsAsync(
                    assignment.RoleId,
                    assignment.WorkSubTypeId,
                    assignment.PermissionId,
                    cancellationToken);
            }

            return await _roleWorkSubTypePermissionRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsAssignment

        /// <summary>
        /// Aynı rol-iş alt türü-yetki ilişkisinin
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsAssignmentAsync(
            int roleId,
            int workSubTypeId,
            int permissionId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");
            ValidateId(workSubTypeId, "İş Alt Türü");
            ValidateId(permissionId, "Yetki");

            if (excludedId.HasValue)
            {
                ValidateId(excludedId.Value, "Hariç Tutulan Atama");
            }

            return await _roleWorkSubTypePermissionRepository
                .ExistsAssignmentAsync(
                    roleId,
                    workSubTypeId,
                    permissionId,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasActivePermission

        /// <summary>
        /// Rolün ilgili iş alt türünde
        /// belirli bir aktif yetkiye sahip
        /// olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActivePermissionAsync(
            int roleId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");
            ValidateId(workSubTypeId, "İş Alt Türü");

            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                throw new ArgumentException(
                    "Yetki kodu boş olamaz.",
                    nameof(permissionCode));
            }

            return await _roleWorkSubTypePermissionRepository
                .HasActivePermissionAsync(
                    roleId,
                    workSubTypeId,
                    permissionCode.Trim(),
                    cancellationToken);
        }

        #endregion

        #region HasUserActivePermission

        /// <summary>
        /// Kullanıcının ilgili organizasyon biriminde,
        /// iş alt türünde ve yetki kodunda
        /// aktif yetkisinin bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasUserActivePermissionAsync(
            int userId,
            int unitId,
            int workSubTypeId,
            string permissionCode,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workSubTypeId, "İş Alt Türü");

            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                throw new ArgumentException(
                    "Yetki kodu boş olamaz.",
                    nameof(permissionCode));
            }

            return await _roleWorkSubTypePermissionRepository
                .HasUserActivePermissionAsync(
                    userId,
                    unitId,
                    workSubTypeId,
                    permissionCode.Trim(),
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
        /// ID değerinin sıfırdan büyük olduğunu doğrular.
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
        /// Atamanın bağlı olduğu rol, iş alt türü
        /// ve yetkinin mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateRelationsAsync(
            int roleId,
            int workSubTypeId,
            int permissionId,
            CancellationToken cancellationToken)
        {
            // ===================================================
            // ROLE
            // ===================================================

            var role = await _roleRepository
                .GetByIdAsync(
                    roleId,
                    cancellationToken);

            if (role == null)
            {
                throw new InvalidOperationException(
                    "Seçilen rol bulunamadı.");
            }

            if (!role.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif role iş alt türü yetkisi atanamaz.");
            }

            // ===================================================
            // WORK SUB TYPE
            // ===================================================

            var workSubType = await _workSubTypeRepository
                .GetByIdAsync(
                    workSubTypeId,
                    cancellationToken);

            if (workSubType == null)
            {
                throw new InvalidOperationException(
                    "Seçilen iş alt türü bulunamadı.");
            }

            if (!workSubType.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif iş alt türüne yetki atanamaz.");
            }

            // ===================================================
            // PERMISSION
            // ===================================================

            var permission = await _permissionRepository
                .GetByIdAsync(
                    permissionId,
                    cancellationToken);

            if (permission == null)
            {
                throw new InvalidOperationException(
                    "Seçilen yetki bulunamadı.");
            }

            if (!permission.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif yetki, iş alt türüne atanamaz.");
            }
        }

        #endregion
    }
}
