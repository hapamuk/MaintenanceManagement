// ÖNEMLİ: Bu EF uyarlayıcısı mevcut mapping'de doğrulanabilen alanları kullanır.
// Asset, AssetMovement, SiteLocation ve durum enumunun SON sürümleriyle test edilmeden
// üretimde kullanılmamalıdır. Özellikle MovementType enum değerleri doğrulanmalıdır.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Application.DTOs.AssetWizard;
using MaintenanceManagement.Application.Interfaces.Services;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.Infrastructure.Services
{
    public sealed class AssetWizardService : IAssetWizardService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAssetDefinitionService _definitions;
        public AssetWizardService(ApplicationDbContext db, IAssetDefinitionService definitions)
        {
            _db = db;
            _definitions = definitions;
        }

        public async Task<AssetWizardPageVm> GetFormAsync(CancellationToken ct = default)
        {
            var units = await _db.Units.AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.Name).Select(x => new WizardOption { Id = x.Id, Text = x.Name, ParentId = x.ParentUnitId }).ToListAsync(ct);
            var categories = await _db.AssetCategories.AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.Name).Select(x => new WizardOption { Id = x.Id, Text = x.Name }).ToListAsync(ct);
            var kinds = await _db.AssetKinds.AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.Name).Select(x => new WizardOption { Id = x.Id, Text = x.Name, GroupId = x.AssetCategoryId }).ToListAsync(ct);
            var brands = await _db.Brands.AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.Name).Select(x => new WizardOption { Id = x.Id, Text = x.Name }).ToListAsync(ct);
            var models = await _db.AssetModels.AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.Name).Select(x => new WizardOption { Id = x.Id, Text = x.Name, GroupId = x.BrandId }).ToListAsync(ct);
            // SiteLocation alan adları (ör. Name) mevcut entity sürümünüzde doğrulanmalıdır.
            var locations = await _db.SiteLocations.AsNoTracking().ToListAsync(ct);
            var users = await _db.Users.AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .Select(x => new WizardOption { Id = x.Id, Text = x.FirstName + " " + x.LastName, GroupId = x.PrimaryUnitId }).ToListAsync(ct);
            var scopes = await _db.Set<AssetCategoryUnit>().AsNoTracking()
                .Select(x => new AssetCategoryScopeRow { CategoryId = x.AssetCategoryId, UnitId = x.UnitId })
                .ToListAsync(ct);
            return new AssetWizardPageVm
            {
                CategoryScopes = scopes,
                Units = units, Categories = categories, Kinds = kinds, Brands = brands, Models = models,
                Locations = locations.Select(x => new WizardOption { Id = x.Id, Text = ReadString(x, "Name", "LocationName", "Code") }).ToList(),
                Users = users
            };
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
            => _db.Assets.AsNoTracking().AnyAsync(x => x.AssetCode == code, ct);

        public async Task<int> CreateAsync(AssetWizardInput input, int actorUserId, CancellationToken ct = default)
        {
            if (actorUserId <= 0) throw new InvalidOperationException("Oturum kullanıcı kimliği bulunamadı.");
            if (input.Status != "Active" && input.Status != "Passive" && input.Status != "Scrap")
                throw new InvalidOperationException("Geçersiz demirbaş durumu.");
            // Kullanılan organizasyon isteğe bağlı. Mevcut Asset modelindeki zorunlu
            // UsingUnitId/UsageUnitId FK alanını bozmamak için boşken sorumlu birim
            // teknik varsayılan değer olur. Gerçek NULL saklamak için nullable FK migration gerekir.
            int effectiveUsingUnitId = input.UsingUnitId > 0
                ? input.UsingUnitId : input.ResponsibleUnitId;
            if (!await _definitions.CategoryAllowsUnitAsync(input.AssetCategoryId, input.ResponsibleUnitId, ct) ||
                (input.UsingUnitId > 0 && !await _definitions.CategoryAllowsUnitAsync(input.AssetCategoryId, input.UsingUnitId, ct)))
                throw new InvalidOperationException("Kategori seçilen organizasyonlardan biri için uygun değil.");
            if (!await _db.Units.AnyAsync(x => x.Id == input.ResponsibleUnitId && x.IsActive, ct) ||
                (input.UsingUnitId > 0 && !await _db.Units.AnyAsync(x => x.Id == input.UsingUnitId && x.IsActive, ct)))
                throw new InvalidOperationException("Seçilen organizasyonlardan biri geçersiz veya pasif.");
            // Yalnızca aktif Genel Müdürlük veya Daire Başkanlığı ana birimi seçilebilir.
            var selectedUnits = await _db.Units.AsNoTracking()
                .Where(x => x.Id == input.ResponsibleUnitId || x.Id == effectiveUsingUnitId)
                .Select(x => new { x.Id, x.Name, x.IsActive })
                .ToListAsync(ct);
            if (selectedUnits.Count != (input.ResponsibleUnitId == effectiveUsingUnitId ? 1 : 2) ||
                selectedUnits.Any(x => !x.IsActive || !IsSelectableOrganization(x.Name)))
                throw new InvalidOperationException("Yalnızca aktif Genel Müdürlük veya Daire Başkanlığı seçilebilir.");
            if (input.AssigneeUserId.HasValue)
            {
                if (input.UsingUnitId <= 0)
                    throw new InvalidOperationException("İlk zimmet için kullanılan organizasyon seçilmelidir.");
                var assignedUnitId = await _db.Users.AsNoTracking()
                    .Where(x => x.Id == input.AssigneeUserId.Value && x.IsActive)
                    .Select(x => (int?)x.PrimaryUnitId).FirstOrDefaultAsync(ct);
                if (!assignedUnitId.HasValue)
                    throw new InvalidOperationException("Zimmet personeli aktif değil veya birimi bulunmuyor.");
                var hierarchy = await _db.Units.AsNoTracking().Where(x => x.IsActive)
                    .Select(x => new { x.Id, x.ParentUnitId }).ToListAsync(ct);
                var parents = hierarchy.ToDictionary(x => x.Id, x => x.ParentUnitId);
                var visited = new HashSet<int>();
                int? current = assignedUnitId;
                bool allowed = false;
                while (current.HasValue && visited.Add(current.Value))
                {
                    if (current.Value == input.UsingUnitId) { allowed=true; break; }
                    current = parents.TryGetValue(current.Value, out var parent) ? parent : null;
                }
                if (!allowed) throw new InvalidOperationException("Zimmet personeli, kullanan organizasyona veya aktif alt birimine bağlı olmalıdır.");
            }
            // Tür alanı kullanıcıya gösterilmez; mevcut Asset.AssetKindId FK alanı
            // bozulmasın diye kategoriye ait ilk aktif tür servis tarafından seçilir.
            // Kategoride hiç aktif tür yoksa yanlış/boş FK yazmak yerine açık hata ver.
            int resolvedKindId = await _db.AssetKinds.AsNoTracking()
                .Where(x => x.IsActive && x.AssetCategoryId == input.AssetCategoryId)
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(ct);
            if (resolvedKindId <= 0)
                throw new InvalidOperationException("Seçilen kategoride aktif demirbaş türü bulunmuyor. Tür tablosu zorunlu FK olduğundan en az bir aktif tür tanımı gereklidir.");
            input.AssetKindId = resolvedKindId;
            if (!await _db.AssetModels.AnyAsync(x => x.Id == input.AssetModelId && x.IsActive && x.BrandId == input.BrandId, ct))
                throw new InvalidOperationException("Marka ve model eşleşmiyor.");
            if (input.SiteLocationId.HasValue &&
                !await _db.SiteLocations.AnyAsync(x => x.Id == input.SiteLocationId, ct))
                throw new InvalidOperationException("Geçersiz konum.");
            if (input.AssigneeUserId.HasValue &&
                !await _db.Users.AnyAsync(x => x.Id == input.AssigneeUserId && x.IsActive, ct))
                throw new InvalidOperationException("Zimmetlenecek personel aktif değil.");

            // Entity'nin mevcut alanları: AssetCode, Name, UnitId, AssetKindId, AssetModelId.
            // Ayrı sorumlu/kullanan birim, barkod, konum alanları var mı kontrollü olarak doğrulanır.
            var asset = new Asset {
                // AssetCode ilk SaveChanges sırasında geçici benzersiz değer taşır.
                // ID üretildikten sonra aynı transaction içinde gerçek koda güncellenir.
                AssetCode = "TMP" + Guid.NewGuid().ToString("N"), Name = input.Name.Trim(), UnitId = input.ResponsibleUnitId,
                AssetKindId = input.AssetKindId, AssetModelId = input.AssetModelId,
                CreatedAt = DateTime.UtcNow
            };
            RequireSet(asset, effectiveUsingUnitId, "UsingUnitId", "UsageUnitId");
            if (!string.IsNullOrWhiteSpace(input.BarcodeNumber)) RequireSet(asset, input.BarcodeNumber.Trim(), "BarcodeNumber", "Barcode");
            if (input.SiteLocationId.HasValue) RequireSet(asset, input.SiteLocationId.Value, "SiteLocationId", "LocationId");
            if (!string.IsNullOrWhiteSpace(input.Description)) RequireSet(asset, input.Description.Trim(), "Description");
            SetEnum(asset, "Status", input.Status, new[] { "Active", "Aktif" }, new[] { "Passive", "Pasif" }, new[] { "Scrap", "Scrapped", "Hurda" });

            await using var transaction = await _db.Database.BeginTransactionAsync(ct);
            _db.Assets.Add(asset);
            await _db.SaveChangesAsync(ct);

            // SQL Server IDENTITY tarafından verilen benzersiz Asset.Id artık mevcut.
            string generatedCode = asset.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (await _db.Assets.AsNoTracking().AnyAsync(x => x.Id != asset.Id && x.AssetCode == generatedCode, ct))
                throw new InvalidOperationException("Üretilen demirbaş kodu eski bir kayıtla çakışıyor. Mevcut kodları kontrol edin.");
            asset.AssetCode = generatedCode;
            // Güncelleme ve diğer hareket kayıtları aynı transaction içinde tamamlanır.

            // Hareket kaydı: gerçek MovementType enumunun 'Registered/Created' üyesi
            // doğrulanmadan keyfî sayısal değer atamayız.
            var movement = new AssetMovement();
            RequireSet(movement, asset.Id, "AssetId");
            RequireSet(movement, actorUserId, "PerformedByUserId");
            RequireSet(movement, DateTime.UtcNow, "MovementDate");
            TrySet(movement, DateTime.UtcNow, "CreatedAt");
            TrySet(movement, "Demirbaş kaydı oluşturuldu.", "Description");
            SetEnumFromCandidates(movement, "MovementType", "Registered", "Created", "Registration");
            _db.AssetMovements.Add(movement);

            if (input.AssigneeUserId.HasValue)
            {
                var assignment = new AssetUser {
                    AssetId = asset.Id, UserId = input.AssigneeUserId.Value,
                    AssignedByUserId = actorUserId, StartDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                _db.AssetUsers.Add(assignment);
                // İlk zimmet için ayrı hareket kaydı, mevcut MovementType enumu
                // doğrulandığında burada eklenmelidir; sessizce yanlış log yazılmaz.
            }
            await _db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return asset.Id;
        }

        public async Task<AssetDetailPageVm?> GetDetailAsync(int assetId, CancellationToken ct = default)
        {
            var asset = await _db.Assets.AsNoTracking()
                .Include(x => x.AssetKind).ThenInclude(x => x.AssetCategory)
                .Include(x => x.AssetModel).ThenInclude(x => x.Brand)
                .Include(x => x.Unit)
                .Include(x => x.AssetUsers).ThenInclude(x => x.User)
                .Include(x => x.AssetMovements).ThenInclude(x => x.PerformedByUser)
                .Include(x => x.WorkOrders)
                .SingleOrDefaultAsync(x => x.Id == assetId, ct);
            if (asset == null) return null;
            string unitName(int? id) => id.HasValue ? (_db.Units.AsNoTracking().Where(x => x.Id == id.Value).Select(x => x.Name).FirstOrDefault() ?? "—") : "—";
            int? usingId = ReadInt(asset, "UsingUnitId", "UsageUnitId");
            return new AssetDetailPageVm {
                Id = asset.Id, Code = asset.AssetCode, Name = asset.Name, CreatedAt = asset.CreatedAt,
                Category = asset.AssetKind?.AssetCategory?.Name ?? "—",
                BrandModel = (asset.AssetModel?.Brand?.Name ?? "—") + " / " + (asset.AssetModel?.Name ?? "—"),
                Status = asset.Status.ToString(), Barcode = ReadString(asset,"BarcodeNumber","Barcode"),
                Location = ReadString(asset,"LocationName","Location"), Description = ReadString(asset,"Description"),
                ResponsibleUnit = asset.Unit?.Name ?? "—", UsingUnit = unitName(usingId),
                Assignments = asset.AssetUsers.OrderByDescending(x => x.StartDate).Select(x => new AssetAssignmentRow {
                    User = (x.User?.FirstName ?? "") + " " + (x.User?.LastName ?? ""), Start=x.StartDate, End=x.EndDate
                }).ToList(),
                WorkOrders = asset.WorkOrders.OrderByDescending(x=>x.CreatedAt).Select(x=>new AssetHistoryRow {
                    Date=x.CreatedAt, Type="İş Emri", Reference=x.WorkOrderNumber, Description=x.Title
                }).ToList(),
                History = asset.AssetMovements.OrderByDescending(x=>x.MovementDate).Select(x=>new AssetHistoryRow {
                    Date=x.MovementDate, Type=ReadString(x,"MovementType"), Description=ReadString(x,"Description"),
                    PerformedBy=x.PerformedByUser == null ? "—" : x.PerformedByUser.FirstName+" "+x.PerformedByUser.LastName
                }).ToList()
            };
        }

        private static bool IsSelectableOrganization(string name) =>
            name.Trim().Equals("Genel Müdürlük", StringComparison.OrdinalIgnoreCase) ||
            name.Trim().EndsWith("Dairesi Başkanlığı", StringComparison.OrdinalIgnoreCase) ||
            name.Trim().EndsWith("Daire Başkanlığı", StringComparison.OrdinalIgnoreCase);

        private static string ReadString(object instance, params string[] names)
        {
            foreach(string name in names) { var prop=instance.GetType().GetProperty(name); if(prop!=null) return prop.GetValue(instance)?.ToString() ?? ""; }
            return "";
        }
        private static int? ReadInt(object instance, params string[] names)
        {
            foreach(var name in names) { var p=instance.GetType().GetProperty(name); if(p==null) continue; var v=p.GetValue(instance); if(v!=null)return Convert.ToInt32(v); }
            return null;
        }
        private static bool TrySet(object instance, object value, params string[] names)
        {
            foreach(string name in names) {
                var p=instance.GetType().GetProperty(name,BindingFlags.Public|BindingFlags.Instance);
                if(p == null || !p.CanWrite) continue;
                var type=Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                p.SetValue(instance, Convert.ChangeType(value,type)); return true;
            }
            return false;
        }
        private static void RequireSet(object instance, object value, params string[] names)
        {
            if(!TrySet(instance,value,names)) throw new InvalidOperationException(instance.GetType().Name+" sınıfında gerekli alan bulunamadı: "+string.Join(" / ",names)+". Entity ve migration kontrol edilmeli.");
        }
        private static void SetEnumFromCandidates(object instance, string propertyName, params string[] candidates)
        {
            var p=instance.GetType().GetProperty(propertyName);
            if(p == null || !p.PropertyType.IsEnum) throw new InvalidOperationException(propertyName+" enumu doğrulanamadı.");
            foreach (var candidate in candidates) if(Enum.GetNames(p.PropertyType).Any(x=>string.Equals(x,candidate,StringComparison.OrdinalIgnoreCase))) {
                p.SetValue(instance,Enum.Parse(p.PropertyType,candidate,true)); return;
            }
            throw new InvalidOperationException("Hareket enumunda kayıt türü bulunamadı. Mevcut enumu inceleyerek eşleştirin.");
        }
        private static void SetEnum(object obj,string propName,string value,string[] active,string[] passive,string[] scrap)
        {
            var names=value=="Active"?active:value=="Passive"?passive:scrap;
            var prop=obj.GetType().GetProperty(propName);
            if(prop==null || !prop.PropertyType.IsEnum) throw new InvalidOperationException("Asset.Status enum alanı doğrulanamadı.");
            foreach(var name in names) if(Enum.GetNames(prop.PropertyType).Any(x=>string.Equals(x,name,StringComparison.OrdinalIgnoreCase))) { prop.SetValue(obj, Enum.Parse(prop.PropertyType,name,true));return; }
            throw new InvalidOperationException("Demirbaş durum enumu uyuşmuyor. Asset.Status için değerleri eşleştirin.");
        }
    }
}
