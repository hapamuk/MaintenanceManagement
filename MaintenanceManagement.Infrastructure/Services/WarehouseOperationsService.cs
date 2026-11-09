using MaintenanceManagement.Application.DTOs.WarehouseOperations;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Application.Interfaces.Repositories;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Domain.Enums;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaintenanceManagement.Infrastructure.Services
{
    /// <summary>
    /// Stok bakiyelerini ve hareket geçmişini transaction içerisinde yönetir.
    /// Ambar listesi bütün aktif ambarları gösterir; stok işlemi yetkisi ayrıca kontrol edilir.
    /// </summary>
    public sealed class WarehouseOperationsService : IWarehouseOperationsService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMaterialCategoryUnitRepository _categoryUnits;

        public WarehouseOperationsService(ApplicationDbContext db, IMaterialCategoryUnitRepository categoryUnits)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _categoryUnits = categoryUnits ?? throw new ArgumentNullException(nameof(categoryUnits));
        }

        #region Helpers

        private async Task RequirePermissionAsync(
            int warehouseId,
            int userId,
            CancellationToken ct)
        {
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Oturum kullanıcı kimliği bulunamadı. Giriş işlemindeki NameIdentifier claim'ini kontrol edin.");
            }

            if (warehouseId <= 0)
            {
                throw new ValidationException("Geçerli bir ambar seçiniz.");
            }

            var warehouse = await _db.Warehouses
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == warehouseId, ct);

            if (warehouse == null || !warehouse.IsActive)
            {
                throw new ValidationException("Ambar bulunamadı veya pasif durumdadır.");
            }

            bool permitted = await _db.WarehousePersonnels
                .AsNoTracking()
                .AnyAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.UserId == userId &&
                    x.User.IsActive, ct);

            if (!permitted)
            {
                throw new UnauthorizedAccessException(
                    "Seçilen ambarda stok işlemi yapma yetkiniz bulunmuyor. " +
                    "Ambar Tanıtımı ekranından görevli personel atamasını kontrol ediniz.");
            }
        }

        private static void CheckQuantity(decimal quantity)
        {
            if (quantity <= 0 || quantity > 999999999m)
            {
                throw new ValidationException(
                    "Miktar sıfırdan büyük ve 999999999 değerinden küçük veya eşit olmalıdır.");
            }
        }

        private async Task EnsureMaterialAsync(int materialId, CancellationToken ct)
        {
            if (materialId <= 0 ||
                !await _db.Materials.AnyAsync(x => x.Id == materialId && x.IsActive, ct))
            {
                throw new ValidationException("Aktif malzeme bulunamadı.");
            }
        }

        // Organizasyon kapsamı: seçilmiş birim, üst ve alt ilişkileriyle değerlendirilir.
        // Daire başkanlığına bağlı ambar, kategoriye seçilmiş herhangi bir alt
        // müdürlük/şeflik üzerinden uygun olabilir. Başka dairenin alt birimleri
        // yalnızca aynı ortak üstte olmak nedeniyle kabul edilmez.
        private async Task<bool> CategoryAllowsWarehouseAsync(
            int materialCategoryId, int? warehouseUnitId, CancellationToken ct)
        {
            var selectedUnitIds = await _categoryUnits.GetUnitIdsByCategoryIdAsync(
                materialCategoryId, ct);

            // Kategori için kapsam seçilmemişse bütün ambarlara açıktır.
            if (selectedUnitIds.Count == 0) return true;
            if (!warehouseUnitId.HasValue) return false;

            var units = await _db.Units.AsNoTracking()
                .Select(x => new { x.Id, x.ParentUnitId })
                .ToListAsync(ct);
            var parents = units.ToDictionary(x => x.Id, x => x.ParentUnitId);
            var selected = selectedUnitIds.ToHashSet();
            int warehouseUnit = warehouseUnitId.Value;

            // 1. Kategoride seçilen birim ambarın kendi birimi veya üstü mü?
            if (IsSameOrDescendant(warehouseUnit, selected, parents))
                return true;

            // 2. Kategoride seçilen alt müdürlük/şeflik, ambarın bağlı olduğu
            //    Daire Başkanlığı ya da Genel Müdürlük altında mı?
            foreach (int selectedUnitId in selected)
            {
                if (IsSameOrDescendant(
                    selectedUnitId,
                    new HashSet<int> { warehouseUnit },
                    parents))
                    return true;
            }

            return false;
        }

        private static bool IsSameOrDescendant(
            int unitId,
            HashSet<int> allowedAncestors,
            Dictionary<int, int?> parents)
        {
            var visited = new HashSet<int>();
            int? current = unitId;
            while (current.HasValue && visited.Add(current.Value))
            {
                if (allowedAncestors.Contains(current.Value)) return true;
                if (!parents.TryGetValue(current.Value, out int? parent)) break;
                current = parent;
            }
            return false;
        }

        private async Task EnsureCategoryAllowedAsync(
            int warehouseId, int materialId, CancellationToken ct)
        {
            var warehouse = await _db.Warehouses.AsNoTracking()
                .Where(x => x.Id == warehouseId && x.IsActive)
                .Select(x => new { x.UnitId }).SingleOrDefaultAsync(ct);
            if (warehouse == null) throw new ValidationException("Ambar bulunamadı veya pasif.");
            var material = await _db.Materials.AsNoTracking()
                .Where(x => x.Id == materialId && x.IsActive)
                .Select(x => new { x.MaterialCategoryId })
                .SingleOrDefaultAsync(ct);
            if (material == null) throw new ValidationException("Aktif malzeme bulunamadı.");
            int? categoryId = material.MaterialCategoryId;
            if (!categoryId.HasValue || categoryId.Value <= 0)
                throw new ValidationException("Malzeme kategoriye bağlanmadan stok işlemi yapılamaz.");
            if (!await CategoryAllowsWarehouseAsync(
                    categoryId.Value, warehouse.UnitId, ct))
                throw new ValidationException(
                    "Malzemenin kategorisi seçilen ambarın organizasyon kapsamına uygun değil.");
        }

        private async Task RequireActiveWarehouseAsync(int warehouseId, CancellationToken ct)
        {
            if (!await _db.Warehouses.AsNoTracking()
                .AnyAsync(x => x.Id == warehouseId && x.IsActive, ct))
                throw new ValidationException("Hedef ambar bulunamadı veya pasif.");
        }

        private static string NewReference(string prefix)
        {
            return prefix + "-" + Guid.NewGuid()
                .ToString("N")
                .Substring(0, 20)
                .ToUpperInvariant();
        }

        private void AddMovement(
            int warehouseId,
            int materialId,
            int userId,
            decimal quantity,
            StockMovementTypeEnum type,
            string reference,
            string? description)
        {
            var now = DateTime.UtcNow;

            _db.StockMovements.Add(new StockMovement
            {
                WarehouseId = warehouseId,
                MaterialId = materialId,
                PerformedByUserId = userId,
                Quantity = quantity,
                MovementType = type,
                ReferenceNumber = reference,
                Description = string.IsNullOrWhiteSpace(description)
                    ? null
                    : description.Trim(),
                MovementDate = now,
                CreatedAt = now
            });
        }

        private async Task ChangeStockAsync(
            int warehouseId,
            int materialId,
            decimal delta,
            CancellationToken ct)
        {
            var stock = await _db.WarehouseStocks
                .SingleOrDefaultAsync(x =>
                    x.WarehouseId == warehouseId &&
                    x.MaterialId == materialId, ct);

            if (stock == null)
            {
                if (delta < 0)
                {
                    throw new ValidationException(
                        "Seçilen ambarda bu malzemeye ait stok kaydı bulunmuyor.");
                }

                stock = new WarehouseStock
                {
                    WarehouseId = warehouseId,
                    MaterialId = materialId,
                    Quantity = delta,
                    CreatedAt = DateTime.UtcNow
                };

                _db.WarehouseStocks.Add(stock);
                return;
            }

            if (stock.Quantity + delta < 0)
            {
                throw new ValidationException(
                    "Yetersiz stok. Mevcut stok miktarından fazla çıkış yapılamaz.");
            }

            stock.Quantity += delta;
            stock.UpdatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Stock In

        public async Task<string> StockInAsync(
            StockOperationInput input,
            int userId,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(input);
            CheckQuantity(input.Quantity);

            await using var transaction = await _db.Database
                .BeginTransactionAsync(IsolationLevel.Serializable, ct);

            await RequirePermissionAsync(input.WarehouseId, userId, ct);
            await EnsureMaterialAsync(input.MaterialId, ct);
            await EnsureCategoryAllowedAsync(input.WarehouseId, input.MaterialId, ct);

            string reference = NewReference("GIR");

            await ChangeStockAsync(
                input.WarehouseId, input.MaterialId, input.Quantity, ct);

            AddMovement(
                input.WarehouseId, input.MaterialId, userId,
                input.Quantity, StockMovementTypeEnum.In,
                reference, input.Description);

            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return reference;
        }

        #endregion

        #region Stock Out

        public async Task<string> StockOutAsync(
            StockOperationInput input,
            int userId,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(input);
            CheckQuantity(input.Quantity);

            await using var transaction = await _db.Database
                .BeginTransactionAsync(IsolationLevel.Serializable, ct);

            await RequirePermissionAsync(input.WarehouseId, userId, ct);
            await EnsureMaterialAsync(input.MaterialId, ct);
            await EnsureCategoryAllowedAsync(input.WarehouseId, input.MaterialId, ct);

            string reference = NewReference("CIK");

            await ChangeStockAsync(
                input.WarehouseId, input.MaterialId, -input.Quantity, ct);

            AddMovement(
                input.WarehouseId, input.MaterialId, userId,
                input.Quantity, StockMovementTypeEnum.Out,
                reference, input.Description);

            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return reference;
        }

        #endregion

        #region Transfer

        public async Task<string> TransferAsync(
            WarehouseTransferInput input,
            int userId,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(input);
            CheckQuantity(input.Quantity);

            if (input.SourceWarehouseId == input.TargetWarehouseId)
            {
                throw new ValidationException(
                    "Kaynak ve hedef ambar farklı olmalıdır.");
            }

            await using var transaction = await _db.Database
                .BeginTransactionAsync(IsolationLevel.Serializable, ct);

            // Kaynakta atama aranır; hedefte bütün aktif ambarlar adaydır.
            await RequirePermissionAsync(input.SourceWarehouseId, userId, ct);
            await RequireActiveWarehouseAsync(input.TargetWarehouseId, ct);
            await EnsureMaterialAsync(input.MaterialId, ct);
            await EnsureCategoryAllowedAsync(input.SourceWarehouseId, input.MaterialId, ct);
            await EnsureCategoryAllowedAsync(input.TargetWarehouseId, input.MaterialId, ct);

            string reference = NewReference("TRF");

            await ChangeStockAsync(
                input.SourceWarehouseId, input.MaterialId, -input.Quantity, ct);
            await ChangeStockAsync(
                input.TargetWarehouseId, input.MaterialId, input.Quantity, ct);

            AddMovement(
                input.SourceWarehouseId, input.MaterialId, userId,
                input.Quantity, StockMovementTypeEnum.Out,
                reference, input.Description);

            AddMovement(
                input.TargetWarehouseId, input.MaterialId, userId,
                input.Quantity, StockMovementTypeEnum.In,
                reference, input.Description);

            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return reference;
        }

        #endregion

        #region Warehouse Options

        // Mevcut interface ile uyumlu: tüm aktif ambarlar, organizasyon yoluyla.
        public async Task<IReadOnlyList<OptionRow>> GetWarehousesAsync(
            int userId, CancellationToken ct = default)
            => await BuildWarehouseOptionsAsync(false, userId, ct);

        // Stok giriş/çıkış ve transfer kaynağı için yalnızca görevlendirilenler.
        public async Task<IReadOnlyList<OptionRow>> GetAuthorizedWarehousesAsync(
            int userId, CancellationToken ct = default)
            => await BuildWarehouseOptionsAsync(true, userId, ct);

        private async Task<IReadOnlyList<OptionRow>> BuildWarehouseOptionsAsync(
            bool onlyAuthorized, int userId, CancellationToken ct)
        {
            if (userId <= 0) throw new UnauthorizedAccessException("Oturum kullanıcı kimliği bulunamadı.");
            var units = await _db.Units.AsNoTracking()
                .Select(x => new { x.Id, x.Name, x.ParentUnitId })
                .ToListAsync(ct);
            var byId = units.ToDictionary(x => x.Id);
            var query = _db.Warehouses.AsNoTracking().Where(x => x.IsActive);
            if (onlyAuthorized)
                query = query.Where(w => _db.WarehousePersonnels.Any(p =>
                    p.WarehouseId == w.Id && p.UserId == userId && p.User.IsActive));
            var warehouses = await query.Select(x => new { x.Id, x.Name, x.Code, x.UnitId })
                .ToListAsync(ct);
            var list = new List<OptionRow>();
            foreach (var w in warehouses)
            {
                var parts = new List<string>();
                var visited = new HashSet<int>();
                int? current = w.UnitId;
                while (current.HasValue && visited.Add(current.Value)
                       && byId.TryGetValue(current.Value, out var u))
                {
                    parts.Add(u.Name);
                    current = u.ParentUnitId;
                }
                parts.Reverse();
                string path = parts.Count == 0 ? "Ortak Ambar" : string.Join(" › ", parts);
                list.Add(new OptionRow
                {
                    Id = w.Id, UnitId = w.UnitId,
                    OrganizationPath = path,
                    Depth = Math.Max(0, parts.Count - 1),
                    Text = w.Code + " - " + w.Name
                });
            }
            return list.OrderBy(x => x.OrganizationPath).ThenBy(x => x.Text).ToList();
        }

        // Ambarın organizasyonunda kullanılabilen aktif malzemeleri getirir.
        public async Task<IReadOnlyList<OptionRow>> GetMaterialsForWarehouseAsync(
            int warehouseId, int userId, bool onlyStocked = false,
            CancellationToken ct = default)
        {
            await RequirePermissionAsync(warehouseId, userId, ct);
            var warehouse = await _db.Warehouses.AsNoTracking()
                .Where(x => x.Id == warehouseId)
                .Select(x => new { x.UnitId }).SingleAsync(ct);
            var materialQuery = _db.Materials.AsNoTracking().Where(x => x.IsActive);
            if (onlyStocked)
                materialQuery = materialQuery.Where(x => _db.WarehouseStocks.Any(s =>
                    s.MaterialId == x.Id && s.WarehouseId == warehouseId && s.Quantity > 0));
            var materials = await materialQuery
                .Select(x => new { x.Id, x.Name, x.MaterialCode, x.MaterialCategoryId })
                .OrderBy(x => x.Name).ToListAsync(ct);
            var result = new List<OptionRow>();
            var allowedByCategory = new Dictionary<int, bool>();
            foreach (var m in materials)
            {
                int? categoryId = m.MaterialCategoryId;
                if (!categoryId.HasValue || categoryId.Value <= 0) continue;
                if (!allowedByCategory.TryGetValue(categoryId.Value, out bool allowed))
                {
                    allowed = await CategoryAllowsWarehouseAsync(
                        categoryId.Value, warehouse.UnitId, ct);
                    allowedByCategory[categoryId.Value] = allowed;
                }
                if (allowed) result.Add(new OptionRow
                {
                    Id = m.Id, Text = m.MaterialCode + " - " + m.Name
                });
            }
            return result;
        }

        // Hedef için kullanıcı ataması gerekmez; kategori uyumluluğu zorunludur.
        public async Task<IReadOnlyList<OptionRow>> GetTransferTargetsAsync(
            int sourceWarehouseId, int materialId, int userId,
            CancellationToken ct = default)
        {
            await RequirePermissionAsync(sourceWarehouseId, userId, ct);
            await EnsureMaterialAsync(materialId, ct);
            await EnsureCategoryAllowedAsync(sourceWarehouseId, materialId, ct);
            var all = await GetWarehousesAsync(userId, ct);
            var categoryId = await _db.Materials.AsNoTracking()
                .Where(x => x.Id == materialId)
                .Select(x => (int?)x.MaterialCategoryId).SingleAsync(ct);
            if (!categoryId.HasValue) return Array.Empty<OptionRow>();
            var result = new List<OptionRow>();
            foreach (var w in all.Where(x => x.Id != sourceWarehouseId))
            {
                if (await CategoryAllowsWarehouseAsync(categoryId.Value, w.UnitId, ct))
                    result.Add(w);
            }
            return result;
        }

        #endregion

        #region Material Options

        public async Task<IReadOnlyList<OptionRow>> GetMaterialsAsync(
            CancellationToken ct = default)
        {
            return await _db.Materials
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new OptionRow
                {
                    Id = x.Id,
                    Text = x.MaterialCode + " - " + x.Name
                })
                .ToListAsync(ct);
        }

        #endregion

        #region Movement History

        public async Task<IReadOnlyList<MovementRow>> GetMovementsAsync(
            int userId,
            int? warehouseId,
            string? search,
            CancellationToken ct = default)
        {
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Oturum kullanıcı kimliği bulunamadı.");
            }

            var query = _db.StockMovements
                .AsNoTracking()
                .Where(x => x.Warehouse.WarehousePersonnels
                    .Any(p => p.UserId == userId));

            if (warehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == warehouseId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim();
                query = query.Where(x =>
                    x.ReferenceNumber.Contains(term) ||
                    x.Material.Name.Contains(term));
            }

            return await query
                .OrderByDescending(x => x.MovementDate)
                .ThenByDescending(x => x.Id)
                .Take(500)
                .Select(x => new MovementRow
                {
                    Id = x.Id,
                    Date = x.MovementDate,
                    Warehouse = x.Warehouse.Name,
                    Material = x.Material.Name,
                    Unit = x.Material.MeasurementUnit.Symbol,
                    Reference = x.ReferenceNumber,
                    Description = x.Description ?? "",
                    Direction = x.MovementType == StockMovementTypeEnum.In
                        ? "Giriş"
                        : "Çıkış",
                    Quantity = x.Quantity,
                    Personnel = x.PerformedByUser.FirstName + " " +
                                x.PerformedByUser.LastName
                })
                .ToListAsync(ct);
        }

        #endregion

        #region Warehouse Stocks

        public async Task<IReadOnlyList<StockRow>> GetStocksAsync(
            int userId,
            int? warehouseId,
            CancellationToken ct = default)
        {
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Oturum kullanıcı kimliği bulunamadı.");
            }

            var query = _db.WarehouseStocks
                .AsNoTracking()
                .Where(x => x.Warehouse.WarehousePersonnels
                    .Any(p => p.UserId == userId));

            if (warehouseId.HasValue)
            {
                query = query.Where(x => x.WarehouseId == warehouseId.Value);
            }

            return await query
                .OrderBy(x => x.Warehouse.Name)
                .ThenBy(x => x.Material.Name)
                .Select(x => new StockRow
                {
                    WarehouseId = x.WarehouseId,
                    Warehouse = x.Warehouse.Name,
                    Material = x.Material.Name,
                    Unit = x.Material.MeasurementUnit.Symbol,
                    Quantity = x.Quantity,
                    Minimum = x.Material.MinimumStockLevel
                })
                .ToListAsync(ct);
        }

        #endregion

        #region Work Order Uses

        public async Task<IReadOnlyList<WorkOrderUseRow>> GetWorkOrderUsesAsync(
            int userId,
            string? search,
            CancellationToken ct = default)
        {
            if (userId <= 0)
            {
                throw new UnauthorizedAccessException(
                    "Oturum kullanıcı kimliği bulunamadı.");
            }

            var query = _db.WorkOrderMaterials
                .AsNoTracking()
                .Where(x => x.Warehouse.WarehousePersonnels
                    .Any(p => p.UserId == userId));

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim();
                query = query.Where(x =>
                    x.WorkOrder.WorkOrderNumber.Contains(term) ||
                    x.Material.Name.Contains(term));
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Take(500)
                .Select(x => new WorkOrderUseRow
                {
                    WorkOrderMaterialId = x.Id,
                    WorkOrderId = x.WorkOrderId,
                    WorkOrderNumber = x.WorkOrder.WorkOrderNumber,
                    WorkOrderTitle = x.WorkOrder.Title,
                    Warehouse = x.Warehouse.Name,
                    Material = x.Material.Name,
                    Unit = x.Material.MeasurementUnit.Symbol,
                    Quantity = x.Quantity,
                    MovementReference = x.StockMovement == null
                        ? ""
                        : x.StockMovement.ReferenceNumber,
                    Date = x.CreatedAt
                })
                .ToListAsync(ct);
        }

        #endregion
    }
}
