using System;
using System.Globalization;
using System.Linq;
using MaintenanceManagement.Domain.Entities;
using MaintenanceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MaintenanceManagement.Infrastructure.Services
{
    /// <summary>
    /// Mevcut SiteLocation sınıfını değiştirmez. EF metadata üzerinden gerçek
    /// alanları tanır; bilinmeyen zorunlu alanları uydurmak yerine anlaşılır hata verir.
    /// </summary>
    public static class AssetPlaceSiteLocationFactory
    {
        public static SiteLocation Create(ApplicationDbContext db,
            string building, string floor, string room,
            double latitude, double longitude)
        {
            var location = new SiteLocation();
            var entity = db.Entry(location);
            var type = entity.Metadata;
            var fullName = $"{building} / {floor} / {room}";
            var nameProperty = new[] { "Name", "LocationName", "Title" }
                .FirstOrDefault(n => type.FindProperty(n) != null);
            if (nameProperty == null)
                throw new InvalidOperationException(
                    "SiteLocation sınıfında Name veya LocationName alanı bulunamadı. " +
                    "SiteLocation.cs dosyanızı paylaşın; alanlara uygun şekilde eşleyelim.");
            Assign(entity, nameProperty, fullName);
            AssignIfExists(entity, "Latitude", latitude);
            AssignIfExists(entity, "Longitude", longitude);
            AssignIfExists(entity, "IsActive", true);
            AssignIfExists(entity, "CreatedAt", DateTime.UtcNow);
            AssignIfExists(entity, "UpdatedAt", null);

            // Konumu ayrıca tutan sınıflarda okunabilir bilgiler sağlanır.
            AssignIfExists(entity, "BuildingName", building);
            AssignIfExists(entity, "FloorName", floor);
            AssignIfExists(entity, "RoomName", room);

            // Zorunlu bilinmeyen alanlara sahte veri yazmayalım.
            var missing = type.GetProperties()
                .Where(p => !p.IsPrimaryKey() && !p.IsForeignKey() &&
                    !p.IsNullable && p.ValueGenerated != ValueGenerated.OnAdd && p.ValueGenerated != ValueGenerated.OnAddOrUpdate &&
                    p.ClrType == typeof(string) &&
                    string.IsNullOrWhiteSpace(entity.Property(p.Name).CurrentValue?.ToString()))
                .Select(p => p.Name).ToArray();
            if (missing.Length > 0)
                throw new InvalidOperationException(
                    "SiteLocation için zorunlu alanlar eksik: " +
                    string.Join(", ", missing) + ". SiteLocation.cs dosyanızdaki " +
                    "bu alanların nasıl doldurulacağını tanımlamamız gerekiyor.");
            return location;
        }

        private static void AssignIfExists(
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entity,
            string name, object? value)
        {
            if (entity.Metadata.FindProperty(name) != null)
                Assign(entity, name, value);
        }

        private static void Assign(
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entity,
            string name, object? value)
        {
            var property = entity.Property(name);
            var type = Nullable.GetUnderlyingType(property.Metadata.ClrType)
                ?? property.Metadata.ClrType;
            if (value == null)
            {
                if (property.Metadata.IsNullable) property.CurrentValue = null;
                return;
            }
            if (type == typeof(string))
            {
                var text = Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
                var limit = property.Metadata.GetMaxLength();
                if (limit.HasValue && text.Length > limit.Value)
                    throw new InvalidOperationException(
                        $"SiteLocation.{name} en fazla {limit.Value} karakter olabilir. " +
                        "Bina/kat/oda adlarını kısaltın.");
                property.CurrentValue = text;
            }
            else if (type == typeof(decimal))
                property.CurrentValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            else if (type == typeof(double))
                property.CurrentValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);
            else if (type == typeof(float))
                property.CurrentValue = Convert.ToSingle(value, CultureInfo.InvariantCulture);
            else if (type == typeof(bool))
                property.CurrentValue = Convert.ToBoolean(value, CultureInfo.InvariantCulture);
            else if (type == typeof(DateTime))
                property.CurrentValue = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
        }
    }
}
