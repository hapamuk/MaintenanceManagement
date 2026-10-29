using AutoMapper;
using MaintenanceManagement.Application.DTOS.TeamMemberDtos;
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
    public class TeamMemberService : ITeamMemberService
    {
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public TeamMemberService(
            ITeamMemberRepository teamMemberRepository,
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            IUnitRepository unitRepository,
            IMapper mapper)
        {
            _teamMemberRepository = teamMemberRepository;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Sistemdeki bütün ekip üyeliklerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamMemberDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var members = await _teamMemberRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultTeamMemberDto>>(
                members);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Ekip üyesinin detaylarını getirir.
        /// </summary>
        public async Task<GetByIdTeamMemberDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var member = await _teamMemberRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (member == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdTeamMemberDto>(
                member);
        }

        #endregion

        #region GetByTeamId

        /// <summary>
        /// Belirtilen ekibin bütün üyeliklerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamMemberDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Ekip");

            var members = await _teamMemberRepository
                .GetByTeamIdAsync(
                    teamId,
                    cancellationToken);

            return _mapper.Map<List<ResultTeamMemberDto>>(
                members);
        }

        #endregion

        #region GetActiveByTeamId

        /// <summary>
        /// Ekipteki aktif ve henüz sona ermemiş
        /// üyelikleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamMemberDto>>
            GetActiveByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Ekip");

            var members = await _teamMemberRepository
                .GetActiveByTeamIdAsync(
                    teamId,
                    cancellationToken);

            return _mapper.Map<List<ResultTeamMemberDto>>(
                members);
        }

        #endregion

        #region GetByUserId

        /// <summary>
        /// Kullanıcının bütün ekip üyeliklerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamMemberDto>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var members = await _teamMemberRepository
                .GetByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultTeamMemberDto>>(
                members);
        }

        #endregion

        #region GetActiveByUserId

        /// <summary>
        /// Kullanıcının aktif ekip üyeliklerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamMemberDto>>
            GetActiveByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var members = await _teamMemberRepository
                .GetActiveByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultTeamMemberDto>>(
                members);
        }

        #endregion

        #region GetActiveByUserAndTeam

        /// <summary>
        /// Kullanıcının belirtilen ekipte bulunan
        /// aktif üyelik kaydını getirir.
        /// </summary>
        public async Task<GetByIdTeamMemberDto?>
            GetActiveByUserAndTeamAsync(
                int userId,
                int teamId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");
            ValidateId(teamId, "Ekip");

            var member = await _teamMemberRepository
                .GetActiveByUserAndTeamAsync(
                    userId,
                    teamId,
                    cancellationToken);

            if (member == null)
            {
                return null;
            }

            return await GetByIdAsync(
                member.Id,
                cancellationToken);
        }

        #endregion

        #region GetEligibleForWorkOrder

        /// <summary>
        /// Seçilen iş alt türünde iş emrine atanabilecek
        /// ekip üyelerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultTeamMemberDto>>
            GetEligibleForWorkOrderAsync(
                int teamId,
                int unitId,
                int workSubTypeId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Ekip");
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workSubTypeId, "İş Alt Türü");

            var eligibleMembers = await _teamMemberRepository
                .GetEligibleForWorkOrderAsync(
                    teamId,
                    unitId,
                    workSubTypeId,
                    cancellationToken);

            return _mapper.Map<List<ResultTeamMemberDto>>(
                eligibleMembers);
        }

        #endregion

        #region IsEligibleForWorkOrder

        /// <summary>
        /// Bir ekip üyesinin seçilen iş emrine
        /// atanabilecek durumda olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> IsEligibleForWorkOrderAsync(
            int teamMemberId,
            int teamId,
            int unitId,
            int workSubTypeId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(teamMemberId, "Ekip Üyesi");
            ValidateId(teamId, "Ekip");
            ValidateId(unitId, "Organizasyon Birimi");
            ValidateId(workSubTypeId, "İş Alt Türü");

            return await _teamMemberRepository
                .IsEligibleForWorkOrderAsync(
                    teamMemberId,
                    teamId,
                    unitId,
                    workSubTypeId,
                    cancellationToken);
        }

        #endregion

        #region HasOpenWorkOrderAssignments

        /// <summary>
        /// Ekip üyesine ait açık iş emri
        /// görevlendirmelerini kontrol eder.
        /// </summary>
        public async Task<bool> HasOpenWorkOrderAssignmentsAsync(
            int teamMemberId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(teamMemberId, "Ekip Üyesi");

            return await _teamMemberRepository
                .HasOpenWorkOrderAssignmentsAsync(
                    teamMemberId,
                    cancellationToken);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni ekip üyeliği oluşturur.
        /// Aynı ekipte aynı kullanıcının ikinci aktif
        /// üyeliğinin oluşturulmasını engeller.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateTeamMemberDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.TeamId, "Ekip");
            ValidateId(dto.UserId, "Kullanıcı");

            // Ekip, kullanıcı ve birim kontrolleri.
            await ValidateTeamAndUserAsync(
                dto.TeamId,
                dto.UserId,
                cancellationToken);

            // Aynı ekibe aynı kullanıcı ikinci kez
            // aktif üye olarak eklenemez.
            bool existsActiveMembership =
                await _teamMemberRepository
                    .ExistsActiveMembershipAsync(
                        dto.TeamId,
                        dto.UserId,
                        null,
                        cancellationToken);

            if (existsActiveMembership)
            {
                throw new InvalidOperationException(
                    "Kullanıcının bu ekipte zaten aktif " +
                    "bir üyeliği bulunmaktadır.");
            }

            // DTO -> Entity dönüşümü.
            var member = _mapper.Map<TeamMember>(dto);

            member.TeamId = dto.TeamId;
            member.UserId = dto.UserId;

            // Üyelik başlangıç tarihi.
            member.JoinedAt = DateTime.UtcNow;

            // Yeni üyelik henüz sonlandırılmamıştır.
            member.LeftAt = null;

            // Yeni kayıt aktif oluşturulur.
            member.IsActive = true;

            var createdMember = await _teamMemberRepository
                .CreateAsync(
                    member,
                    cancellationToken);

            return createdMember.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut ekip üyeliğinin kullanıcı veya
        /// ekip bağlantısını kontrollü şekilde günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateTeamMemberDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Ekip Üyeliği");
            ValidateId(dto.TeamId, "Ekip");
            ValidateId(dto.UserId, "Kullanıcı");

            var existingMember = await _teamMemberRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingMember == null)
            {
                return false;
            }

            // Sona ermiş üyelikler değiştirilemez.
            if (existingMember.LeftAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Sona ermiş bir ekip üyeliği " +
                    "güncellenemez. Yeni üyelik oluşturunuz.");
            }

            bool relationChanged =
                existingMember.TeamId != dto.TeamId ||
                existingMember.UserId != dto.UserId;

            // İlişkide değişiklik yoksa tekrar kaydetme.
            if (!relationChanged)
            {
                return true;
            }

            // Açık iş emri görevlendirmesi bulunan
            // üyenin ekip veya kullanıcı ilişkisi
            // değiştirilemez.
            bool hasOpenAssignments =
                await _teamMemberRepository
                    .HasOpenWorkOrderAssignmentsAsync(
                        existingMember.Id,
                        cancellationToken);

            if (hasOpenAssignments)
            {
                throw new InvalidOperationException(
                    "Ekip üyesine ait açık iş emirleri " +
                    "bulunduğu için üyelik değiştirilemez.");
            }

            // Yeni ekip ve kullanıcı ilişkilerini doğrula.
            await ValidateTeamAndUserAsync(
                dto.TeamId,
                dto.UserId,
                cancellationToken);

            // Hedef ekipte mevcut aktif üyelik kontrolü.
            bool existsActiveMembership =
                await _teamMemberRepository
                    .ExistsActiveMembershipAsync(
                        dto.TeamId,
                        dto.UserId,
                        existingMember.Id,
                        cancellationToken);

            if (existsActiveMembership)
            {
                throw new InvalidOperationException(
                    "Kullanıcının seçilen ekipte başka bir " +
                    "aktif üyelik kaydı bulunmaktadır.");
            }

            // DTO -> Mevcut Entity.
            // Id, JoinedAt, LeftAt, IsActive ve
            // tarih alanları korunur.
            _mapper.Map(dto, existingMember);

            return await _teamMemberRepository
                .UpdateAsync(
                    existingMember,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Ekip üyeliğinin aktiflik durumunu değiştirir.
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

            var member = await _teamMemberRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (member == null)
            {
                return false;
            }

            // Sona ermiş bir üyelik tekrar aktif yapılamaz.
            if (isActive && member.LeftAt.HasValue)
            {
                throw new InvalidOperationException(
                    "Sona ermiş ekip üyeliği yeniden " +
                    "aktif hale getirilemez.");
            }

            if (member.IsActive == isActive)
            {
                return true;
            }

            if (isActive)
            {
                // Kullanıcı, ekip ve birim aktif olmalıdır.
                await ValidateTeamAndUserAsync(
                    member.TeamId,
                    member.UserId,
                    cancellationToken);

                // Aynı kullanıcı-ekip ikilisinde başka
                // aktif üyelik bulunmamalıdır.
                bool existsActiveMembership =
                    await _teamMemberRepository
                        .ExistsActiveMembershipAsync(
                            member.TeamId,
                            member.UserId,
                            member.Id,
                            cancellationToken);

                if (existsActiveMembership)
                {
                    throw new InvalidOperationException(
                        "Kullanıcının bu ekipte başka bir " +
                        "aktif üyeliği bulunmaktadır.");
                }
            }
            else
            {
                // Açık iş emrine atanmış üyeyi pasife alma.
                bool hasOpenAssignments =
                    await _teamMemberRepository
                        .HasOpenWorkOrderAssignmentsAsync(
                            id,
                            cancellationToken);

                if (hasOpenAssignments)
                {
                    throw new InvalidOperationException(
                        "Ekip üyesinin açık iş emri " +
                        "görevlendirmesi bulunmaktadır. " +
                        "Öncelikle görevleri sonuçlandırınız.");
                }
            }

            return await _teamMemberRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region EndMembership

        /// <summary>
        /// Ekip üyesinin üyeliğini sona erdirir.
        /// </summary>
        public async Task<bool> EndMembershipAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            var member = await _teamMemberRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (member == null)
            {
                return false;
            }

            // Üyelik zaten sonlandırılmış.
            if (member.LeftAt.HasValue)
            {
                return false;
            }

            // Açık iş emri bulunan üyelik sonlandırılamaz.
            bool hasOpenAssignments =
                await _teamMemberRepository
                    .HasOpenWorkOrderAssignmentsAsync(
                        id,
                        cancellationToken);

            if (hasOpenAssignments)
            {
                throw new InvalidOperationException(
                    "Ekip üyesine ait açık iş emri " +
                    "görevlendirmesi bulunmaktadır. " +
                    "Üyelik sonlandırılamaz.");
            }

            // Repository, LeftAt ve IsActive alanlarını
            // birlikte günceller.
            return await _teamMemberRepository
                .EndMembershipAsync(
                    id,
                    cancellationToken);
        }

        #endregion

        #region ExistsActiveMembership

        /// <summary>
        /// Ekip ve kullanıcı için başka aktif
        /// üyelik bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsActiveMembershipAsync(
            int teamId,
            int userId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Ekip");
            ValidateId(userId, "Kullanıcı");

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulan Üyelik");
            }

            return await _teamMemberRepository
                .ExistsActiveMembershipAsync(
                    teamId,
                    userId,
                    excludedId,
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
        /// ID bilgisinin geçerli olduğunu doğrular.
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
        /// Ekip, kullanıcı ve ekibin bağlı olduğu
        /// organizasyon biriminin mevcut ve
        /// aktif olduğunu kontrol eder.
        /// </summary>
        private async Task ValidateTeamAndUserAsync(
            int teamId,
            int userId,
            CancellationToken cancellationToken)
        {
            // ===================================================
            // TEAM
            // ===================================================

            var team = await _teamRepository
                .GetByIdAsync(
                    teamId,
                    cancellationToken);

            if (team == null)
            {
                throw new InvalidOperationException(
                    "Seçilen ekip bulunamadı.");
            }

            if (!team.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif ekibe personel atanamaz.");
            }

            // ===================================================
            // UNIT
            // ===================================================

            var unit = await _unitRepository
                .GetByIdAsync(
                    team.UnitId,
                    cancellationToken);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    "Ekibin bağlı olduğu organizasyon " +
                    "birimi bulunamadı.");
            }

            if (!unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif organizasyon birimindeki " +
                    "ekibe personel atanamaz.");
            }

            // ===================================================
            // USER
            // ===================================================

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
                    "Pasif kullanıcı ekibe atanamaz.");
            }

            // Kullanıcı PrimaryUnitId ile Team.UnitId
            // aynı olmak zorunda değildir.
            //
            // İş emri bazında görevlendirme uygunluğu
            // ayrıca Repository üzerinde kontrol edilir.
        }

        #endregion
    }
}
