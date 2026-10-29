using AutoMapper;
using MaintenanceManagement.Application.DTOS.StockMovementDtos;
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
    public class StockMovementService : IStockMovementService
    {
        #region Constants

        private const int MaxReferenceNumberLength = 100;

        private const int MaxDescriptionLength = 1000;

        private const int MaxTake = 1000;

        #endregion

        #region Fields

        private readonly IStockMovementRepository
            _stockMovementRepository;

        private readonly IWarehouseRepository
            _warehouseRepository;

        private readonly IMaterialRepository
            _materialRepository;

        private readonly IUserRepository
            _userRepository;

        private readonly IMapper
            _mapper;

        #endregion

        #region Constructor

        public StockMovementService(
            IStockMovementRepository stockMovementRepository,
            IWarehouseRepository warehouseRepository,
            IMaterialRepository materialRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _stockMovementRepository =
                stockMovementRepository
                ?? throw new ArgumentNullException(
                    nameof(stockMovementRepository));

            _warehouseRepository =
                warehouseRepository
                ?? throw new ArgumentNullException(
                    nameof(warehouseRepository));

            _materialRepository =
                materialRepository
                ?? throw new ArgumentNullException(
                    nameof(materialRepository));

            _userRepository =
                userRepository
                ?? throw new ArgumentNullException(
                    nameof(userRepository));

            _mapper = mapper
                ?? throw new ArgumentNullException(
                    nameof(mapper));
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün stok hareketlerini
        /// ilişkili bilgileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var movements = await _stockMovementRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return MapResults(
                movements
                    .OrderByDescending(x => x.MovementDate)
                    .ThenByDescending(x => x.Id));
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID değerine göre stok hareketi detayı.
        /// </summary>
        public async Task<GetByIdStockMovementDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var movement = await _stockMovementRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (movement == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdStockMovementDto>(
                movement);
        }

        #endregion

        #region GetByWarehouseId

        /// <summary>
        /// Ambara ait son stok hareketlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetByWarehouseIdAsync(
                int warehouseId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateId(warehouseId, "Ambar");

            ValidateTake(take);

            var movements = await _stockMovementRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    take,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                movements,
                cancellationToken);
        }

        #endregion

        #region GetByMaterialId

        /// <summary>
        /// Malzemeye ait son stok hareketleri.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetByMaterialIdAsync(
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateId(materialId, "Malzeme");

            ValidateTake(take);

            var movements = await _stockMovementRepository
                .GetByMaterialIdAsync(
                    materialId,
                    take,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                movements,
                cancellationToken);
        }

        #endregion

        #region GetByWarehouseAndMaterialId

        /// <summary>
        /// Belirli ambar ve malzeme için
        /// son stok hareketlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateId(warehouseId, "Ambar");

            ValidateId(materialId, "Malzeme");

            ValidateTake(take);

            var movements = await _stockMovementRepository
                .GetByWarehouseAndMaterialIdAsync(
                    warehouseId,
                    materialId,
                    take,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                movements,
                cancellationToken);
        }

        #endregion

        #region GetByPerformedByUserId

        /// <summary>
        /// Kullanıcının gerçekleştirdiği
        /// son stok hareketlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetByPerformedByUserIdAsync(
                int userId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            ValidateId(userId, "Kullanıcı");

            ValidateTake(take);

            var movements = await _stockMovementRepository
                .GetByPerformedByUserIdAsync(
                    userId,
                    take,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                movements,
                cancellationToken);
        }

        #endregion

        #region GetByDateRange

        /// <summary>
        /// UTC tarih aralığındaki hareketleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var movements = await _stockMovementRepository
                .GetByDateRangeAsync(
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                movements,
                cancellationToken);
        }

        #endregion

        #region GetByReferenceNumber

        /// <summary>
        /// Referans numarasına ait stok hareketlerini
        /// getirir. Aynı transferde iki hareket
        /// aynı referans numarasını taşıyabilir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetByReferenceNumberAsync(
                string referenceNumber,
                CancellationToken cancellationToken = default)
        {
            string normalizedReference =
                NormalizeRequiredReferenceNumber(
                    referenceNumber);

            var movements = await _stockMovementRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            var filteredMovements = movements
                .Where(x =>
                    string.Equals(
                        x.ReferenceNumber,
                        normalizedReference,
                        StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToList();

            return MapResults(filteredMovements);
        }

        #endregion

        #region GetByTransferGroupId

        /// <summary>
        /// Aynı transfer grubu içerisindeki
        /// stok giriş ve çıkış hareketlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetByTransferGroupIdAsync(
                Guid transferGroupId,
                CancellationToken cancellationToken = default)
        {
            if (transferGroupId == Guid.Empty)
            {
                throw new ValidationException(
                    "Geçerli bir transfer grubu seçiniz.");
            }

            var movements = await _stockMovementRepository
                .GetByTransferGroupIdAsync(
                    transferGroupId,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                movements,
                cancellationToken);
        }

        #endregion

        #region GetByMovementType

        /// <summary>
        /// Stok giriş veya çıkış türüne göre
        /// hareket kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetByMovementTypeAsync(
                StockMovementTypeEnum movementType,
                CancellationToken cancellationToken = default)
        {
            ValidateMovementType(movementType);

            var movements = await _stockMovementRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return MapResults(
                movements
                    .Where(x =>
                        x.MovementType == movementType)
                    .OrderByDescending(x => x.MovementDate)
                    .ThenByDescending(x => x.Id));
        }

        #endregion

        #region Create

        /// <summary>
        /// Stok giriş veya çıkış hareketi oluşturur.
        ///
        /// CreateStockMovementDto içinde
        /// PerformedByUserId, MovementDate ve
        /// TransferGroupId bulunmaz.
        ///
        /// Kullanıcı kimliği oturumdan alınmalıdır.
        /// Hareket tarihi UTC olarak oluşturulur.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateStockMovementDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.WarehouseId, "Ambar");

            ValidateId(dto.MaterialId, "Malzeme");

            ValidateId(currentUserId, "Kullanıcı");

            ValidateMovementType(dto.MovementType);

            ValidateQuantity(dto.Quantity);

            // =====================================
            // 2. REFERANS NUMARASI
            // =====================================

            string? referenceNumber =
                NormalizeReferenceNumber(
                    dto.ReferenceNumber);

            // =====================================
            // 3. AÇIKLAMA
            // =====================================

            string? description =
                NormalizeDescription(
                    dto.Description);

            // =====================================
            // 4. KULLANICI DOĞRULAMASI
            // =====================================

            await ValidateActorAsync(
                currentUserId,
                cancellationToken);

            // =====================================
            // 5. AMBAR DOĞRULAMASI
            // =====================================

            await ValidateWarehouseAsync(
                dto.WarehouseId,
                cancellationToken);

            // =====================================
            // 6. MALZEME DOĞRULAMASI
            // =====================================

            await ValidateMaterialAsync(
                dto.MaterialId,
                cancellationToken);

            // =====================================
            // 7. HAREKET TARİHİ
            // =====================================

            DateTime movementDateUtc =
                DateTime.UtcNow;

            // =====================================
            // 8. REPOSITORY KAYIT İŞLEMİ
            // =====================================

            // Bu metot hem StockMovement
            // kaydını hem de WarehouseStock
            // bakiyesini aynı transaction
            // içerisinde yönetmelidir.

            var createdMovement =
                await _stockMovementRepository
                    .RecordMovementAsync(
                        dto.WarehouseId,
                        dto.MaterialId,
                        currentUserId,
                        dto.MovementType,
                        dto.Quantity,
                        movementDateUtc,
                        referenceNumber,
                        description,
                        cancellationToken);

            // =====================================
            // 9. HAREKET ID DÖNDÜR
            // =====================================

            return createdMovement.Id;
        }

        #endregion

        #region TransferStock

        /// <summary>
        /// İki ambar arasında malzeme transferi yapar.
        ///
        /// Kaynak ambar: Out
        /// Hedef ambar: In
        ///
        /// İki hareket aynı TransferGroupId
        /// değerine sahip olmalıdır.
        /// </summary>
        public async Task<(int OutMovementId, int InMovementId)>
            TransferStockAsync(
                int fromWarehouseId,
                int toWarehouseId,
                int materialId,
                decimal quantity,
                int currentUserId,
                string? referenceNumber = null,
                string? description = null,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMALARI
            // =====================================

            ValidateId(
                fromWarehouseId,
                "Kaynak Ambar");

            ValidateId(
                toWarehouseId,
                "Hedef Ambar");

            ValidateId(
                materialId,
                "Malzeme");

            ValidateId(
                currentUserId,
                "Kullanıcı");

            // =====================================
            // 2. AYNI AMBAR KONTROLÜ
            // =====================================

            if (fromWarehouseId == toWarehouseId)
            {
                throw new ValidationException(
                    "Kaynak ve hedef ambar " +
                    "aynı olamaz.");
            }

            // =====================================
            // 3. MİKTAR DOĞRULAMASI
            // =====================================

            ValidateQuantity(quantity);

            // =====================================
            // 4. REFERANS VE AÇIKLAMA
            // =====================================

            string? normalizedReference =
                NormalizeReferenceNumber(
                    referenceNumber);

            string? normalizedDescription =
                NormalizeDescription(
                    description);

            // =====================================
            // 5. KULLANICI DOĞRULAMASI
            // =====================================

            await ValidateActorAsync(
                currentUserId,
                cancellationToken);

            // =====================================
            // 6. KAYNAK AMBAR
            // =====================================

            await ValidateWarehouseAsync(
                fromWarehouseId,
                cancellationToken);

            // =====================================
            // 7. HEDEF AMBAR
            // =====================================

            await ValidateWarehouseAsync(
                toWarehouseId,
                cancellationToken);

            // =====================================
            // 8. MALZEME
            // =====================================

            await ValidateMaterialAsync(
                materialId,
                cancellationToken);

            // =====================================
            // 9. TRANSFER TARİHİ
            // =====================================

            DateTime movementDateUtc =
                DateTime.UtcNow;

            // =====================================
            // 10. REPOSITORY TRANSFER İŞLEMİ
            // =====================================

            // Repository bu işlemi tek transaction'da
            // gerçekleştirmeli ve yetersiz stok
            // durumunda hiçbir kayıt oluşturmamalıdır.

            var transferResult =
                await _stockMovementRepository
                    .TransferStockAsync(
                        fromWarehouseId,
                        toWarehouseId,
                        materialId,
                        currentUserId,
                        quantity,
                        movementDateUtc,
                        normalizedReference,
                        normalizedDescription,
                        cancellationToken);

            // =====================================
            // 11. OLUŞTURULAN HAREKETLER
            // =====================================

            return (
                OutMovementId:
                    transferResult.OutMovement.Id,

                InMovementId:
                    transferResult.InMovement.Id
            );
        }

        #endregion

        #region Update

        /// <summary>
        /// Stok hareketinin yalnızca referans
        /// ve açıklama bilgilerini günceller.
        ///
        /// Miktar, hareket türü, ambar ve malzeme
        /// ilişkileri değiştirilmez.
        ///
        /// Transfer hareketleri bu normal
        /// güncelleme yöntemiyle değiştirilmez.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateStockMovementDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.Id, "Stok Hareketi");

            ValidateId(currentUserId, "Kullanıcı");

            // =====================================
            // 2. REFERANS VE AÇIKLAMA
            // =====================================

            string? referenceNumber =
                NormalizeReferenceNumber(
                    dto.ReferenceNumber);

            string? description =
                NormalizeDescription(
                    dto.Description);

            // =====================================
            // 3. KULLANICI DOĞRULAMASI
            // =====================================

            await ValidateActorAsync(
                currentUserId,
                cancellationToken);

            // =====================================
            // 4. MEVCUT HAREKETİ GETİR
            // =====================================

            var movement = await _stockMovementRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (movement == null)
            {
                return false;
            }

            // =====================================
            // 5. İŞLEMİ YAPAN KULLANICI
            // =====================================

            if (movement.PerformedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Stok hareketini yalnızca " +
                    "işlemi gerçekleştiren kullanıcı " +
                    "güncelleyebilir.");
            }

            // =====================================
            // 6. TRANSFER GEÇMİŞİNİ KORU
            // =====================================

            if (movement.TransferGroupId.HasValue)
            {
                throw new InvalidOperationException(
                    "Transfer hareketleri tek tek " +
                    "güncellenemez. Transferin giriş " +
                    "ve çıkış kayıtları birlikte " +
                    "değerlendirilmelidir.");
            }

            // =====================================
            // 7. DEĞİŞİKLİK KONTROLÜ
            // =====================================

            if (movement.ReferenceNumber == referenceNumber &&
                movement.Description == description)
            {
                return true;
            }

            // =====================================
            // 8. DÜZENLENEBİLİR ALANLAR
            // =====================================

            // DTO'dan bütün Entity'ye mapping
            // uygulanmıyor; sadece izin verilen
            // alanlar değiştiriliyor.

            movement.ReferenceNumber =
                referenceNumber;

            movement.Description =
                description;

            movement.UpdatedAt =
                DateTime.UtcNow;

            // =====================================
            // 9. REPOSITORY GÜNCELLEMESİ
            // =====================================

            return await _stockMovementRepository
                .UpdateAsync(
                    movement,
                    cancellationToken);
        }

        #endregion

        #region GetTotalInQuantity

        /// <summary>
        /// Belirli ambar ve malzeme için
        /// toplam giriş hareketi miktarı.
        /// </summary>
        public async Task<decimal> GetTotalInQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(warehouseId, "Ambar");

            ValidateId(materialId, "Malzeme");

            var movements = await _stockMovementRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return movements
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId &&
                    x.MovementType == StockMovementTypeEnum.In)
                .Sum(x => x.Quantity);
        }

        #endregion

        #region GetTotalOutQuantity

        /// <summary>
        /// Belirli ambar ve malzeme için
        /// toplam çıkış hareketi miktarı.
        /// </summary>
        public async Task<decimal> GetTotalOutQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(warehouseId, "Ambar");

            ValidateId(materialId, "Malzeme");

            var movements = await _stockMovementRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return movements
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId &&
                    x.MovementType == StockMovementTypeEnum.Out)
                .Sum(x => x.Quantity);
        }

        #endregion

        #region GetNetMovementQuantity

        /// <summary>
        /// Giriş ve çıkış hareketlerinin
        /// miktar farkını hesaplar.
        ///
        /// Bu değer hareket geçmişinin netidir.
        /// Mevcut bakiye ayrıca WarehouseStock
        /// üzerinden sorgulanmalıdır.
        /// </summary>
        public async Task<decimal> GetNetMovementQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(warehouseId, "Ambar");

            ValidateId(materialId, "Malzeme");

            var movements = await _stockMovementRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            var filteredMovements = movements
                .Where(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId)
                .ToList();

            decimal totalIn = filteredMovements
                .Where(x =>
                    x.MovementType == StockMovementTypeEnum.In)
                .Sum(x => x.Quantity);

            decimal totalOut = filteredMovements
                .Where(x =>
                    x.MovementType == StockMovementTypeEnum.Out)
                .Sum(x => x.Quantity);

            return totalIn - totalOut;
        }

        #endregion

        #region Private Mapping Methods

        /// <summary>
        /// Entity koleksiyonunu Result DTO listesine
        /// dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultStockMovementDto>
            MapResults(
                IEnumerable<StockMovement> movements)
        {
            return _mapper.Map<
                List<ResultStockMovementDto>>(
                    movements.ToList());
        }

        /// <summary>
        /// Seçilen hareket kayıtlarını ilişkileri
        /// yüklenmiş Entity kayıtları üzerinden
        /// Result DTO'ya dönüştürür.
        /// </summary>
        private async Task<IReadOnlyList<ResultStockMovementDto>>
            MapResultsWithDetailsAsync(
                IEnumerable<StockMovement> movements,
                CancellationToken cancellationToken)
        {
            var selectedIds = movements
                .Select(x => x.Id)
                .ToHashSet();

            if (selectedIds.Count == 0)
            {
                return new List<ResultStockMovementDto>();
            }

            var detailedMovements =
                await _stockMovementRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            var filteredMovements = detailedMovements
                .Where(x => selectedIds.Contains(x.Id))
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .ToList();

            return MapResults(filteredMovements);
        }

        #endregion

        #region Private Relation Validation Methods

        /// <summary>
        /// Kullanıcının mevcut ve aktif olduğunu
        /// doğrular.
        /// </summary>
        private async Task ValidateActorAsync(
            int currentUserId,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository
                .GetByIdAsync(
                    currentUserId,
                    cancellationToken);

            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı bulunamadı veya " +
                    "pasif durumdadır.");
            }
        }

        /// <summary>
        /// Ambarın mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateWarehouseAsync(
            int warehouseId,
            CancellationToken cancellationToken)
        {
            var warehouse = await _warehouseRepository
                .GetByIdAsync(
                    warehouseId,
                    cancellationToken);

            if (warehouse == null || !warehouse.IsActive)
            {
                throw new InvalidOperationException(
                    "Ambar bulunamadı veya " +
                    "pasif durumdadır.");
            }
        }

        /// <summary>
        /// Malzemenin mevcut ve aktif olduğunu doğrular.
        /// </summary>
        private async Task ValidateMaterialAsync(
            int materialId,
            CancellationToken cancellationToken)
        {
            var material = await _materialRepository
                .GetByIdAsync(
                    materialId,
                    cancellationToken);

            if (material == null || !material.IsActive)
            {
                throw new InvalidOperationException(
                    "Malzeme bulunamadı veya " +
                    "pasif durumdadır.");
            }
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO DataAnnotations doğrulaması.
        /// </summary>
        private static void ValidateDto<T>(T dto)
            where T : class
        {
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);
        }

        /// <summary>
        /// ID değerinin pozitif olmasını sağlar.
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
        /// Listeleme için kayıt sayısını doğrular.
        /// </summary>
        private static void ValidateTake(int take)
        {
            if (take <= 0 || take > MaxTake)
            {
                throw new ValidationException(
                    "Listeleme kayıt sayısı " +
                    "1 ile 1000 arasında olmalıdır.");
            }
        }

        /// <summary>
        /// Hareket türünü doğrular.
        /// </summary>
        private static void ValidateMovementType(
            StockMovementTypeEnum movementType)
        {
            if (movementType != StockMovementTypeEnum.In &&
                movementType != StockMovementTypeEnum.Out)
            {
                throw new ValidationException(
                    "Geçersiz stok hareket türü. " +
                    "Yalnızca Stok Girişi ve " +
                    "Stok Çıkışı kullanılabilir.");
            }
        }

        /// <summary>
        /// Stok miktarının sıfırdan büyük
        /// olduğunu doğrular.
        /// </summary>
        private static void ValidateQuantity(
            decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Stok hareket miktarı " +
                    "sıfırdan büyük olmalıdır.");
            }
        }

        /// <summary>
        /// Opsiyonel referans numarasını
        /// doğrular ve normalize eder.
        /// </summary>
        private static string? NormalizeReferenceNumber(
            string? referenceNumber)
        {
            if (string.IsNullOrWhiteSpace(referenceNumber))
            {
                return null;
            }

            string normalized =
                referenceNumber.Trim();

            if (normalized.Length > MaxReferenceNumberLength)
            {
                throw new ValidationException(
                    "Referans numarası en fazla " +
                    "100 karakter olabilir.");
            }

            if (normalized.Any(char.IsControl))
            {
                throw new ValidationException(
                    "Referans numarası geçersiz " +
                    "karakter içeriyor.");
            }

            return normalized;
        }

        /// <summary>
        /// Arama için referans numarasını
        /// zorunlu olarak doğrular.
        /// </summary>
        private static string NormalizeRequiredReferenceNumber(
            string? referenceNumber)
        {
            return NormalizeReferenceNumber(referenceNumber)
                ?? throw new ValidationException(
                    "Referans numarası zorunludur.");
        }

        /// <summary>
        /// Açıklamayı doğrular ve normalize eder.
        /// </summary>
        private static string? NormalizeDescription(
            string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            string normalized =
                description.Trim();

            if (normalized.Length > MaxDescriptionLength)
            {
                throw new ValidationException(
                    "Stok hareketi açıklaması " +
                    "en fazla 1000 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// UTC tarih aralığı doğrulaması.
        /// </summary>
        private static void ValidateDateRange(
            DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (startDateUtc.Kind != DateTimeKind.Utc ||
                endDateUtc.Kind != DateTimeKind.Utc)
            {
                throw new ValidationException(
                    "Başlangıç ve bitiş tarihleri " +
                    "UTC formatında olmalıdır.");
            }

            if (endDateUtc < startDateUtc)
            {
                throw new ValidationException(
                    "Bitiş tarihi başlangıç " +
                    "tarihinden önce olamaz.");
            }
        }

        #endregion
    }
}
