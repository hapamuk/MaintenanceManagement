using AutoMapper;
using MaintenanceManagement.Application.DTOS.WarehouseStockDtos;
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
    public class WarehouseStockService
        : IWarehouseStockService
    {
        #region Fields

        private readonly IWarehouseStockRepository
            _warehouseStockRepository;

        private readonly IWarehouseRepository
            _warehouseRepository;

        private readonly IMaterialRepository
            _materialRepository;

        private readonly IMapper _mapper;

        #endregion

        #region Constructor

        public WarehouseStockService(
            IWarehouseStockRepository warehouseStockRepository,
            IWarehouseRepository warehouseRepository,
            IMaterialRepository materialRepository,
            IMapper mapper)
        {
            _warehouseStockRepository =
                warehouseStockRepository
                ?? throw new ArgumentNullException(
                    nameof(warehouseStockRepository));

            _warehouseRepository =
                warehouseRepository
                ?? throw new ArgumentNullException(
                    nameof(warehouseRepository));

            _materialRepository =
                materialRepository
                ?? throw new ArgumentNullException(
                    nameof(materialRepository));

            _mapper = mapper
                ?? throw new ArgumentNullException(
                    nameof(mapper));
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün ambar stok kayıtlarını
        /// ilişkili verileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var stocks = await _warehouseStockRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return MapResults(
                stocks
                    .OrderBy(x => x.WarehouseId)
                    .ThenBy(x => x.MaterialId)
                    .ThenBy(x => x.Id));
        }

        #endregion

        #region GetById

        /// <summary>
        /// Stok kaydını ID değerine göre getirir.
        /// </summary>
        public async Task<GetByIdWarehouseStockDto?>
            GetByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var stock = await _warehouseStockRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (stock == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWarehouseStockDto>(
                stock);
        }

        #endregion

        #region GetByWarehouseAndMaterialId

        /// <summary>
        /// Ambar ve malzeme bilgilerine göre
        /// tekil stok kaydı getirir.
        ///
        /// Liste dönüş tiplerinden kaynaklanan
        /// .Id derleme hatalarını önlemek için
        /// önce kayıt seçilir.
        /// </summary>
        public async Task<GetByIdWarehouseStockDto?>
            GetByWarehouseAndMaterialIdAsync(
                int warehouseId,
                int materialId,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMALARI
            // =====================================

            ValidateId(
                warehouseId,
                "Ambar");

            ValidateId(
                materialId,
                "Malzeme");

            // =====================================
            // 2. AMBAR STOKLARI
            // =====================================

            var stocks = await _warehouseStockRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            // =====================================
            // 3. İLGİLİ MALZEMEYİ BUL
            // =====================================

            var stock = stocks.FirstOrDefault(x =>
                x.MaterialId == materialId);

            if (stock == null)
            {
                return null;
            }

            // =====================================
            // 4. DETAYLI DTO
            // =====================================

            return await GetByIdAsync(
                stock.Id,
                cancellationToken);
        }

        #endregion

        #region GetByWarehouseId

        /// <summary>
        /// Belirli ambardaki bütün
        /// stok kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            var stocks = await _warehouseStockRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                stocks,
                cancellationToken);
        }

        #endregion

        #region GetByMaterialId

        /// <summary>
        /// Malzemenin bütün ambarlardaki
        /// stok kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                materialId,
                "Malzeme");

            var stocks = await _warehouseStockRepository
                .GetByMaterialIdAsync(
                    materialId,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                stocks,
                cancellationToken);
        }

        #endregion

        #region GetPositiveStocksByWarehouseId

        /// <summary>
        /// Ambardaki pozitif stok bakiyeli
        /// kayıtları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetPositiveStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            var stocks = await _warehouseStockRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            var positiveStocks = stocks
                .Where(x => x.Quantity > 0)
                .ToList();

            return await MapResultsWithDetailsAsync(
                positiveStocks,
                cancellationToken);
        }

        #endregion

        #region GetZeroStocksByWarehouseId

        /// <summary>
        /// Ambardaki sıfır stok bakiyeli
        /// kayıtları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetZeroStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            var stocks = await _warehouseStockRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            var zeroStocks = stocks
                .Where(x => x.Quantity == 0)
                .ToList();

            return await MapResultsWithDetailsAsync(
                zeroStocks,
                cancellationToken);
        }

        #endregion

        #region GetCriticalStocks

        /// <summary>
        /// Malzemenin minimum stok seviyesine
        /// eşit veya altında kalan kayıtları getirir.
        ///
        /// Karşılaştırma mevcut stok
        /// kayıtları üzerinden yapılır.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetCriticalStocksAsync(
                int? warehouseId = null,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. AMBAR FİLTRESİ
            // =====================================

            ValidateOptionalId(
                warehouseId,
                "Ambar");

            // =====================================
            // 2. STOK KAYITLARINI GETİR
            // =====================================

            var allStocks = await _warehouseStockRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            var filteredStocks = allStocks
                .Where(x =>
                    !warehouseId.HasValue ||
                    x.WarehouseId == warehouseId.Value)
                .ToList();

            // =====================================
            // 3. KRİTİK STOK LİSTESİ
            // =====================================

            var criticalStocks =
                new List<WarehouseStock>();

            // =====================================
            // 4. MALZEME BİLGİLERİNİ KONTROL ET
            // =====================================

            var materialCache =
                new Dictionary<int, Material?>();

            foreach (var stock in filteredStocks)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Material? material = stock.Material;

                if (material == null)
                {
                    if (!materialCache.TryGetValue(
                        stock.MaterialId,
                        out material))
                    {
                        material = await _materialRepository
                            .GetByIdAsync(
                                stock.MaterialId,
                                cancellationToken);

                        materialCache[stock.MaterialId] =
                            material;
                    }
                }

                if (material == null)
                {
                    continue;
                }

                // Minimum stok seviyesi sıfırsa
                // bu malzeme için pozitif bir
                // kritik stok eşiği tanımlanmamıştır.
                if (material.MinimumStockLevel <= 0)
                {
                    continue;
                }

                if (stock.Quantity <=
                    material.MinimumStockLevel)
                {
                    criticalStocks.Add(stock);
                }
            }

            // =====================================
            // 5. DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(
                criticalStocks
                    .OrderBy(x => x.WarehouseId)
                    .ThenBy(x => x.MaterialId));
        }

        #endregion

        #region GetStockQuantity

        /// <summary>
        /// Ambar ve malzemeye göre
        /// mevcut stok miktarını getirir.
        /// </summary>
        public async Task<decimal> GetStockQuantityAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            ValidateId(
                materialId,
                "Malzeme");

            return await _warehouseStockRepository
                .GetStockQuantityAsync(
                    warehouseId,
                    materialId,
                    cancellationToken);
        }

        #endregion

        #region GetAvailableStockQuantityByMaterialId

        /// <summary>
        /// Malzemenin kullanılabilir
        /// toplam stok miktarını getirir.
        /// </summary>
        public async Task<decimal>
            GetAvailableStockQuantityByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                materialId,
                "Malzeme");

            return await _warehouseStockRepository
                .GetAvailableStockQuantityByMaterialIdAsync(
                    materialId,
                    cancellationToken);
        }

        #endregion

        #region GetTotalQuantityByWarehouseId

        /// <summary>
        /// Ambardaki stok miktarlarının
        /// sayısal toplamını getirir.
        ///
        /// Farklı ölçü birimleri içeren
        /// bir toplam fiziksel olarak
        /// anlamlı olmayabilir.
        /// </summary>
        public async Task<decimal>
            GetTotalQuantityByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            var stocks = await _warehouseStockRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            return stocks.Sum(x => x.Quantity);
        }

        #endregion

        #region HasSufficientStock

        /// <summary>
        /// Ambardaki miktarın talep edilen
        /// miktarı karşılayıp karşılamadığını kontrol eder.
        ///
        /// Bu bir ön kontroldür.
        /// Stok çıkışı anında tekrar doğrulanmalıdır.
        /// </summary>
        public async Task<bool> HasSufficientStockAsync(
            int warehouseId,
            int materialId,
            decimal requiredQuantity,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID DOĞRULAMALARI
            // =====================================

            ValidateId(
                warehouseId,
                "Ambar");

            ValidateId(
                materialId,
                "Malzeme");

            // =====================================
            // 2. TALEP EDİLEN MİKTAR
            // =====================================

            ValidateQuantity(requiredQuantity);

            // =====================================
            // 3. MEVCUT STOK
            // =====================================

            decimal currentQuantity =
                await _warehouseStockRepository
                    .GetStockQuantityAsync(
                        warehouseId,
                        materialId,
                        cancellationToken);

            // =====================================
            // 4. YETERLİLİK KONTROLÜ
            // =====================================

            return currentQuantity >= requiredQuantity;
        }

        #endregion

        #region Create

        /// <summary>
        /// Ambar ve malzeme için sıfır
        /// bakiyeli başlangıç stok kaydı oluşturur.
        ///
        /// İlk stok girişi bu işlemde yapılmaz.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWarehouseStockDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            ValidateId(
                dto.WarehouseId,
                "Ambar");

            ValidateId(
                dto.MaterialId,
                "Malzeme");

            // =====================================
            // 2. AMBARI GETİR
            // =====================================

            var warehouse = await _warehouseRepository
                .GetByIdAsync(
                    dto.WarehouseId,
                    cancellationToken);

            if (warehouse == null ||
                !warehouse.IsActive)
            {
                throw new InvalidOperationException(
                    "Ambar bulunamadı veya " +
                    "pasif durumdadır.");
            }

            // =====================================
            // 3. MALZEMEYİ GETİR
            // =====================================

            var material = await _materialRepository
                .GetByIdAsync(
                    dto.MaterialId,
                    cancellationToken);

            if (material == null ||
                !material.IsActive)
            {
                throw new InvalidOperationException(
                    "Malzeme bulunamadı veya " +
                    "pasif durumdadır.");
            }

            // =====================================
            // 4. MÜKERRER STOK KAYDI
            // =====================================

            var existingStocks =
                await _warehouseStockRepository
                    .GetByWarehouseIdAsync(
                        dto.WarehouseId,
                        cancellationToken);

            bool exists = existingStocks.Any(x =>
                x.MaterialId == dto.MaterialId);

            if (exists)
            {
                throw new ValidationException(
                    "Seçilen malzeme için bu ambarda " +
                    "zaten bir stok kaydı bulunmaktadır.");
            }

            // =====================================
            // 5. DTO -> ENTITY
            // =====================================

            var stock = _mapper.Map<WarehouseStock>(dto);

            stock.WarehouseId = dto.WarehouseId;

            stock.MaterialId = dto.MaterialId;

            // =====================================
            // 6. BAŞLANGIÇ BAKİYESİ
            // =====================================

            // İlk stok girişi StockMovement
            // üzerinden yapılmalıdır.
            stock.Quantity = 0m;

            // =====================================
            // 7. VERİTABANI KAYDI
            // =====================================

            var createdStock =
                await _warehouseStockRepository
                    .CreateAsync(
                        stock,
                        cancellationToken);

            // =====================================
            // 8. KAYIT ID
            // =====================================

            return createdStock.Id;
        }

        #endregion

        #region Exists

        /// <summary>
        /// Ambar ve malzeme için stok
        /// kaydı mevcut mu?
        /// </summary>
        public async Task<bool> ExistsAsync(
            int warehouseId,
            int materialId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            ValidateId(
                materialId,
                "Malzeme");

            var stocks = await _warehouseStockRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            return stocks.Any(x =>
                x.MaterialId == materialId);
        }

        #endregion

        #region GetStockRecordCountByWarehouseId

        /// <summary>
        /// Ambardaki stok kayıtlarının sayısı.
        /// </summary>
        public async Task<int>
            GetStockRecordCountByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            var stocks = await _warehouseStockRepository
                .GetByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            return stocks.Count();
        }

        #endregion

        #region GetStockRecordCountByMaterialId

        /// <summary>
        /// Malzemenin kaç stok kaydı
        /// bulunduğunu getirir.
        /// </summary>
        public async Task<int>
            GetStockRecordCountByMaterialIdAsync(
                int materialId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                materialId,
                "Malzeme");

            var stocks = await _warehouseStockRepository
                .GetByMaterialIdAsync(
                    materialId,
                    cancellationToken);

            return stocks.Count();
        }

        #endregion

        #region Private Mapping Methods

        /// <summary>
        /// WarehouseStock Entity koleksiyonunu
        /// ResultWarehouseStockDto listesine dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultWarehouseStockDto>
            MapResults(
                IEnumerable<WarehouseStock> stocks)
        {
            return _mapper.Map<List<ResultWarehouseStockDto>>(
                stocks.ToList());
        }

        /// <summary>
        /// Seçili stok kayıtlarının ilişkilerini
        /// yüklenmiş Entity kayıtlarıyla eşleştirir.
        /// </summary>
        private async Task<IReadOnlyList<ResultWarehouseStockDto>>
            MapResultsWithDetailsAsync(
                IEnumerable<WarehouseStock> stocks,
                CancellationToken cancellationToken)
        {
            // =====================================
            // 1. SEÇİLİ KAYIT ID'LERİ
            // =====================================

            var selectedIds = stocks
                .Select(x => x.Id)
                .ToHashSet();

            // =====================================
            // 2. BOŞ LİSTE
            // =====================================

            if (selectedIds.Count == 0)
            {
                return new List<ResultWarehouseStockDto>();
            }

            // =====================================
            // 3. DETAYLI STOK KAYITLARI
            // =====================================

            var detailedStocks =
                await _warehouseStockRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            // =====================================
            // 4. FİLTRELEME
            // =====================================

            var filteredStocks = detailedStocks
                .Where(x => selectedIds.Contains(x.Id))
                .OrderBy(x => x.WarehouseId)
                .ThenBy(x => x.MaterialId)
                .ToList();

            // =====================================
            // 5. DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(filteredStocks);
        }

        #endregion

        #region Private Validation Methods

        /// <summary>
        /// DTO üzerindeki DataAnnotations
        /// doğrulamalarını gerçekleştirir.
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
        /// ID değerinin sıfırdan büyük
        /// olduğunu kontrol eder.
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
        /// Opsiyonel ID değerini kontrol eder.
        /// </summary>
        private static void ValidateOptionalId(
            int? id,
            string fieldName)
        {
            if (id.HasValue)
            {
                ValidateId(
                    id.Value,
                    fieldName);
            }
        }

        /// <summary>
        /// Stok miktarının sıfırdan büyük
        /// olduğunu kontrol eder.
        /// </summary>
        private static void ValidateQuantity(
            decimal quantity)
        {
            if (quantity <= 0)
            {
                throw new ValidationException(
                    "Talep edilen stok miktarı " +
                    "sıfırdan büyük olmalıdır.");
            }
        }

        #endregion
    }
}
