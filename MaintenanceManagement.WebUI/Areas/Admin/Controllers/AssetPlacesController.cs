using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaintenanceManagement.Infrastructure.Data;
using MaintenanceManagement.Infrastructure.Services;
using MaintenanceManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceManagement.WebUI.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public sealed class AssetPlacesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AssetPlacesController(ApplicationDbContext db) => _db = db;

        // Eski verilerin değiştirilmediği, yeni oda ile yeni konumun birlikte kaydedildiği işlem.
        private IActionResult PlaceError(string message)
        {
            TempData["AssetPlaceError"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var savedLocations = await _db.SiteLocations.AsNoTracking().ToListAsync(ct);
            ViewBag.Locations = savedLocations.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = x.Id.ToString(),
                Text = (x.GetType().GetProperty("Name")?.GetValue(x)?.ToString()
                    ?? x.GetType().GetProperty("LocationName")?.GetValue(x)?.ToString()
                    ?? "Konum") + " (ID: " + x.Id + ")"
            }).ToList();
            return View();
        }

        // Sadece aktif bina, kat ve odalar. Kaydı olmayan odalar da görünür ancak demirbaşa seçilemez.
        [HttpGet]
        public async Task<IActionResult> Hierarchy(CancellationToken ct = default)
        {
            var data = await _db.AssetBuildings.AsNoTracking().Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new {
                    x.Id, x.Name, x.Latitude, x.Longitude, x.Province, x.District, x.Neighborhood,
                    Floors = x.Floors.Where(f => f.IsActive).OrderBy(f => f.Name)
                        .Select(f => new { f.Id, f.Name,
                            Rooms = f.Rooms.Where(r => r.IsActive).OrderBy(r => r.Name)
                                .Select(r => new { r.Id, r.Name, r.SiteLocationId }) })
                }).ToListAsync(ct);
            return Json(data);
        }

        // Düzenleme ekranında pasif kayıtlar da listelenir; normal sihirbazdaki Hierarchy aktifleri gösterir.
        [HttpGet]
        public async Task<IActionResult> ManagementHierarchy(CancellationToken ct = default)
        {
            var rows = await _db.AssetBuildings.AsNoTracking().OrderBy(x => x.Name)
                .Select(x => new {
                    x.Id, x.Name, x.Latitude, x.Longitude, x.Province, x.District, x.Neighborhood, x.IsActive,
                    Floors = x.Floors.OrderBy(f => f.Name).Select(f => new {
                        f.Id, f.Name, f.IsActive,
                        Rooms = f.Rooms.OrderBy(r => r.Name).Select(r => new { r.Id, r.Name, r.IsActive, r.SiteLocationId })
                    })
                }).ToListAsync(ct);
            return Json(rows);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBuilding(string name, string latitude, string longitude,
            string? province, string? district, string? neighborhood, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200 ||
                !TryParseCoordinate(latitude, -90, 90, out var latitudeValue) ||
                !TryParseCoordinate(longitude, -180, 180, out var longitudeValue))
            {
                TempData["AssetPlaceError"] = "Bina adı veya harita koordinatları geçersiz. Haritadan konum seçin.";
                return RedirectToAction(nameof(Index));
            }

            _db.AssetBuildings.Add(new AssetBuilding
            {
                Name = name.Trim(),
                Latitude = latitudeValue,
                Longitude = longitudeValue,
                Province = CleanArea(province),
                District = CleanArea(district),
                Neighborhood = CleanArea(neighborhood)
            });
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFloor(int buildingId, string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length>100 ||
                !await _db.AssetBuildings.AnyAsync(x=>x.Id==buildingId && x.IsActive,ct))
                return BadRequest("Bina veya kat adı geçersiz.");
            if(await _db.AssetBuildingFloors.AnyAsync(x=>x.AssetBuildingId==buildingId && x.Name==name.Trim(),ct))
                return BadRequest("Bu binada aynı kat zaten mevcut.");
            _db.AssetBuildingFloors.Add(new AssetBuildingFloor { AssetBuildingId=buildingId, Name=name.Trim() });
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoom(int floorId, string name, int? siteLocationId, CancellationToken ct = default)
        {
            var roomName = name?.Trim() ?? string.Empty;
            if (roomName.Length == 0 || roomName.Length > 100)
                return PlaceError("Oda adı boş veya geçersiz.");

            var floor = await _db.AssetBuildingFloors
                .Include(x => x.Building)
                .SingleOrDefaultAsync(x => x.Id == floorId, ct);
            if (floor == null || !floor.IsActive || !floor.Building.IsActive)
                return PlaceError("Aktif bina ve kat seçiniz.");

            if (await _db.AssetBuildingRooms.AnyAsync(
                x => x.AssetBuildingFloorId == floorId && x.Name == roomName, ct))
                return PlaceError("Bu katta aynı isimli oda zaten mevcut.");

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                int locationId;
                if (siteLocationId.HasValue)
                {
                    // Önceden oluşturulmuş kayıtla isteğe bağlı eşleştirme hâlâ desteklenir.
                    var location = await _db.SiteLocations.FindAsync(
                        new object[] { siteLocationId.Value }, ct);
                    if (location == null)
                        return PlaceError("Seçilen mevcut konum bulunamadı.");
                    if (await _db.AssetBuildingRooms.AnyAsync(
                        x => x.SiteLocationId == siteLocationId.Value, ct))
                        return PlaceError("Bu konum zaten başka bir odaya bağlı.");
                    locationId = siteLocationId.Value;
                }
                else
                {
                    var location = AssetPlaceSiteLocationFactory.Create(
                        _db, floor.Building.Name, floor.Name, roomName,
                        floor.Building.Latitude, floor.Building.Longitude);
                    _db.SiteLocations.Add(location);
                    await _db.SaveChangesAsync(ct);
                    locationId = location.Id;
                }

                _db.AssetBuildingRooms.Add(new AssetBuildingRoom
                {
                    AssetBuildingFloorId = floorId,
                    Name = roomName,
                    SiteLocationId = locationId,
                    IsActive = true
                });
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                TempData["AssetPlaceSuccess"] =
                    "Oda kaydedildi ve konumla otomatik eşleştirildi (Konum ID: " + locationId + ").";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                await tx.RollbackAsync(ct);
                return PlaceError(ex.Message);
            }
            catch (DbUpdateException)
            {
                await tx.RollbackAsync(ct);
                return PlaceError("Oda ve konum veritabanına yazılamadı. SiteLocation alanlarını ve zorunlu ilişkileri kontrol edin.");
            }
        }
        // İsteğe bağlı alanlarda yanlış veya çok uzun metin veritabanına gönderilmesin.
        private static string? CleanArea(string? value)
        {
            var text = value?.Trim();
            return string.IsNullOrWhiteSpace(text) ? null : text.Length <= 120 ? text : text[..120];
        }

        // Hem 40.7002606 (HTML number/JS) hem 40,7002606 (tr-TR) kabul edilir.
        // Thousands separator yasak: 40.7002606 asla 407002606 olarak yorumlanmaz.
        private static bool TryParseCoordinate(string? value, double minimum, double maximum, out double result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(value)) return false;
            var normalized = value.Trim().Replace(',', '.');
            if (!double.TryParse(normalized,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out result)) return false;
            return double.IsFinite(result) && result >= minimum && result <= maximum;
        }

        // Bina/kat/oda kayıtları silinmeden güncellenir; var olan demirbaş bağlantıları korunur.
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBuilding(int id, string name, string latitude, string longitude,
            bool isActive, string? province, string? district, string? neighborhood, CancellationToken ct=default)
        {
            var b=await _db.AssetBuildings.SingleOrDefaultAsync(x=>x.Id==id,ct);
            if(b==null)return NotFound();
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 200 ||
                !TryParseCoordinate(latitude, -90, 90, out var latitudeValue) ||
                !TryParseCoordinate(longitude, -180, 180, out var longitudeValue))
            {
                TempData["AssetPlaceError"] = "Bina adı veya koordinat geçersiz.";
                return RedirectToAction(nameof(Index));
            }

            b.Name = name.Trim();
            b.Latitude = latitudeValue;
            b.Longitude = longitudeValue;
            b.Province = CleanArea(province);
            b.District = CleanArea(district);
            b.Neighborhood = CleanArea(neighborhood);
            b.IsActive = isActive;
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateFloor(int id,string name,bool isActive,CancellationToken ct=default)
        {
            var f=await _db.AssetBuildingFloors.SingleOrDefaultAsync(x=>x.Id==id,ct);
            if(f==null)return NotFound();
            if(string.IsNullOrWhiteSpace(name)||name.Trim().Length>100)return BadRequest("Kat adı geçersiz.");
            if(await _db.AssetBuildingFloors.AnyAsync(x=>x.AssetBuildingId==f.AssetBuildingId && x.Id!=id && x.Name==name.Trim(),ct))
                return BadRequest("Bu binada aynı isimde kat mevcut.");
            f.Name=name.Trim();f.IsActive=isActive;
            await _db.SaveChangesAsync(ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoom(int id, string name, int? siteLocationId, bool isActive, CancellationToken ct = default)
        {
            var room = await _db.AssetBuildingRooms
                .Include(x => x.Floor).ThenInclude(f => f.Building)
                .SingleOrDefaultAsync(x => x.Id == id, ct);
            if (room == null) return NotFound();
            var newName = name?.Trim() ?? string.Empty;
            if (newName.Length == 0 || newName.Length > 100)
                return PlaceError("Oda adı geçersiz.");
            if (await _db.AssetBuildingRooms.AnyAsync(x =>
                x.AssetBuildingFloorId == room.AssetBuildingFloorId &&
                x.Id != id && x.Name == newName, ct))
                return PlaceError("Bu katta aynı isimde oda mevcut.");

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                if (siteLocationId.HasValue && siteLocationId.Value != room.SiteLocationId)
                {
                    var other = await _db.SiteLocations.FindAsync(
                        new object[] { siteLocationId.Value }, ct);
                    if (other == null) return PlaceError("Seçilen konum bulunamadı.");
                    if (await _db.AssetBuildingRooms.AnyAsync(x =>
                        x.Id != id && x.SiteLocationId == siteLocationId.Value, ct))
                        return PlaceError("Bu konum başka bir odaya bağlı.");
                    // Mevcut demirbaşlar eski SiteLocationId'de kalır; kendiliğinden taşınmaz.
                    room.SiteLocationId = siteLocationId.Value;
                }
                else if (!room.SiteLocationId.HasValue)
                {
                    // Eski V6'da konumsuz oluşturulmuş odaları da onarır.
                    var location = AssetPlaceSiteLocationFactory.Create(
                        _db, room.Floor.Building.Name, room.Floor.Name, newName,
                        room.Floor.Building.Latitude, room.Floor.Building.Longitude);
                    _db.SiteLocations.Add(location);
                    await _db.SaveChangesAsync(ct);
                    room.SiteLocationId = location.Id;
                }
                // Boş select değeri mevcut eşleştirmeyi SİLMEZ.
                room.Name = newName;
                room.IsActive = isActive;
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
                TempData["AssetPlaceSuccess"] = "Oda bilgileri güncellendi. Konum eşleştirmesi korundu.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                await tx.RollbackAsync(ct);
                return PlaceError(ex.Message);
            }
            catch (DbUpdateException)
            {
                await tx.RollbackAsync(ct);
                return PlaceError("Oda güncellenemedi. Veritabanı zorunlu alanlarını kontrol edin.");
            }
        }

    }
}
