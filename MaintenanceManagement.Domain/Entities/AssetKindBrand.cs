namespace MaintenanceManagement.Domain.Entities
{
    // Bir demirbaş türüne birden çok marka; bir markaya birden çok tür bağlanabilir.
    public sealed class AssetKindBrand
    {
        public int AssetKindId { get; set; }
        public int BrandId { get; set; }
    }
}
