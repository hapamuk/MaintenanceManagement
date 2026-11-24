namespace MaintenanceManagement.Domain.Entities
{
    // Mevcut Asset entity'si değiştirilmeden üst-alt varlık ilişkisini saklar.
    public sealed class AssetParentLink
    {
        public int AssetId { get; set; }
        public int ParentAssetId { get; set; }
    }
}
