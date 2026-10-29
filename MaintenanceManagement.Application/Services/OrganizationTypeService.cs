using AutoMapper;
using MaintenanceManagement.Application.DTOS.OrganizationTypeDtos;
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
    public class OrganizationTypeService : IOrganizationTypeService
    {
        private readonly IOrganizationTypeRepository _organizationTypeRepository;
        private readonly IMapper _mapper;

        public OrganizationTypeService(
            IOrganizationTypeRepository organizationTypeRepository,
            IMapper mapper)
        {
            _organizationTypeRepository = organizationTypeRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Bütün organizasyon türlerini getirir.
        /// Aktif ve pasif kayıtlar dahildir.
        /// </summary>
        public async Task<IReadOnlyList<ResultOrganizationTypeDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var organizationTypes =
                await _organizationTypeRepository
                    .GetAllWithUnitsAsync(cancellationToken);

            var result = _mapper.Map<
                List<ResultOrganizationTypeDto>>(
                    organizationTypes);

            return result;
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Yalnızca aktif organizasyon türlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultOrganizationTypeDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var organizationTypes =
                await _organizationTypeRepository
                    .GetActiveOrganizationTypesAsync(
                        cancellationToken);

            var result = _mapper.Map<
                List<ResultOrganizationTypeDto>>(
                    organizationTypes);

            return result;
        }

        #endregion

        #region GetById

        /// <summary>
        /// Organizasyon türünün detaylarını
        /// ilişkili birim bilgileriyle getirir.
        /// </summary>
        public async Task<GetByIdOrganizationTypeDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var organizationType =
                await _organizationTypeRepository
                    .GetByIdWithUnitsAsync(
                        id,
                        cancellationToken);

            if (organizationType == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdOrganizationTypeDto>(
                organizationType);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni organizasyon türü oluşturur.
        /// Ad ve kod benzersizlik kontrollerini gerçekleştirir.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateOrganizationTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateOrganizationType(
                dto.Name,
                dto.Code,
                dto.Description);

            string name = dto.Name.Trim();
            string code = dto.Code.Trim();

            string? description =
                string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim();

            // Organizasyon türü adı benzersiz olmalıdır.
            bool existsName =
                await _organizationTypeRepository
                    .ExistsNameAsync(
                        name,
                        null,
                        cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu organizasyon türü adıyla daha önce " +
                    "kayıt oluşturulmuştur.");
            }

            // Organizasyon türü kodu benzersiz olmalıdır.
            bool existsCode =
                await _organizationTypeRepository
                    .ExistsCodeAsync(
                        code,
                        null,
                        cancellationToken);

            if (existsCode)
            {
                throw new InvalidOperationException(
                    "Bu organizasyon türü koduyla daha önce " +
                    "kayıt oluşturulmuştur.");
            }

            // Create DTO -> OrganizationType Entity
            var organizationType =
                _mapper.Map<OrganizationType>(dto);

            organizationType.Name = name;
            organizationType.Code = code;
            organizationType.Description = description;

            // Yeni organizasyon türü varsayılan olarak aktiftir.
            organizationType.IsActive = true;

            // CreatedAt ve veritabanına kaydetme işlemi
            // mevcut GenericRepository tarafından yapılır.
            var createdOrganizationType =
                await _organizationTypeRepository
                    .CreateAsync(
                        organizationType,
                        cancellationToken);

            return createdOrganizationType.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut organizasyon türünün bilgilerini günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateOrganizationTypeDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.Id <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir organizasyon türü seçiniz.");
            }

            ValidateOrganizationType(
                dto.Name,
                dto.Code,
                dto.Description);

            // Güncellenecek organizasyon türünü getir.
            var existingOrganizationType =
                await _organizationTypeRepository
                    .GetByIdAsync(
                        dto.Id,
                        cancellationToken);

            if (existingOrganizationType == null)
            {
                return false;
            }

            string name = dto.Name.Trim();
            string code = dto.Code.Trim();

            string? description =
                string.IsNullOrWhiteSpace(dto.Description)
                    ? null
                    : dto.Description.Trim();

            // Mevcut kaydın kendi ID değerini
            // benzersizlik kontrolünden hariç tut.
            bool existsName =
                await _organizationTypeRepository
                    .ExistsNameAsync(
                        name,
                        dto.Id,
                        cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu isimle başka bir organizasyon türü " +
                    "kaydı bulunmaktadır.");
            }

            bool existsCode =
                await _organizationTypeRepository
                    .ExistsCodeAsync(
                        code,
                        dto.Id,
                        cancellationToken);

            if (existsCode)
            {
                throw new InvalidOperationException(
                    "Bu kodla başka bir organizasyon türü " +
                    "kaydı bulunmaktadır.");
            }

            // DTO -> Mevcut Entity
            // Id, IsActive, CreatedAt ve Units korunur.
            _mapper.Map(dto, existingOrganizationType);

            existingOrganizationType.Name = name;
            existingOrganizationType.Code = code;
            existingOrganizationType.Description = description;

            // UpdatedAt ve SaveChanges işlemleri
            // mevcut GenericRepository tarafından yapılır.
            return await _organizationTypeRepository
                .UpdateAsync(
                    existingOrganizationType,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Organizasyon türünün aktiflik durumunu değiştirir.
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

            var existingOrganizationType =
                await _organizationTypeRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (existingOrganizationType == null)
            {
                return false;
            }

            // Zaten istenen durumdaysa
            // tekrar güncelleme yapma.
            if (existingOrganizationType.IsActive == isActive)
            {
                return true;
            }

            // Organizasyon türünü pasife alırken
            // bağlı birimlerin kullanımı ayrıca
            // iş kurallarına göre değerlendirilmelidir.
            return await _organizationTypeRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Organizasyon türü adının başka bir
        /// kayıt tarafından kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Organizasyon türü adı boş olamaz.",
                    nameof(name));
            }

            return await _organizationTypeRepository
                .ExistsNameAsync(
                    name.Trim(),
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Organizasyon türü kodunun başka bir
        /// kayıt tarafından kullanılıp kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "Organizasyon türü kodu boş olamaz.",
                    nameof(code));
            }

            return await _organizationTypeRepository
                .ExistsCodeAsync(
                    code.Trim(),
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasUnits

        /// <summary>
        /// Organizasyon türüne bağlı
        /// birim olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasUnitsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _organizationTypeRepository
                .HasUnitsAsync(
                    id,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// Organizasyon türünün zorunlu alanlarını ve
        /// Entity üzerindeki karakter sınırlarını doğrular.
        /// </summary>
        private static void ValidateOrganizationType(
            string? name,
            string? code,
            string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Organizasyon türü adı zorunludur.");
            }

            if (name.Trim().Length > 150)
            {
                throw new ArgumentException(
                    "Organizasyon türü adı en fazla " +
                    "150 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "Organizasyon türü kodu zorunludur.");
            }

            if (code.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "Organizasyon türü kodu en fazla " +
                    "50 karakter olabilir.");
            }

            if (!string.IsNullOrEmpty(description) &&
                description.Trim().Length > 500)
            {
                throw new ArgumentException(
                    "Açıklama en fazla 500 karakter olabilir.");
            }
        }

        #endregion
    }
}
