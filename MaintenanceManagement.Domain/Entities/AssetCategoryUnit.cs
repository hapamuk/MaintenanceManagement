namespace MaintenanceManagement.Domain.Entities
{
    // AssetCategory <-> Unit kapsamı. Unit ve AssetCategory kayıtları tekrar oluşturulmaz.
    public sealed class AssetCategoryUnit
    {
        public int AssetCategoryId { get; set; }
        public int UnitId { get; set; }
    }
}
