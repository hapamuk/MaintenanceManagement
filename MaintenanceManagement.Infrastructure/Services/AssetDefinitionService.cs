using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetDefinitions;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.Infrastructure.Services
{
    public sealed class AssetDefinitionService : IAssetDefinitionService
    {
        private readonly ApplicationDbContext _db;
        public AssetDefinitionService(ApplicationDbContext db) => _db = db;

        // Kategoriye bağlanabilecek birimler yalnızca Genel Müdürlük / Daire Başkanlığı.
        // Mevcut projede OrganizationTypeEnum değerlerinin tam adları doğrulanınca
        // bu yardımcı metot enum karşılaştırmasına çevrilmelidir.
        private static bool IsScopeRoot(string name) =>
            name.Equals("Genel Müdürlük", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("Dairesi Başkanlığı", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("Daire Başkanlığı", StringComparison.OrdinalIgnoreCase);

        public async Task<DefinitionPageVm> GetPageAsync(string type, CancellationToken ct = default)
        {
            var units = await _db.Units.AsNoTracking().Where(x => x.IsActive)
                .Select(x => new DefinitionOption { Id = x.Id, Name = x.Name, ParentId = x.ParentUnitId, IsActive = x.IsActive })
                .OrderBy(x => x.Name).ToListAsync(ct);
            var categoryList = await _db.AssetCategories.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new DefinitionOption { Id = x.Id, Name = x.Name, IsActive = x.IsActive }).ToListAsync(ct);
            var brands = await _db.Brands.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new DefinitionOption { Id = x.Id, Name = x.Name, IsActive = x.IsActive }).ToListAsync(ct);
            var models = await _db.AssetModels.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new DefinitionOption { Id = x.Id, Name = x.Name, IsActive = x.IsActive, BrandId = x.BrandId }).ToListAsync(ct);
            var allowedUnits = units.Where(x => IsScopeRoot(x.Name)).ToList();
            var allUnitById = units.ToDictionary(x => x.Id);
            var rootIds = allowedUnits.Select(x => x.Id).ToHashSet();
            foreach (var unit in allowedUnits)
            {
                int? ancestor = unit.ParentId;
                var visited = new HashSet<int>();
                while (ancestor.HasValue && visited.Add(ancestor.Value))
                {
                    if (rootIds.Contains(ancestor.Value)) break;
                    ancestor = allUnitById.TryGetValue(ancestor.Value, out var parent)
                        ? parent.ParentId : null;
                }
                unit.ParentId = ancestor.HasValue && rootIds.Contains(ancestor.Value) ? ancestor : null;
            }
            return new DefinitionPageVm
            {
                Type = type, PageTitle = type == "category" ? "Demirbaş Kategori Tanıtımı" :
                    type == "brand" ? "Marka Tanıtımı" : "Model Tanıtımı",
                Categories = categoryList, Brands = brands, Models = models,
                Units = allowedUnits
            };
        }

        // Düzenleme formunda kategoriye ait mevcut organizasyonları işaretlemek için.
        public async Task<IReadOnlyList<int>> GetCategoryUnitIdsAsync(
            int categoryId, CancellationToken ct = default)
        {
            if (categoryId <= 0 ||
                !await _db.AssetCategories.AsNoTracking().AnyAsync(x => x.Id == categoryId, ct))
                throw new ValidationException("Kategori bulunamadı.");

            return await _db.Set<AssetCategoryUnit>()
                .AsNoTracking()
                .Where(x => x.AssetCategoryId == categoryId)
                .Select(x => x.UnitId)
                .ToListAsync(ct);
        }

        public async Task SaveCategoryAsync(AssetCategoryDefinitionInput input, CancellationToken ct = default)
        {
            string name = (input.Name ?? "").Trim();
            if (name.Length == 0) throw new ValidationException("Kategori adı zorunludur.");
            var ids = input.UnitIds.Distinct().ToList();
            if (ids.Count == 0) throw new ValidationException("En az bir organizasyon seçiniz.");
            var selected = await _db.Units.AsNoTracking().Where(x => ids.Contains(x.Id) && x.IsActive)
                .Select(x => new { x.Id, x.Name }).ToListAsync(ct);
            if (selected.Count != ids.Count || selected.Any(x => !IsScopeRoot(x.Name)))
                throw new ValidationException("Yalnızca aktif Genel Müdürlük veya Daire Başkanlığı seçilebilir.");
            if (await _db.AssetCategories.AnyAsync(x => x.Name == name && x.Id != input.Id, ct))
                throw new ValidationException("Bu kategori adı zaten kayıtlı.");
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            AssetCategory category;
            if (input.Id == 0)
            {
                category = new AssetCategory { Name = name, IsActive = input.IsActive, CreatedAt = DateTime.UtcNow };
                _db.AssetCategories.Add(category);
                await _db.SaveChangesAsync(ct);
            }
            else
            {
                category = await _db.AssetCategories.SingleOrDefaultAsync(x => x.Id == input.Id, ct)
                    ?? throw new ValidationException("Kategori bulunamadı.");
                category.Name = name; category.IsActive = input.IsActive; category.UpdatedAt = DateTime.UtcNow;
                var links = await _db.Set<AssetCategoryUnit>().Where(x => x.AssetCategoryId == input.Id).ToListAsync(ct);
                _db.RemoveRange(links);
            }
            _db.Set<AssetCategoryUnit>().AddRange(ids.Select(id => new AssetCategoryUnit { AssetCategoryId = category.Id, UnitId = id }));
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        public async Task SaveBrandAsync(BrandDefinitionInput input, CancellationToken ct = default)
        {
            string name = (input.Name ?? "").Trim();
            if (name.Length == 0) throw new ValidationException("Marka adı zorunludur.");
            if (await _db.Brands.AnyAsync(x => x.Name == name && x.Id != input.Id, ct))
                throw new ValidationException("Marka zaten kayıtlı.");
            if (input.Id == 0) _db.Brands.Add(new Brand { Name = name, IsActive = input.IsActive, CreatedAt = DateTime.UtcNow });
            else
            {
                var brand = await _db.Brands.SingleOrDefaultAsync(x => x.Id == input.Id, ct)
                    ?? throw new ValidationException("Marka bulunamadı.");
                brand.Name = name; brand.IsActive = input.IsActive; brand.UpdatedAt = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync(ct);
        }

        public async Task SaveModelAsync(AssetModelDefinitionInput input, CancellationToken ct = default)
        {
            string name = (input.Name ?? "").Trim();
            if (name.Length == 0) throw new ValidationException("Model adı zorunludur.");
            if (!await _db.Brands.AnyAsync(x => x.Id == input.BrandId && x.IsActive, ct))
                throw new ValidationException("Aktif bir marka seçiniz.");
            if (await _db.AssetModels.AnyAsync(x => x.Name == name && x.BrandId == input.BrandId && x.Id != input.Id, ct))
                throw new ValidationException("Aynı markada bu model zaten bulunuyor.");
            if (input.Id == 0) _db.AssetModels.Add(new AssetModel { BrandId = input.BrandId, Name = name, IsActive = input.IsActive, CreatedAt = DateTime.UtcNow });
            else
            {
                var model = await _db.AssetModels.SingleOrDefaultAsync(x => x.Id == input.Id, ct)
                    ?? throw new ValidationException("Model bulunamadı.");
                model.BrandId = input.BrandId; model.Name = name; model.IsActive = input.IsActive; model.UpdatedAt = DateTime.UtcNow;
            }
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<DefinitionOption>> GetModelsForBrandAsync(int brandId, CancellationToken ct = default)
            => await _db.AssetModels.AsNoTracking().Where(x => x.BrandId == brandId && x.IsActive)
                .OrderBy(x => x.Name).Select(x => new DefinitionOption { Id = x.Id, Name = x.Name, BrandId = x.BrandId, IsActive = x.IsActive }).ToListAsync(ct);

        public async Task<bool> CategoryAllowsUnitAsync(int categoryId, int unitId, CancellationToken ct = default)
        {
            var allowedIds = await _db.Set<AssetCategoryUnit>().AsNoTracking()
                .Where(x => x.AssetCategoryId == categoryId).Select(x => x.UnitId).ToListAsync(ct);
            if (allowedIds.Count == 0) return false; // Demirbaş kategorilerinde kapsam zorunlu.
            var parentMap = await _db.Units.AsNoTracking().Select(x => new { x.Id, x.ParentUnitId })
                .ToDictionaryAsync(x => x.Id, x => x.ParentUnitId, ct);
            var visited = new HashSet<int>();
            int? cursor = unitId;
            while (cursor.HasValue && visited.Add(cursor.Value))
            {
                if (allowedIds.Contains(cursor.Value)) return true;
                if (!parentMap.TryGetValue(cursor.Value, out var parent)) break;
                cursor = parent;
            }
            return false;
        }

        public async Task<string> IssueAssetCodeAsync(CancellationToken ct = default)
        {
            // SQL Server sequence: kod sayfa açıldığında ayrılır; kullanılmayan numaralarda boşluk normaldir.
            var conn = _db.Database.GetDbConnection();
            bool openedHere = conn.State != ConnectionState.Open;
            if (openedHere) await conn.OpenAsync(ct);
            try
            {
                using var command = conn.CreateCommand();
                command.CommandText = "SELECT NEXT VALUE FOR dbo.AssetCodeSequence";
                var value = Convert.ToInt64(await command.ExecuteScalarAsync(ct));
                return value.ToString("D6");
            }
            finally { if (openedHere) await conn.CloseAsync(); }
        }
    }
}
