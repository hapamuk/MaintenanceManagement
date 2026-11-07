using AutoMapper;
using MaintenanceManagement.Application.DTOS.MaterialDtos;
using MaintenanceManagement.Application.DTOS.StockMovementDtos;
using MaintenanceManagement.Application.DTOS.WarehouseDtos;
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
    public class WarehouseService : IWarehouseService
    {
        #region Constants

        private const int MaxCodeLength = 100;

        private const int MaxNameLength = 150;

        private const int MaxLocationLength = 250;

        private const int MaxDescriptionLength = 500;

        private const int MaxStockMovementTake = 1000;

        #endregion

        #region Fields

        private readonly IWarehouseRepository
            _warehouseRepository;

        private readonly IUnitRepository
            _unitRepository;

        private readonly IMapper
            _mapper;

        #endregion

        #region Constructor

        public WarehouseService(
            IWarehouseRepository warehouseRepository,
            IUnitRepository unitRepository,
            IMapper mapper)
        {
            _warehouseRepository = warehouseRepository
                ?? throw new ArgumentNullException(
                    nameof(warehouseRepository));

            _unitRepository = unitRepository
                ?? throw new ArgumentNullException(
                    nameof(unitRepository));

            _mapper = mapper
                ?? throw new ArgumentNullException(
                    nameof(mapper));
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Bütün ambarları ilişkili bilgileriyle getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseDto>>
            GetAllAsync(
                CancellationToken cancellationToken = default)
        {
            var warehouses = await _warehouseRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            return MapResults(
                warehouses
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Code));
        }

        #endregion

        #region GetById

        /// <summary>
        /// ID değerine göre ambar detayını getirir.
        /// </summary>
        public async Task<GetByIdWarehouseDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return null;
            }

            var warehouse = await _warehouseRepository
                .GetByIdWithDetailsAsync(
                    id,
                    cancellationToken);

            if (warehouse == null)
            {
                return null;
            }

            return _mapper.Map<GetByIdWarehouseDto>(
                warehouse);
        }

        #endregion

        #region GetByCode

        /// <summary>
        /// Ambar koduna göre detaylı kayıt getirir.
        ///
        /// Repository tarafından döndürülen
        /// List<Warehouse> nesnesi üzerinde
        /// doğrudan Id kullanılmaz.
        /// </summary>
        public async Task<GetByIdWarehouseDto?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. AMBAR KODUNU DOĞRULA
            // =====================================

            string normalizedCode = ValidateCode(code);

            // =====================================
            // 2. DETAYLI AMBAR LİSTESİNİ GETİR
            // =====================================

            var warehouses = await _warehouseRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            // =====================================
            // 3. KODA GÖRE TEK KAYIT SEÇ
            // =====================================

            var warehouse = warehouses
                .FirstOrDefault(x =>
                    string.Equals(
                        x.Code,
                        normalizedCode,
                        StringComparison.OrdinalIgnoreCase));

            // =====================================
            // 4. KAYIT KONTROLÜ
            // =====================================

            if (warehouse == null)
            {
                return null;
            }

            // =====================================
            // 5. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<GetByIdWarehouseDto>(
                warehouse);
        }

        #endregion

        #region GetByName

        /// <summary>
        /// Ambar adına göre detaylı kayıt getirir.
        ///
        /// Ambar adı benzersiz olmak zorunda
        /// olmadığından ilk eşleşen kayıt döndürülür.
        /// </summary>
        public async Task<GetByIdWarehouseDto?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. AMBAR ADINI DOĞRULA
            // =====================================

            string normalizedName = ValidateName(name);

            // =====================================
            // 2. DETAYLI AMBAR LİSTESİNİ GETİR
            // =====================================

            var warehouses = await _warehouseRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

            // =====================================
            // 3. ADA GÖRE TEK KAYIT SEÇ
            // =====================================

            var warehouse = warehouses
                .OrderBy(x => x.Id)
                .FirstOrDefault(x =>
                    string.Equals(
                        x.Name,
                        normalizedName,
                        StringComparison.CurrentCultureIgnoreCase));

            // =====================================
            // 4. KAYIT KONTROLÜ
            // =====================================

            if (warehouse == null)
            {
                return null;
            }

            // =====================================
            // 5. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<GetByIdWarehouseDto>(
                warehouse);
        }

        #endregion

        #region GetActive

        /// <summary>
        /// Aktif ambarları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseDto>>
            GetActiveAsync(
                CancellationToken cancellationToken = default)
        {
            var warehouses = await _warehouseRepository
                .GetActiveWarehousesAsync(
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                warehouses,
                cancellationToken);
        }

        #endregion

        #region GetInactive

        /// <summary>
        /// Pasif ambarları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseDto>>
            GetInactiveAsync(
                CancellationToken cancellationToken = default)
        {
            var warehouses = await _warehouseRepository
                .GetInactiveWarehousesAsync(
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                warehouses,
                cancellationToken);
        }

        #endregion

        #region GetByUnitId

        /// <summary>
        /// Organizasyon birimine doğrudan
        /// bağlı ambarları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseDto>>
            GetByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Organizasyon Birimi");

            var warehouses = await _warehouseRepository
                .GetByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                warehouses,
                cancellationToken);
        }

        #endregion

        #region GetActiveByUnitId

        /// <summary>
        /// Organizasyon birimine bağlı aktif
        /// ambarları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseDto>>
            GetActiveByUnitIdAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Organizasyon Birimi");

            var warehouses = await _warehouseRepository
                .GetActiveByUnitIdAsync(
                    unitId,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                warehouses,
                cancellationToken);
        }

        #endregion

        #region GetSharedWarehouses

        /// <summary>
        /// Organizasyon birimine bağlı olmayan
        /// ortak ambarları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseDto>>
            GetSharedWarehousesAsync(
                bool onlyActive = true,
                CancellationToken cancellationToken = default)
        {
            var warehouses = await _warehouseRepository
                .GetSharedWarehousesAsync(
                    onlyActive,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                warehouses,
                cancellationToken);
        }

        #endregion

        #region GetAvailableForUnit

        /// <summary>
        /// Belirli organizasyon biriminin
        /// kullanabileceği ambarları getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseDto>>
            GetAvailableForUnitAsync(
                int unitId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                unitId,
                "Organizasyon Birimi");

            var warehouses = await _warehouseRepository
                .GetAvailableForUnitAsync(
                    unitId,
                    cancellationToken);

            return await MapResultsWithDetailsAsync(
                warehouses,
                cancellationToken);
        }

        #endregion

        #region Search

        /// <summary>
        /// Ambarları arama metni, birim ve
        /// aktiflik durumuna göre filtreler.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseDto>>
            SearchAsync(
                string searchText,
                int? unitId = null,
                bool onlyActive = true,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ARAMA METNİ KONTROLÜ
            // =====================================

            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new ValidationException(
                    "Ambar arama metni boş olamaz.");
            }

            string normalizedSearchText =
                searchText.Trim();

            if (normalizedSearchText.Length > 250)
            {
                throw new ValidationException(
                    "Arama metni en fazla 250 karakter olabilir.");
            }

            // =====================================
            // 2. BİRİM ID DOĞRULAMASI
            // =====================================

            ValidateOptionalId(
                unitId,
                "Organizasyon Birimi");

            // =====================================
            // 3. REPOSITORY ARAMASI
            // =====================================

            var warehouses = await _warehouseRepository
                .SearchWarehousesAsync(
                    normalizedSearchText,
                    unitId,
                    onlyActive,
                    cancellationToken);

            // =====================================
            // 4. DTO DÖNÜŞÜMÜ
            // =====================================

            return await MapResultsWithDetailsAsync(
                warehouses,
                cancellationToken);
        }

        #endregion

        #region GetStocksByWarehouseId

        /// <summary>
        /// Ambardaki bütün stok kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            var stocks = await _warehouseRepository
                .GetStocksByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            return _mapper.Map<List<ResultWarehouseStockDto>>(
                stocks);
        }

        #endregion

        #region GetPositiveStocksByWarehouseId

        /// <summary>
        /// Ambarda miktarı sıfırdan büyük
        /// stok kayıtlarını getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultWarehouseStockDto>>
            GetPositiveStocksByWarehouseIdAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            var stocks = await _warehouseRepository
                .GetPositiveStocksByWarehouseIdAsync(
                    warehouseId,
                    cancellationToken);

            return _mapper.Map<List<ResultWarehouseStockDto>>(
                stocks);
        }

        #endregion

        #region GetStockByWarehouseAndMaterialId

        /// <summary>
        /// Belirli ambar ve malzemeye ait
        /// stok kaydını getirir.
        /// </summary>
        public async Task<ResultWarehouseStockDto?>
            GetStockByWarehouseAndMaterialIdAsync(
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
            // 2. STOK KAYDINI GETİR
            // =====================================

            var stock = await _warehouseRepository
                .GetStockByWarehouseAndMaterialIdAsync(
                    warehouseId,
                    materialId,
                    cancellationToken);

            if (stock == null)
            {
                return null;
            }

            // =====================================
            // 3. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<ResultWarehouseStockDto>(
                stock);
        }

        #endregion

        #region GetMaterialsInStock

        /// <summary>
        /// Ambarda pozitif stoğu bulunan
        /// malzemeleri getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultMaterialDto>>
            GetMaterialsInStockAsync(
                int warehouseId,
                CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            var materials = await _warehouseRepository
                .GetMaterialsInStockAsync(
                    warehouseId,
                    cancellationToken);

            return _mapper.Map<List<ResultMaterialDto>>(
                materials);
        }

        #endregion

        #region GetStockQuantity

        /// <summary>
        /// Ambar ve malzeme için mevcut
        /// stok miktarını getirir.
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

            return await _warehouseRepository
                .GetStockQuantityAsync(
                    warehouseId,
                    materialId,
                    cancellationToken);
        }

        #endregion

        #region GetStockRecordCount

        /// <summary>
        /// Ambardaki toplam stok kayıt sayısını getirir.
        /// </summary>
        public async Task<int> GetStockRecordCountAsync(
            int warehouseId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            return await _warehouseRepository
                .GetStockRecordCountAsync(
                    warehouseId,
                    cancellationToken);
        }

        #endregion

        #region GetMaterialInStockCount

        /// <summary>
        /// Ambarda pozitif stoğu bulunan
        /// farklı malzeme sayısını getirir.
        /// </summary>
        public async Task<int> GetMaterialInStockCountAsync(
            int warehouseId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            return await _warehouseRepository
                .GetMaterialInStockCountAsync(
                    warehouseId,
                    cancellationToken);
        }

        #endregion

        #region GetStockMovementsByWarehouseId

        /// <summary>
        /// Ambardaki son stok hareketlerini getirir.
        /// </summary>
        public async Task<IReadOnlyList<ResultStockMovementDto>>
            GetStockMovementsByWarehouseIdAsync(
                int warehouseId,
                int take = 100,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. AMBAR ID
            // =====================================

            ValidateId(
                warehouseId,
                "Ambar");

            // =====================================
            // 2. KAYIT SAYISI KONTROLÜ
            // =====================================

            if (take <= 0 || take > MaxStockMovementTake)
            {
                throw new ValidationException(
                    "Stok hareketi kayıt sayısı " +
                    "1 ile 1000 arasında olmalıdır.");
            }

            // =====================================
            // 3. STOK HAREKETLERİNİ GETİR
            // =====================================

            var movements = await _warehouseRepository
                .GetStockMovementsByWarehouseIdAsync(
                    warehouseId,
                    take,
                    cancellationToken);

            // =====================================
            // 4. DTO DÖNÜŞÜMÜ
            // =====================================

            return _mapper.Map<List<ResultStockMovementDto>>(
                movements);
        }

        #endregion

        #region Create

        /// <summary>
        /// Yeni ambar oluşturur.
        ///
        /// Ambar kodu benzersiz olmalıdır.
        /// UnitId null ise ortak ambar kabul edilir.
        /// </summary>
        public async Task<int> CreateAsync(
            CreateWarehouseDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. DTO DOĞRULAMASI
            // =====================================

            ValidateDto(dto);

            // =====================================
            // 2. AMBAR KODU
            // =====================================

            string code = ValidateCode(dto.Code);

            // =====================================
            // 3. AMBAR ADI
            // =====================================

            string name = ValidateName(dto.Name);

            // =====================================
            // 4. KONUM
            // =====================================

            string? location = NormalizeOptionalText(
                dto.Location,
                MaxLocationLength,
                "Ambar Konumu");

            // =====================================
            // 5. AÇIKLAMA
            // =====================================

            string? description = NormalizeOptionalText(
                dto.Description,
                MaxDescriptionLength,
                "Ambar Açıklaması");

            // =====================================
            // 6. ORGANİZASYON BİRİMİ
            // =====================================

            await ValidateUnitAsync(
                dto.UnitId,
                cancellationToken);

            // =====================================
            // 7. AMBAR KODU BENZERSİZLİĞİ
            // =====================================

            bool codeExists = await _warehouseRepository
                .ExistsCodeAsync(
                    code,
                    null,
                    cancellationToken);

            if (codeExists)
            {
                throw new ValidationException(
                    "Bu ambar kodu sistemde " +
                    "zaten kullanılmaktadır.");
            }

            // =====================================
            // 8. DTO -> ENTITY
            // =====================================

            var warehouse = _mapper.Map<Warehouse>(dto);

            warehouse.Code = code;

            warehouse.Name = name;

            warehouse.UnitId = dto.UnitId;

            warehouse.Location = location;

            warehouse.Description = description;

            // =====================================
            // 9. BAŞLANGIÇ DURUMU
            // =====================================

            warehouse.IsActive = true;

            // =====================================
            // 10. REPOSITORY KAYDI
            // =====================================

            var createdWarehouse = await _warehouseRepository
                .CreateAsync(
                    warehouse,
                    cancellationToken);

            // =====================================
            // 11. KAYIT ID
            // =====================================

            return createdWarehouse.Id;
        }

        #endregion

        #region Update

        /// <summary>
        /// Mevcut ambarın düzenlenebilir
        /// bilgilerini günceller.
        ///
        /// Ambar kodu oluşturulduktan sonra değiştirilemez.
        /// Stok veya işlem geçmişi bulunan
        /// ambarın bağlı birimi değiştirilemez.
        /// </summary>
        public async Task<bool> UpdateAsync(
            UpdateWarehouseDto dto,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // =====================================
            // 1. AMBAR ID DOĞRULAMASI
            // =====================================

            ValidateId(
                dto.Id,
                "Ambar");

            // =====================================
            // 2. MEVCUT AMBARI GETİR
            // =====================================

            var warehouse = await _warehouseRepository
                .GetByIdAsync(
                    dto.Id,
                    cancellationToken);

            if (warehouse == null)
            {
                return false;
            }

            // =====================================
            // 3. AMBAR KODUNU KORU
            // =====================================

            // Kod kullanıcı tarafından değiştirilemez.
            // Gelen DTO.Code dikkate alınmaz.
            string originalCode = ValidateCode(warehouse.Code);

            dto.Code = originalCode;

            // DTO doğrulaması mevcut kod atandıktan sonra yapılır.
            ValidateDto(dto);

            // =====================================
            // 4. GİRİLEN DİĞER DEĞERLERİ NORMALİZE ET
            // =====================================

            string name = ValidateName(dto.Name);

            string? location = NormalizeOptionalText(
                dto.Location,
                MaxLocationLength,
                "Ambar Konumu");

            string? description = NormalizeOptionalText(
                dto.Description,
                MaxDescriptionLength,
                "Ambar Açıklaması");

            // =====================================
            // 5. BİRİM DEĞİŞİKLİĞİ KONTROLÜ
            // =====================================

            bool unitChanged =
                warehouse.UnitId != dto.UnitId;

            if (unitChanged)
            {
                // =================================
                // 5.1 YENİ BİRİMİ DOĞRULA
                // =================================

                await ValidateUnitAsync(
                    dto.UnitId,
                    cancellationToken);

                // =================================
                // 5.2 STOK KAYITLARI
                // =================================

                int stockCount = await _warehouseRepository
                    .GetStockRecordCountAsync(
                        warehouse.Id,
                        cancellationToken);

                if (stockCount > 0)
                {
                    throw new InvalidOperationException(
                        "Stok kayıtları bulunan " +
                        "ambarın organizasyon birimi " +
                        "değiştirilemez.");
                }

                // =================================
                // 5.3 STOK HAREKETLERİ
                // =================================

                var movements = await _warehouseRepository
                    .GetStockMovementsByWarehouseIdAsync(
                        warehouse.Id,
                        1,
                        cancellationToken);

                if (movements.Any())
                {
                    throw new InvalidOperationException(
                        "Stok hareketi geçmişi " +
                        "bulunan ambarın organizasyon " +
                        "birimi değiştirilemez.");
                }

                // =================================
                // 5.4 İŞ EMRİ MALZEME KAYITLARI
                // =================================

                var warehouseDetails =
                    await _warehouseRepository
                        .GetByIdWithDetailsAsync(
                            warehouse.Id,
                            cancellationToken);

                if (warehouseDetails?.WorkOrderMaterials?.Any()
                    == true)
                {
                    throw new InvalidOperationException(
                        "İş emirlerinde malzeme " +
                        "kullanımı bulunan ambarın " +
                        "organizasyon birimi değiştirilemez.");
                }
            }

            // =====================================
            // 6. DÜZENLENEBİLİR ALANLAR
            // =====================================

            // Mevcut Entity üzerinde yalnızca
            // düzenlenmesine izin verilen alanları
            // güncelliyoruz.
            //
            // ID, CreatedAt, IsActive,
            // WarehouseStocks, StockMovements
            // ve WorkOrderMaterials korunur.

            // Veritabanındaki ambar kodu korunur.
            warehouse.Code = originalCode;

            warehouse.Name = name;

            warehouse.UnitId = dto.UnitId;

            warehouse.Location = location;

            warehouse.Description = description;

            // =====================================
            // 7. GÜNCELLEME TARİHİ
            // =====================================

            warehouse.UpdatedAt = DateTime.UtcNow;

            // =====================================
            // 8. REPOSITORY GÜNCELLEME
            // =====================================

            return await _warehouseRepository
                .UpdateAsync(
                    warehouse,
                    cancellationToken);
        }

        #endregion

        #region ChangeStatus

        /// <summary>
        /// Ambarı aktif veya pasif yapar.
        ///
        /// Pozitif stok bakiyesi bulunan ambar
        /// pasif hale getirilemez.
        /// </summary>
        public async Task<bool> ChangeStatusAsync(
            int id,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. ID KONTROLÜ
            // =====================================

            ValidateId(
                id,
                "Ambar");

            // =====================================
            // 2. MEVCUT AMBARI GETİR
            // =====================================

            var warehouse = await _warehouseRepository
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (warehouse == null)
            {
                return false;
            }

            // =====================================
            // 3. DURUM DEĞİŞMEDİYSE
            // =====================================

            if (warehouse.IsActive == isActive)
            {
                return true;
            }

            // =====================================
            // 4. PASİFLEŞTİRME KONTROLÜ
            // =====================================

            if (!isActive)
            {
                bool hasPositiveStock =
                    await _warehouseRepository
                        .HasPositiveStockAsync(
                            id,
                            cancellationToken);

                if (hasPositiveStock)
                {
                    throw new InvalidOperationException(
                        "Ambarda pozitif stok bakiyesi " +
                        "bulunduğundan pasifleştirilemez.");
                }
            }

            // =====================================
            // 5. AKTİFLEŞTİRME KONTROLÜ
            // =====================================

            if (isActive)
            {
                await ValidateUnitAsync(
                    warehouse.UnitId,
                    cancellationToken);
            }

            // =====================================
            // 6. DURUM DEĞİŞİKLİĞİ
            // =====================================

            return await _warehouseRepository
                .ChangeStatusAsync(
                    id,
                    isActive,
                    cancellationToken);
        }

        #endregion

        #region ExistsCode

        /// <summary>
        /// Ambar kodunun sistemde kullanılıp
        /// kullanılmadığını kontrol eder.
        /// </summary>
        public async Task<bool> ExistsCodeAsync(
            string code,
            int? excludedId = null,
            CancellationToken cancellationToken = default)
        {
            // =====================================
            // 1. KOD DOĞRULAMASI
            // =====================================

            string normalizedCode =
                ValidateCode(code);

            // =====================================
            // 2. HARİÇ TUTULACAK ID
            // =====================================

            ValidateOptionalId(
                excludedId,
                "Hariç Tutulacak Ambar");

            // =====================================
            // 3. REPOSITORY KONTROLÜ
            // =====================================

            return await _warehouseRepository
                .ExistsCodeAsync(
                    normalizedCode,
                    excludedId,
                    cancellationToken);
        }

        #endregion

        #region HasPositiveStock

        /// <summary>
        /// Ambarda pozitif stok bakiyesi
        /// bulunup bulunmadığını kontrol eder.
        /// </summary>
        public async Task<bool> HasPositiveStockAsync(
            int warehouseId,
            CancellationToken cancellationToken = default)
        {
            ValidateId(
                warehouseId,
                "Ambar");

            return await _warehouseRepository
                .HasPositiveStockAsync(
                    warehouseId,
                    cancellationToken);
        }

        #endregion

        #region Private Mapping Methods

        /// <summary>
        /// Warehouse Entity listesini
        /// ResultWarehouseDto listesine dönüştürür.
        /// </summary>
        private IReadOnlyList<ResultWarehouseDto> MapResults(
            IEnumerable<Warehouse> warehouses)
        {
            var warehouseList = warehouses.ToList();

            return _mapper.Map<List<ResultWarehouseDto>>(
                warehouseList);
        }

        /// <summary>
        /// Repository tarafından seçilen ambarların
        /// detaylı kayıtlarını yükler ve DTO'ya dönüştürür.
        /// </summary>
        private async Task<IReadOnlyList<ResultWarehouseDto>>
            MapResultsWithDetailsAsync(
                IEnumerable<Warehouse> warehouses,
                CancellationToken cancellationToken)
        {
            // =====================================
            // 1. SEÇİLEN AMBAR ID'LERİ
            // =====================================

            var selectedIds = warehouses
                .Select(x => x.Id)
                .ToHashSet();

            // =====================================
            // 2. BOŞ LİSTE KONTROLÜ
            // =====================================

            if (selectedIds.Count == 0)
            {
                return new List<ResultWarehouseDto>();
            }

            // =====================================
            // 3. DETAYLI AMBAR KAYITLARINI GETİR
            // =====================================

            var detailedWarehouses =
                await _warehouseRepository
                    .GetAllWithDetailsAsync(
                        cancellationToken);

            // =====================================
            // 4. KAYITLARI FİLTRELE
            // =====================================

            var filteredWarehouses = detailedWarehouses
                .Where(x => selectedIds.Contains(x.Id))
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Code)
                .ToList();

            // =====================================
            // 5. DTO DÖNÜŞÜMÜ
            // =====================================

            return MapResults(filteredWarehouses);
        }

        #endregion

        #region Private Relation Methods

        /// <summary>
        /// Organizasyon biriminin mevcut ve
        /// aktif olduğunu doğrular.
        ///
        /// UnitId null olduğunda ortak ambar
        /// kabul edilir.
        /// </summary>
        private async Task ValidateUnitAsync(
            int? unitId,
            CancellationToken cancellationToken)
        {
            // =====================================
            // 1. ORTAK AMBAR KONTROLÜ
            // =====================================

            if (!unitId.HasValue)
            {
                return;
            }

            // =====================================
            // 2. BİRİM ID KONTROLÜ
            // =====================================

            ValidateId(
                unitId.Value,
                "Organizasyon Birimi");

            // =====================================
            // 3. BİRİMİ GETİR
            // =====================================

            var unit = await _unitRepository
                .GetByIdAsync(
                    unitId.Value,
                    cancellationToken);

            // =====================================
            // 4. BİRİM AKTİFLİĞİ
            // =====================================

            if (unit == null || !unit.IsActive)
            {
                throw new InvalidOperationException(
                    "Organizasyon birimi bulunamadı " +
                    "veya pasif durumdadır.");
            }
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
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                validateAllProperties: true);
        }

        /// <summary>
        /// ID değerinin sıfırdan büyük
        /// olduğunu doğrular.
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
        /// Nullable ID değerini doğrular.
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
        /// Ambar kodunu doğrular ve normalize eder.
        /// </summary>
        private static string ValidateCode(
            string? code)
        {
            // =====================================
            // 1. ZORUNLU ALAN
            // =====================================

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException(
                    "Ambar kodu zorunludur.");
            }

            // =====================================
            // 2. BOŞLUKLARI TEMİZLE
            // =====================================

            string normalized = code.Trim();

            // =====================================
            // 3. UZUNLUK KONTROLÜ
            // =====================================

            if (normalized.Length > MaxCodeLength)
            {
                throw new ValidationException(
                    "Ambar kodu en fazla " +
                    "100 karakter olabilir.");
            }

            // =====================================
            // 4. GEÇERSİZ KARAKTER KONTROLÜ
            // =====================================

            if (normalized.Any(char.IsControl))
            {
                throw new ValidationException(
                    "Ambar kodu geçersiz " +
                    "karakter içeriyor.");
            }

            return normalized;
        }

        /// <summary>
        /// Ambar adını doğrular ve normalize eder.
        /// </summary>
        private static string ValidateName(
            string? name)
        {
            // =====================================
            // 1. ZORUNLU ALAN
            // =====================================

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException(
                    "Ambar adı zorunludur.");
            }

            // =====================================
            // 2. BOŞLUKLARI TEMİZLE
            // =====================================

            string normalized = name.Trim();

            // =====================================
            // 3. UZUNLUK KONTROLÜ
            // =====================================

            if (normalized.Length > MaxNameLength)
            {
                throw new ValidationException(
                    "Ambar adı en fazla " +
                    "150 karakter olabilir.");
            }

            // =====================================
            // 4. GEÇERSİZ KARAKTER KONTROLÜ
            // =====================================

            if (normalized.Any(char.IsControl))
            {
                throw new ValidationException(
                    "Ambar adı geçersiz " +
                    "karakter içeriyor.");
            }

            return normalized;
        }

        /// <summary>
        /// Opsiyonel metin alanını normalize eder
        /// ve uzunluğunu doğrular.
        /// </summary>
        private static string? NormalizeOptionalText(
            string? value,
            int maxLength,
            string fieldName)
        {
            // =====================================
            // 1. BOŞ METİN KONTROLÜ
            // =====================================

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // =====================================
            // 2. BOŞLUKLARI TEMİZLE
            // =====================================

            string normalized = value.Trim();

            // =====================================
            // 3. UZUNLUK KONTROLÜ
            // =====================================

            if (normalized.Length > maxLength)
            {
                throw new ValidationException(
                    $"{fieldName} en fazla " +
                    $"{maxLength} karakter olabilir.");
            }

            return normalized;
        }

        #endregion
    }

}
