using System.Collections.Generic;

namespace MaintenanceManagement.Domain.Entities
{
    public sealed class AssetBuilding
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        // İsteğe bağlı idari konum etiketleri (OSM haritası için ücretli API gerekmez).
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<AssetBuildingFloor> Floors { get; set; } = new List<AssetBuildingFloor>();
    }
    public sealed class AssetBuildingFloor
    {
        public int Id { get; set; }
        public int AssetBuildingId { get; set; }
        public AssetBuilding Building { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<AssetBuildingRoom> Rooms { get; set; } = new List<AssetBuildingRoom>();
    }
    public sealed class AssetBuildingRoom
    {
        public int Id { get; set; }
        public int AssetBuildingFloorId { get; set; }
        public AssetBuildingFloor Floor { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        // Mevcut Asset.SiteLocationId bağlantısını korumak için oda mevcut SiteLocation kaydıyla ilişkilendirilir.
        public int? SiteLocationId { get; set; }
        public SiteLocation? SiteLocation { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
