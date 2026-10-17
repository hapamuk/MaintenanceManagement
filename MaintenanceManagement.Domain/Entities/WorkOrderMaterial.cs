using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WorkOrderMaterial : BaseEntity
    {
        [Required]
        [Display(Name = "İş Emri")]
        public int WorkOrderId { get; set; }

        [Required]
        [Display(Name = "Malzeme")]
        public int MaterialId { get; set; }

        [Required]
        [Display(Name = "Ambar")]
        public int WarehouseId { get; set; }

        [Range(typeof(decimal), "0.001", "999999999.999")]
        [Display(Name = "Kullanılan Miktar")]
        public decimal Quantity { get; set; }

        [Range(typeof(decimal), "0", "999999999.99")]
        [Display(Name = "Birim Maliyet")]
        public decimal? UnitCost { get; set; }

        [Display(Name = "Stok Hareketi")]
        public int? StockMovementId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public WorkOrder WorkOrder { get; set; } = null!;

        public Material Material { get; set; } = null!;

        public Warehouse Warehouse { get; set; } = null!;

        public StockMovement? StockMovement { get; set; }
    }
}
