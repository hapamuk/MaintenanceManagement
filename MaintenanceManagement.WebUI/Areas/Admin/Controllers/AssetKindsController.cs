using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize] // Gerekirse mevcut Admin rol politikanızı ayrıca uygulayın.
    public sealed class AssetKindsController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AssetKindsController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> Index(int? editId, CancellationToken ct)
        {
            var vm = new AssetKindSetupVm {
                Categories = await _db.AssetCategories.AsNoTracking().Where(x => x.IsActive)
                    .OrderBy(x => x.Name).Select(x => new SetupOption { Id=x.Id, Name=x.Name }).ToListAsync(ct),
                Brands = await _db.Brands.AsNoTracking().Where(x => x.IsActive)
                    .OrderBy(x => x.Name).Select(x => new SetupOption { Id=x.Id, Name=x.Name }).ToListAsync(ct),
                Kinds = await _db.AssetKinds.AsNoTracking()
                    .OrderBy(x => x.AssetCategory.Name).ThenBy(x => x.Name)
                    .Select(x => new KindListItem { Id=x.Id, Name=x.Name, Category=x.AssetCategory.Name, IsActive=x.IsActive })
                    .ToListAsync(ct)
            };
            if (editId.HasValue) {
                var kind=await _db.AssetKinds.AsNoTracking().SingleOrDefaultAsync(x=>x.Id==editId.Value,ct);
                if(kind is null)return NotFound();
                vm.Id=kind.Id;vm.Name=kind.Name;vm.CategoryId=kind.AssetCategoryId;vm.IsActive=kind.IsActive;
                vm.BrandIds=await _db.Set<AssetKindBrand>().AsNoTracking().Where(x=>x.AssetKindId==kind.Id)
                    .Select(x=>x.BrandId).ToListAsync(ct);
            }
            return View(vm);
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(AssetKindSetupInput input,CancellationToken ct)
        {
            if(!ModelState.IsValid){TempData["ErrorMessage"]="Tür adı ve kategori zorunludur.";return RedirectToAction(nameof(Index),new{editId=input.Id>0?(int?)input.Id:null});}
            if(!await _db.AssetCategories.AnyAsync(x=>x.Id==input.CategoryId&&x.IsActive,ct))
            {TempData["ErrorMessage"]="Kategori geçersiz.";return RedirectToAction(nameof(Index));}
            var ids=(input.BrandIds??new List<int>()).Distinct().ToList();
            if(ids.Count==0){TempData["ErrorMessage"]="En az bir marka seçmelisiniz.";return RedirectToAction(nameof(Index),new{editId=input.Id>0?(int?)input.Id:null});}
            if(await _db.Brands.CountAsync(x=>ids.Contains(x.Id)&&x.IsActive,ct)!=ids.Count)
            {TempData["ErrorMessage"]="Seçilen markalardan biri pasif veya bulunamadı.";return RedirectToAction(nameof(Index));}
            var name=input.Name.Trim();
            if(await _db.AssetKinds.AnyAsync(x=>x.Id!=input.Id&&x.AssetCategoryId==input.CategoryId&&x.Name==name,ct))
            {TempData["ErrorMessage"]="Bu kategoride aynı adlı tür zaten mevcut.";return RedirectToAction(nameof(Index),new{editId=input.Id>0?(int?)input.Id:null});}
            await using var tx=await _db.Database.BeginTransactionAsync(ct);
            AssetKind kind;
            if(input.Id==0){kind=new AssetKind{ Name=name,AssetCategoryId=input.CategoryId,IsActive=input.IsActive,CreatedAt=DateTime.UtcNow };_db.AssetKinds.Add(kind);await _db.SaveChangesAsync(ct);}
            else {
                kind=await _db.AssetKinds.SingleOrDefaultAsync(x=>x.Id==input.Id,ct) ?? throw new InvalidOperationException("Tür bulunamadı.");
                // Kullanılmış türün kategorisini değiştirmek mevcut demirbaşları bozabilir.
                if(kind.AssetCategoryId!=input.CategoryId&&await _db.Assets.AnyAsync(x=>x.AssetKindId==kind.Id,ct))
                    throw new InvalidOperationException("Bu türde demirbaş kayıtları var; kategorisi değiştirilemez.");
                kind.Name=name;kind.AssetCategoryId=input.CategoryId;kind.IsActive=input.IsActive;kind.UpdatedAt=DateTime.UtcNow;
                var old=await _db.Set<AssetKindBrand>().Where(x=>x.AssetKindId==kind.Id).ToListAsync(ct);
                // Var olan demirbaşların markalarının bağlantısını kaldıramayız.
                var requiredBrandIds=await _db.Assets.AsNoTracking().Where(x=>x.AssetKindId==kind.Id)
                    .Select(x=>x.AssetModel.BrandId).Distinct().ToListAsync(ct);
                if(requiredBrandIds.Except(ids).Any())throw new InvalidOperationException("Bu türde kayıtlı demirbaşların kullandığı marka bağlantısını kaldıramazsınız.");
                _db.Set<AssetKindBrand>().RemoveRange(old);
            }
            _db.Set<AssetKindBrand>().AddRange(ids.Select(id=>new AssetKindBrand{AssetKindId=kind.Id,BrandId=id}));
            await _db.SaveChangesAsync(ct);await tx.CommitAsync(ct);
            TempData["SuccessMessage"]="Demirbaş türü ve marka bağlantıları kaydedildi.";
            return RedirectToAction(nameof(Index));
        }
    }
    public sealed class SetupOption{public int Id{get;set;}public string Name{get;set;}="";}
    public sealed class KindListItem{public int Id{get;set;}public string Name{get;set;}="";public string Category{get;set;}="";public bool IsActive{get;set;}}
    public class AssetKindSetupInput
    {
        public int Id{get;set;}
        [Required,StringLength(150)]public string Name{get;set;}="";
        [Range(1,int.MaxValue)]public int CategoryId{get;set;}
        public bool IsActive{get;set;}=true;
        public List<int> BrandIds{get;set;}=new();
    }
    public sealed class AssetKindSetupVm:AssetKindSetupInput
    {
        public List<SetupOption> Categories{get;set;}=new();
        public List<SetupOption> Brands{get;set;}=new();
        public List<KindListItem> Kinds{get;set;}=new();
    }
}
