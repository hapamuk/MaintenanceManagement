using AutoMapper;
using MaintenanceManagement.Application.DTOS.PermissionDtos;
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
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public PermissionService(
            IPermissionRepository permissionRepository,
            IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Aktif ve pasif bütün yetkileri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultPermissionDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var permissions = await _permissionRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultPermissionDto>>(
                permissions);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Yalnızca aktif yetkileri listeler.
        /// </summary>
        public async Task<IReadOnlyList<ResultPermissionDto>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var permissions = await _permissionRepository
                .GetActivePermissionsAsync(cancellationToken);

            return _mapper.Map<List<ResultPermissionDto>>(
                permissions);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Yetkinin detaylarını ve ilişkili
        /// rol/iş alt türü yetki sayılarını getirir.
        /// </summary>
        public async Task<GetByIdPermissionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var permission = await _permissionRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (permission == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdPermissionDto>(
                permission);
        }

        #endregion

        #region GetByModule

        /// <summary>
        /// Belirtilen modüle ait aktif yetkileri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultPermissionDto>> GetByModuleAsync(
            string module,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(module))
            {
                throw new ArgumentException(
                    "Modül adı boş olamaz.",
                    nameof(module));
            }

            var permissions = await _permissionRepository
                .GetPermissionsByModuleAsync(
                    module.Trim(),
                    cancellationToken);

            return _mapper.Map<List<ResultPermissionDto>>(
                permissions);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni yetki oluşturur.
        /// Yetki adı modül içinde, yetki kodu
        /// sistem genelinde benzersiz olmalıdır.
        /// </summary>
        public async Task<int> CreateAsync(
            CreatePermissionDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidatePermissionFields(
                dto.Name,
                dto.Code,
                dto.Module,
                dto.Description);

            string name = dto.Name.Trim();
            string code = dto.Code.Trim();
            string module = NormalizeModule(dto.Module);

            string? description =
                NormalizeDescription(dto.Description);

            // Aynı modülde aynı adla yetki oluşturulamaz.
            bool existsName = await _permissionRepository
                .ExistsNameAsync(
                    name,
                    module,
                    null,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu modülde aynı isimle başka bir " +
                    "yetki kaydı bulunmaktadır.");
            }

            // Yetki kodu bütün sistemde benzersiz olmalıdır.
            bool existsCode = await _permissionRepository
                .ExistsCodeAsync(
                    code,
                    null,
                    cancellationToken);

            if (existsCode)
            {
                throw new InvalidOperationException(
                    "Bu yetki koduyla daha önce " +
                    "kayıt oluşturulmuştur.");
            }

            // DTO -> Permission Entity
            var permission = _mapper.Map<Permission>(dto);

            permission.Name = name;
            permission.Code = code;
            permission.Module = module;
            permission.Description = description;

            // Yeni yetkiler varsayılan olarak aktif oluşturulur.
            permission.IsActive = true;

            // GenericRepository CreatedAt ve
            // SaveChanges işlemlerini gerçekleştirir.
            var createdPermission = await _permissionRepository
                .CreateAsync(
                    permission,
                    cancellationToken);

            return createdPermission.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut yetkinin adını, kodunu,
        /// modülünü ve açıklamasını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdatePermissionDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            if (dto.Id <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir yetki seçiniz.");
            }

            ValidatePermissionFields(
                dto.Name,
                dto.Code,
                dto.Module,
                dto.Description);

            // Güncellenecek yetkiyi getir.
            var existingPermission = await _permissionRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingPermission == null)
            {
                return false;
            }

            string name = dto.Name.Trim();
            string code = dto.Code.Trim();
            string module = NormalizeModule(dto.Module);

            string? description =
                NormalizeDescription(dto.Description);

            // Güncellenen yetkinin kendi ID değerini
            // benzersizlik sorgularından hariç tut.
            bool existsName = await _permissionRepository
                .ExistsNameAsync(
                    name,
                    module,
                    existingPermission.Id,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu modülde aynı isimle başka " +
                    "bir yetki bulunmaktadır.");
            }

            bool existsCode = await _permissionRepository
                .ExistsCodeAsync(
                    code,
                    existingPermission.Id,
                    cancellationToken);

            if (existsCode)
            {
                throw new InvalidOperationException(
                    "Bu yetki kodu başka bir " +
                    "kayıt tarafından kullanılmaktadır.");
            }

            // DTO -> Mevcut Entity
            _mapper.Map(dto, existingPermission);

            // Normalize edilen bilgileri ata.
            existingPermission.Name = name;
            existingPermission.Code = code;
            existingPermission.Module = module;
            existingPermission.Description = description;

            // Id, IsActive, CreatedAt ve ilişkili
            // yetki koleksiyonları korunur.
            return await _permissionRepository
                .UpdateAsync(
                    existingPermission,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Yetkinin aktif veya pasif durumunu değiştirir.
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

            var existingPermission = await _permissionRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingPermission == null)
            {
                return false;
            }

            // Yetki zaten istenilen durumdaysa
            // tekrar güncelleme yapma.
            if (existingPermission.IsActive == isActive)
            {
                return true;
            }

            // Mevcut tasarımda rol veya iş alt türü
            // yetki ataması olan kayıtları pasife
            // almadan önce bu ilişkileri kontrol et.
            if (!isActive)
            {
                bool hasAssignments = await _permissionRepository
                    .HasAssignmentsAsync(
                        id,
                        cancellationToken);

                if (hasAssignments)
                {
                    throw new InvalidOperationException(
                        "Bu yetki rol veya iş alt türü " +
                        "atamalarında kullanılmaktadır. " +
                        "Yetki pasife alınmadan önce " +
                        "ilgili atamalar kaldırılmalıdır.");
                }
            }

            return await _permissionRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Modül bazında yetki adı benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            string module,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Yetki adı boş olamaz.",
                    nameof(name));
            }

            return await _permissionRepository
                .ExistsNameAsync(
                    name.Trim(),
                    NormalizeModule(module),
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Yetki kodunun sistem genelinde
        /// benzersiz olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "Yetki kodu boş olamaz.",
                    nameof(code));
            }

            return await _permissionRepository
                .ExistsCodeAsync(
                    code.Trim(),
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasAssignments

        /// <summary>
        /// Yetkinin rol veya iş alt türü
        /// atamalarında kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasAssignmentsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _permissionRepository
                .HasAssignmentsAsync(
                    id,
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
        /// Yetki için zorunlu alanları ve karakter
        /// sınırlarını kontrol eder.
        /// </summary>
        private static void ValidatePermissionFields(
            string? name,
            string? code,
            string? module,
            string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Yetki adı zorunludur.");
            }

            if (name.Trim().Length > 150)
            {
                throw new ValidationException(
                    "Yetki adı en fazla 150 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException(
                    "Yetki kodu zorunludur.");
            }

            if (code.Trim().Length > 150)
            {
                throw new ValidationException(
                    "Yetki kodu en fazla 150 karakter olabilir.");
            }

            if (module != null && module.Trim().Length > 100)
            {
                throw new ValidationException(
                    "Modül adı en fazla 100 karakter olabilir.");
            }

            if (description != null &&
                description.Trim().Length > 500)
            {
                throw new ValidationException(
                    "Açıklama en fazla 500 karakter olabilir.");
            }
        }

        /// <summary>
        /// Modül bilgisini temizler.
        /// Module alanı boşsa string.Empty döndürür.
        /// </summary>
        private static string NormalizeModule(string? module)
        {
            return module?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Boş açıklamaları null olarak düzenler.
        /// </summary>
        private static string? NormalizeDescription(
            string? description)
        {
            return string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();
        }

        #endregion
    }
}
