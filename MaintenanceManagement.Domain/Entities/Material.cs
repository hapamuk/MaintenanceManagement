using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class Material : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Malzeme Kodu")]
        public string MaterialCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Malzeme Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Ölçü Birimi")]
        public int MeasurementUnitId { get; set; }

        [Display(Name = "Malzeme Kategorisi")]
        public int? MaterialCategoryId { get; set; }

        [Display(Name = "Sorumlu Birim")]
        public int? UnitId { get; set; }

        [Range(
     typeof(decimal),
     "0",
     "999999999.999",
     ParseLimitsInInvariantCulture = true)]
        [Display(Name = "Minimum Stok")]
        public decimal MinimumStockLevel { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public MeasurementUnit MeasurementUnit { get; set; } = null!;

        public MaterialCategory? MaterialCategory { get; set; }

        public Unit? Unit { get; set; }

        public ICollection<WorkOrderMaterial> WorkOrderMaterials
        { get; set; } = new HashSet<WorkOrderMaterial>();

        public ICollection<StockMovement> StockMovements
        { get; set; } = new HashSet<StockMovement>();

        public ICollection<WarehouseStock> WarehouseStocks
        { get; set; } = new HashSet<WarehouseStock>();
    }
}
