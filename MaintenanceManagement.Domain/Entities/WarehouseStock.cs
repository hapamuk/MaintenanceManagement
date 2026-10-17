using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaintenanceManagement.Domain.Entities
{
    public class WarehouseStock : BaseEntity
    {
        [Required]
        [Display(Name = "Ambar")]
        public int WarehouseId { get; set; }

        [Required]
        [Display(Name = "Malzeme")]
        public int MaterialId { get; set; }

        [Range(typeof(decimal), "0", "999999999.999")]
        [Display(Name = "Mevcut Miktar")]
        public decimal Quantity { get; set; }

        [Display(Name = "Son Hareket Tarihi")]
        public DateTime? LastMovementDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } =
            Array.Empty<byte>();

        public Warehouse Warehouse { get; set; } = null!;

        public Material Material { get; set; } = null!;
    }
}
