using AutoMapper;
using MaintenanceManagement.Application.DTOS.AssetUserDtos;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Application.Services
{
    public class AssetUserService : IAssetUserService
    {
        private readonly IAssetUserRepository _assetUserRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public AssetUserService(
            IAssetUserRepository assetUserRepository,
            IAssetRepository assetRepository,
            IUserRepository userRepository,
            IUnitRepository unitRepository,
            IMapper mapper)
        {
            _assetUserRepository = assetUserRepository;
            _assetRepository = assetRepository;
            _userRepository = userRepository;
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Bütün zimmet kayıtlarını ilişkili
        /// demirbaş ve kullanıcı bilgileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetUserDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var assignments = await _assetUserRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetUserDto>>(
                assignments);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Zimmet kaydını detay bilgileriyle getirir.
        /// </summary>
        public async Task<GetByIdAssetUserDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var assignment = await _assetUserRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdAssetUserDto>(
                assignment);
        }

        #endregion

        #region GetByAssetId

        /// <summary>
        /// Belirtilen demirbaşa ait
        /// bütün zimmet geçmişini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetUserDto>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            var assignments = await _assetUserRepository
                .GetByAssetIdAsync(
                    assetId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetUserDto>>(
                assignments);
        }

        #endregion

        #region GetByUserId

        /// <summary>
        /// Kullanıcının aktif ve iade edilmiş
        /// bütün zimmetlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetUserDto>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var assignments = await _assetUserRepository
                .GetByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetUserDto>>(
                assignments);
        }

        #endregion

        #region GetActiveAssignments

        /// <summary>
        /// Sistemdeki aktif zimmetleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetUserDto>>
            GetActiveAssignmentsAsync(
                CancellationToken cancellationToken = default)
        {
            var assignments = await _assetUserRepository
                .GetActiveAssignmentsAsync(
                    cancellationToken);

            return _mapper.Map<List<ResultAssetUserDto>>(
                assignments);
        }

        #endregion

        #region GetActiveAssignmentByAssetId

        /// <summary>
        /// Demirbaşın mevcut aktif zimmetini getirir.
        /// </summary>
        public async Task<GetByIdAssetUserDto?>
            GetActiveAssignmentByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            var assignment = await _assetUserRepository
                .GetActiveAssignmentByAssetIdAsync(
                    assetId,
                    cancellationToken);

            if (assignment == null)
            {
                return null;
            }

            // Detay DTO'sunda ilişkisel bilgileri
            // eksiksiz doldurabilmek için
            // detay sorgusunu kullan.
            return await GetByIdAsync(
                assignment.Id,
                cancellationToken);
        }

        #endregion

        #region GetActiveAssignmentsByUserId

        /// <summary>
        /// Kullanıcının aktif zimmetlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetUserDto>>
            GetActiveAssignmentsByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var assignments = await _assetUserRepository
                .GetActiveAssignmentsByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetUserDto>>(
                assignments);
        }

        #endregion

        #region GetAssignmentsMadeByUserId

        /// <summary>
        /// Belirtilen kullanıcının gerçekleştirdiği
        /// zimmet atama işlemlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetUserDto>>
            GetAssignmentsMadeByUserIdAsync(
                int assignedByUserId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                assignedByUserId,
                "Zimmet İşlemini Oluşturan Kullanıcı");

            var assignments = await _assetUserRepository
                .GetAssignmentsMadeByUserIdAsync(
                    assignedByUserId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetUserDto>>(
                assignments);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni demirbaş zimmet kaydı oluşturur.
        ///
        /// Aynı demirbaşın iade edilmemiş
        /// başka bir zimmet kaydı bulunamaz.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateAssetUserDto dto,
            int? assignedByUserId = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // ==========================================
            // 1. DTO DOĞRULAMASI
            // ==========================================

            ValidateDto(dto);

            ValidateId(dto.AssetId, "Demirbaş");
            ValidateId(dto.UserId, "Kullanıcı");

            if (assignedByUserId.HasValue)
            {
                ValidateId(
                    assignedByUserId.Value,
                    "Zimmet İşlemini Oluşturan Kullanıcı");
            }

            // ==========================================
            // 2. AÇIKLAMA KONTROLÜ
            // ==========================================

            string? description =
                ValidateAndNormalizeDescription(
                    dto.Description);

            // ==========================================
            // 3. DEMİRBAŞ DOĞRULAMASI
            // ==========================================

            await ValidateAssetAsync(
                dto.AssetId,
                cancellationToken);

            // ==========================================
            // 4. ZİMMETLENECEK KULLANICI
            // ==========================================

            await ValidateUserAsync(
                dto.UserId,
                cancellationToken);

            // ==========================================
            // 5. İŞLEMİ OLUŞTURAN KULLANICI
            // ==========================================

            if (assignedByUserId.HasValue)
            {
                await ValidateAssignedByUserAsync(
                    assignedByUserId.Value,
                    cancellationToken);
            }

            // ==========================================
            // 6. MÜKERRER AÇIK ZİMMET KONTROLÜ
            // ==========================================

            bool hasUnreturnedAssignment =
                await _assetUserRepository
                    .HasUnreturnedAssignmentAsync(
                        dto.AssetId,
                        cancellationToken);

            if (hasUnreturnedAssignment)
            {
                throw new InvalidOperationException(
                    "Bu demirbaşın iade edilmemiş " +
                    "başka bir zimmet kaydı bulunmaktadır. " +
                    "Yeni zimmet oluşturulmadan önce " +
                    "mevcut zimmet iade edilmelidir.");
            }

            // ==========================================
            // 7. DTO -> ENTITY
            // ==========================================

            var assignment = _mapper.Map<AssetUser>(dto);

            assignment.AssetId = dto.AssetId;
            assignment.UserId = dto.UserId;

            assignment.AssignedByUserId = assignedByUserId;

            assignment.Description = description;

            // ==========================================
            // 8. ZİMMET TARİHLERİ
            // ==========================================

            assignment.StartDate = DateTime.UtcNow;

            assignment.EndDate = null;

            // ==========================================
            // 9. KAYIT İŞLEMİ
            // ==========================================

            var createdAssignment = await _assetUserRepository
                .CreateAsync(
                    assignment,
                    cancellationToken);

            return createdAssignment.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Açık zimmet kaydının açıklamasını günceller.
        ///
        /// AssetId, UserId, AssignedByUserId,
        /// StartDate ve EndDate değiştirilmez.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateAssetUserDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Zimmet Kaydı");

            string? description =
                ValidateAndNormalizeDescription(
                    dto.Description);

            // ==========================================
            // 1. MEVCUT KAYDI GETİR
            // ==========================================

            var existingAssignment = await _assetUserRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingAssignment == null)
            {
                return false;
            }

            // ==========================================
            // 2. ZİMMET DURUMUNU KONTROL ET
            // ==========================================

            if (existingAssignment.EndDate.HasValue)
            {
                throw new InvalidOperationException(
                    "İade edilmiş zimmet kaydı normal " +
                    "güncelleme işlemine kapalıdır.");
            }

            // ==========================================
            // 3. DTO -> MEVCUT ENTITY
            // ==========================================

            _mapper.Map(dto, existingAssignment);

            existingAssignment.Description = description;

            // ==========================================
            // 4. GÜNCELLEME
            // ==========================================

            return await _assetUserRepository
                .UpdateAsync(
                    existingAssignment,
                    cancellationToken);
        }

        #endregion

        #region ReturnAsset

        /// <summary>
        /// Demirbaş zimmetini iade ederek sonlandırır.
        /// İade tarihi sistem tarafından UTC atanır.
        /// </summary>
        public async Task<bool> ReturnAssetAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return false;
            }

            // ==========================================
            // 1. ZİMMET KAYDINI GETİR
            // ==========================================

            var assignment = await _assetUserRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (assignment == null)
            {
                return false;
            }

            // ==========================================
            // 2. DAHA ÖNCE İADE EDİLMİŞ Mİ?
            // ==========================================

            if (assignment.EndDate.HasValue)
            {
                throw new InvalidOperationException(
                    "Bu demirbaşın zimmet iadesi " +
                    "daha önce gerçekleştirilmiştir.");
            }

            // ==========================================
            // 3. İADE TARİHİNİ BELİRLE
            // ==========================================

            DateTime returnedAtUtc = DateTime.UtcNow;

            if (returnedAtUtc < assignment.StartDate)
            {
                throw new InvalidOperationException(
                    "Zimmet iade tarihi, zimmet başlangıç " +
                    "tarihinden önce olamaz.");
            }

            // ==========================================
            // 4. İADE İŞLEMİ
            // ==========================================

            // Repository EndDate alanını günceller
            // ve değişikliği veritabanına kaydeder.
            return await _assetUserRepository
                .ReturnAssetAsync(
                    id,
                    returnedAtUtc,
                    cancellationToken);
        }

        #endregion

        #region HasActiveAssignment

        /// <summary>
        /// Demirbaşa ait aktif zimmet var mı kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssignmentAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            return await _assetUserRepository
                .HasActiveAssignmentAsync(
                    assetId,
                    cancellationToken);
        }

        #endregion

        #region HasUnreturnedAssignment

        /// <summary>
        /// Demirbaşa ait EndDate alanı boş
        /// olan zimmet kaydı var mı kontrol eder.
        /// </summary>
        public async Task<bool> HasUnreturnedAssignmentAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            return await _assetUserRepository
                .HasUnreturnedAssignmentAsync(
                    assetId,
                    cancellationToken);
        }

        #endregion

        #region HasActiveAssignmentsByUserId

        /// <summary>
        /// Kullanıcının aktif zimmeti bulunuyor mu kontrol eder.
        /// </summary>
        public async Task<bool> HasActiveAssignmentsByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            return await _assetUserRepository
                .HasActiveAssignmentsByUserIdAsync(
                    userId,
                    cancellationToken);
        }

        #endregion

        #region IsAssetAssignedToUser

        /// <summary>
        /// Belirli demirbaşın seçilen kullanıcıya
        /// aktif zimmetli olup olmadığını kontrol eder.
        /// </summary>
        public async Task<bool> IsAssetAssignedToUserAsync(
            int assetId,
            int userId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");
            ValidateId(userId, "Kullanıcı");

            return await _assetUserRepository
                .IsAssetAssignedToUserAsync(
                    assetId,
                    userId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO DataAnnotations doğrulamalarını çalıştırır.
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
        /// ID bilgisinin sıfırdan büyük olduğunu doğrular.
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
        /// Açıklama alanını doğrular ve temizler.
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
                    "Zimmet açıklaması en fazla " +
                    "500 karakter olabilir.");
            }

            return normalizedDescription;
        }

        /// <summary>
        /// Demirbaşın mevcut, aktif ve
        /// kullanılabilir durumda olduğunu doğrular.
        /// </summary>
        private async Task ValidateAssetAsync(
            int assetId,
            CancellationToken cancellationToken)
        {
            // ==========================================
            // ASSET
            // ==========================================

            var asset = await _assetRepository
                .GetByIdAsync(
                    assetId,
                    cancellationToken);

            if (asset == null)
            {
                throw new InvalidOperationException(
                    "Zimmetlenecek demirbaş bulunamadı.");
            }

            if (asset.Status != AssetStatusEnum.Active)
            {
                throw new InvalidOperationException(
                    "Yalnızca aktif durumdaki demirbaşlar " +
                    "personele zimmetlenebilir.");
            }

            // ==========================================
            // UNIT
            // ==========================================

            var unit = await _unitRepository
                .GetByIdAsync(
                    asset.UnitId,
                    cancellationToken);

            if (unit == null)
            {
                throw new InvalidOperationException(
                    "Demirbaşın bağlı olduğu " +
                    "organizasyon birimi bulunamadı.");
            }

            if (!unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif organizasyon birimine bağlı " +
                    "demirbaş zimmetlenemez.");
            }
        }

        /// <summary>
        /// Zimmetlenecek kullanıcının mevcut ve
        /// aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateUserAsync(
            int userId,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByIdAsync(
                    userId,
                    cancellationToken);

            if (user == null)
            {
                throw new InvalidOperationException(
                    "Zimmetlenecek kullanıcı bulunamadı.");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif kullanıcıya demirbaş zimmetlenemez.");
            }

            // Kullanıcının HasSystemAccess değeri
            // zimmet alabilmesi için zorunlu değildir.
        }

        /// <summary>
        /// Zimmet işlemini gerçekleştiren
        /// kullanıcının mevcut, aktif ve
        /// sistem erişimine sahip olduğunu doğrular.
        /// </summary>
        private async Task ValidateAssignedByUserAsync(
            int assignedByUserId,
            CancellationToken cancellationToken)
        {
            var assignedByUser = await _userRepository
                .GetByIdAsync(
                    assignedByUserId,
                    cancellationToken);

            if (assignedByUser == null)
            {
                throw new InvalidOperationException(
                    "Zimmet işlemini gerçekleştiren " +
                    "kullanıcı bulunamadı.");
            }

            if (!assignedByUser.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif kullanıcı zimmet işlemi " +
                    "gerçekleştiremez.");
            }

            if (!assignedByUser.HasSystemAccess)
            {
                throw new InvalidOperationException(
                    "Sistem erişimi bulunmayan kullanıcı " +
                    "zimmet işlemi gerçekleştiremez.");
            }

            // Rol bazlı işlem yetkisi ayrıca
            // Controller/API yetkilendirme katmanında
            // doğrulanmalıdır.
        }

        #endregion
    }
}
