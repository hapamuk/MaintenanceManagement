using AutoMapper;
using MaintenanceManagement.Application.DTOS.UnitDtos;
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
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IOrganizationTypeRepository _organizationTypeRepository;
        private readonly IMapper _mapper;

        public UnitService(
            IUnitRepository unitRepository,
            IOrganizationTypeRepository organizationTypeRepository,
            IMapper mapper)
        {
            _unitRepository = unitRepository;
            _organizationTypeRepository = organizationTypeRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Sistemdeki bütün organizasyon birimlerini
        /// ilişkili bilgilerle birlikte getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var units = await _unitRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultUnitDto>>(units);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Yalnızca aktif organizasyon birimlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitDto>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var units = await _unitRepository
                .GetActiveUnitsAsync(cancellationToken);

            return _mapper.Map<List<ResultUnitDto>>(units);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Birimin detaylarını ve ilişkili kayıt sayılarını getirir.
        /// </summary>
        public async Task<GetByIdUnitDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var unit = await _unitRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (unit == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdUnitDto>(unit);
        }

        #endregion

        #region GetByOrganizationType

        /// <summary>
        /// Belirtilen organizasyon türündeki birimleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitDto>>
            GetByOrganizationTypeAsync(
                int organizationTypeId,
                CancellationToken cancellationToken = default)
        {
            if (organizationTypeId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(organizationTypeId),
                    "Geçerli bir organizasyon türü seçiniz.");
            }

            var units = await _unitRepository
                .GetUnitsByOrganizationTypeIdAsync(
                    organizationTypeId,
                    cancellationToken);

            return _mapper.Map<List<ResultUnitDto>>(units);
        }

        #endregion

        #region GetChildUnits

        /// <summary>
        /// Belirtilen birimin doğrudan alt birimlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultUnitDto>>
            GetChildUnitsAsync(
                int parentUnitId,
                CancellationToken cancellationToken = default)
        {
            if (parentUnitId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(parentUnitId),
                    "Geçerli bir üst birim seçiniz.");
            }

            var units = await _unitRepository
                .GetChildUnitsAsync(
                    parentUnitId,
                    cancellationToken);

            return _mapper.Map<List<ResultUnitDto>>(units);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni organizasyon birimi oluşturur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateUnitDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // Zorunlu alanları kontrol et.
            ValidateUnitFields(
                dto.Name,
                dto.Code,
                dto.OrganizationTypeId,
                dto.ParentUnitId);

            string name = dto.Name.Trim();
            string code = dto.Code.Trim();

            // Organizasyon türünün geçerli olduğunu kontrol et.
            await ValidateOrganizationTypeAsync(
                dto.OrganizationTypeId,
                cancellationToken);

            // Seçilen üst birimi doğrula.
            await ValidateParentHierarchyAsync(
                dto.ParentUnitId,
                null,
                cancellationToken);

            // Birim adı ve kodu benzersizliğini kontrol et.
            await ValidateUniquenessAsync(
                name,
                code,
                dto.ParentUnitId,
                null,
                cancellationToken);

            // DTO -> Entity dönüşümü
            var unit = _mapper.Map<Unit>(dto);

            unit.Name = name;
            unit.Code = code;

            // Yeni birim varsayılan olarak aktiftir.
            unit.IsActive = true;

            // CreatedAt ve SaveChanges işlemleri
            // mevcut GenericRepository tarafından yapılır.
            var createdUnit = await _unitRepository
                .CreateAsync(
                    unit,
                    cancellationToken);

            return createdUnit.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut organizasyon birimini günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateUnitDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.Id <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir organizasyon birimi seçiniz.");
            }

            ValidateUnitFields(
                dto.Name,
                dto.Code,
                dto.OrganizationTypeId,
                dto.ParentUnitId);

            var existingUnit = await _unitRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingUnit == null)
            {
                return false;
            }

            string name = dto.Name.Trim();
            string code = dto.Code.Trim();

            // Organizasyon türünü doğrula.
            await ValidateOrganizationTypeAsync(
                dto.OrganizationTypeId,
                cancellationToken);

            // Üst birim ilişkisini kontrol et.
            // Birim kendi altına veya alt birimlerinin
            // altına taşınamaz.
            await ValidateParentHierarchyAsync(
                dto.ParentUnitId,
                existingUnit.Id,
                cancellationToken);

            // Kendi ID değerini benzersizlik
            // sorgularından hariç tut.
            await ValidateUniquenessAsync(
                name,
                code,
                dto.ParentUnitId,
                existingUnit.Id,
                cancellationToken);

            // DTO -> Mevcut Entity
            _mapper.Map(dto, existingUnit);

            existingUnit.Name = name;
            existingUnit.Code = code;

            // Id, IsActive, CreatedAt ve navigation
            // koleksiyonları mapping tarafından korunur.
            return await _unitRepository
                .UpdateAsync(
                    existingUnit,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Organizasyon biriminin aktiflik durumunu değiştirir.
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

            var existingUnit = await _unitRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingUnit == null)
            {
                return false;
            }

            if (existingUnit.IsActive == isActive)
            {
                return true;
            }

            return await _unitRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Aynı üst birim altında aynı adla
        /// kayıt bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            string name,
            int? parentUnitId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Birim adı boş olamaz.",
                    nameof(name));
            }

            return await _unitRepository
                .ExistsNameAsync(
                    name.Trim(),
                    parentUnitId,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Birim kodunun kullanım durumunu kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "Birim kodu boş olamaz.",
                    nameof(code));
            }

            return await _unitRepository
                .ExistsCodeAsync(
                    code.Trim(),
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasChildUnits

        /// <summary>
        /// Organizasyon birimine bağlı
        /// alt birim olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasChildUnitsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _unitRepository
                .HasChildUnitsAsync(
                    id,
                    cancellationToken);
        }

        #endregion

        #region HasActiveTeams

        /// <summary>
        /// Birime bağlı aktif ekip bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveTeamsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _unitRepository
                .HasActiveTeamsAsync(
                    id,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// Birimin zorunlu alanlarını kontrol eder.
        /// </summary>
        private static void ValidateUnitFields(
            string? name,
            string? code,
            int organizationTypeId,
            int? parentUnitId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Birim adı zorunludur.");
            }

            if (name.Trim().Length > 150)
            {
                throw new ArgumentException(
                    "Birim adı en fazla 150 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "Birim kodu zorunludur.");
            }

            if (code.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "Birim kodu en fazla 50 karakter olabilir.");
            }

            if (organizationTypeId <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir organizasyon türü seçiniz.");
            }

            if (parentUnitId.HasValue &&
                parentUnitId.Value <= 0)
            {
                throw new ArgumentException(
                    "Geçerli bir üst birim seçiniz.");
            }
        }

        /// <summary>
        /// Organizasyon türünün mevcut ve aktif
        /// olduğunu doğrular.
        /// </summary>
        private async Task ValidateOrganizationTypeAsync(
            int organizationTypeId,
            CancellationToken cancellationToken)
        {
            var organizationType =
                await _organizationTypeRepository
                    .GetByIdAsync(
                        organizationTypeId,
                        cancellationToken);

            if (organizationType == null)
            {
                throw new InvalidOperationException(
                    "Seçilen organizasyon türü bulunamadı.");
            }

            if (!organizationType.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif organizasyon türü için " +
                    "birim oluşturulamaz veya güncellenemez.");
            }
        }

        /// <summary>
        /// Üst birim ilişkisini ve hiyerarşide
        /// döngü oluşup oluşmadığını kontrol eder.
        /// </summary>
        private async Task ValidateParentHierarchyAsync(
            int? parentUnitId,
            int? currentUnitId,
            CancellationToken cancellationToken)
        {
            if (!parentUnitId.HasValue)
            {
                return;
            }

            var visitedIds = new HashSet<int>();

            int? ancestorId = parentUnitId;

            while (ancestorId.HasValue)
            {
                int id = ancestorId.Value;

                // Birim kendisine veya kendi altına bağlanamaz.
                if (currentUnitId.HasValue &&
                    id == currentUnitId.Value)
                {
                    throw new InvalidOperationException(
                        "Bir organizasyon birimi kendisine " +
                        "veya kendi alt birimine bağlanamaz.");
                }

                // Veritabanındaki mevcut döngüleri de tespit et.
                if (!visitedIds.Add(id))
                {
                    throw new InvalidOperationException(
                        "Organizasyon hiyerarşisinde " +
                        "döngü tespit edildi.");
                }

                var ancestor = await _unitRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

                if (ancestor == null)
                {
                    throw new InvalidOperationException(
                        "Seçilen üst birim veya bağlı olduğu " +
                        "üst birimlerden biri bulunamadı.");
                }

                // Doğrudan seçilen üst birim aktif olmalıdır.
                if (id == parentUnitId.Value &&
                    !ancestor.IsActive)
                {
                    throw new InvalidOperationException(
                        "Pasif bir organizasyon birimi " +
                        "üst birim olarak seçilemez.");
                }

                ancestorId = ancestor.ParentUnitId;
            }
        }

        /// <summary>
        /// Birim adı ve kodu benzersizliğini kontrol eder.
        /// </summary>
        private async Task ValidateUniquenessAsync(
            string name,
            string code,
            int? parentUnitId,
            int? excludedId,
            CancellationToken cancellationToken)
        {
            // Birim adı, aynı üst birim altında benzersizdir.
            bool existsName = await _unitRepository
                .ExistsNameAsync(
                    name,
                    parentUnitId,
                    excludedId,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Aynı üst birime bağlı bu isimde " +
                    "başka bir organizasyon birimi bulunmaktadır.");
            }

            // Birim kodu bütün sistemde benzersizdir.
            bool existsCode = await _unitRepository
                .ExistsCodeAsync(
                    code,
                    excludedId,
                    cancellationToken);

            if (existsCode)
            {
                throw new InvalidOperationException(
                    "Bu birim koduyla başka bir kayıt bulunmaktadır.");
            }
        }

        #endregion
    }
}
