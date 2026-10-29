using AutoMapper;
using MaintenanceManagement.Application.DTOS.AssetMovementDtos;
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
    public class AssetMovementService : IAssetMovementService
    {
        private readonly IAssetMovementRepository _assetMovementRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAssetUserRepository _assetUserRepository;
        private readonly IMapper _mapper;

        public AssetMovementService(
            IAssetMovementRepository assetMovementRepository,
            IAssetRepository assetRepository,
            IUnitRepository unitRepository,
            IUserRepository userRepository,
            IAssetUserRepository assetUserRepository,
            IMapper mapper)
        {
            _assetMovementRepository = assetMovementRepository;
            _assetRepository = assetRepository;
            _unitRepository = unitRepository;
            _userRepository = userRepository;
            _assetUserRepository = assetUserRepository;
            _mapper = mapper;
        }

        #region GetAll

        /// <summary>
        /// Bütün demirbaş hareketlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetMovementDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var movements = await _assetMovementRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return _mapper.Map<List<ResultAssetMovementDto>>(
                movements);
        }

        #endregion

        #region GetById

        /// <summary>
        /// Hareket kaydını detaylarıyla getirir.
        /// </summary>
        public async Task<GetByIdAssetMovementDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var movement = await _assetMovementRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (movement == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdAssetMovementDto>(
                movement);
        }

        #endregion

        #region GetByAssetId

        /// <summary>
        /// Demirbaşa ait bütün hareketleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetMovementDto>>
            GetByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            var movements = await _assetMovementRepository
                .GetMovementsByAssetIdAsync(
                    assetId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetMovementDto>>(
                movements);
        }

        #endregion

        #region GetLatestByAssetId

        /// <summary>
        /// Demirbaşın en son hareketini getirir.
        /// </summary>
        public async Task<GetByIdAssetMovementDto?>
            GetLatestByAssetIdAsync(
                int assetId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            var movement = await _assetMovementRepository
                .GetLatestMovementByAssetIdAsync(
                    assetId,
                    cancellationToken);

            if (movement == null)
            {
                return null;
            }

            return await GetByIdAsync(
                movement.Id,
                cancellationToken);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Belirtilen organizasyon birimiyle
        /// ilişkili hareketleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetMovementDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Organizasyon Birimi");

            var movements = await _assetMovementRepository
                .GetMovementsByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetMovementDto>>(
                movements);
        }

        #endregion

        #region GetByUserId

        /// <summary>
        /// Kullanıcıyla ilişkili hareketleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetMovementDto>>
            GetByUserIdAsync(
                int userId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            var movements = await _assetMovementRepository
                .GetMovementsByUserIdAsync(
                    userId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetMovementDto>>(
                movements);
        }

        #endregion

        #region GetByPerformedUserId

        /// <summary>
        /// Belirtilen kullanıcının gerçekleştirdiği
        /// hareketleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetMovementDto>>
            GetByPerformedUserIdAsync(
                int performedByUserId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                performedByUserId,
                "İşlemi Gerçekleştiren Kullanıcı");

            var movements = await _assetMovementRepository
                .GetMovementsByPerformedUserIdAsync(
                    performedByUserId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetMovementDto>>(
                movements);
        }

        #endregion

        #region GetByType

        /// <summary>
        /// Hareket türüne göre kayıtları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetMovementDto>>
            GetByTypeAsync(
                AssetMovementTypeEnum movementType,
                CancellationToken cancellationToken = default)
        {
            ValidateMovementType(movementType);

            var movements = await _assetMovementRepository
                .GetMovementsByTypeAsync(
                    movementType,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetMovementDto>>(
                movements);
        }

        #endregion

        #region GetByDateRange

        /// <summary>
        /// UTC tarih aralığındaki hareketleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetMovementDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "Tarih aralığı UTC olarak gönderilmelidir.");
            }

            if (endDateUtc < startDateUtc)
            {
                throw new ArgumentException(
                    "Bitiş tarihi başlangıç tarihinden " +
                    "önce olamaz.");
            }

            var movements = await _assetMovementRepository
                .GetMovementsByDateRangeAsync(
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetMovementDto>>(
                movements);
        }

        #endregion

        #region GetTransfersBetweenUnits

        /// <summary>
        /// Kaynak ve hedef birim arasındaki
        /// demirbaş devirlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultAssetMovementDto>>
            GetTransfersBetweenUnitsAsync(
                int fromUnitId,
                int toUnitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(fromUnitId, "Kaynak Birim");
            ValidateId(toUnitId, "Hedef Birim");

            if (fromUnitId == toUnitId)
            {
                throw new ValidationException(
                    "Kaynak ve hedef birim aynı olamaz.");
            }

            var movements = await _assetMovementRepository
                .GetTransfersBetweenUnitsAsync(
                    fromUnitId,
                    toUnitId,
                    cancellationToken);

            return _mapper.Map<List<ResultAssetMovementDto>>(
                movements);
        }

        #endregion

        #region Create

        /// <summary>
        /// Hareket türüne göre gerekli kontrolleri
        /// gerçekleştirerek yeni hareket kaydeder.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateAssetMovementDto dto,
            int performedByUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.AssetId, "Demirbaş");

            ValidateId(
                performedByUserId,
                "İşlemi Gerçekleştiren Kullanıcı");

            ValidateMovementType(dto.MovementType);

            string? description = NormalizeDescription(
                dto.Description);

            // Zimmet ve iade hareketlerinde AssetUser
            // kaydının da aynı transaction içerisinde
            // güncellenmesi gerekir.
            //
            // Mevcut RegisterMovementAsync bunu yapmadığı
            // için bu iki tür burada doğrudan kaydedilmez.
            if (dto.MovementType == AssetMovementTypeEnum.Assignment ||
                dto.MovementType == AssetMovementTypeEnum.Return)
            {
                throw new InvalidOperationException(
                    "Zimmet ve zimmet iadesi hareketleri, " +
                    "AssetUser kaydıyla aynı transaction " +
                    "içerisinde oluşturulmalıdır. Mevcut " +
                    "hareket Repository metodu tek başına " +
                    "bu işlemi desteklememektedir.");
            }

            // İşlem kullanıcısını doğrula.
            await ValidatePerformedByUserAsync(
                performedByUserId,
                cancellationToken);

            // Demirbaş kaydını getir.
            var asset = await _assetRepository
                .GetByIdAsync(
                    dto.AssetId,
                    cancellationToken);

            if (asset == null)
            {
                throw new InvalidOperationException(
                    "İşlem yapılacak demirbaş bulunamadı.");
            }

            int? newUnitId = null;

            AssetStatusEnum? newAssetStatus = null;

            // Hareket türüne göre iş kuralları.
            switch (dto.MovementType)
            {
                case AssetMovementTypeEnum.Registration:

                    await ValidateRegistrationAsync(
                        dto,
                        asset,
                        cancellationToken);

                    break;

                case AssetMovementTypeEnum.Transfer:

                    await ValidateTransferAsync(
                        dto,
                        asset,
                        cancellationToken);

                    newUnitId = dto.ToUnitId;

                    break;

                case AssetMovementTypeEnum.Scrap:

                    await ValidateScrapAsync(
                        dto,
                        asset,
                        cancellationToken);

                    newAssetStatus = AssetStatusEnum.Scrapped;

                    break;

                default:

                    throw new ValidationException(
                        "Desteklenmeyen hareket türü.");
            }

            // DTO -> Entity
            var movement = _mapper.Map<AssetMovement>(dto);

            movement.AssetId = dto.AssetId;

            movement.FromUnitId = dto.FromUnitId;
            movement.ToUnitId = dto.ToUnitId;

            movement.FromUserId = dto.FromUserId;
            movement.ToUserId = dto.ToUserId;

            movement.MovementType = dto.MovementType;

            movement.PerformedByUserId = performedByUserId;

            movement.MovementDate = DateTime.UtcNow;

            movement.Description = description;

            // Hareket, gerekli birim/durum değişikliğiyle
            // birlikte Repository üzerinden kaydedilir.
            var createdMovement =
                await _assetMovementRepository
                    .RegisterMovementAsync(
                        movement,
                        newUnitId,
                        newAssetStatus,
                        cancellationToken);

            return createdMovement.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut hareket kaydının yalnızca
        /// açıklama alanını günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateAssetMovementDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "Demirbaş Hareketi");

            string? description = NormalizeDescription(
                dto.Description);

            var existingMovement = await _assetMovementRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (existingMovement == null)
            {
                return false;
            }

            // DTO mapping yalnızca Description
            // alanını değiştirecek şekilde tanımlıdır.
            _mapper.Map(dto, existingMovement);

            existingMovement.Description = description;

            return await _assetMovementRepository
                .UpdateAsync(
                    existingMovement,
                    cancellationToken);
        }

        #endregion

        #region HasMovements

        /// <summary>
        /// Demirbaşa ait hareket kaydı var mı kontrol eder.
        /// </summary>
        public async Task<bool> HasMovementsAsync(
            int assetId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(assetId, "Demirbaş");

            return await _assetMovementRepository
                .HasMovementsAsync(
                    assetId,
                    cancellationToken);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO DataAnnotations kurallarını çalıştırır.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            var context = new ValidationContext(dto);

            Validator.ValidateObject(
                dto,
                context,
                validateAllProperties: true);
        }

        /// <summary>
        /// ID değerinin pozitif olduğunu doğrular.
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
        /// Hareket türünün geçerli Enum üyesi
        /// olduğunu doğrular.
        /// </summary>
        private static void ValidateMovementType(
            AssetMovementTypeEnum movementType)
        {
            if (!Enum.IsDefined(
                    typeof(AssetMovementTypeEnum),
                    movementType))
            {
                throw new ValidationException(
                    "Geçersiz demirbaş hareket türü.");
            }
        }

        /// <summary>
        /// Açıklamayı temizler.
        /// </summary>
        private static string? NormalizeDescription(
            string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            string normalized = description.Trim();

            if (normalized.Length > 1000)
            {
                throw new ValidationException(
                    "Hareket açıklaması en fazla " +
                    "1000 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// İşlemi gerçekleştiren kullanıcının
        /// mevcut, aktif ve sistem erişimine
        /// sahip olduğunu doğrular.
        /// </summary>
        private async Task ValidatePerformedByUserAsync(
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
                    "İşlemi gerçekleştiren kullanıcı bulunamadı.");
            }

            if (!user.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif kullanıcı demirbaş " +
                    "hareketi gerçekleştiremez.");
            }

            if (!user.HasSystemAccess)
            {
                throw new InvalidOperationException(
                    "Sistem erişimi bulunmayan kullanıcı " +
                    "demirbaş hareketi gerçekleştiremez.");
            }
        }

        /// <summary>
        /// Hedef organizasyon biriminin mevcut
        /// ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateTargetUnitAsync(
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
                    "Hedef organizasyon birimi bulunamadı.");
            }

            if (!unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Pasif organizasyon birimine " +
                    "demirbaş devredilemez.");
            }
        }

        /// <summary>
        /// İlk kayıt hareketini doğrular.
        /// </summary>
        private async Task ValidateRegistrationAsync(
            CreateAssetMovementDto dto,
            Asset asset,
            CancellationToken cancellationToken)
        {
            if (dto.FromUnitId.HasValue ||
                dto.FromUserId.HasValue ||
                dto.ToUserId.HasValue)
            {
                throw new ValidationException(
                    "İlk kayıt işleminde kaynak birim " +
                    "ve kullanıcı hareketi bulunamaz.");
            }

            if (!dto.ToUnitId.HasValue ||
                dto.ToUnitId.Value != asset.UnitId)
            {
                throw new ValidationException(
                    "İlk kayıt hedef birimi, demirbaşın " +
                    "mevcut birimiyle aynı olmalıdır.");
            }

            // Aynı demirbaşa birden fazla ilk kayıt
            // hareketi oluşturulmasını engelle.
            bool hasMovements = await _assetMovementRepository
                .HasMovementsAsync(
                    asset.Id,
                    cancellationToken);

            if (hasMovements)
            {
                throw new InvalidOperationException(
                    "Bu demirbaş için hareket geçmişi " +
                    "bulunmaktadır. İlk kayıt hareketi " +
                    "yeniden oluşturulamaz.");
            }

            if (asset.Status != AssetStatusEnum.Active)
            {
                throw new InvalidOperationException(
                    "İlk kayıt hareketi için demirbaş " +
                    "aktif durumda olmalıdır.");
            }

            await ValidateTargetUnitAsync(
                asset.UnitId,
                cancellationToken);
        }

        /// <summary>
        /// Birimler arası devir hareketini doğrular.
        /// </summary>
        private async Task ValidateTransferAsync(
            CreateAssetMovementDto dto,
            Asset asset,
            CancellationToken cancellationToken)
        {
            if (asset.Status == AssetStatusEnum.Scrapped)
            {
                throw new InvalidOperationException(
                    "Hurdaya ayrılmış demirbaş devredilemez.");
            }

            if (!dto.FromUnitId.HasValue ||
                !dto.ToUnitId.HasValue)
            {
                throw new ValidationException(
                    "Devir işleminde kaynak ve hedef " +
                    "birim zorunludur.");
            }

            if (dto.FromUnitId.Value != asset.UnitId)
            {
                throw new InvalidOperationException(
                    "Kaynak birim, demirbaşın güncel " +
                    "organizasyon birimiyle eşleşmiyor.");
            }

            if (dto.FromUnitId == dto.ToUnitId)
            {
                throw new ValidationException(
                    "Kaynak ve hedef birim aynı olamaz.");
            }

            if (dto.FromUserId.HasValue ||
                dto.ToUserId.HasValue)
            {
                throw new ValidationException(
                    "Birimler arası devirde kullanıcı " +
                    "zimmeti bu işlemle değiştirilmez. " +
                    "Öncelikle zimmet işlemlerini tamamlayınız.");
            }

            await ValidateTargetUnitAsync(
                dto.ToUnitId.Value,
                cancellationToken);

            // Açık zimmeti bulunan demirbaş doğrudan
            // başka bir birime devredilemez.
            bool hasUnreturnedAssignment =
                await _assetUserRepository
                    .HasUnreturnedAssignmentAsync(
                        asset.Id,
                        cancellationToken);

            if (hasUnreturnedAssignment)
            {
                throw new InvalidOperationException(
                    "Demirbaşın iade edilmemiş zimmeti " +
                    "bulunmaktadır. Birimler arası devirden " +
                    "önce zimmet iadesi yapılmalıdır.");
            }

            // Açık iş emri varsa işin sorumlu birimiyle
            // demirbaş birimi arasında tutarsızlık oluşabilir.
            bool hasOpenWorkOrders = await _assetRepository
                .HasOpenWorkOrdersAsync(
                    asset.Id,
                    cancellationToken);

            if (hasOpenWorkOrders)
            {
                throw new InvalidOperationException(
                    "Demirbaşa ait açık iş emirleri " +
                    "bulunduğu için devir yapılamaz.");
            }
        }

        /// <summary>
        /// Hurdaya ayırma hareketini doğrular.
        /// </summary>
        private async Task ValidateScrapAsync(
            CreateAssetMovementDto dto,
            Asset asset,
            CancellationToken cancellationToken)
        {
            if (asset.Status == AssetStatusEnum.Scrapped)
            {
                throw new InvalidOperationException(
                    "Demirbaş daha önce hurdaya ayrılmıştır.");
            }

            if (!dto.FromUnitId.HasValue ||
                dto.FromUnitId.Value != asset.UnitId)
            {
                throw new ValidationException(
                    "Hurdaya ayırma işleminde kaynak birim " +
                    "demirbaşın mevcut birimi olmalıdır.");
            }

            if (dto.ToUnitId.HasValue ||
                dto.FromUserId.HasValue ||
                dto.ToUserId.HasValue)
            {
                throw new ValidationException(
                    "Hurdaya ayırma hareketinde hedef birim " +
                    "ve kullanıcı değişikliği yapılamaz.");
            }

            // Açık iş emirleri
            bool hasOpenWorkOrders = await _assetRepository
                .HasOpenWorkOrdersAsync(
                    asset.Id,
                    cancellationToken);

            if (hasOpenWorkOrders)
            {
                throw new InvalidOperationException(
                    "Açık iş emri bulunan demirbaş " +
                    "hurdaya ayrılamaz.");
            }

            // Açık zimmetler
            bool hasUnreturnedAssignment =
                await _assetUserRepository
                    .HasUnreturnedAssignmentAsync(
                        asset.Id,
                        cancellationToken);

            if (hasUnreturnedAssignment)
            {
                throw new InvalidOperationException(
                    "İade edilmemiş zimmeti bulunan " +
                    "demirbaş hurdaya ayrılamaz.");
            }

            // Aktif bakım planları
            bool hasMaintenancePlans = await _assetRepository
                .HasActiveMaintenancePlansAsync(
                    asset.Id,
                    cancellationToken);

            if (hasMaintenancePlans)
            {
                throw new InvalidOperationException(
                    "Aktif bakım planları bulunan " +
                    "demirbaş hurdaya ayrılamaz.");
            }
        }
    }
        #endregion
    }
