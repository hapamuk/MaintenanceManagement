using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class Warehouse : BaseEntity
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Ambar Kodu")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Ambar Adı")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Birim")]
        public int? UnitId { get; set; }

        [StringLength(250)]
        [Display(Name = "Konum")]
        public string? Location { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public Unit? Unit { get; set; }

        public ICollection<WarehouseStock> WarehouseStocks
        { get; set; } = new HashSet<WarehouseStock>();

        public ICollection<StockMovement> StockMovements
        { get; set; } = new HashSet<StockMovement>();

        public ICollection<WorkOrderMaterial> WorkOrderMaterials
        { get; set; } = new HashSet<WorkOrderMaterial>();

        public ICollection<WarehousePersonnel> WarehousePersonnels { get; set; }
    = new HashSet<WarehousePersonnel>();
    }
}
