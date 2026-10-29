using AutoMapper;
using MaintenanceManagement.Application.DTOS.WorkOrderMaterialDtos;
using MaintenanceManagement.Application.Interfaces;
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

    public class WorkOrderMaterialService
       : IWorkOrderMaterialService
    {
        #region Constants

        private const string ExecutePermission =
            "WORK_ORDER_EXECUTE";

        private const int MaxDescriptionLength = 1000;

        #endregion

        #region Fields

        private readonly IWorkOrderMaterialRepository
            _workOrderMaterialRepository;

        private readonly IWorkOrderRepository
            _workOrderRepository;

        private readonly IStockMovementRepository
            _stockMovementRepository;

        private readonly IWorkOrderPersonnelRepository
            _workOrderPersonnelRepository;

        private readonly IUserRepository
            _userRepository;

        private readonly IRoleWorkSubTypePermissionService
            _permissionService;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WorkOrderMaterialService(
            IWorkOrderMaterialRepository workOrderMaterialRepository,
            IWorkOrderRepository workOrderRepository,
            IStockMovementRepository stockMovementRepository,
            IWorkOrderPersonnelRepository workOrderPersonnelRepository,
            IUserRepository userRepository,
            IRoleWorkSubTypePermissionService permissionService,
            IMapper mapper)
        {
            _workOrderMaterialRepository =
                workOrderMaterialRepository;

            _workOrderRepository =
                workOrderRepository;

            _stockMovementRepository =
                stockMovementRepository;

            _workOrderPersonnelRepository =
                workOrderPersonnelRepository;

            _userRepository =
                userRepository;

            _permissionService =
                permissionService;

            _mapper =
                mapper;
        }

        #endregion

        #region GetAll

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var materials = await _workOrderMaterialRepository
                .GetAllWithDetailsAsync(cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region GetById

        public async Task<GetByIdWorkOrderMaterialDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var material = await _workOrderMaterialRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            return material == null
                ? null
                : _mapper.Map<GetByIdWorkOrderMaterialDto>(
                    material);
        }

        #endregion

        #region GetByWorkOrderId

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByWorkOrderIdAsync(
                int workOrderId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            var materials = await _workOrderMaterialRepository
                .GetByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region GetByMaterialId

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(materialId, "Malzeme");

            var materials = await _workOrderMaterialRepository
                .GetByMaterialIdAsync(
                    materialId,
                    cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region GetByWarehouseId

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(warehouseId, "Ambar");

            var materials = await _workOrderMaterialRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region GetByWorkOrderAndMaterialId

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByWorkOrderAndMaterialIdAsync(
                int workOrderId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");
            ValidateId(materialId, "Malzeme");

            var materials = await _workOrderMaterialRepository
                .GetByWorkOrderAndMaterialIdAsync(
                    workOrderId,
                    materialId,
                    cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region GetByWorkOrderAndWarehouseId

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByWorkOrderAndWarehouseIdAsync(
                int workOrderId,
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");
            ValidateId(warehouseId, "Ambar");

            var materials = await _workOrderMaterialRepository
                .GetByWorkOrderAndWarehouseIdAsync(
                    workOrderId,
                    warehouseId,
                    cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region GetByStockMovementId

        public async Task<GetByIdWorkOrderMaterialDto?>
            GetByStockMovementIdAsync(
                int stockMovementId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                stockMovementId,
                "Stok Hareketi");

            var material = await _workOrderMaterialRepository
                .GetByStockMovementIdAsync(
                    stockMovementId,
                    cancellationToken);

            if (material == null)
            {
                return null;
            }

            return await GetByIdAsync(
                material.Id,
                cancellationToken);
        }

        #endregion

        #region GetByResponsibleUnitId

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByResponsibleUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(unitId, "Sorumlu Birim");

            var materials = await _workOrderMaterialRepository
                .GetByResponsibleUnitIdAsync(
                    unitId,
                    cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region GetByTeamId

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByTeamIdAsync(
                int teamId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(teamId, "Takım");

            var materials = await _workOrderMaterialRepository
                .GetByTeamIdAsync(
                    teamId,
                    cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region GetByDateRange

        public async Task<IReadOnlyList<ResultWorkOrderMaterialDto>>
            GetByDateRangeAsync(
                DateTime startDateUtc,
                DateTime endDateUtc,
                CancellationToken cancellationToken = default)
        {
            ValidateDateRange(
                startDateUtc,
                endDateUtc);

            var materials = await _workOrderMaterialRepository
                .GetByDateRangeAsync(
                    startDateUtc,
                    endDateUtc,
                    cancellationToken);

            return MapResults(materials);
        }

        #endregion

        #region Create

        /// <summary>
        /// Gerçekleşmiş stok çıkışını iş emrindeki
        /// malzeme kullanım kaydına bağlar.
        ///
        /// StockMovementId değeri stok çıkış
        /// işleminden elde edilmelidir.
        ///
        /// Bu metot stok miktarını düşürmez.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWorkOrderMaterialDto dto,
            int currentUserId,
            int stockMovementId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(dto.WorkOrderId, "İş Emri");
            ValidateId(dto.MaterialId, "Malzeme");
            ValidateId(dto.WarehouseId, "Ambar");
            ValidateId(currentUserId, "Kullanıcı");
            ValidateId(stockMovementId, "Stok Hareketi");

            ValidateQuantity(dto.Quantity);
            ValidateUnitCost(dto.UnitCost);

            string? description =
                NormalizeDescription(dto.Description);

            // =====================================
            // 2. İŞ EMRİNİ GETİR
            // =====================================

            var workOrder = await GetExecutableWorkOrderAsync(
                dto.WorkOrderId,
                cancellationToken);

            // =====================================
            // 3. PERSONEL VE YETKİ KONTROLÜ
            // =====================================

            await ValidateActorAsync(
                workOrder,
                currentUserId,
                cancellationToken);

            // =====================================
            // 4. MALZEME ENTITY'SİNİ OLUŞTUR
            // =====================================

            var workOrderMaterial =
                _mapper.Map<WorkOrderMaterial>(dto);

            workOrderMaterial.WorkOrderId =
                dto.WorkOrderId;

            workOrderMaterial.MaterialId =
                dto.MaterialId;

            workOrderMaterial.WarehouseId =
                dto.WarehouseId;

            workOrderMaterial.Quantity =
                dto.Quantity;

            workOrderMaterial.UnitCost =
                dto.UnitCost;

            workOrderMaterial.Description =
                description;

            // Güvenilir stok işleminden gelen ID.
            workOrderMaterial.StockMovementId =
                stockMovementId;

            // =====================================
            // 5. STOK ÇIKIŞINI DOĞRULA
            // =====================================

            await ValidateStockMovementAsync(
                workOrderMaterial,
                currentUserId,
                cancellationToken);

            // =====================================
            // 6. MÜKERRER BAĞLANTI KONTROLÜ
            // =====================================

            bool linked = await _workOrderMaterialRepository
                .ExistsStockMovementLinkAsync(
                    stockMovementId,
                    null,
                    cancellationToken);

            if (linked)
            {
                throw new InvalidOperationException(
                    "Bu stok çıkış hareketi başka " +
                    "bir iş emri malzeme kaydına " +
                    "zaten bağlanmıştır.");
            }

            // =====================================
            // 7. REPOSITORY KAYDI
            // =====================================

            var createdMaterial =
                await _workOrderMaterialRepository
                    .RegisterIssuedMaterialAsync(
                        workOrderMaterial,
                        cancellationToken);

            return createdMaterial.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Malzemenin yalnızca birim maliyet
        /// ve açıklamasını günceller.
        ///
        /// Stok çıkış miktarı ve ilişkiler
        /// değiştirilemez.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWorkOrderMaterialDto dto,
            int currentUserId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            ValidateDto(dto);

            ValidateId(dto.Id, "İş Emri Malzeme Kaydı");
            ValidateId(currentUserId, "Kullanıcı");

            ValidateUnitCost(dto.UnitCost);

            string? description =
                NormalizeDescription(dto.Description);

            // =====================================
            // 1. MEVCUT KAYDI GETİR
            // =====================================

            var existingMaterial =
                await _workOrderMaterialRepository
                    .GetByIdAsync(
                        dto.Id,
                        cancellationToken);

            if (existingMaterial == null)
            {
                return false;
            }

            // =====================================
            // 2. İŞ EMRİNİ DOĞRULA
            // =====================================

            var workOrder = await GetExecutableWorkOrderAsync(
                existingMaterial.WorkOrderId,
                cancellationToken);

            // =====================================
            // 3. YETKİ KONTROLÜ
            // =====================================

            await ValidateActorAsync(
                workOrder,
                currentUserId,
                cancellationToken);

            // =====================================
            // 4. STOK HAREKETİ BAĞLANTISI
            // =====================================

            if (!existingMaterial.StockMovementId.HasValue)
            {
                throw new InvalidOperationException(
                    "Malzeme kaydının stok hareketi " +
                    "bağlantısı bulunmamaktadır.");
            }

            await ValidateStockMovementAsync(
                existingMaterial,
                currentUserId,
                cancellationToken);

            // =====================================
            // 5. GÜNCELLENEBİLİR ALANLAR
            // =====================================

            // DTO yalnızca bu iki alanı günceller.
            // ID, WorkOrderId, MaterialId,
            // WarehouseId, Quantity, StockMovementId
            // ve CreatedAt değiştirilmez.

            existingMaterial.UnitCost =
                dto.UnitCost;

            existingMaterial.Description =
                description;

            existingMaterial.UpdatedAt =
                DateTime.UtcNow;

            // =====================================
            // 6. GÜNCELLEME
            // =====================================

            return await _workOrderMaterialRepository
                .UpdateAsync(
                    existingMaterial,
                    cancellationToken);
        }

        #endregion

        #region HasMaterialsByWorkOrderId

        public async Task<bool> HasMaterialsByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            return await _workOrderMaterialRepository
                .HasMaterialsByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region ExistsStockMovementLink

        public async Task<bool> ExistsStockMovementLinkAsync(
            int stockMovementId,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                stockMovementId,
                "Stok Hareketi");

            if (excludedId.HasValue)
            {
                ValidateId(
                    excludedId.Value,
                    "Hariç Tutulacak Kayıt");
            }

            return await _workOrderMaterialRepository
                .ExistsStockMovementLinkAsync(
                    stockMovementId,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region GetMaterialRecordCountByWorkOrderId

        public async Task<int> GetMaterialRecordCountByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            return await _workOrderMaterialRepository
                .GetMaterialRecordCountByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalQuantityByWorkOrderAndMaterialId

        public async Task<decimal>
            GetTotalQuantityByWorkOrderAndMaterialIdAsync(
                int workOrderId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");
            ValidateId(materialId, "Malzeme");

            return await _workOrderMaterialRepository
                .GetTotalQuantityByWorkOrderAndMaterialIdAsync(
                    workOrderId,
                    materialId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalQuantityByMaterialId

        public async Task<decimal> GetTotalQuantityByMaterialIdAsync(
            int materialId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(materialId, "Malzeme");

            return await _workOrderMaterialRepository
                .GetTotalQuantityByMaterialIdAsync(
                    materialId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalCostByWorkOrderId

        public async Task<decimal> GetTotalCostByWorkOrderIdAsync(
            int workOrderId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");

            return await _workOrderMaterialRepository
                .GetTotalCostByWorkOrderIdAsync(
                    workOrderId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalCostByWorkOrderAndMaterialId

        public async Task<decimal>
            GetTotalCostByWorkOrderAndMaterialIdAsync(
                int workOrderId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(workOrderId, "İş Emri");
            ValidateId(materialId, "Malzeme");

            return await _workOrderMaterialRepository
                .GetTotalCostByWorkOrderAndMaterialIdAsync(
                    workOrderId,
                    materialId,
                    cancellationToken);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Entity listesini Result DTO listesine dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultWorkOrderMaterialDto>
            MapResults(
                IEnumerable<WorkOrderMaterial> materials)
        {
            return _mapper.Map<List<ResultWorkOrderMaterialDto>>(
                materials.ToList());
        }

        /// <summary>
        /// İş emrinin malzeme kaydına uygun
        /// durumda olduğunu doğrular.
        /// </summary>
        private async Task<WorkOrder> GetExecutableWorkOrderAsync(
            int workOrderId,
            CancellationToken cancellationToken)
        {
            var workOrder = await _workOrderRepository
                .GetByIdAsync(
                    workOrderId,
                    cancellationToken);

            if (workOrder == null)
            {
                throw new InvalidOperationException(
                    "İş emri bulunamadı.");
            }

            if (workOrder.Status != WorkStatusEnum.InProgress)
            {
                throw new InvalidOperationException(
                    "Malzeme işlemleri yalnızca " +
                    "İşe Başlandı durumundaki " +
                    "iş emirlerinde yapılabilir.");
            }

            return workOrder;
        }

        /// <summary>
        /// Kullanıcının aktif, iş emrine atanmış
        /// ve iş alt türünde yetkili olduğunu doğrular.
        /// </summary>
        private async Task ValidateActorAsync(
            WorkOrder workOrder,
            int currentUserId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. KULLANICI DOĞRULAMASI
            // =====================================

            var user = await _userRepository
                .GetByIdAsync(
                    currentUserId,
                    cancellationToken);

            if (user == null ||
                !user.IsActive ||
                !user.HasSystemAccess)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı aktif değildir veya " +
                    "sistem erişimi bulunmamaktadır.");
            }

            // =====================================
            // 2. AKTİF PERSONEL ATAMASI
            // =====================================

            bool isAssigned =
                await _workOrderPersonnelRepository
                    .IsUserAssignedAsync(
                        workOrder.Id,
                        currentUserId,
                        cancellationToken);

            if (!isAssigned)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcı bu iş emrine aktif " +
                    "personel olarak atanmamıştır.");
            }

            // =====================================
            // 3. İŞ ALT TÜRÜ YETKİSİ
            // =====================================

            bool hasPermission = await _permissionService
                .HasUserActivePermissionAsync(
                    currentUserId,
                    workOrder.ResponsibleUnitId,
                    workOrder.WorkSubTypeId,
                    ExecutePermission,
                    cancellationToken);

            if (!hasPermission)
            {
                throw new UnauthorizedAccessException(
                    "Kullanıcının bu iş emrinde " +
                    "malzeme işlemi yapma yetkisi yoktur.");
            }
        }

        /// <summary>
        /// Stok hareketinin iş emri malzeme
        /// kaydı için uygun olduğunu doğrular.
        /// </summary>
        private async Task ValidateStockMovementAsync(
            WorkOrderMaterial material,
            int currentUserId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. STOK HAREKETİ ID KONTROLÜ
            // =====================================

            if (!material.StockMovementId.HasValue ||
                material.StockMovementId.Value <= 0)
            {
                throw new ValidationException(
                    "Geçerli bir stok çıkış hareketi " +
                    "bağlantısı zorunludur.");
            }

            // =====================================
            // 2. STOK HAREKETİNİ GETİR
            // =====================================

            var movement = await _stockMovementRepository
                .GetByIdAsync(
                    material.StockMovementId.Value,
                    cancellationToken);

            if (movement == null)
            {
                throw new InvalidOperationException(
                    "Stok çıkış hareketi bulunamadı.");
            }

            // =====================================
            // 3. STOK ÇIKIŞ TÜRÜ
            // =====================================

            if (movement.MovementType != StockMovementTypeEnum.Out)
            {
                throw new InvalidOperationException(
                    "İş emri malzeme kullanımına " +
                    "yalnızca Stok Çıkışı bağlanabilir.");
            }

            // =====================================
            // 4. TRANSFER KONTROLÜ
            // =====================================

            if (movement.TransferGroupId.HasValue)
            {
                throw new InvalidOperationException(
                    "Ambarlar arası transfer hareketi " +
                    "iş emri malzeme tüketimi olarak " +
                    "kullanılamaz.");
            }

            // =====================================
            // 5. MALZEME VE AMBAR EŞLEŞMESİ
            // =====================================

            if (movement.MaterialId != material.MaterialId ||
                movement.WarehouseId != material.WarehouseId)
            {
                throw new InvalidOperationException(
                    "Stok hareketindeki malzeme veya " +
                    "ambar, iş emri malzeme kaydıyla " +
                    "eşleşmiyor.");
            }

            // =====================================
            // 6. MİKTAR EŞLEŞMESİ
            // =====================================

            if (movement.Quantity != material.Quantity)
            {
                throw new InvalidOperationException(
                    "Stok çıkış miktarı ile iş emrinde " +
                    "kullanılan miktar aynı olmalıdır.");
            }

            // =====================================
            // 7. HAREKETİ YAPAN PERSONEL
            // =====================================

            if (movement.PerformedByUserId != currentUserId)
            {
                throw new UnauthorizedAccessException(
                    "Bu stok hareketini gerçekleştiren " +
                    "kullanıcı ile işlem yapan " +
                    "kullanıcı aynı olmalıdır.");
            }
        }

        /// <summary>
        /// DTO üzerindeki DataAnnotations
        /// doğrulamalarını çalıştırır.
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
        /// Pozitif ID doğrulaması.
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
        /// Kullanılan miktar sıfırdan büyük olmalıdır.
        /// </summary>
        private static void ValidateQuantity(
            decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Kullanılan malzeme miktarı " +
                    "sıfırdan büyük olmalıdır.");
            }
        }

        /// <summary>
        /// Birim maliyet negatif olamaz.
        /// Null değer maliyetin bilinmediğini belirtir.
        /// </summary>
        private static void ValidateUnitCost(
            decimal? unitCost)
        {
            if (unitCost.HasValue &&
                unitCost.Value < 0)
            {
                throw new ValidationException(
                    "Malzeme birim maliyeti " +
                    "negatif olamaz.");
            }
        }

        /// <summary>
        /// Açıklamayı temizler ve doğrular.
        /// </summary>
        private static string? NormalizeDescription(
            string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            string normalized = description.Trim();

            if (normalized.Length > MaxDescriptionLength)
            {
                throw new ValidationException(
                    "Malzeme açıklaması en fazla " +
                    "1000 karakter olabilir.");
            }

            return normalized;
        }

        /// <summary>
        /// UTC tarih aralığını doğrular.
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