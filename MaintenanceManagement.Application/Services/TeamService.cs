using AutoMapper;
using MaintenanceManagement.Application.DTOS.TeamDtos;
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
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public TeamService(
            ITeamRepository teamRepository,
            IUnitRepository unitRepository,
            IMapper mapper)
        {
            _teamRepository = teamRepository;
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Sistemdeki bütün ekipleri ilişkili
        /// organizasyon birimi bilgileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var teams = await _teamRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultTeamDto>>(teams);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Yalnızca aktif ekipleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamDto>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var teams = await _teamRepository
                .GetActiveTeamsAsync(cancellationToken);

            return _mapper.Map<List<ResultTeamDto>>(teams);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Ekibin detaylarını, aktif üye sayısını
        /// ve bağlı iş emri sayısını getirir.
        /// </summary>
        public async Task<GetByIdTeamDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var team = await _teamRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (team == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdTeamDto>(team);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Belirtilen organizasyon birimine ait
        /// aktif ve pasif bütün ekipleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var teams = await _teamRepository
                .GetTeamsByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultTeamDto>>(teams);
        }

        #endregion

        #region GetActiveByUnitId

        /// <summary>
        /// Belirtilen organizasyon birimindeki
        /// aktif ekipleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamDto>>
            GetActiveByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var teams = await _teamRepository
                .GetActiveTeamsByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultTeamDto>>(teams);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni ekip oluşturur.
        /// Ekip adı ve kodu aynı birim içinde benzersizdir.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateTeamDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // DTO DataAnnotations doğrulaması
            ValidateDto(dto);

            ValidateId(dto.UnitId, "Organizasyon Birimi");

            ValidateTeamFields(
                dto.Name,
                dto.Code);

            string name = dto.Name.Trim();
            string code = dto.Code.Trim();

            string? description =
                NormalizeDescription(dto.Description);

            // Seçilen organizasyon birimi
            // mevcut ve aktif olmalıdır.
            await ValidateUnitAsync(
                dto.UnitId,
                cancellationToken);

            // Aynı birimde mükerrer ekip adı veya
            // kodu bulunmamalıdır.
            await ValidateUniquenessAsync(
                dto.UnitId,
                name,
                code,
                null,
                cancellationToken);

            // CreateTeamDto -> Team
            var team = _mapper.Map<Team>(dto);

            team.UnitId = dto.UnitId;
            team.Name = name;
            team.Code = code;
            team.Description = description;

            // Yeni ekip aktif oluşturulur.
            team.IsActive = true;

            // Kayıt işlemi GenericRepository üzerinden yapılır.
            var createdTeam = await _teamRepository
                .CreateAsync(
                    team,
                    cancellationToken);

            return createdTeam.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut ekibin adı, kodu ve açıklamasını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateTeamDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Ekip");
            ValidateId(dto.UnitId, "Organizasyon Birimi");

            ValidateTeamFields(
                dto.Name,
                dto.Code);

            // Güncellenecek ekibi getir.
            var existingTeam = await _teamRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingTeam == null)
            {
                return false;
            }

            // Ekibin organizasyon birimi normal güncellemede
            // değiştirilemez. Böylece eski iş emri ve
            // personel kayıtlarının birim ilişkisi korunur.
            if (existingTeam.UnitId != dto.UnitId)
            {
                throw new InvalidOperationException(
                    "Ekibin bağlı olduğu organizasyon birimi " +
                    "normal güncelleme işleminde değiştirilemez.");
            }

            string name = dto.Name.Trim();
            string code = dto.Code.Trim();

            string? description =
                NormalizeDescription(dto.Description);

            // Organizasyon birimini doğrula.
            await ValidateUnitAsync(
                existingTeam.UnitId,
                cancellationToken);

            // Kendi ID değerini mükerrerlik
            // sorgularından hariç tut.
            await ValidateUniquenessAsync(
                existingTeam.UnitId,
                name,
                code,
                existingTeam.Id,
                cancellationToken);

            // UpdateTeamDto -> Mevcut Team Entity
            _mapper.Map(dto, existingTeam);

            existingTeam.Name = name;
            existingTeam.Code = code;
            existingTeam.Description = description;

            // Id, UnitId, IsActive, CreatedAt ve
            // navigation koleksiyonları korunur.
            return await _teamRepository
                .UpdateAsync(
                    existingTeam,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Ekibin aktif veya pasif durumunu değiştirir.
        /// Aktif üyeleri veya açık iş emirleri olan
        /// ekiplerin pasife alınmasına izin verilmez.
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

            var existingTeam = await _teamRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingTeam == null)
            {
                return false;
            }

            if (existingTeam.IsActive == isActive)
            {
                return true;
            }

            // Pasif ekip yeniden aktifleştirilecekse
            // organizasyon biriminin aktif olması gerekir.
            if (isActive)
            {
                await ValidateUnitAsync(
                    existingTeam.UnitId,
                    cancellationToken);
            }
            else
            {
                // Aktif ekip üyeleri kontrolü
                bool hasActiveMembers = await _teamRepository
                    .HasActiveMembersAsync(
                        id,
                        cancellationToken);

                if (hasActiveMembers)
                {
                    throw new InvalidOperationException(
                        "Ekipte aktif üyeler bulunduğu için " +
                        "ekip pasife alınamaz. Öncelikle aktif " +
                        "ekip üyeliklerini sonlandırınız.");
                }

                // Açık iş emri kontrolü
                bool hasOpenWorkOrders = await _teamRepository
                    .HasOpenWorkOrdersAsync(
                        id,
                        cancellationToken);

                if (hasOpenWorkOrders)
                {
                    throw new InvalidOperationException(
                        "Ekibe bağlı açık iş emirleri bulunduğu " +
                        "için ekip pasife alınamaz. Öncelikle " +
                        "ilgili iş emirlerini sonuçlandırınız.");
                }
            }

            return await _teamRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsName

        /// <summary>
        /// Birim bazında ekip adı benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsNameAsync(
            int unitId,
            string name,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Ekip adı boş olamaz.",
                    nameof(name));
            }

            return await _teamRepository
                .ExistsNameAsync(
                    unitId,
                    name.Trim(),
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Birim bazında ekip kodu benzersizliğini kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            int unitId,
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException(
                    "Ekip kodu boş olamaz.",
                    nameof(code));
            }

            return await _teamRepository
                .ExistsCodeAsync(
                    unitId,
                    code.Trim(),
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveMembers

        /// <summary>
        /// Ekipte aktif üyeler bulunup
        /// bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveMembersAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            if (teamId <= 0)
            {
                return false;
            }

            return await _teamRepository
                .HasActiveMembersAsync(
                    teamId,
                    cancellationToken);
        }

        #endregion

        #region HasOpenWorkOrders

        /// <summary>
        /// Ekibe bağlı açık iş emri
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrdersAsync(
            int teamId,
            CancellationToken cancellationToken = default)
        {
            if (teamId <= 0)
            {
                return false;
            }

            return await _teamRepository
                .HasOpenWorkOrdersAsync(
                    teamId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerinde bulunan DataAnnotations
        /// doğrulamalarını gerçekleştirir.
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
        /// Ekip adı ve kodunun zorunlu olmasını sağlar.
        /// Karakter uzunluğu sınırları mevcut DTO
        /// üzerindeki DataAnnotations ile doğrulanır.
        /// </summary>
        private static void ValidateTeamFields(
            string? name,
            string? code)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Ekip adı zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException(
                    "Ekip kodu zorunludur.");
            }
        }

        /// <summary>
        /// Açıklama alanını düzenler.
        /// Boş açıklama null olarak saklanır.
        /// </summary>
        private static string? NormalizeDescription(
            string? description)
        {
            return string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim();
        }

        /// <summary>
        /// Organizasyon biriminin mevcut ve aktif
        /// olduğunu doğrular.
        /// </summary>
        private async Task ValidateUnitAsync(
            int unitId,
            CancellationToken cancellationToken)
        {
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
                    "Pasif organizasyon birimine ekip atanamaz.");
            }
        }

        /// <summary>
        /// Aynı birimde aynı ad veya kodla
        /// ikinci ekip oluşturulmasını engeller.
        /// </summary>
        private async Task ValidateUniquenessAsync(
            int unitId,
            string name,
            string code,
            int? excludedId,
            CancellationToken cancellationToken)
        {
            // Ekip adı kontrolü
            bool existsName = await _teamRepository
                .ExistsNameAsync(
                    unitId,
                    name,
                    excludedId,
                    cancellationToken);

            if (existsName)
            {
                throw new InvalidOperationException(
                    "Bu organizasyon biriminde aynı ekip " +
                    "adıyla başka bir kayıt bulunmaktadır.");
            }

            // Ekip kodu kontrolü
            bool existsCode = await _teamRepository
                .ExistsCodeAsync(
                    unitId,
                    code,
                    excludedId,
                    cancellationToken);

            if (existsCode)
            {
                throw new InvalidOperationException(
                    "Bu organizasyon biriminde aynı ekip " +
                    "koduyla başka bir kayıt bulunmaktadır.");
            }
        }

        #endregion
    }
}
