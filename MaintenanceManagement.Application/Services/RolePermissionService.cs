using AutoMapper;
using MaintenanceManagement.Application.DTOS.RolePermissionDtos;
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
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public RolePermissionService(
            IRolePermissionRepository rolePermissionRepository,
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper)
        {
            _rolePermissionRepository = rolePermissionRepository;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Bütün rol-yetki atamalarını getirir.
        /// Aktif ve pasif ilişkiler dahildir.
        /// </summary>
        public async Task<IReadOnlyList<ResultRolePermissionDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var rolePermissions = await _rolePermissionRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultRolePermissionDto>>(
                rolePermissions);
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID bilgisine göre rol-yetki ilişkisini getirir.
        /// </summary>
        public async Task<GetByIdRolePermissionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var rolePermission = await _rolePermissionRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (rolePermission == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdRolePermissionDto>(
                rolePermission);
        }

        #endregion

        #region GetByRoleId

        /// <summary>
        /// Role atanmış bütün yetki ilişkilerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultRolePermissionDto>>
            GetByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");

            var rolePermissions = await _rolePermissionRepository
                .GetByRoleIdAsync(
                    roleId,
                    cancellationToken);

            return _mapper.Map<List<ResultRolePermissionDto>>(
                rolePermissions);
        }

        #endregion

        #region GetActiveByRoleId

        /// <summary>
        /// Role ait aktif yetki atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultRolePermissionDto>>
            GetActiveByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");

            var rolePermissions = await _rolePermissionRepository
                .GetActiveByRoleIdAsync(
                    roleId,
                    cancellationToken);

            return _mapper.Map<List<ResultRolePermissionDto>>(
                rolePermissions);
        }

        #endregion

        #region GetByPermissionId

        /// <summary>
        /// Belirtilen yetkinin bağlı olduğu
        /// rol-yetki ilişkilerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultRolePermissionDto>>
            GetByPermissionIdAsync(
                int permissionId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(permissionId, "Yetki");

            var rolePermissions = await _rolePermissionRepository
                .GetByPermissionIdAsync(
                    permissionId,
                    cancellationToken);

            return _mapper.Map<List<ResultRolePermissionDto>>(
                rolePermissions);
        }

        #endregion

        #region GetByRoleAndPermission

        /// <summary>
        /// Rol ve yetki ID değerlerine göre ilişkiyi getirir.
        /// </summary>
        public async Task<GetByIdRolePermissionDto?>
            GetByRoleAndPermissionAsync(
                int roleId,
                int permissionId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");
            ValidateId(permissionId, "Yetki");

            var rolePermission = await _rolePermissionRepository
                .GetByRoleAndPermissionAsync(
                    roleId,
                    permissionId,
                    cancellationToken);

            if (rolePermission == null)
            {
                return null;
            }

            // Detay DTO'sunda Role ve Permission
            // navigation alanlarını doğru doldurabilmek
            // için detay sorgusunu kullan.
            return await GetByIdAsync(
                rolePermission.Id,
                cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni rol-yetki ilişkisi oluşturur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateRolePermissionDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.RoleId, "Rol");
            ValidateId(dto.PermissionId, "Yetki");

            // Rol ve yetki mevcut ve aktif olmalıdır.
            await ValidateRoleAndPermissionAsync(
                dto.RoleId,
                dto.PermissionId,
                cancellationToken);

            // Aynı rol ve yetki ikilisi ikinci kez oluşturulamaz.
            bool existsAssignment = await _rolePermissionRepository
                .ExistsAssignmentAsync(
                    dto.RoleId,
                    dto.PermissionId,
                    null,
                    cancellationToken);

            if (existsAssignment)
            {
                throw new InvalidOperationException(
                    "Bu yetki seçilen role daha önce atanmıştır.");
            }

            // DTO -> Entity dönüşümü
            var rolePermission = _mapper.Map<RolePermission>(dto);

            rolePermission.RoleId = dto.RoleId;
            rolePermission.PermissionId = dto.PermissionId;

            // Yeni atama aktif oluşturulur.
            rolePermission.IsActive = true;

            // CreatedAt ve SaveChanges işlemleri
            // mevcut GenericRepository tarafından yönetilir.
            var createdRolePermission = await _rolePermissionRepository
                .CreateAsync(
                    rolePermission,
                    cancellationToken);

            return createdRolePermission.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut rol-yetki ilişkisinin rol veya
        /// yetki bağlantısını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateRolePermissionDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Rol-Yetki Kaydı");
            ValidateId(dto.RoleId, "Rol");
            ValidateId(dto.PermissionId, "Yetki");

            var existingRolePermission = await _rolePermissionRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingRolePermission == null)
            {
                return false;
            }

            // Seçilen rol ve yetki mevcut ve aktif olmalıdır.
            await ValidateRoleAndPermissionAsync(
                dto.RoleId,
                dto.PermissionId,
                cancellationToken);

            // Güncellenen kaydın kendi ID değerini
            // benzersizlik kontrolünden hariç tut.
            bool existsAssignment = await _rolePermissionRepository
                .ExistsAssignmentAsync(
                    dto.RoleId,
                    dto.PermissionId,
                    existingRolePermission.Id,
                    cancellationToken);

            if (existsAssignment)
            {
                throw new InvalidOperationException(
                    "Bu rol ve yetki arasında başka bir " +
                    "atama kaydı bulunmaktadır.");
            }

            // DTO -> Mevcut Entity
            // Id, IsActive, CreatedAt ve
            // navigation özellikleri korunur.
            _mapper.Map(dto, existingRolePermission);

            return await _rolePermissionRepository
                .UpdateAsync(
                    existingRolePermission,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Rol-yetki ilişkisinin aktiflik durumunu değiştirir.
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

            var rolePermission = await _rolePermissionRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (rolePermission == null)
            {
                return false;
            }

            // Kayıt zaten istenen durumdaysa
            // tekrar güncelleme yapma.
            if (rolePermission.IsActive == isActive)
            {
                return true;
            }

            // Pasif ilişki tekrar aktif yapılırken
            // rolün ve yetkinin hâlâ aktif olması gerekir.
            if (isActive)
            {
                await ValidateRoleAndPermissionAsync(
                    rolePermission.RoleId,
                    rolePermission.PermissionId,
                    cancellationToken);
            }

            return await _rolePermissionRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsAssignment

        /// <summary>
        /// Aynı rol-yetki ilişkisinin daha önce
        /// tanımlanıp tanımlanmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsAssignmentAsync(
            int roleId,
            int permissionId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");
            ValidateId(permissionId, "Yetki");

            if (excludedId.HasValue)
            {
                ValidateId(excludedId.Value, "Hariç Tutulacak Kayıt");
            }

            return await _rolePermissionRepository
                .ExistsAssignmentAsync(
                    roleId,
                    permissionId,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasActivePermission

        /// <summary>
        /// Rolün verilen yetki koduna sahip
        /// aktif bir ataması olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActivePermissionAsync(
            int roleId,
            string permissionCode,
            CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");

            if (string.IsNullOrWhiteSpace(permissionCode))
            {
                throw new ArgumentException(
                    "Yetki kodu boş olamaz.",
                    nameof(permissionCode));
            }

            return await _rolePermissionRepository
                .HasActivePermissionAsync(
                    roleId,
                    permissionCode.Trim(),
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerinde tanımlı DataAnnotations
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
        /// ID alanının geçerli olduğunu kontrol eder.
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
        /// Rol ve yetkinin mevcut ve aktif
        /// olduğunu doğrular.
        /// </summary>
        private async Task ValidateRoleAndPermissionAsync(
            int roleId,
            int permissionId,
            CancellationToken cancellationToken)
        {
            // Rol kontrolü
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
                    "Pasif role yetki atanamaz.");
            }

            // Yetki kontrolü
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
                    "Pasif yetki bir role atanamaz.");
            }
        }

        #endregion
    }
}
