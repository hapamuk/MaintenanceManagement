using AutoMapper;
using MaintenanceManagement.Application.DTOS.UserUnitRoleDtos;
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
    public class UserUnitRoleService : IUserUnitRoleService
    {
        private readonly IUserUnitRoleRepository _userUnitRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public UserUnitRoleService(
            IUserUnitRoleRepository userUnitRoleRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitRepository unitRepository,
            IMapper mapper)
        {
            _userUnitRoleRepository = userUnitRoleRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Bütün kullanıcı-rol atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserUnitRoleDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var assignments = await _userUnitRoleRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultUserUnitRoleDto>>(
                assignments);
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID değerine göre atama detaylarını getirir.
        /// </summary>
        public async Task<GetByIdUserUnitRoleDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var assignment = await _userUnitRoleRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdUserUnitRoleDto>(
                assignment);
        }

        #endregion

        #region GetByUserId

        /// <summary>
        /// Kullanıcıya ait bütün rol atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserUnitRoleDto>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var assignments = await _userUnitRoleRepository
                .GetByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultUserUnitRoleDto>>(
                assignments);
        }

        #endregion

        #region GetActiveByUserId

        /// <summary>
        /// Kullanıcının aktif rol atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserUnitRoleDto>>
            GetActiveByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var assignments = await _userUnitRoleRepository
                .GetActiveByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultUserUnitRoleDto>>(
                assignments);
        }

        #endregion

        #region GetByRoleId

        /// <summary>
        /// Belirtilen role ait kullanıcı atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserUnitRoleDto>>
            GetByRoleIdAsync(
                int roleId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(roleId, "Rol");

            var assignments = await _userUnitRoleRepository
                .GetByRoleIdAsync(
                    roleId,
                    cancellationToken);

            return _mapper.Map<List<ResultUserUnitRoleDto>>(
                assignments);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Belirtilen birimdeki rollere ait
        /// kullanıcı atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserUnitRoleDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var assignments = await _userUnitRoleRepository
                .GetByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultUserUnitRoleDto>>(
                assignments);
        }

        #endregion

        #region GetActiveByUserAndUnit

        /// <summary>
        /// Kullanıcının belirli birimdeki aktif
        /// rol atamalarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUserUnitRoleDto>>
            GetActiveByUserAndUnitAsync(
                int userId,
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(unitId, "Organizasyon Birimi");

            var assignments = await _userUnitRoleRepository
                .GetActiveByUserAndUnitAsync(
                    userId,
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultUserUnitRoleDto>>(
                assignments);
        }

        #endregion

        #region GetByUserAndRole

        /// <summary>
        /// Kullanıcı ve rol ikilisine ait atamayı getirir.
        /// </summary>
        public async Task<GetByIdUserUnitRoleDto?>
            GetByUserAndRoleAsync(
                int userId,
                int roleId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(roleId, "Rol");

            var assignment = await _userUnitRoleRepository
                .GetByUserAndRoleAsync(
                    userId,
                    roleId,
                    cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            // Navigation bilgilerini eksiksiz almak
            // için detay sorgusunu kullan.
            return await GetByIdAsync(
                assignment.Id,
                cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Kullanıcıya yeni rol ataması oluşturur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateUserUnitRoleDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.UserId, "Kullanıcı");
            ValidateId(dto.RoleId, "Rol");

            // Kullanıcı, rol ve rolün organizasyon
            // birimi geçerli ve aktif olmalıdır.
            await ValidateAssignmentRelationsAsync(
                dto.UserId,
                dto.RoleId,
                cancellationToken);

            // Aynı kullanıcıya aynı rol ikinci kez atanamaz.
            bool existsAssignment =
                await _userUnitRoleRepository
                    .ExistsAssignmentAsync(
                        dto.UserId,
                        dto.RoleId,
                        null,
                        cancellationToken);

            if (existsAssignment)
            {
                throw new InvalidOperationException(
                    "Bu rol kullanıcıya daha önce atanmıştır. " +
                    "Atama pasifse mevcut kayıt yeniden " +
                    "aktif hale getirilmelidir.");
            }

            // DTO -> Entity
            var assignment = _mapper.Map<UserUnitRole>(dto);

            assignment.UserId = dto.UserId;
            assignment.RoleId = dto.RoleId;

            // Atama tarihi sistem tarafından belirlenir.
            assignment.AssignedAt = DateTime.UtcNow;

            // Yeni atama aktif oluşturulur.
            assignment.IsActive = true;

            // GenericRepository kayıt işlemini gerçekleştirir.
            var createdAssignment =
                await _userUnitRoleRepository
                    .CreateAsync(
                        assignment,
                        cancellationToken);

            return createdAssignment.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut kullanıcı-rol atamasını günceller.
        /// Atama tarihi ve aktiflik durumu korunur.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateUserUnitRoleDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Atama");
            ValidateId(dto.UserId, "Kullanıcı");
            ValidateId(dto.RoleId, "Rol");

            var existingAssignment =
                await _userUnitRoleRepository
                    .GetByIdAsync(
                        dto.Id,
                        cancellationToken);

            if (existingAssignment == null)
            {
                return false;
            }

            // Kullanıcı ve yeni rol ilişkilerini doğrula.
            await ValidateAssignmentRelationsAsync(
                dto.UserId,
                dto.RoleId,
                cancellationToken);

            // Kendi kaydını mükerrerlik
            // kontrolünden hariç tut.
            bool existsAssignment =
                await _userUnitRoleRepository
                    .ExistsAssignmentAsync(
                        dto.UserId,
                        dto.RoleId,
                        existingAssignment.Id,
                        cancellationToken);

            if (existsAssignment)
            {
                throw new InvalidOperationException(
                    "Bu kullanıcı ve rol arasında başka " +
                    "bir atama kaydı bulunmaktadır.");
            }

            // DTO -> Mevcut Entity
            // Id, AssignedAt, IsActive ve tarih alanları korunur.
            _mapper.Map(dto, existingAssignment);

            return await _userUnitRoleRepository
                .UpdateAsync(
                    existingAssignment,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Kullanıcı-rol atamasını aktif veya pasif yapar.
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

            var assignment = await _userUnitRoleRepository
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

            // Pasif kayıt yeniden aktif yapılırken
            // ilişkili kullanıcı, rol ve birim
            // tekrar doğrulanmalıdır.
            if (isActive)
            {
                await ValidateAssignmentRelationsAsync(
                    assignment.UserId,
                    assignment.RoleId,
                    cancellationToken);
            }

            return await _userUnitRoleRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsAssignment

        /// <summary>
        /// Kullanıcı-rol atamasının mükerrerlik kontrolü.
        /// </summary>
        public async Task<bool> ExistsAssignmentAsync(
            int userId,
            int roleId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(roleId, "Rol");

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak Atama");
            }

            return await _userUnitRoleRepository
                .ExistsAssignmentAsync(
                    userId,
                    roleId,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveRoleInUnit

        /// <summary>
        /// Kullanıcının belirtilen organizasyon
        /// biriminde aktif rolü bulunuyor mu kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveRoleInUnitAsync(
            int userId,
            int unitId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(unitId, "Organizasyon Birimi");

            return await _userUnitRoleRepository
                .HasActiveRoleInUnitAsync(
                    userId,
                    unitId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveRole

        /// <summary>
        /// Kullanıcının belirtilen role ait
        /// aktif ataması bulunuyor mu kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveRoleAsync(
            int userId,
            int roleId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(roleId, "Rol");

            return await _userUnitRoleRepository
                .HasActiveRoleAsync(
                    userId,
                    roleId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations kurallarını çalıştırır.
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
        /// ID bilgisinin geçerli olduğunu kontrol eder.
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
        /// Kullanıcı, rol ve rolün bağlı olduğu birimin
        /// mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateAssignmentRelationsAsync(
            int userId,
            int roleId,
            CancellationToken cancellationToken)
        {
            #region User Validation

            var user = await _userRepository
                .GetByIdAsync(
                    userId,
                    cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Seçilen kullanıcı bulunamadı.");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif kullanıcıya rol atanamaz.");
            }

            #endregion

            #region Role Validation

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
                    "Pasif rol kullanıcıya atanamaz.");
            }

            #endregion

            #region Unit Validation

            // UserUnitRole üzerinde UnitId bulunmaz.
            // Organizasyon birimi Role.UnitId üzerinden alınır.
            var unit = await _unitRepository
                .GetByIdAsync(
                    role.UnitId,
                    cancellationToken);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    "Rolün bağlı olduğu organizasyon " +
                    "birimi bulunamadı.");
            }

            if (!unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif organizasyon birimine bağlı " +
                    "rol kullanıcıya atanamaz.");
            }

            #endregion
        }

        #endregion
    }
}
