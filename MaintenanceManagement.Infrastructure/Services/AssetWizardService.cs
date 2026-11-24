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
            var kindBrands = await _db.Set<AssetKindBrand>().AsNoTracking()
                .Select(x => new AssetBrandKindLinkRow { KindId = x.AssetKindId, BrandId = x.BrandId }).ToListAsync(ct);
            var parentLinks = await _db.Set<AssetParentLink>().AsNoTracking().ToListAsync(ct);
            var linkLookup = parentLinks.ToDictionary(x => x.AssetId, x => (int?)x.ParentAssetId);
            // Her varlığın kategorisi, bağlı olduğu AssetKind üzerinden gelir.
            var assetTreeRows = await _db.Assets.AsNoTracking()
                .Select(x => new { x.Id, x.AssetCode, x.Name, CategoryId = x.AssetKind.AssetCategoryId })
                .ToListAsync(ct);
            var assetTree = assetTreeRows.Select(x => new AssetTreeRow {
                Id = x.Id, Code = x.AssetCode, Name = x.Name, CategoryId = x.CategoryId,
                ParentAssetId = linkLookup.TryGetValue(x.Id, out var parentId) ? parentId : null
            }).ToList();
            var scopes = await _db.Set<AssetCategoryUnit>().AsNoTracking()
                .Select(x => new AssetCategoryScopeRow { CategoryId = x.AssetCategoryId, UnitId = x.UnitId })
                .ToListAsync(ct);
            return new AssetWizardPageVm
            {
                CategoryScopes = scopes, KindBrands = kindBrands, AssetTree = assetTree,
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
            // Kullanılan organizasyon isteğe bağlıdır; boşsa NULL saklanır.
            if (!await _definitions.CategoryAllowsUnitAsync(input.AssetCategoryId, input.ResponsibleUnitId, ct) ||
                (input.UsingUnitId > 0 && !await _definitions.CategoryAllowsUnitAsync(input.AssetCategoryId, input.UsingUnitId, ct)))
                throw new InvalidOperationException("Kategori seçilen organizasyonlardan biri için uygun değil.");
            if (!await _db.Units.AnyAsync(x => x.Id == input.ResponsibleUnitId && x.IsActive, ct) ||
                (input.UsingUnitId > 0 && !await _db.Units.AnyAsync(x => x.Id == input.UsingUnitId && x.IsActive, ct)))
                throw new InvalidOperationException("Seçilen organizasyonlardan biri geçersiz veya pasif.");
            // Yalnızca aktif Genel Müdürlük veya Daire Başkanlığı ana birimi seçilebilir.
            var selectedUnitIds = input.UsingUnitId > 0
                ? new[] { input.ResponsibleUnitId, input.UsingUnitId }.Distinct().ToArray()
                : new[] { input.ResponsibleUnitId };
            var selectedUnits = await _db.Units.AsNoTracking()
                .Where(x => selectedUnitIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.IsActive })
                .ToListAsync(ct);
            if (selectedUnits.Count != selectedUnitIds.Length ||
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
            // Tür kategoriye bağlıdır; artık kullanıcı seçer, otomatik ilk tür atanmaz.
            if (!await _db.AssetKinds.AnyAsync(x => x.Id == input.AssetKindId &&
                    x.IsActive && x.AssetCategoryId == input.AssetCategoryId, ct))
                throw new InvalidOperationException("Seçilen demirbaş türü kategoriye bağlı değil veya pasif.");
            if (!await _db.Brands.AnyAsync(x => x.Id == input.BrandId && x.IsActive, ct))
                throw new InvalidOperationException("Marka bulunamadı veya pasif.");
            if (!await _db.Set<AssetKindBrand>().AnyAsync(x =>
                    x.AssetKindId == input.AssetKindId && x.BrandId == input.BrandId, ct))
                throw new InvalidOperationException("Seçilen marka bu demirbaş türüne bağlı değil. Tür Tanıtımı ekranından markayı ilişkilendirin.");
            if (input.ParentAssetId.HasValue &&
                !await _db.Assets.AsNoTracking().AnyAsync(x => x.Id == input.ParentAssetId.Value, ct))
                throw new InvalidOperationException("Seçilen üst varlık bulunamadı.");
            if (!await _db.AssetModels.AnyAsync(x => x.Id == input.AssetModelId && x.IsActive && x.BrandId == input.BrandId, ct))
                throw new InvalidOperationException("Marka ve model eşleşmiyor.");
            if (input.SiteLocationId.HasValue &&
                !await _db.SiteLocations.AnyAsync(x => x.Id == input.SiteLocationId, ct))
                throw new InvalidOperationException("Geçersiz konum.");
            if (input.AssigneeUserId.HasValue &&
                !await _db.Users.AnyAsync(x => x.Id == input.AssigneeUserId && x.IsActive, ct))
                throw new InvalidOperationException("Zimmetlenecek personel aktif değil.");

            // Doğrudan, tip güvenli alan ataması; reflection ile eksik alan aranmıyor.
            var asset = new Asset
            {
                AssetCode = "TMP" + Guid.NewGuid().ToString("N"),
                Name = input.Name.Trim(),
                UnitId = input.ResponsibleUnitId,
                UsingUnitId = input.UsingUnitId > 0 ? input.UsingUnitId : null,
                AssetKindId = input.AssetKindId,
                AssetModelId = input.AssetModelId,
                BarcodeNumber = string.IsNullOrWhiteSpace(input.BarcodeNumber)
                    ? null : input.BarcodeNumber.Trim(),
                SiteLocationId = input.SiteLocationId,
                Description = string.IsNullOrWhiteSpace(input.Description)
                    ? null : input.Description.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            SetEnum(asset, "Status", input.Status,
                new[] { "Active", "Aktif" },
                new[] { "Passive", "Pasif" },
                new[] { "Scrap", "Scrapped", "Hurda" });

            await using var transaction = await _db.Database.BeginTransactionAsync(ct);
            _db.Assets.Add(asset);
            await _db.SaveChangesAsync(ct);

            // Üst varlık, mevcut Asset tablosunu değiştirmeden bağlı tabloda saklanır.
            if (input.ParentAssetId.HasValue)
                _db.Set<AssetParentLink>().Add(new AssetParentLink {
                    AssetId = asset.Id, ParentAssetId = input.ParentAssetId.Value
                });

            // SQL Server IDENTITY tarafından verilen benzersiz Asset.Id artık mevcut.
            string generatedCode = asset.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (await _db.Assets.AsNoTracking().AnyAsync(x => x.Id != asset.Id && x.AssetCode == generatedCode, ct))
                throw new InvalidOperationException("Üretilen demirbaş kodu eski bir kayıtla çakışıyor. Mevcut kodları kontrol edin.");
            asset.AssetCode = generatedCode;
            _db.Set<AssetAuditEvent>().Add(new AssetAuditEvent
            {
                AssetId = asset.Id,
                EventType = "Kayıt",
                FieldName = "AssetCode",
                OldValue = null,
                NewValue = generatedCode,
                Description = "Demirbaş sisteme tanıtıldı. Kod: " + generatedCode,
                PerformedByUserId = actorUserId,
                OccurredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });

            // Güncelleme ve diğer hareket kayıtları aynı transaction içinde tamamlanır.

            // Yeni kayıt olayı AssetAuditEvents tablosunda tutulur.
            // AssetMovementTypeEnum üyeleri projede doğrulanmadığından
            // burada rastgele enum ile ayrı kayıt oluşturulmaz.

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


        // V8.3: Aynı altı adımlı ekranı mevcut demirbaş bilgileriyle doldurur.
        public async Task<AssetWizardPageVm?> GetEditFormAsync(int id, CancellationToken ct = default)
        {
            var asset = await _db.Assets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (asset == null) return null;
            var vm = await GetFormAsync(ct);
            var parentId = await _db.Set<AssetParentLink>().AsNoTracking()
                .Where(x => x.AssetId == id).Select(x => (int?)x.ParentAssetId)
                .FirstOrDefaultAsync(ct);
            vm.Input = new AssetWizardInput
            {
                Name = asset.Name, AssetCode = asset.AssetCode,
                AssetCategoryId = await _db.AssetKinds.AsNoTracking()
                    .Where(x => x.Id == asset.AssetKindId)
                    .Select(x => x.AssetCategoryId).FirstAsync(ct),
                AssetKindId = asset.AssetKindId,
                BrandId = await _db.AssetModels.AsNoTracking()
                    .Where(x => x.Id == asset.AssetModelId)
                    .Select(x => x.BrandId).FirstAsync(ct),
                AssetModelId = asset.AssetModelId,
                ParentAssetId = parentId,
                ResponsibleUnitId = asset.UnitId,
                UsingUnitId = asset.UsingUnitId ?? 0,
                BarcodeNumber = asset.BarcodeNumber,
                SiteLocationId = asset.SiteLocationId,
                Status = asset.Status.ToString(),
                Description = asset.Description,
                AssigneeUserId = null // Düzenleme, eski zimmeti değiştirmez.
            };
            // Varlık kendisini üst varlık olarak seçemez.
            vm.AssetTree = vm.AssetTree.Where(x => x.Id != id).ToList();
            return vm;
        }

        // Alanları mevcut varlığın ÜZERİNDE günceller; yeni Asset INSERT edilmez.
        public async Task UpdateAsync(int id, AssetWizardInput input, int actorUserId, CancellationToken ct = default)
        {
            if (actorUserId <= 0) throw new InvalidOperationException("Oturum kullanıcı kimliği bulunamadı.");
            var asset = await _db.Assets.FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new InvalidOperationException("Düzenlenecek demirbaş bulunamadı.");
            if (string.IsNullOrWhiteSpace(input.Name)) throw new InvalidOperationException("Demirbaş adı zorunludur.");
            if (input.ResponsibleUnitId <= 0 ||
                !await _db.Units.AnyAsync(x => x.Id == input.ResponsibleUnitId && x.IsActive, ct))
                throw new InvalidOperationException("Aktif sorumlu organizasyon seçilmelidir.");
            if (input.UsingUnitId > 0 &&
                !await _db.Units.AnyAsync(x => x.Id == input.UsingUnitId && x.IsActive, ct))
                throw new InvalidOperationException("Kullanılan organizasyon aktif olmalıdır.");
            if (!await _definitions.CategoryAllowsUnitAsync(input.AssetCategoryId, input.ResponsibleUnitId, ct) ||
                (input.UsingUnitId > 0 && !await _definitions.CategoryAllowsUnitAsync(input.AssetCategoryId, input.UsingUnitId, ct)))
                throw new InvalidOperationException("Kategori seçilen organizasyonun kapsamında değil.");
            if (!await _db.AssetKinds.AnyAsync(x => x.Id == input.AssetKindId && x.IsActive && x.AssetCategoryId == input.AssetCategoryId, ct))
                throw new InvalidOperationException("Kategori ve demirbaş türü uyuşmuyor.");
            if (!await _db.Set<AssetKindBrand>().AnyAsync(x => x.AssetKindId == input.AssetKindId && x.BrandId == input.BrandId, ct))
                throw new InvalidOperationException("Demirbaş türü ve marka uyuşmuyor.");
            if (!await _db.AssetModels.AnyAsync(x => x.Id == input.AssetModelId && x.IsActive && x.BrandId == input.BrandId, ct))
                throw new InvalidOperationException("Marka ve model uyuşmuyor.");
            if (input.SiteLocationId.HasValue &&
                !await _db.SiteLocations.AnyAsync(x => x.Id == input.SiteLocationId.Value, ct))
                throw new InvalidOperationException("Konum bulunamadı.");
            if (input.ParentAssetId == id)
                throw new InvalidOperationException("Bir demirbaş kendisinin üst varlığı olamaz.");
            if (input.ParentAssetId.HasValue &&
                !await _db.Assets.AnyAsync(x => x.Id == input.ParentAssetId.Value, ct))
                throw new InvalidOperationException("Üst varlık bulunamadı.");
            // Mevcut üst-alt ağacında döngü oluşmasını engelle.
            var links = await _db.Set<AssetParentLink>().AsNoTracking().ToListAsync(ct);
            var parentByChild = links.ToDictionary(x => x.AssetId, x => x.ParentAssetId);
            var visited = new HashSet<int>();
            int? cursor = input.ParentAssetId;
            while (cursor.HasValue && visited.Add(cursor.Value))
            {
                if (cursor.Value == id)
                    throw new InvalidOperationException("Bu seçim üst-alt varlık ilişkisinde döngü oluşturur.");
                cursor = parentByChild.TryGetValue(cursor.Value, out int parent) ? parent : null;
            }
            if (cursor.HasValue) throw new InvalidOperationException("Mevcut üst varlık ağacında döngü var.");
            if (!Enum.TryParse<MaintenanceManagement.Domain.Enums.AssetStatusEnum>(input.Status, true, out var status) ||
                !Enum.IsDefined(typeof(MaintenanceManagement.Domain.Enums.AssetStatusEnum), status))
                throw new InvalidOperationException("Geçersiz demirbaş durumu.");

            // Eski değerler ilişkiler değişmeden ÖNCE alınır. Olay kayıtları daha sonra güncellenemez.
            var oldKindId = asset.AssetKindId;
            var oldModelId = asset.AssetModelId;
            var oldResponsibleUnitId = asset.UnitId;
            var oldUsingUnitId = asset.UsingUnitId;
            var oldLocationId = asset.SiteLocationId;
            var oldName = asset.Name;
            var oldBarcode = asset.BarcodeNumber;
            var oldDescription = asset.Description;
            var oldStatus = asset.Status;
            var oldLink = await _db.Set<AssetParentLink>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.AssetId == id, ct);
            int? oldParentId = oldLink?.ParentAssetId;

            // Snapshot adlarını DB'den alıp, olay açıklamasına anlık haliyle kaydet.
            var unitNames = await _db.Units.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
            var kindNames = await _db.AssetKinds.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
            var modelNames = await _db.AssetModels.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
            var kindCategoryIds = await _db.AssetKinds.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.AssetCategoryId, ct);
            var categoryNames = await _db.AssetCategories.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
            var modelBrandIds = await _db.AssetModels.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.BrandId, ct);
            var brandNames = await _db.Brands.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.Name, ct);
            // SiteLocation entity alanları projeden projeye değişebildiği için bellekte okunur.
            var locations = await _db.SiteLocations.AsNoTracking().ToListAsync(ct);
            var locationNames = locations.ToDictionary(x => x.Id,
                x => ReadString(x, "Name", "LocationName", "Code"));
            var parentIds = new[] { oldParentId, input.ParentAssetId }.Where(x => x.HasValue)
                .Select(x => x!.Value).Distinct().ToArray();
            var parentNames = await _db.Assets.AsNoTracking()
                .Where(x => parentIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.AssetCode + " - " + x.Name, ct);

            string FormatName(int? value, IDictionary<int, string> lookup) =>
                !value.HasValue ? "(Seçilmedi)" :
                lookup.TryGetValue(value.Value, out var label) ? label : "#" + value.Value;
            var auditRows = new List<AssetAuditEvent>();
            void Record<T>(string field, string label, T before, T after)
            {
                if (EqualityComparer<T>.Default.Equals(before, after)) return;
                var oldLabel = Convert.ToString(before) ?? "(Boş)";
                var newLabel = Convert.ToString(after) ?? "(Boş)";
                auditRows.Add(new AssetAuditEvent
                {
                    AssetId = id, EventType = label, FieldName = field,
                    OldValue = oldLabel, NewValue = newLabel,
                    Description = label + ": " + oldLabel + " → " + newLabel,
                    PerformedByUserId = actorUserId, OccurredAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                });
            }
            // İş emri ve zimmet bu günlükte tutulmaz: kendi tablolarından gösterilir.
            Record("Name", "Demirbaş Adı Değişikliği", oldName, input.Name.Trim());
            Record("UnitId", "Sorumlu Organizasyon Değişikliği",
                FormatName(oldResponsibleUnitId, unitNames),
                FormatName(input.ResponsibleUnitId, unitNames));
            Record("UsingUnitId", "Kullanılan Organizasyon Değişikliği",
                FormatName(oldUsingUnitId, unitNames),
                FormatName(input.UsingUnitId > 0 ? input.UsingUnitId : (int?)null, unitNames));
            Record("SiteLocationId", "Konum Değişikliği",
                FormatName(oldLocationId, locationNames), FormatName(input.SiteLocationId, locationNames));
            Record("AssetCategoryId", "Kategori Değişikliği",
                FormatName(kindCategoryIds.TryGetValue(oldKindId, out var oldCategory) ? oldCategory : (int?)null, categoryNames),
                FormatName(input.AssetCategoryId, categoryNames));
            Record("BrandId", "Marka Değişikliği",
                FormatName(modelBrandIds.TryGetValue(oldModelId, out var oldBrand) ? oldBrand : (int?)null, brandNames),
                FormatName(input.BrandId, brandNames));
            Record("AssetKindId", "Demirbaş Türü Değişikliği",
                FormatName(oldKindId, kindNames), FormatName(input.AssetKindId, kindNames));
            Record("AssetModelId", "Model Değişikliği",
                FormatName(oldModelId, modelNames), FormatName(input.AssetModelId, modelNames));
            Record("BarcodeNumber", "Barkod Değişikliği", oldBarcode ?? "(Boş)",
                string.IsNullOrWhiteSpace(input.BarcodeNumber) ? "(Boş)" : input.BarcodeNumber.Trim());
            Record("Description", "Açıklama Değişikliği", oldDescription ?? "(Boş)",
                string.IsNullOrWhiteSpace(input.Description) ? "(Boş)" : input.Description.Trim());
            Record("Status", "Durum Değişikliği", oldStatus.ToString(), status.ToString());
            Record("ParentAssetId", "Üst Varlık Değişikliği",
                FormatName(oldParentId, parentNames), FormatName(input.ParentAssetId, parentNames));

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            asset.Name = input.Name.Trim();
            asset.UnitId = input.ResponsibleUnitId;
            asset.UsingUnitId = input.UsingUnitId > 0 ? input.UsingUnitId : null;
            asset.AssetKindId = input.AssetKindId;
            asset.AssetModelId = input.AssetModelId;
            asset.BarcodeNumber = string.IsNullOrWhiteSpace(input.BarcodeNumber) ? null : input.BarcodeNumber.Trim();
            asset.SiteLocationId = input.SiteLocationId;
            asset.Description = string.IsNullOrWhiteSpace(input.Description) ? null : input.Description.Trim();
            asset.Status = status;
            asset.UpdatedAt = DateTime.UtcNow;
            // AssetCode, Id, CreatedAt ve mevcut zimmetler değiştirilmez.
            var link = await _db.Set<AssetParentLink>().FirstOrDefaultAsync(x => x.AssetId == id, ct);
            if (input.ParentAssetId.HasValue)
            {
                if (link == null) _db.Set<AssetParentLink>().Add(new AssetParentLink { AssetId = id, ParentAssetId = input.ParentAssetId.Value });
                else link.ParentAssetId = input.ParentAssetId.Value;
            }
            else if (link != null) _db.Set<AssetParentLink>().Remove(link);
            _db.Set<AssetAuditEvent>().AddRange(auditRows);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        // Zimmetin kaynağı AssetUsers tablosudur. Devirde eski satır kapanır, yeni satır açılır.
        public async Task AssignExistingAsync(int id, int assigneeUserId, int actorUserId, CancellationToken ct = default)
        {
            if (actorUserId <= 0) throw new InvalidOperationException("Oturum kullanıcı kimliği bulunamadı.");
            var asset = await _db.Assets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new InvalidOperationException("Demirbaş bulunamadı.");
            if (!asset.UsingUnitId.HasValue)
                throw new InvalidOperationException("Zimmet işlemi için önce Kullanılan Organizasyon seçilmelidir.");
            var unitId = await _db.Users.AsNoTracking()
                .Where(x => x.Id == assigneeUserId && x.IsActive)
                .Select(x => (int?)x.PrimaryUnitId).FirstOrDefaultAsync(ct);
            if (!unitId.HasValue) throw new InvalidOperationException("Seçilen personel aktif değil.");
            var units = await _db.Units.AsNoTracking().Where(x => x.IsActive)
                .Select(x => new { x.Id, x.ParentUnitId }).ToListAsync(ct);
            var parents = units.ToDictionary(x => x.Id, x => x.ParentUnitId);
            var visited = new HashSet<int>();
            int? currentUnit = unitId;
            bool allowed = false;
            while (currentUnit.HasValue && visited.Add(currentUnit.Value))
            {
                if (currentUnit.Value == asset.UsingUnitId.Value) { allowed = true; break; }
                currentUnit = parents.TryGetValue(currentUnit.Value, out var parent) ? parent : null;
            }
            if (!allowed)
                throw new InvalidOperationException("Personel kullanılan organizasyona veya aktif alt birimine bağlı değil.");

            await using var tx = await _db.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable, ct);
            var open = await _db.AssetUsers
                .Where(x => x.AssetId == id && x.EndDate == null)
                .OrderByDescending(x => x.StartDate).ToListAsync(ct);
            if (open.Count > 1)
                throw new InvalidOperationException("Birden fazla açık zimmet var; önce kayıtları düzeltin.");
            if (open.Count == 1 && open[0].UserId == assigneeUserId)
                throw new InvalidOperationException("Demirbaş zaten bu personele zimmetli.");
            var now = DateTime.UtcNow;
            if (open.Count == 1)
            {
                open[0].EndDate = now;
                open[0].UpdatedAt = now;
            }
            _db.AssetUsers.Add(new AssetUser
            {
                AssetId = id, UserId = assigneeUserId, AssignedByUserId = actorUserId,
                StartDate = now, CreatedAt = now
            });
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        public async Task ReturnExistingAsync(int id, int actorUserId, CancellationToken ct = default)
        {
            if (actorUserId <= 0) throw new InvalidOperationException("Oturum kullanıcı kimliği bulunamadı.");
            await using var tx = await _db.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable, ct);
            var open = await _db.AssetUsers
                .Where(x => x.AssetId == id && x.EndDate == null).ToListAsync(ct);
            if (open.Count != 1)
                throw new InvalidOperationException("İade için tek bir aktif zimmet kaydı bulunmalıdır.");
            open[0].EndDate = DateTime.UtcNow;
            open[0].UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        public async Task<AssetDetailPageVm?> GetDetailAsync(int assetId, CancellationToken ct = default)
        {
            var asset = await _db.Assets.AsNoTracking()
                .Include(x => x.AssetKind).ThenInclude(x => x.AssetCategory)
                .Include(x => x.AssetModel).ThenInclude(x => x.Brand)
                .Include(x => x.Unit)
                .Include(x => x.UsingUnit)
                .Include(x => x.SiteLocation)
                .Include(x => x.AssetUsers).ThenInclude(x => x.User)
                .Include(x => x.AssetMovements).ThenInclude(x => x.PerformedByUser)
                .Include(x => x.WorkOrders)
                .SingleOrDefaultAsync(x => x.Id == assetId, ct);
            if (asset == null) return null;
            return new AssetDetailPageVm {
                Id = asset.Id, Code = asset.AssetCode, Name = asset.Name, CreatedAt = asset.CreatedAt,
                Category = asset.AssetKind?.AssetCategory?.Name ?? "—",
                BrandModel = (asset.AssetModel?.Brand?.Name ?? "—") + " / " + (asset.AssetModel?.Name ?? "—"),
                Status = asset.Status.ToString(), Barcode = asset.BarcodeNumber ?? "",
                Location = asset.SiteLocation == null ? "—" : ReadString(asset.SiteLocation, "Name", "LocationName", "Code"), Description = asset.Description ?? "",
                ResponsibleUnit = asset.Unit?.Name ?? "—", UsingUnit = asset.UsingUnit?.Name ?? "—",
                Assignments = asset.AssetUsers.OrderByDescending(x => x.StartDate).Select(x => new AssetAssignmentRow {
                    User = (x.User?.FirstName ?? "") + " " + (x.User?.LastName ?? ""), Start=x.StartDate, End=x.EndDate
                }).ToList(),
                WorkOrders = asset.WorkOrders.OrderByDescending(x=>x.CreatedAt).Select(x=>new AssetHistoryRow {
                    Date=x.CreatedAt, Type="İş Emri", Reference=x.WorkOrderNumber, Description=x.Title
                }).ToList(),
                History = await BuildOtherHistoryAsync(asset, ct)
            };
        }

        private async Task<List<AssetHistoryRow>> BuildOtherHistoryAsync(Asset asset, CancellationToken ct)
        {
            var auditRows = await (
                from log in _db.Set<AssetAuditEvent>().AsNoTracking()
                join user in _db.Users.AsNoTracking() on log.PerformedByUserId equals user.Id into users
                from user in users.DefaultIfEmpty()
                where log.AssetId == asset.Id
                orderby log.OccurredAt descending
                select new AssetHistoryRow
                {
                    Date = log.OccurredAt,
                    Type = log.EventType,
                    Description = log.Description,
                    Reference = "",
                    PerformedBy = user == null ? "—" : user.FirstName + " " + user.LastName
                }).ToListAsync(ct);

            // Eski AssetMovements korunur. Zimmet ve iş emri hareketleri bu sekmede TEKRAR gösterilmez.
            var legacy = asset.AssetMovements
                .Where(x => !IsAssignmentOrWorkOrderType(ReadString(x, "MovementType")) &&
                    !(auditRows.Any(a => a.Type == "Kayıt") &&
                      (ReadString(x, "MovementType").Equals("Registered", StringComparison.OrdinalIgnoreCase) ||
                       ReadString(x, "MovementType").Equals("Created", StringComparison.OrdinalIgnoreCase) ||
                       ReadString(x, "MovementType").Equals("Registration", StringComparison.OrdinalIgnoreCase))))
                .Select(x => new AssetHistoryRow
                {
                    Date = x.MovementDate, Type = ReadString(x, "MovementType"),
                    Description = ReadString(x, "Description"),
                    Reference = "",
                    PerformedBy = x.PerformedByUser == null ? "—" :
                        x.PerformedByUser.FirstName + " " + x.PerformedByUser.LastName
                });
            return auditRows.Concat(legacy).OrderByDescending(x => x.Date).ToList();
        }

        private static bool IsAssignmentOrWorkOrderType(string type)
        {
            // Bu filtre yalnızca geçmiş AssetMovement kayıtları içindir.
            // Gerçek enum paylaşılınca kesin enum üyeleriyle değiştirilebilir.
            string t = type.ToLowerInvariant();
            return t.Contains("zimmet") || t.Contains("assign") || t.Contains("return") ||
                   t.Contains("transfer") || t.Contains("workorder") || t.Contains("iş emri") ||
                   t.Contains("work order") || t.Contains("maintenance");
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
