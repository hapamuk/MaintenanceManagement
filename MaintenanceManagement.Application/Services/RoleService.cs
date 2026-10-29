using AutoMapper;
using MaintenanceManagement.Application.DTOS.RoleDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(
            IRoleRepository roleRepository,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Bütün rolleri ilişkili birim bilgileriyle
        /// birlikte getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultRoleDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var roles = await _roleRepository
                .GetAllWithDetailsAsync(cancellationToken);

            var result = _mapper
                .Map<List<ResultRoleDto>>(roles);

            return result;
        }

        #endregion

        #region GetById

        /// <summary>
        /// Rolün detaylarını ve ilişkili kayıt sayılarını getirir.
        /// </summary>
        public async Task<GetByIdRoleDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var role = await _roleRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (role == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdRoleDto>(role);
        }

        #endregion

        #region GetByUnit

        /// <summary>
        /// Organizasyon birimine bağlı aktif rolleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultRoleDto>> GetByUnitAsync(
            int unitId,
            CancellationToken cancellationToken = default)
        {
            if (unitId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(unitId),
                    "Geçerli bir organizasyon birimi seçiniz.");
            }

            var roles = await _roleRepository
                .GetByUnitAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultRoleDto>>(roles);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni rol oluşturur.
        /// Rol kodu aynı organizasyon biriminde benzersizdir.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateRoleDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.UnitId <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir organizasyon birimi seçiniz.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException(
                    "Rol adı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                throw new ArgumentException(
                    "Rol kodu boş olamaz.");
            }

            // Kullanıcıdan gelen bilgileri düzenle.
            var roleName = dto.Name.Trim();
            var roleCode = dto.Code.Trim();

            var description = string.IsNullOrWhiteSpace(
                dto.Description)
                    ? null
                    : dto.Description.Trim();

            // Aynı birimde aynı kodla rol bulunamaz.
            var existsCode = await _roleRepository
                .ExistsCodeAsync(
                    dto.UnitId,
                    roleCode,
                    null,
                    cancellationToken);

            if (existsCode)
            {
                throw new InvalidOperationException(
                    "Bu organizasyon biriminde aynı rol kodu " +
                    "ile kayıt bulunmaktadır.");
            }

            // DTO -> Entity
            var role = _mapper.Map<Role>(dto);

            role.Name = roleName;
            role.Code = roleCode;
            role.Description = description;

            // Yeni roller varsayılan olarak aktiftir.
            role.IsActive = true;

            // GenericRepository CreatedAt ve
            // SaveChanges işlemlerini gerçekleştirir.
            var createdRole = await _roleRepository
                .CreateAsync(
                    role,
                    cancellationToken);

            return createdRole.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut rolün bilgilerini günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateRoleDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.Id <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir rol seçiniz.");
            }

            if (dto.UnitId <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir organizasyon birimi seçiniz.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException(
                    "Rol adı boş olamaz.");
            }

            if (string.IsNullOrWhiteSpace(dto.Code))
            {
                throw new ArgumentException(
                    "Rol kodu boş olamaz.");
            }

            // Mevcut rolü getir.
            var existingRole = await _roleRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingRole == null)
            {
                return false;
            }

            // Rolün bağlı olduğu birimin değiştirilmesini
            // normal güncelleme işleminde engelle.
            //
            // Çünkü kullanıcı-rol atamaları ve yetkiler
            // mevcut organizasyon birimine bağlı olabilir.
            if (existingRole.UnitId != dto.UnitId)
            {
                throw new InvalidOperationException(
                    "Rolün bağlı olduğu organizasyon birimi " +
                    "normal güncelleme işleminde değiştirilemez.");
            }

            var roleName = dto.Name.Trim();
            var roleCode = dto.Code.Trim();

            var description = string.IsNullOrWhiteSpace(
                dto.Description)
                    ? null
                    : dto.Description.Trim();

            // Kendi kaydını benzersizlik kontrolünden hariç tut.
            var existsCode = await _roleRepository
                .ExistsCodeAsync(
                    existingRole.UnitId,
                    roleCode,
                    existingRole.Id,
                    cancellationToken);

            if (existsCode)
            {
                throw new InvalidOperationException(
                    "Bu organizasyon biriminde aynı rol kodu " +
                    "ile başka bir kayıt bulunmaktadır.");
            }

            // DTO -> Mevcut Entity
            _mapper.Map(dto, existingRole);

            // Normalize edilmiş değerleri uygula.
            existingRole.Name = roleName;
            existingRole.Code = roleCode;
            existingRole.Description = description;

            // IsActive, UnitId ve CreatedAt
            // mapping tarafından değiştirilmez.
            return await _roleRepository
                .UpdateAsync(
                    existingRole,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Rolün aktif/pasif durumunu günceller.
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

            var existingRole = await _roleRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingRole == null)
            {
                return false;
            }

            // Zaten istenen durumdaysa
            // tekrar veritabanı güncellemesi yapma.
            if (existingRole.IsActive == isActive)
            {
                return true;
            }

            return await _roleRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Birim bazında rol kodu benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (unitId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(unitId),
                    "Geçerli bir organizasyon birimi seçiniz.");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "Rol kodu boş olamaz.",
                    nameof(code));
            }

            return await _roleRepository
                .ExistsCodeAsync(
                    unitId,
                    code.Trim(),
                    excludedId,
                    cancellationToken);
        }

        #endregion
    }
}
